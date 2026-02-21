using System.Reflection;
using gop.Data;
using gop.Data.Entities;
using gop.Enums;
using gop.Interfaces;
using gop.Repositories.GeneralRepos;
using gop.Requests.AdminRequests;
using gop.Responses.AdminResponses;
using gop.Security;
using gop.Security.CurrentUser;
using gop.Utilities;
using Microsoft.EntityFrameworkCore;

namespace gop.Services.AdminServices;

/// <summary>
/// Interface for admin user management
/// </summary>
public interface IAdminUserService
{
    /// <summary>
    /// To fetch all the users that the admin has added.
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse<PagedResult<AdminUserListResponse>>> GetUsersAsync(AdminGetUserListRequest request);
    
    /// <summary>
    /// To fetch the user by id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ApiResponse<AdminUserResponse>> GetUserByIdAsync(Guid userId);
    
    /// <summary>
    /// To create user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse> CreateUserAsync(AdminCreateUserRequest request);
    
    /// <summary>
    /// to update the user details
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse> UpdateUserAsync(Guid userId, AdminUpdateUserRequest request);
    
    /// <summary>
    /// to resend the invitation email/notification
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse> ResendInvitationEmailAsync(ResendInvitationEmailRequest request);
    
    /// <summary>
    /// To remove the user from db
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ApiResponse> DeleteUserAsync(Guid userId);

    /// <summary>
    /// Get SRO Rule Format
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse<GetSroRuleResponse>> GetSroRuleAsync();
}

/// <summary>
/// Admin User creation service
/// </summary>
public class AdminUserService : IAdminUserService
{
    private readonly IUserRepo _repo;
    private readonly ICurrentUserProvider _currentUser;
    private readonly IEmailSender _emailSender;
    private readonly DatabaseContext _context;
    private readonly ITokenClaimsService _tokenClaimsService;
    private readonly ILogsRepo _logsRepo;

    /// <summary>
    /// Ctor - Admin user service
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="emailSender"></param>
    /// <param name="currentUser"></param>
    /// <param name="tokenClaimsService"></param>
    /// <param name="context"></param>
    /// <param name="logsRepo"></param>
    public AdminUserService(IUserRepo repo, IEmailSender emailSender, ICurrentUserProvider currentUser, ITokenClaimsService tokenClaimsService, DatabaseContext context, ILogsRepo logsRepo)
    {
        _repo = repo;
        _emailSender = emailSender;
        _currentUser = currentUser;
        _tokenClaimsService = tokenClaimsService;
        _context = context;
        _logsRepo = logsRepo;
    }
    
    /// <summary>
    /// Get the users list
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse<PagedResult<AdminUserListResponse>>> GetUsersAsync(AdminGetUserListRequest request)
    {
        var response = new ApiResponse<PagedResult<AdminUserListResponse>>();
        try
        {
            var users = await _repo.GetPagedUsersAsync(request.PageNumber, request.PageSize);
            if (!users.Items.Any())
            {
                response.Status = 404;
                response.Message = "No users found";
                return response;
            }
            var usersList = new List<AdminUserListResponse>();
            foreach (var user in users.Items)
            {
                var userDetails = new AdminUserListResponse();
                userDetails.Id = user.Id;
                userDetails.FullName = user.FullName;
                userDetails.Ministry = user.Ministry;
                userDetails.Email = user.Email;
                userDetails.Phone =  user.Phone;
                userDetails.Role = user.Role.ToString();
                userDetails.Status = user.Status.ToString();
                usersList.Add(userDetails);
            }

            var pagedResponse = new PagedResult<AdminUserListResponse>(usersList, users.TotalCount, users.PageNumber, users.PageSize);
            
            response.Status = 200;
            response.Message = "Users data retrieved successfully!";
            response.Data = pagedResponse;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }
    
    /// <summary>
    /// Get user by id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ApiResponse<AdminUserResponse>> GetUserByIdAsync(Guid userId)
    {
        var response = new ApiResponse<AdminUserResponse>();
        try
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user is null)
            {
                response.Status = 404;
                response.Message = "The requested user does not exist!";
                return response;
            }

            var userDetails = new AdminUserResponse();
            userDetails.Id = user.Id;
            userDetails.FullName = user.FullName;
            userDetails.Ministry = user.Ministry;
            userDetails.Email = user.Email;
            userDetails.Phone = user.Phone;
            
            response.Status = 200;
            response.Message = "User details fetched successfully!";
            response.Data = userDetails;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }

