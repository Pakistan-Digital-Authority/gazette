using gop.Repositories;
using gop.Requests.PublicRequests;
using gop.Utilities;

namespace gop.Services.Public;

/// <summary>
/// Service interface for verifying SRO numbers via public API
/// </summary>
public interface IPublicVerifySroService
{
    /// <summary>
    /// Verify SRO number asynchronously
    /// </summary>
    /// <param name="request">Request containing the SRO number</param>
    /// <returns>API response with verification result</returns>
    Task<ApiResponse> VerifySroNumberAsync(VerifySroNumberRequest request);
}

/// <summary>
/// Implementation of the SRO verification service
/// </summary>
public class PublicVerifySroService : IPublicVerifySroService
{
    private readonly ISroVerificationRepo _repo;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="repo"></param>
    public PublicVerifySroService(ISroVerificationRepo repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Verify sro number - service
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse> VerifySroNumberAsync(VerifySroNumberRequest request)
    {
        var response = new ApiResponse();

        try
        {
            var isValid = await _repo.IsValidSroNumberAsync(request.SroNumber);
            if (!isValid)
            {
                response.Status = 422;
                response.Message = "The provided SRO number is invalid";
                return response;
            }
            response.Status = 200;
            response.Message = "The provided SRO number is valid.";
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = $"Error occurred while verifying SRO number: {ex.Message}";
        }

        return response;
    }
}