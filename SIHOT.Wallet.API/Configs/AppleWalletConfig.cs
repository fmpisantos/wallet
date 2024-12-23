namespace SIHOT.Wallet.API.Configs
{
    public class AppleWalletConfig : WalletConfig<AppleWalletConfig>
    {
        internal static string KeyPassphrase = "";
        internal static readonly string WWDRPath = "certs/WWDR.pem";
        internal static readonly string GuestPassTypeIdentifier = "pass.com.fm.santos.example.generic";
        internal static readonly string TeamIdentifier = "C5S4NV2VC6";

        public AppleWalletConfig() { }

        internal override void LoadPlatformSpecificSecrets(IConfiguration configuration, string platform)
        {
            KeyPassphrase = configuration[$"{platform}:key_passphrase"] ?? "";
        }
    }
}
