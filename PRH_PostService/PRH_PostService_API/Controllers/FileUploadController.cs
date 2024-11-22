using Application.Interfaces.Services;
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
            // Kiểm tra file null hoặc rỗng
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            // Kiểm tra định dạng file
            if (!IsImageFile(file.FileName))
                return BadRequest("Chỉ cho phép upload các file hình ảnh (.jpg, .jpeg, .png, .gif, .bmp, .webp).");

            await using var stream = file.OpenReadStream();
            var fileName = file.FileName;

            // Upload file lên Firebase và nhận URL
            var fileUrl = await _firebaseService.UploadFileAsync(stream, fileName);

            return Ok(new { Url = fileUrl });
        }

        private bool IsImageFile(string fileName)
        {
            // Các phần mở rộng file được phép
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
    }
}
