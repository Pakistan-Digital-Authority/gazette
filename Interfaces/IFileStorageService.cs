using System.Dynamic;

namespace gop.Interfaces;

/// <summary>
/// To storage files - on any bucket
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// To get the file URL
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="timeOutInMinutes"></param>
    /// <returns></returns>
    Task<ExpandoObject> GetFileUrlAsync(string fileName, int timeOutInMinutes = 30);

    /// <summary>
    /// To upload file
    /// </summary>
    /// <param name="fileContent"></param>
    /// <param name="fileName"></param>
    /// <param name="fileContentType"></param>
    /// <returns></returns>
    Task<ExpandoObject> UploadFileAsync(IFormFile fileContent, string fileName, string fileContentType = null);

    /// <summary>
    /// To duplicate the file on the storage
    /// </summary>
    /// <param name="originalFileName"></param>
    /// <param name="newFileName"></param>
    /// <returns></returns>
    Task<ExpandoObject> DuplicateFileAsync(string originalFileName, string newFileName);

    /// <summary>
    /// To delete/remove the file from the bucket
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<ExpandoObject> DeleteFileAsync(string fileName);

    /// <summary>
    /// To uploade base64 file
    /// </summary>
    /// <param name="base64File"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<ExpandoObject> UploadBase64FileAsync(string base64File, string fileName);
}