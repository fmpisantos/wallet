using System.Text;
using SIHOT.Wallet.API.Configs;
using SIHOT.Wallet.API.Models;
using SIHOT.Wallet.API.Models.Apple;

namespace SIHOT.Wallet.API.Services
{
    public class ApplePassService
    {
        public byte[] CreateGuestPassFile(GuestPass guestPass)
        {
            ApplePassBuilder builder = CreateBuilder(guestPass);

            return builder.GeneratePkPass();
        }

        public string CreateGuestPassUrl(GuestPass guestPass)
        {
            string outputDir = Path.Combine(Environment.CurrentDirectory, guestPass.SerialNumber);
            string pkPassFileName = "guestPass.pkpass";

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            ApplePassBuilder builder = CreateBuilder(guestPass);

            string filePath = Path.Combine(outputDir, pkPassFileName);
            File.WriteAllBytes(filePath, builder.GeneratePkPass());
            return filePath;
        }

        private async Task<string> GenerateAuthToken(string serialNumber)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var dataToHash = $"{serialNumber}:{timestamp}:{AppleWalletConfig.TeamIdentifier}";

            using var sha = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
            return Convert.ToBase64String(hashBytes);
        }

        private ApplePassBuilder CreateBuilder(GuestPass guestPass)
        {
            string description = "Guest pass";
            string logoText = "";
            string ForegroundColor = AppleWalletConfig.ForegroundColor;
            string BackgroundColor = AppleWalletConfig.BackgroundColor;
            string LabelColor = AppleWalletConfig.LabelColor;
            string assetsSource = Path.Combine(Environment.CurrentDirectory, "assets");

            Pass pass = new Pass
            {
                PassTypeIdentifier = AppleWalletConfig.GuestPassTypeIdentifier,
                SerialNumber = guestPass.SerialNumber,
                TeamIdentifier = AppleWalletConfig.TeamIdentifier,
                OrganizationName = AppleWalletConfig.OrganizationName,
                WebServiceURL = AppleWalletConfig.WebServiceURL,
                AuthenticationToken = await GenerateAuthToken(guestPass.SerialNumber).Result,
                Description = description,
                LogoText = logoText,
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
                LabelColor = LabelColor,
                Generic = new Generic
                {
                    PrimaryFields = new List<Field>
                    {
                        new Field { Key = "primary", Label = "Guest pass", Value = $"{guestPass.FirstName} {guestPass.LastName}" }
                    },
                    SecondaryFields = new List<Field>
                    {
                            new Field { Key = "secondary0", Label = "Hotel", Value = guestPass.Hotel },
                            new Field { Key = "secondary1", Label = "Reservation", Value = guestPass.RESUNR.ToString() },
                            new Field { Key = "secondary3", Label = "Room", Value = guestPass.Room.ToString() }
                    },
                    AuxiliaryFields = new List<Field>
                    {
                            new Field { Key = "auxiliary0", Label = "From", Value = guestPass.From },
                            new Field { Key = "auxiliary1", Label = "To", Value = guestPass.To },
                    },
                    BackFields = new List<Field>
                    {
                            new Field { Key = "back0", Label = "HotelName", Value = guestPass.HotelName },
                            new Field { Key = "back1", Label = "Hotel number", Value = guestPass.Hotel },
                            new Field { Key = "back2", Label = "Address", Value = guestPass.Address },
                            new Field { Key = "back3", Label = "Reservation", Value = guestPass.RESUNR.ToString()},
                            new Field { Key = "back4", Label = "Room", Value = guestPass.Room.ToString()},
                            new Field { Key = "back5", Label = "From", Value = guestPass.From },
                            new Field { Key = "back6", Label = "To", Value = guestPass.To },
                            new Field { Key = "back8", Label = "FirstName", Value = guestPass.FirstName },
                            new Field { Key = "back9", Label = "LastName", Value = guestPass.LastName }
                    },
                },
                Barcodes = new List<Barcode>
                {
                    new Barcode
                    {
                        Format = "PKBarcodeFormatQR",
                        Message = guestPass.Qrcodecontent,
                        MessageEncoding = "iso-8859-1"
                    }
                }

            };

            return new ApplePassBuilder(pass, assetsSource);
        }
    }
}
