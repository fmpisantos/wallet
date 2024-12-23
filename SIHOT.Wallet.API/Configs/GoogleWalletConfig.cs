using Google.Apis.Walletobjects.v1.Data;
using Newtonsoft.Json;

namespace SIHOT.Wallet.API.Configs
{
    public class GoogleWalletConfig : WalletConfig<GoogleWalletConfig>
    {
        internal static string Credentials = "";

        internal static readonly string IssuerId = "3388000000022735188";
        internal static readonly string ClassId = "SIHOTGUESTPASS";
        internal static readonly string OutUrl = "https://pay.google.com/gp/v/save/";
        internal static readonly string WalletUrl = "https://walletobjects.googleapis.com/walletobjects/v1/genericClass://pay.google/save/";
        internal static readonly string PassUrl = "https://walletobjects.googleapis.com/walletobjects/v1/genericObject";
        internal static readonly string Origin = "www.sihot.com";
        internal static readonly Image Logo = new Image
        {
            SourceUri = new ImageUri
            {
                Uri = "https://sihot.com/wp-content/uploads/2020/08/sihot_flosse.jpg"
            },
            ContentDescription = new LocalizedString
            {
                DefaultValue = new TranslatedString
                {
                    Language = "en-US",
                    Value = "SIHOT"
                }
            }
        };
        internal static readonly Image WideLogo = new Image
        {
            SourceUri = new ImageUri
            {
                Uri = "https://i0.wp.com/hubos.com/wp-content/uploads/2021/08/Sihot.png?fit=300%2C300&ssl=1"
            },
            ContentDescription = new LocalizedString
            {
                DefaultValue = new TranslatedString
                {
                    Language = "en-US",
                    Value = "SIHOT"
                }
            }
        };

        public GoogleWalletConfig() { }

        internal override void LoadPlatformSpecificSecrets(IConfiguration configuration, string platform)
        {
            Credentials = JsonConvert.SerializeObject(new GoogleCredentials(PrivateKeyId, PrivateKey));
        }

        public class GoogleCredentials
        {
            public string type = "service_account";
            public string project_id = "light-tribute-434509-t8";
            public string client_email = "sihot-wallet-test@light-tribute-434509-t8.iam.gserviceaccount.com";
            public string client_id = "109356315787837975365";
            public string auth_uri = "https://accounts.google.com/o/oauth2/auth";
            public string token_uri = "https://oauth2.googleapis.com/token";
            public string auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs";
            public string client_x509_cert_url = "https://www.googleapis.com/robot/v1/metadata/x509/sihot-wallet-test%40light-tribute-434509-t8.iam.gserviceaccount.com";
            public string universe_domain = "googleapis.com";
            public readonly string private_key_id;
            public readonly string private_key;

            public GoogleCredentials(string privateKeyId, string privateKey)
            {
                private_key_id = privateKeyId;
                private_key = privateKey;
            }
        }
    }
}
