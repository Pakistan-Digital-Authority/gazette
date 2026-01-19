using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using gop.Data;
using gop.Data.Entities;
using gop.Enums;
using gop.Interfaces;
using gop.Options;
using gop.Repositories.GeneralRepos;
using gop.Requests.AuthRequests;
using gop.Responses.AuthResponses;
using gop.Security;
using gop.Security.CurrentUser;
using gop.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace gop.Services;

/// <summary>
/// Interface for authentication - using AD
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Service for authentication
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse<TokenResponse>> AuthenticateAsync(LogInRequest request);

    /// <summary>
    /// For Refresh Token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Change AD user password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse> SendResetPasswordEmailAsync(ChangePasswordRequest request);

    Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request);
    /// <summary>
    /// To verify the account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse> VerifyAccountAsync(AccountVerificationRequest request);
    
    /// <summary>
    /// Logout Interface
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse> LogoutAsync();
    
    /// <summary>
    /// Get User Profile Interface
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse<ProfileInfo>> ProfileAsync();

    /// <summary>
    /// To accept the invitation
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse> AcceptInvitationAsync(AcceptInvitationRequest request);


}

/// <summary>
/// The Authentication Service - That will authenticate the user from AD
/// </summary>
/// <param name="authOptions"></param>
/// <param name="hashService"></param>
/// <param name="tokenClaimsService"></param>
/// <param name="context"></param>
/// <param name="configuration"></param>
public class AuthService(IOptions<AuthOptions> authOptions, ICurrentUserProvider currentUser, IHashService hashService, ITokenClaimsService tokenClaimsService,IConfiguration configuration, DatabaseContext context, IUserRepo repo, IEmailSender emailSender, IDateTimeProvider dateTimeProvider, ILogsRepo logsRepo) : IAuthService
{
    private readonly AuthOptions _authOptions = authOptions.Value;
    private readonly IUserRepo _repo = repo;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogsRepo _logsRepo = logsRepo;
    
