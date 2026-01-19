using gop.Data;
using gop.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace gop.Repositories.GeneralRepos;

/// <summary>
/// Act Reference
/// </summary>
public interface IActReferenceRepo
{
    /// <summary>
    /// To find by title
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    Task<ActReference?> FindByTitleAsync(string title);
    /// <summary>
    /// To find the add async
    /// </summary>
    /// <param name="actReference"></param>
    /// <returns></returns>
    Task AddAsync(ActReference actReference);
}

/// <summary>
/// To handle the reference
/// </summary>
public class ActReferenceRepo : IActReferenceRepo
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="context"></param>
    public ActReferenceRepo(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Find by title
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    public async Task<ActReference?> FindByTitleAsync(string title)
    {
        return await _context.ActReferences.FirstOrDefaultAsync(a => a.Title == title);
    }

    /// <summary>
    /// Add act reference
    /// </summary>
    /// <param name="actReference"></param>
    public async Task AddAsync(ActReference actReference)
    {
        await _context.ActReferences.AddAsync(actReference);
        await _context.SaveChangesAsync();
    }
}