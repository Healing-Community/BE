using Application.Services;
using Microsoft.AspNetCore.Mvc;


namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly FirebaseService _firebaseService;

        public FileUploadController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            await using var stream = file.OpenReadStream();
            var fileName = file.FileName;

            // Upload file lên Firebase và nhận URL
            var fileUrl = await _firebaseService.UploadFileAsync(stream, fileName);

            return Ok(new { Url = fileUrl });
        }
    }
}
