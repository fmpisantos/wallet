using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterForPushNotifications([FromBody] PushNotificationRegistration registration)
        {
            try
            {
                Console.WriteLine($"Device registered for push notifications: {registration.DeviceLibraryIdentifier}");
                return Ok("Device registered for push notifications.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("update")]
        public IActionResult UpdatePass([FromBody] PassUpdate passUpdate)
        {
            try
            {
                Console.WriteLine($"Pass updated: {passUpdate.SerialNumber}");
                return Ok("Pass update sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("revoke")]
        public IActionResult RevokePass([FromBody] PassRevokeRequest passRevokeRequest)
        {
            try
            {
                Console.WriteLine($"Pass revoked: {passRevokeRequest.SerialNumber}");
                return Ok("Pass has been revoked.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("sendnotification")]
        public IActionResult SendPushNotification([FromBody] PushNotification pushNotification)
        {
            try
            {
                Console.WriteLine($"Push notification sent: {pushNotification.DeviceLibraryIdentifier}");
                return Ok("Push notification sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
    public class PushNotificationRegistration
    {
        public string? PassTypeIdentifier { get; set; }
        public string? DeviceLibraryIdentifier { get; set; }
        public string? PushToken { get; set; }
    }

    public class PassUpdate
    {
        public string? PassTypeIdentifier { get; set; }
        public string? SerialNumber { get; set; }
        public List<string>? UpdatedFields { get; set; }
    }

    public class PassRevokeRequest
    {
        public string? PassTypeIdentifier { get; set; }
        public string? SerialNumber { get; set; }
    }

    public class PushNotification
    {
        public string? DeviceLibraryIdentifier { get; set; }
        public string? PushToken { get; set; }
        public string? Message { get; set; }
        public string? Action { get; set; }
    }
}
