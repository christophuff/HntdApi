using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HntdApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public ImageController(IConfiguration config)
    {
        _config = config;
        _httpClient = new HttpClient();
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided");
        }

        try
        {
            var keyId = _config["B2:KeyId"];
            var appKey = _config["B2:ApplicationKey"];
            var bucketId = _config["B2:BucketId"];
            var bucketName = _config["B2:BucketName"];

            if (string.IsNullOrEmpty(keyId) || string.IsNullOrEmpty(appKey))
            {
                Console.WriteLine("B2 credentials missing!");
                return StatusCode(500, "B2 credentials not configured");
            }

            // Step 1: Authorize with B2
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{keyId}:{appKey}"));
            
            var authRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.backblazeb2.com/b2api/v2/b2_authorize_account");
            authRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);
            
            var authResponse = await _httpClient.SendAsync(authRequest);
            var authContent = await authResponse.Content.ReadAsStringAsync();

            if (!authResponse.IsSuccessStatusCode)
            {
                return StatusCode(500, $"B2 auth failed: {authContent}");
            }

            var authData = JsonSerializer.Deserialize<JsonElement>(authContent);
            var apiUrl = authData.GetProperty("apiUrl").GetString();
            var authToken = authData.GetProperty("authorizationToken").GetString();

            // Step 2: Get upload URL
            var getUploadUrlRequest = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}/b2api/v2/b2_get_upload_url");
            getUploadUrlRequest.Headers.TryAddWithoutValidation("Authorization", authToken);
            getUploadUrlRequest.Content = new StringContent(
                JsonSerializer.Serialize(new { bucketId }),
                Encoding.UTF8,
                "application/json"
            );

            var uploadUrlResponse = await _httpClient.SendAsync(getUploadUrlRequest);
            var uploadUrlContent = await uploadUrlResponse.Content.ReadAsStringAsync();

            if (!uploadUrlResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get upload URL failed: {uploadUrlContent}");
                return StatusCode(500, $"Failed to get upload URL: {uploadUrlContent}");
            }

            var uploadUrlData = JsonSerializer.Deserialize<JsonElement>(uploadUrlContent);
            var uploadUrl = uploadUrlData.GetProperty("uploadUrl").GetString();
            var uploadAuthToken = uploadUrlData.GetProperty("authorizationToken").GetString();

            // Step 3: Upload file
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            var sha1 = Convert.ToHexString(System.Security.Cryptography.SHA1.HashData(fileBytes)).ToLower();

            var uploadRequest = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
            uploadRequest.Headers.TryAddWithoutValidation("Authorization", uploadAuthToken);
            uploadRequest.Headers.Add("X-Bz-File-Name", fileName);
            uploadRequest.Headers.Add("X-Bz-Content-Sha1", sha1);
            uploadRequest.Content = new ByteArrayContent(fileBytes);
            uploadRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            var uploadResponse = await _httpClient.SendAsync(uploadRequest);
            var uploadContent = await uploadResponse.Content.ReadAsStringAsync();
            
            if (!uploadResponse.IsSuccessStatusCode)
            {
                return StatusCode(500, $"Failed to upload to B2: {uploadContent}");
            }

            var publicUrl = $"https://f005.backblazeb2.com/file/{bucketName}/{fileName}";
            
            return Ok(new { url = publicUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Upload failed: {ex.Message}");
        }
    }
}