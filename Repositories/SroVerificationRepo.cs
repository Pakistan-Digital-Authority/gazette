using gop.Data;
using Microsoft.EntityFrameworkCore;

namespace gop.Repositories;

/// <summary>
/// Repository interface for SRO verification related data operations
/// </summary>
public interface ISroVerificationRepo
{
    /// <summary>
    /// Checks if the given SRO number exists and is valid
    /// </summary>
    /// <param name="sroNumber">SRO number to verify</param>
    /// <returns>True if valid; otherwise, false</returns>
    Task<bool> IsValidSroNumberAsync(string sroNumber);
}

/// <summary>
/// SRO Verification Repo
/// </summary>
public class SroVerificationRepo : ISroVerificationRepo
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// THE CTOR
    /// </summary>
    /// <param name="context"></param>
    public SroVerificationRepo(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Check if the SRO is valid or not
    /// </summary>
    /// <param name="sroNumber"></param>
    /// <returns></returns>
    public async Task<bool> IsValidSroNumberAsync(string sroNumber)
    {
        if (string.IsNullOrWhiteSpace(sroNumber))
            return false;
        return await _context.Notices.AnyAsync(sro => sro.SroNumber == sroNumber);
    }
}
