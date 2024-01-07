using Microsoft.AspNetCore.Mvc;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.IO;
using System.Threading.Tasks;

[Route("api/images")]
[ApiController]
public class ImagesController : ControllerBase
{
    [HttpPost("grayscale")]
    public async Task<IActionResult> GrayscaleImage()
    {
        try
        {
            // Read the image from the request
            using (var memoryStream = new MemoryStream())
            {
                await Request.Body.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Process the image to grayscale using OpenCV
                using (var image = new Mat())
                {
                    CvInvoke.Imdecode(memoryStream.ToArray(), ImreadModes.Color, image);

                    if (image.IsEmpty)
                    {
                        return BadRequest("Invalid image format");
                    }

                    CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Gray);

                    byte[] processedImageData = CvInvoke.Imencode(".jpg", image);

                    // Return the processed image
                    return File(processedImageData, "image/jpeg");

                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}