    /// <summary>
    /// The main authentication - which is responsible for authentication and creating jwt token for the user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse<TokenResponse>> AuthenticateAsync(LogInRequest request)
    {
        var response = new ApiResponse<TokenResponse>();
        try
        {
            var user = await context.Users
                .Include(u => u.Tokens.OrderByDescending(t => t.ExpiresAt))
                .FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null)
            {
                response.Status = (int)StatusCodeEnum.NotFound;
                response.Message = $"No such user found against the email address \'{request.Email}\'";
                return response;
            }

            #region --- Checking if the account is blocked/inactive.
            
            if (user.IsLocked())
            {
                response.Status = (int)StatusCodeEnum.Locked;
                response.Message = "“Your account is temporarily locked. It will be automatically unlocked after a few minutes.";
                return response;
            }
            
            if (!user.IsActive())
            {
                response.Status = (int)StatusCodeEnum.Forbidden;
                response.Message = "Your account is currently inactive. Please contact our support team for assistance.";
                return response;
            }

            #endregion
            
            // Checking if the password is not matching the original password
            if (!hashService.Compare(request.Password, user.HashPassword))
            {
                // If the login is invalid, the number of failures will be incremented.
                // If the access attempts limit is reached, the account will be blocked for a certain period.
                var lockedTimeSpan = TimeSpan.FromSeconds(_authOptions.SecondsBlocked);
                user.IncrementFailures(_authOptions.MaximumAttempts, lockedTimeSpan);

                if (user.IsLocked())
                {
                    // sending email - account blocked here
                    const string subject = "You account has been temporarily locked";
                    var rootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var emailTemplateFolderName = "Infrastructure/EmailService/Templates";
                    var folderPath = Path.Combine(rootFolderPath, emailTemplateFolderName);
                    var mailPath = Path.Combine(folderPath, "AccountLocked.html");
            
                    string mailBody = await File.ReadAllTextAsync(mailPath);
                    await _emailSender.SendEmailAsync(request.Email, subject, mailBody);
                    
                    await context.SaveChangesAsync();
                    response.Status = (int)StatusCodeEnum.Locked;
                    response.Message = "Your account has been temporarily locked due to multiple unsuccessful login attempts. It will be automatically unlocked after a few minutes.";
                    return response;
                }

                await context.SaveChangesAsync();
                
                response.Status = (int)StatusCodeEnum.Unauthorized;
                response.Message = "The email or password you entered is incorrect. Please try again.";
                return response;
            }

            #region -- Generating Claims & Token

            string sessionId = Guid.NewGuid().ToString();
            // Generating the rules (roles).
            var newTokenGenerator = new TokenGenerator(tokenClaimsService);
            var claims = newTokenGenerator.GenerateClaims(user, sessionId);

            // Generating the access token.
            var (accessToken, createdAt, expiresAt) = tokenClaimsService.GenerateAccessToken(claims);

            // Generating the refresh token.
            var refreshToken = tokenClaimsService.GenerateRefreshToken();

            // Linking the refresh token to the user.
            user.AddToken(new Token(accessToken, refreshToken, createdAt, expiresAt));
            
            var loginToken = new TokenResponse(user.Id, accessToken, createdAt, expiresAt, refreshToken);

            #endregion

            #region -- Generate Signin Logs

            var newLoginHistory = new LoginHistory();
            newLoginHistory.UserId = user.Id;
            newLoginHistory.LoginTime = DateTime.UtcNow;
            newLoginHistory.DeviceInfo = request.DeviceInfo;
            newLoginHistory.IpAddress = currentUser.GetIpAddress() ?? "Unknown";
            newLoginHistory.Latitude = request.Latitude;
            newLoginHistory.Longitude = request.Longitude;
            newLoginHistory.Message = $"User \"{user.FullName}\" successfully logged in at {DateTime.UtcNow:u}. IP Address: {newLoginHistory.IpAddress}, Device: {newLoginHistory.DeviceInfo}, Location: {newLoginHistory.Latitude?.ToString() ?? "N/A"}, {newLoginHistory.Longitude?.ToString() ?? "N/A"}.";
            newLoginHistory.SessionId = sessionId;

            context.LoginHistories.Add(newLoginHistory);
            #endregion
            
            await context.SaveChangesAsync();
            
            #region -- Creating Logs

            var log = new Log
            {
                Level = LogLevelEnum.Info,
                Title = "User Login",
                Message = user.Email + " from " + currentUser.GetIpAddress(),
                Source = nameof(AuthService),
                Action = nameof(AuthenticateAsync),
                HttpMethod = "POST",
                RequestPath = "authenticate",
                StatusCode = 200,
                UserId = user.Id,
                UserEmail = user.Email,
                IpAddress = currentUser.GetIpAddress(),
                CreatedAt = DateTime.UtcNow
            };

            var createLog = await _logsRepo.CreateLogAsync(log);

            #endregion
            
            response.Status = (int)StatusCodeEnum.Ok;
            response.Message = "Logged in successfully!";
            response.Data = loginToken;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = (int)StatusCodeEnum.InternalServerError;
            response.Message = ex.InnerException?.Message ?? ex.Message;
            return response;
        }
    }
    
    /// <summary>
    /// Refresh Token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var response = new ApiResponse<TokenResponse>();
        try
        {
            // var user = await repository.GetByRefreshTokenAsync(request.Token);
            var user = await context.Users
                .Include(u => u.Tokens.Where(t => t.Refresh == request.Token))
                .FirstOrDefaultAsync(u => u.Tokens.Any(t => t.Refresh == request.Token));

            if (user == null)
            {
                response.Status = (int)StatusCodeEnum.Unauthorized;
                response.Message = "The provided refresh token is either invalid or expired.";
                return response;
            }

            // Checking if the refresh token has expired.
            var token = user.Tokens.FirstOrDefault(t => t.Refresh == request.Token);

            if (token?.IsValid() == false)
            {
                response.Status = (int)StatusCodeEnum.Unauthorized;
                response.Message = "The provided refresh token is either invalid or expired.";
                return response;
            }

            // Generating the rules (roles).
            string sessionId = Guid.NewGuid().ToString();
            var newTokenGenerator = new TokenGenerator(tokenClaimsService);
            var claims = newTokenGenerator.GenerateClaims(user, sessionId);

            // Generating a new access token.
            var (accessToken, createdAt, expiresAt) = tokenClaimsService.GenerateAccessToken(claims);

            // Revoking (canceling) the current refresh token.
            token!.Revoke(createdAt);

            // Generating a new refresh token.
            var newRefreshToken = tokenClaimsService.GenerateRefreshToken();

            // Linking the new refresh token to the user.
            user.AddToken(new Token(accessToken, newRefreshToken, createdAt, expiresAt));

            await context.SaveChangesAsync();

            var tokenResponse = new TokenResponse(user.Id, accessToken, createdAt, expiresAt, newRefreshToken);
            response.Status = (int)StatusCodeEnum.Ok;
            response.Message = "Token refreshed successfully.";
            response.Data = tokenResponse;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = (int)StatusCodeEnum.InternalServerError;
            response.Message = ex?.InnerException?.Message ?? ex.Message;
            return response;
        }
    }
    
    /// <summary>
    /// Change AD Server Password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse> SendResetPasswordEmailAsync(ChangePasswordRequest request)
    {
        var response = new ApiResponse();
        try
        {
            
            var userInfo = await context.Users
                .Include(u => u.Tokens.OrderByDescending(t => t.ExpiresAt))
                .FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (userInfo == null)
            {
                response.Status = 404;
                response.Message = "The provided user doesn't exist on the server!";
                return response;
            }
            
            var newTokenGenerator =  new TokenGenerator(tokenClaimsService);
            var newToken = newTokenGenerator.GenerateVerificationToken(userInfo, TokenTypeEnum.ResetPassword);
            // sending email - with token on mail
            const string subject = "Password Reset Request";
            var rootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var emailTemplateFolderName = "Infrastructure/EmailService/Templates";
            var folderPath = Path.Combine(rootFolderPath, emailTemplateFolderName);
            var welcomeEmailPath = Path.Combine(folderPath, "ForgotPassword.html");
            
            string mailBody = await File.ReadAllTextAsync(welcomeEmailPath);
            mailBody = mailBody.Replace("{{token}}", newToken.Access);
            await _emailSender.SendEmailAsync(request.Email, subject, mailBody);

            await context.SaveChangesAsync();
            
            response.Status = 200;
            response.Message = $"We've sent an email to '{request.Email}' with instructions to reset your password. Please check your inbox and follow the steps provided.";
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex?.InnerException?.Message ?? ex.Message;
            return response;
        }
    }

    /// <summary>
    /// To reset the password using the token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var response = new ApiResponse();
        try
        {
            var token = await context.Tokens.FirstOrDefaultAsync(t => t.Access == request.Token);
            if (token is null)
            {
                response.Status = 401;
                response.Message = "The provided refresh token is either invalid or expired.";
                return response;
            }
            
            bool isValid = token.IsValid();
            if (!isValid)
            {
                response.Status = 401;
                response.Message = "The provided refresh token is either invalid or expired.";
                return response;
            }

            var extractedToken = new JwtSecurityTokenHandler().ReadJwtToken(request.Token);
            string userEmail = extractedToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;

            var userInfo = await _repo.GetByEmailAsync(userEmail);
            if (userInfo is null)
            {
                response.Status = 404;
                response.Message = "We couldn’t find an account associated with that email address.";
                return response;
            }

            if (hashService.Compare(request.NewPassword, userInfo.HashPassword))
            {
                response.Status = 422;
                response.Message = "The new password cannot be the same as the current password. Please choose a different one";
                return response;
            }

            userInfo.HashPassword = hashService.Hash(request.NewPassword);
            userInfo.Status = UserStatusEnum.Active;
            
            token!.Revoke(DateTime.UtcNow);

            userInfo.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            response.Status = 200;
            response.Message = "Your password has been updated successfully. You can now log in with your new credentials.";
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex?.InnerException?.Message ?? ex.Message;
            return response;
        }
    }
    /// <summary>
    /// To verify the account - using token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse> VerifyAccountAsync(AccountVerificationRequest request)
    {
        var response = new ApiResponse();
        try
        {
            var token = await context.Tokens.FirstOrDefaultAsync(t => t.Access == request.Token && t.TokenType == TokenTypeEnum.AccountVerification);
            if (token is null)
            {
                response.Status = 401;
                response.Message = "The provided refresh token is either invalid or expired.";
                return response;
            }

            bool isValid = token.IsValid();
            if (!isValid)
            {
                response.Status = 401;
                response.Message = "The provided refresh token is either invalid or expired.";
                return response;
            }

            var extractedToken = new JwtSecurityTokenHandler().ReadJwtToken(request.Token);
            string userEmail = extractedToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;

            var userInfo = await _repo.GetByEmailAsync(userEmail);
            if (userInfo is null)
            {
                response.Status = 404;
                response.Message = "The email address you provided does not exist in our records. Please ensure you have entered a valid email address.";
                return response;
            }

            if (userInfo.Status == UserStatusEnum.Active)
            {
                response.Status = 409;
                response.Message = "Your account has already been verified.";
                return response;
            }

            userInfo.Status = UserStatusEnum.Active;
            token!.Revoke(DateTime.UtcNow);
            
            // sending email - password successfully reset
            const string subject = "Password Reset Successfully!";
            var rootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var emailTemplateFolderName = "Infrastructure/EmailService/Templates";
            var folderPath = Path.Combine(rootFolderPath, emailTemplateFolderName);
            var welcomeEmailPath = Path.Combine(folderPath, "PasswordResetSuccess.html");
            
            string mailBody = await File.ReadAllTextAsync(welcomeEmailPath);
            
            await _emailSender.SendEmailAsync(userEmail, subject, mailBody);
            
            userInfo.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            response.Status = 200;
            response.Message = "Congratulations! Your account has been verified successfully.";
            return response;
        }
        catch (Exception excp)
        {
            response.Status = 500;
            response.Message = excp.InnerException?.Message ?? excp.Message;
            return response;
        }
    }

    /// <summary>
    /// Logout Implementation against the token
    /// </summary>
    /// <returns>Message if the account token has been revoke successfully</returns>
    public async Task<ApiResponse> LogoutAsync()
    {
        var response = new ApiResponse();
        try
        {
            var userInfo = currentUser.GetCurrentUser();
            var token = currentUser.GetAccessToken();
            
            var user = await context.Users
                .Include(u => u.Tokens.OrderByDescending(t => t.ExpiresAt))
                .FirstOrDefaultAsync(u => u.Id == userInfo.Id);
            
            if (user == null)
            {
                response.Status = (int)StatusCodeEnum.NotFound;
                response.Message = "There is no active user session found. Please log in to continue.";
                return response;
            }
            
            var checkToken = user.Tokens.FirstOrDefault(t => t.Access == token);
            if (checkToken?.IsValid() == false)
            {
                response.Status = (int)StatusCodeEnum.Unauthorized;
                response.Message = "The access token is invalid or has expired.";
                return response;
            }
            
            var currentToken = user.Tokens.FirstOrDefault(t => t.Access == token);
            
            currentToken!.Revoke(DateTime.UtcNow);
            user.UpdatedAt = DateTime.UtcNow;
            
            await context.SaveChangesAsync();
            
            response.Status = (int)StatusCodeEnum.Ok;
            response.Message = "You have been logout successfully!";
            return response;
        }
        catch (Exception ex)
        {
            response.Status = (int)StatusCodeEnum.InternalServerError;
            response.Message = ex?.InnerException?.Message ?? ex.Message;
            return response;
        }
    }

    /// <summary>
    /// Get User profile details
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse<ProfileInfo>> ProfileAsync()
    {
        var response = new ApiResponse<ProfileInfo>();
        try
        {
            var userInfo = currentUser.GetCurrentUser();
            var token = currentUser.GetAccessToken();
            
            var user = await context.Users
                .Include(u => u.Tokens.OrderByDescending(t => t.ExpiresAt))
                .FirstOrDefaultAsync(u => u.Id == userInfo.Id);
            
            if (user == null)
            {
                response.Status = (int)StatusCodeEnum.NotFound;
                response.Message = "There is no active user session found. Please log in to continue.";
                return response;
            }
            
            var checkToken = user.Tokens.FirstOrDefault(t => t.Access == token);
            if (checkToken?.IsValid() == false)
            {
                response.Status = (int)StatusCodeEnum.Unauthorized;
                response.Message = "The access token is invalid or has expired.";
                return response;
            }

            var profileInfo = new ProfileInfo();
            profileInfo.Id = user.Id;
            profileInfo.FullName = user.FullName;
            profileInfo.Email = user.Email;
            profileInfo.Phone = user.Phone;
            profileInfo.Status = user.Status;
            
            response.Status = (int)StatusCodeEnum.Ok;
            response.Message = "Profile info fetched successfully!";
            response.Data = profileInfo;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = (int)StatusCodeEnum.InternalServerError;
            response.Message = ex?.InnerException?.Message ?? ex.Message;
            return response;
        }
    }

    /// <summary>
    /// To Accept the invitation
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse> AcceptInvitationAsync(AcceptInvitationRequest request)
    {
        var response = new ApiResponse();
        try
        {
            var token = await context.Tokens.FirstOrDefaultAsync(t => t.Access == request.Token && t.TokenType == TokenTypeEnum.Invitation);
            if (token is null)
            {
                response.Status = 401;
                response.Message = "The provided refresh token is either invalid or expired.";
                return response;
            }
            bool isValid = token.IsValid();
            if (!isValid)
            {
                response.Status = 401;
                response.Message = "The provided refresh token is either invalid or expired.";
                return response;
            }

            var extractedToken = new JwtSecurityTokenHandler().ReadJwtToken(request.Token);
            string userEmail = extractedToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
            var extractedUser = await context.Users
                .Include(u => u.Tokens.OrderByDescending(t => t.ExpiresAt))
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (extractedUser is null)
            {
                response.Status = 401;
                response.Message = "The provided refresh token is either invalid or expired.";
                return response;
            }
            if (extractedUser.Status == UserStatusEnum.Active)
            {
                response.Status = 409;
                response.Message = "Your account has already been verified.";
                return response;
            }
            extractedUser.Status = UserStatusEnum.Active;
            extractedUser.HashPassword = hashService.Hash(request.NewPassword);
            
            // sending email - account verified
            const string subject = "Account Verified";
            var rootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var emailTemplateFolderName = "Infrastructure/EmailService/Templates";
            var folderPath = Path.Combine(rootFolderPath, emailTemplateFolderName);
            var welcomeEmailPath = Path.Combine(folderPath, "AccountVerified.html");
            string mailBody = await File.ReadAllTextAsync(welcomeEmailPath);
            await _emailSender.SendEmailAsync(userEmail, subject, mailBody);
            
            token!.Revoke(DateTime.UtcNow);

            extractedUser.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            response.Status = 200;
            response.Message = "Your account has been activated successfully!";
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}