using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private static QRCodeGenerator _generator;
        private PngByteQRCode _qrCode;

        // GET api/values
        [HttpGet]
        public FileContentResult Get([FromQuery]string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                number = "19132706063";
            }

            _generator = new QRCodeGenerator();
            QRCodeData qrCodeData = _generator.CreateQrCode($"sms:+{number}?body=Hi", QRCodeGenerator.ECCLevel.Q);
            _qrCode = new PngByteQRCode(qrCodeData);

            return File(_qrCode.GetGraphic(10), "image/png");
           
        }
    }
}
