using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace FileUploadDownload.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpPost]
        [Route("UploadFile")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
        {
            string filename = file.FileName;
            try
            {
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files");
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);
                if (System.IO.File.Exists(exactpath))
                {
                    return BadRequest("File already exists. Try requesting an update request.");
                }
                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(filename);
        }

        [HttpGet]
        [Route("DownloadFile")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);
            var provider = new FileExtensionContentTypeProvider();
            if(!provider.TryGetContentType(filename, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
            return File(bytes, contentType, Path.GetFileName(filepath));
        }

        [HttpPut]
        [Route("UpdateFile")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFile(IFormFile file, CancellationToken cancellationToken)
        {
            string filename = file.FileName;
            try
            {
                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);
                if (!System.IO.File.Exists(exactpath))
                {
                    return BadRequest("File doesn't exist.");
                }
                System.IO.File.Delete(exactpath);
                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(filename);
        }

        [HttpDelete]
        [Route("DeleteFile")]
        public IActionResult DeleteFile(string filename)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            else
            {
                return NotFound("File not found");
            }
            return Ok("File has been deleted");
        }
    }
}