    /// <summary>
    /// To create a new user - though admin
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ApiResponse> CreateUserAsync(AdminCreateUserRequest request)
    {
        var response = new ApiResponse();
        try
        {
            var currentUser = _currentUser.GetCurrentUser();
            var exists = await _repo.ExistsByEmailAsync(request.Email);
            if (exists)
            {
                response.Status = 409;
                response.Message = $"The provided email {request.Email} already exists. Please try another one.";
                return response;
            }
            
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.Name,
                Ministry = request.Ministry,
                Email = request.Email,
                Status = request.Status,
                Role = request.Role,
                ParentId = currentUser.Id
            };
            var newTokenGenerator = new TokenGenerator(_tokenClaimsService);
            var newToken = newTokenGenerator.GenerateVerificationToken(newUser, TokenTypeEnum.Invitation);
            
            var isCreated = await _repo.AddAsync(newUser);
            if (!isCreated)
            {
                response.Status = 500;
                response.Message = "Failed to create user";
                return response;
            }
            
            #region -- Creating Logs

            var log = new Log
            {
                Level = LogLevelEnum.Info,
                Title = "User Created",
                Message = newUser.Ministry + "  publisher account",
                Source = nameof(AdminUserService),
                Action = nameof(CreateUserAsync),
                HttpMethod = "POST",
                RequestPath = "create-user",
                StatusCode = 200,
                UserId = currentUser.Id,
                UserEmail = currentUser.Email,
                IpAddress = _currentUser.GetIpAddress(),
                CreatedAt = DateTime.UtcNow
            };

            var createLog = await _logsRepo.CreateLogAsync(log);

            #endregion
            
            // sending email
            const string subject = "You have been invited to join GOP as a Publisher";
            var rootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var emailTemplateFolderName = "Infrastructure/EmailService/Templates";
            var folderPath = Path.Combine(rootFolderPath, emailTemplateFolderName);
            var welcomeEmailPath = Path.Combine(folderPath, "Invitation.html");
            
            string mailBody = await File.ReadAllTextAsync(welcomeEmailPath);
            mailBody = mailBody.Replace("{{invitationLink}}", $"http://localhost:3000/auth/accept-invitation?token={newToken.Access}");
            mailBody = mailBody.Replace("{{token}}", newToken.Access);
            await _emailSender.SendEmailAsync(request.Email, subject, mailBody);
            
            response.Status = 200;
            response.Message = "User created successfully! A verification email has been sent to the provided email address";
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }

    /// <summary>
    /// User the requested user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse> UpdateUserAsync(Guid userId, AdminUpdateUserRequest request)
    {
        var response = new ApiResponse();
        try
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user is null)
            {
                response.Status = 404;
                response.Message = "The requested user does not exist!";
                return response;
            }
            
            // Email uniqueness check (only if changed)
            // if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            // {
            //     var emailExists = await _repo.ExistsByEmailAsync(request.Email);
            //     if (emailExists)
            //     {
            //         response.Status = 409;
            //         response.Message = $"The email {request.Email} is already in use.";
            //         return response;
            //     }
            // }
            
            user.FullName = request.Name;
            user.Ministry = request.Ministry;
            // user.Email = request.Email;
            user.Phone = request.Phone;
            user.Status = request.Status;
            user.Role = request.Role;

            var isUpdated = await _repo.UpdateAsync(user);
            if (!isUpdated)
            {
                response.Status = 500;
                response.Message = "Failed to update user details.";
                return response;
            }
            // user details update - do whatever the logic is for the updated user here
            
            response.Status = 200;
            response.Message = "User details updated successfully!";
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }

    /// <summary>
    /// To resend the invitation
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse> ResendInvitationEmailAsync(ResendInvitationEmailRequest request)
    {
        var response = new ApiResponse();
        try
        {
            var user = await _repo.GetByIdAsync(request.UserId);
            if (user is null)
            {
                response.Status = 404;
                response.Message = "The requested user does not exist!";
                return response;
            }

            if (user.Status == UserStatusEnum.Active)
            {
                response.Status = 409;
                response.Message = "The requested account has been already activated.";
                return response;
            }
            
            var newTokenGenerator = new TokenGenerator(_tokenClaimsService);
            var newToken = newTokenGenerator.GenerateVerificationToken(user, TokenTypeEnum.Invitation);
            
            // sending email - resending invitation email
            const string subject = "You have been invited to join GOP as a Publisher";
            var rootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var emailTemplateFolderName = "Infrastructure/EmailService/Templates";
            var folderPath = Path.Combine(rootFolderPath, emailTemplateFolderName);
            var welcomeEmailPath = Path.Combine(folderPath, "Invitation.html");
            
            string mailBody = await File.ReadAllTextAsync(welcomeEmailPath);
            mailBody = mailBody.Replace("{{invitationLink}}", $"http://localhost:3000/auth/accept-invitation?token={newToken.Access}");
            mailBody = mailBody.Replace("{{token}}", newToken.Access);
            await _emailSender.SendEmailAsync(user.Email, subject, mailBody);

            await _context.SaveChangesAsync();
            
            response.Status = 200;
            response.Message = "A verification email has been sent to the provided email address";
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }

    /// <summary>
    /// Remove the user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ApiResponse> DeleteUserAsync(Guid userId)
    {
        var response = new ApiResponse();
        try
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user is null)
            {
                response.Status = 404;
                response.Message = "The requested user does not exist!";
                return response;
            }

            var isDeleted = await _repo.DeleteAsync(user);
            if (!isDeleted)
            {
                response.Status = 500;
                response.Message = "Failed to delete the user.";
                return response;
            }
            
            response.Status = 200;
            response.Message = "User deleted successfully!";
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }

    /// <summary>
    /// Get Sro Rule Formatting
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse<GetSroRuleResponse>> GetSroRuleAsync()
    {
        var response = new ApiResponse<GetSroRuleResponse>();
        try
        {
            var currentYear = DateTime.UtcNow.Year;
            
            var sroCounter = await _context.SroCounters.OrderBy(q => q.CreatedAt).FirstOrDefaultAsync(s => s.Year == currentYear);

            if (sroCounter == null)
            {
                sroCounter = new SroCounter
                {
                    Year = currentYear,
                    CurrentCounter = 1,
                    UpdatedAt = DateTime.UtcNow
                };
            }
            
            var sroRuleResponse = new GetSroRuleResponse
            {
                SroPatternFormat =  "S.R.O. {XXXX}({PART})/{YYYY}",
                CurrentCounter = sroCounter.CurrentCounter,
                NextAvailable = sroCounter.CurrentCounter + 1,
            };

            
            response.Status = 200;
            response.Message = "SRO Numbering Rules fetched successfully!";
            response.Data = sroRuleResponse;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }
}