using gop.Data;
using gop.Data.Entities;
using gop.Enums;
using gop.Extensions;
using gop.Utilities;
using Microsoft.EntityFrameworkCore;

namespace gop.Repositories.GeneralRepos;

/// <summary>
/// User repo interface
/// </summary>
public interface IUserRepo
{
    /// <summary>
    /// to get the paginated list of user
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    Task<PagedResult<User>> GetPagedUsersAsync(int pageNumber, int pageSize, string? search = null);
    
    /// <summary>
    /// To add the user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<bool> AddAsync(User user);
    
    /// <summary>
    /// To user the user details
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(User user);
    
    /// <summary>
    /// To delete/remove the user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(User user);
    
    /// <summary>
    /// To get the details of the user by id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<User?> GetByIdAsync(Guid userId);
    
    /// <summary>
    /// To get the details of the user by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<User?> GetByEmailAsync(string email);
    
    /// <summary>
    /// to check the details of the user by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<bool> ExistsByEmailAsync(string email);
    
    /// <summary>
    /// to check the user by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> ExistsByIdAsync(Guid id);
}

/// <summary>
/// User repo
/// </summary>
public class UserRepo : IUserRepo
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// CTOR - for user repo
    /// </summary>
    /// <param name="context"></param>
    public UserRepo(DatabaseContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// To add a user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> AddAsync(User user)
    {
        _context.Users.Add(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// To update user details
    /// </summary>
    /// <param name="user"></param>
    public async Task<bool> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        var affectedRows = await _context.SaveChangesAsync();
        return affectedRows > 0;
    }

    /// <summary>
    /// To delete/remove a user
    /// </summary>
    /// <param name="user"></param>
    public async Task<bool> DeleteAsync(User user)
    {
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        var affectedRows = await _context.SaveChangesAsync();
        return affectedRows > 0;
    }

    /// <summary>
    /// To get the user details by id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
    
    /// <summary>
    /// To get the user details by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// To get the user by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
    }
    
    /// <summary>
    /// To get the user by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id && !u.IsDeleted);
    }

    /// <summary>
    /// To get the paginated users list
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<PagedResult<User>> GetPagedUsersAsync(int pageNumber, int pageSize, string? search)
    {
        var query = _context.Users.AsQueryable();

        query = query.Where(u => !u.IsDeleted);

        // if (status.HasValue)
        //     query = query.Where(u => u.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));

        int skipCount = pageNumber * pageSize;

        return await query
            .OrderBy(u => u.FullName)
            .ToPagedResultAsync(pageNumber, pageSize);
    }
}