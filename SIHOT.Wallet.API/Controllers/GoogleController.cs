using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Walletobjects.v1;
using Microsoft.AspNetCore.Mvc;
using SIHOT.Wallet.API.Configs;
using SIHOT.Wallet.API.Models.Google;
using SIHOT.Wallet.API.Services;

namespace SIHOT.Wallet.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GoogleController : ControllerBase
    {
        private GooglePassService googlePassService;

        public GoogleController()
        {
            ServiceAccountCredential credentials = (ServiceAccountCredential)GoogleCredential
                            .FromJson(GoogleWalletConfig.Credentials)
                            .CreateScoped(new List<string>
                            {
                            WalletobjectsService.Scope.WalletObjectIssuer
                            })
                            .UnderlyingCredential;
            WalletobjectsService service = new WalletobjectsService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials
                });
            this.googlePassService = new GooglePassService(credentials, service);
        }

        [HttpPost]
        [Route("guestpass")]
        public async Task<IActionResult> CreateGuestPass([FromBody] GoogleGuestPass passObjectModel)
        {
            return Ok(googlePassService.BuildPass(GoogleWalletConfig.IssuerId, GoogleWalletConfig.ClassId, passObjectModel, false));
        }

        [HttpGet]
        [Route("guestpass")]
        public async Task<IActionResult> GetGuestPass([FromQuery] GoogleGuestPass passObjectModel)
        {
            return Redirect(googlePassService.BuildPass(GoogleWalletConfig.IssuerId, GoogleWalletConfig.ClassId, passObjectModel, false));
        }
    }
}
