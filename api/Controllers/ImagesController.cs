using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        public FileContentResult Get()
        {
            _generator = new QRCodeGenerator();
            QRCodeData qrCodeData = _generator.CreateQrCode("sms:+14252509682", QRCodeGenerator.ECCLevel.Q);
            _qrCode = new PngByteQRCode(qrCodeData);

            return File(_qrCode.GetGraphic(10), "image/png");
           
        }
    }
}
