using Microsoft.AspNetCore.Mvc;
using SIHOT.Wallet.API.Models;
using SIHOT.Wallet.API.Services;

namespace SIHOT.Wallet.API.Controllers
{
    [ApiController]
    [Route("v1")]
    public class AppleController : Controller
    {
        private readonly ApplePassService _passService;

        public AppleController(ApplePassService passService)
        {
            _passService = passService;
        }

        [HttpPost("guestpass")]
        public IActionResult PostCreateGuestPass([FromBody] GuestPass pass)
        {
            try
            {
                Console.WriteLine($"PostCreateGuestPass: {pass.SerialNumber}");
                var passBytes = _passService.CreateGuestPassFile(pass);
                return File(passBytes, "application/vnd.apple.pkpass", "guestPass.pkpass");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("guestpass")]
        public IActionResult GetGuestPass([FromQuery] GuestPass guestPass)
        {
            try
            {
                Console.WriteLine($"GetGuestPass: {guestPass.SerialNumber}");
                var passBytes = _passService.CreateGuestPassFile(guestPass);
                return File(passBytes, "application/vnd.apple.pkpass", "guestPass.pkpass");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("passes/{passTypeId}/{serialNumber}")]
        public async Task<IActionResult> GetLatestPass(string passTypeId, string serialNumber)
        {
            try
            {
                Console.WriteLine($"GetLatestPass: {passTypeId} {serialNumber}");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("devices/{deviceLibraryId}/registrations/{passTypeId}/{serialNumber}")]
        public async Task<IActionResult> RegisterDevice(
            string deviceLibraryId,
            string passTypeId,
            string serialNumber,
            [FromBody] DeviceRegistration registration)
        {
            try
            {
                Console.WriteLine($"RegisterDevice: {deviceLibraryId} {passTypeId} {serialNumber} {registration.PushToken}");

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("devices/{deviceLibraryId}/registrations/{passTypeId}/{serialNumber}")]
        public async Task<IActionResult> UnregisterDevice(
            string deviceLibraryId,
            string passTypeId,
            string serialNumber)
        {
            try
            {
                Console.WriteLine($"UnregisterDevice: {deviceLibraryId} {passTypeId} {serialNumber}");

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("devices/{deviceLibraryId}/registrations/{passTypeId}")]
        public async Task<IActionResult> GetSerialNumbers(
            string deviceLibraryId,
            string passTypeId)
        {
            try
            {
                Console.WriteLine($"GetSerialNumbers: {deviceLibraryId} {passTypeId}");

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("log")]
        public IActionResult GetLog()
        {
            try
            {
                Console.WriteLine("GetLog");

                return Ok("Log");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class DeviceRegistration
    {
        public string? PushToken { get; set; }
    }
}
