using gop.Data;
using gop.Data.Entities;
using gop.Enums;
using gop.Responses;
using Microsoft.EntityFrameworkCore;

namespace gop.Services;

/// <summary>
/// For Sro Numbering
/// </summary>
public interface ISroNumberService
{
    /// <summary>
    /// TO generate SRO number
    /// </summary>
    /// <param name="part"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    Task<SroCounterResponse> GenerateAsync(SroPartEnum part, int year);
}

/// <summary>
/// For SRO Numbering - service
/// </summary>
public class SroNumberService : ISroNumberService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="context"></param>
    public SroNumberService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// TO generate a SRO Number
    /// </summary>
    /// <param name="part"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    public async Task<SroCounterResponse> GenerateAsync(SroPartEnum part, int year)
    {
        var counter = await _context.SroCounters.FirstOrDefaultAsync(x => x.Year == year);

        if (counter == null)
        {
            counter = new SroCounter
            {
                Year = year,
                CurrentCounter = 0,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };
            _context.SroCounters.Add(counter);
        }

        counter.CurrentCounter += 1;
        counter.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        var formattedCounter = FormatCounter(counter.CurrentCounter);
        var slug = $"s-r-o-{formattedCounter}-{part}-{year}";
        
        
        var sroResponse = new SroCounterResponse();
        sroResponse.Numbering = $"S.R.O. {formattedCounter}({part})/{year}";
        sroResponse.CurrentCounter = counter.CurrentCounter;
        sroResponse.Year = counter.Year;
        sroResponse.Slug = slug;
        
        return sroResponse;
    }
    
    private static string FormatCounter(int counter)
    {
        return counter < 10000
            ? counter.ToString("D4")   // 0001 → 9999
            : counter.ToString();      // 10000+
    }
}