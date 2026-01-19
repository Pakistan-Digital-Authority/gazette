using Ardalis.GuardClauses;
using gop.Interfaces;

namespace gop.Services;

/// <summary>
/// Comparing and hashing strings
/// </summary>
/// <param name="logger"></param>
public class BCryptHashService(ILogger<BCryptHashService> logger) : IHashService
{
    private readonly ILogger<BCryptHashService> _logger = logger;

    /// <summary>
    /// Compare the provided string and hash
    /// </summary>
    /// <param name="text"></param>
    /// <param name="hash"></param>
    /// <returns></returns>
    public bool Compare(string text, string hash)
    {
        Guard.Against.NullOrWhiteSpace(text);
        Guard.Against.NullOrWhiteSpace(hash);

        try
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(text, hash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while verifying the HASH with BCrypt: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Hash the provided string
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string Hash(string text)
    {
        Guard.Against.NullOrWhiteSpace(text);

        try
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating the HASH with BCrypt: {Message}", ex.Message);
            throw;
        }
    }
}