using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIHOT.Wallet.API.Models;
using SIHOT.Wallet.API.Services;

namespace SIHOT.Wallet.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppleController : Controller
    {
        private readonly ApplePassService service;

        public AppleController(ApplePassService passService)
        {
            service = passService;
        }

        [HttpPost]
        [Route("guestpass")]
        public IActionResult PostCreateGuestPass([FromBody] GuestPass pass)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, $"{pass.SerialNumber}/guestPass.pkpass");
            try
            {
                Response.Headers.ContentType = "application/vnd.apple.pkpass";

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "guestPass.pkpass",
                    Inline = false,
                };

                Response.Headers.Append("Content-Disposition", cd.ToString());

                return File(service.CreateGuestPassFile(pass), "application/vnd.apple.pkpass", "yourpass.pkpass");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Route("guestpass")]
        public IActionResult GetGuestPass([FromQuery] GuestPass guestPass)
        {
            try
            {
                var passBytes = service.CreateGuestPassFile(guestPass);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "guestPass.pkpass",
                    Inline = false,
                };

                Response.Headers.Append("Content-Disposition", cd.ToString());
                Response.Headers.ContentType = "application/vnd.apple.pkpass";

                return File(passBytes, "application/vnd.apple.pkpass", "guestPass.pkpass");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
