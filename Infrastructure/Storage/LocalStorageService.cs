using System.Dynamic;
using System.Reflection;
using gop.Interfaces;

namespace gop.Infrastructure.Storage;

/// <summary>
/// To store file locally
/// </summary>
public class LocalStorageService : IFileStorageService
{
    /// <summary>
    /// Root folder path of the application.
    /// </summary>
    private static readonly string RootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    /// <summary>
    /// Name of the folder containing the configurations files.
    /// </summary>
    private const string LocalUploadsFolderName = "LocalUploads";

    /// <summary>
    /// combining the root folder path and configurations folder.
    /// </summary>
    private static readonly string FolderPath = Path.Combine(RootFolderPath, LocalUploadsFolderName);

    /// <summary>
    /// To implement the ILogger for Google Cloud Storage Service.
    /// </summary>
    private readonly ILogger<LocalStorageService> _logger;

    /// <summary>
    /// To get the web host environment details.
    /// </summary>
    private readonly IHostEnvironment _hostEnvironment;

    /// <summary>
    /// CTR
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="hostEnvironment"></param>
    public LocalStorageService(ILogger<LocalStorageService> logger, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }


    /// <summary>
    /// To get the full resolved path of local storage
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="timeOutInMinutes"></param>
    /// <returns></returns>
    public async Task<ExpandoObject> GetFileUrlAsync(string fileName, int timeOutInMinutes = 30)
    {
        dynamic response = new ExpandoObject();
        try
        {
            // Combine folder path with file name to get the full file path
            string filePath = Path.Combine(FolderPath, fileName);

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                response.Status = 404;
                response.Message = "No such file found against the provided image path!";
                return response;
            }

            // Get the absolute path to the file
            string absoluteFilePath = Path.GetFullPath(filePath);

            response.Status = 200;
            response.Message = "File URL generated successfully";
            response.Location = absoluteFilePath;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Local Storage Service -> (Get File URL Error): {ex.InnerException?.Message ?? ex.Message}");
            response.Status = 500;
            response.Message = ex.InnerException?.Message ?? ex.Message;
            return response;
        }
    }

    /// <summary>
    /// To upload files on local storage
    /// </summary>
    /// <param name="fileContent"></param>
    /// <param name="fileName"></param>
    /// <param name="fileContentType"></param>
    /// <returns></returns>
    public async Task<ExpandoObject> UploadFileAsync(IFormFile fileContent, string fileName, string fileContentType = null)
    {
        dynamic response = new ExpandoObject();
        try
        {
            string directoryName = Path.GetDirectoryName(fileName);
            string fileNameOnly = Path.GetFileName(fileName);

            // If directory name is not empty, ensure that it exists
            if (!string.IsNullOrEmpty(directoryName))
            {
                string fullDirectoryPath = Path.Combine(FolderPath, directoryName);
                if (!Directory.Exists(fullDirectoryPath))
                {
                    Directory.CreateDirectory(fullDirectoryPath);
                }
            }

            string filePath = Path.Combine(FolderPath, fileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileContent.CopyToAsync(fileStream);
            }

            response.Status = 200;
            response.Message = "File uploaded successfully!";

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Local Storage Service -> (Upload File Error): {ex.InnerException?.Message ?? ex.Message}");
            response.Status = 500;
            response.Message = ex.InnerException?.Message ?? ex.Message;
            return response;
        }
    }

    /// <summary>
    /// NOT IMPLEMENTED - NO NEED IN THIS PROJECT
    /// </summary>
    /// <param name="originalFileName"></param>
    /// <param name="newFileName"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ExpandoObject> DuplicateFileAsync(string originalFileName, string newFileName)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// NOT IMPLEMENTED - IN THIS PROJECT
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ExpandoObject> DeleteFileAsync(string fileName)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// NOT IMPLEMENTED - NO NEED ON THIS PROJECT
    /// </summary>
    /// <param name="base64File"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ExpandoObject> UploadBase64FileAsync(string base64File, string fileName)
    {
        throw new System.NotImplementedException();
    }
}