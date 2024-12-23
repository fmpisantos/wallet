namespace SIHOT.Wallet.API.Configs
{
    public abstract class WalletConfig<T> where T : WalletConfig<T>, new()
    {
        internal static string PrivateKeyId = "";
        internal static string PrivateKey = "";

        internal static readonly string OrganizationName = "SIHOT";
        internal static readonly string HexBackgroundColor = "#535c6a";
        internal static readonly string BackgroundColor = "rgb(83, 92, 106)";
        internal static readonly string ForegroundColor = "rgb(255, 255, 255)";
        internal static readonly string LabelColor = "rgb(255, 255, 255)";

        internal static void LoadSecrets(IConfiguration configuration, string platform)
        {
            PrivateKeyId = configuration[$"{platform}:private_key_id"] ?? "";
            PrivateKey = configuration[$"{platform}:private_key"] ?? "";
            var instance = new T();
            instance.LoadPlatformSpecificSecrets(configuration, platform);
        }

        internal virtual void LoadPlatformSpecificSecrets(IConfiguration configuration, string platform) { }
    }
}
