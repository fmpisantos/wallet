using Newtonsoft.Json;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using SIHOT.Wallet.API.Configs;
using SIHOT.Wallet.API.Services;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace SIHOT.Wallet.API.Models.Apple
{
    public class ApplePassBuilder
    {
        private readonly Pass _pass;
        private readonly string _assetsSource;

        public ApplePassBuilder(Pass pass, string assetsSource)
        {
            _pass = pass ?? throw new ArgumentNullException(nameof(pass));
            _assetsSource = assetsSource ?? throw new ArgumentNullException(nameof(assetsSource));
        }

        public byte[] GeneratePkPass()
        {
            using (MemoryStream zipMemoryStream = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
                {
                    Dictionary<string, string> manifest = AddAssetsToZip(zipArchive);
                    manifest.Add("pass.json", AddPassJsonToZip(zipArchive));
                    byte[] manifestData = GenerateManifest(zipArchive, manifest);
                    GenerateSignature(zipArchive, manifestData);
                }
                return zipMemoryStream.ToArray();
            }
        }

        private Dictionary<string, string> AddAssetsToZip(ZipArchive zipArchive)
        {
            string[] files = Directory.GetFiles(_assetsSource);
            var hashes = new Dictionary<string, string>();
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                ZipArchiveEntry entry = zipArchive.CreateEntry(fileName);
                using (SHA1 sha1 = SHA1.Create())
                using (Stream entryStream = entry.Open())
                using (Stream fileStream = File.OpenRead(file))
                {
                    fileStream.CopyTo(entryStream);
                    fileStream.Position = 0;
                    byte[] hash = sha1.ComputeHash(fileStream);
                    string fileHash = BitConverter.ToString(hash).Replace("-", "").ToLower();
                    hashes.Add(fileName, fileHash);
                }
            }

            return hashes;
        }

        private string AddPassJsonToZip(ZipArchive zipArchive)
        {
            string passJson = JsonConvert.SerializeObject(_pass, Formatting.Indented);
            ZipArchiveEntry entry = zipArchive.CreateEntry("pass.json");
            using (SHA1 sha1 = SHA1.Create())
            using (StreamWriter writer = new StreamWriter(entry.Open()))
            {
                writer.Write(passJson);
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(passJson));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private byte[] GenerateManifest(ZipArchive zipArchive, Dictionary<string, string> files)
        {
            byte[] manifestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(files, Formatting.Indented));
            ZipArchiveEntry manifestEntry = zipArchive.CreateEntry("manifest.json");

            using (Stream entryStream = manifestEntry.Open())
            {
                entryStream.Write(manifestBytes, 0, manifestBytes.Length);
            }
            return manifestBytes;
        }

        private void GenerateSignature(ZipArchive zipArchive, byte[] manifestData)
        {
            X509Certificate signerCert = LoadCertificateFromString(AppleWalletConfig.PrivateKeyId);
            AsymmetricKeyParameter signerKey = LoadPrivateKeyFromString(AppleWalletConfig.PrivateKey, AppleWalletConfig.KeyPassphrase);
            X509Certificate additionalCert = LoadCertificate(AppleWalletConfig.WWDRPath);

            CmsProcessableByteArray msg = new CmsProcessableByteArray(manifestData);
            CmsSignedDataGenerator signer = new CmsSignedDataGenerator();
            signer.AddSigner(signerKey, signerCert, CmsSignedDataGenerator.DigestSha256);

            List<X509Certificate> certList = new List<X509Certificate> { signerCert, additionalCert };
            signer.AddCertificates(X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(certList)));

            CmsSignedData signedData = signer.Generate(msg, true);

            ZipArchiveEntry manifestEntry = zipArchive.CreateEntry("signature");

            byte[] signature = signedData.GetEncoded();

            using (Stream entryStream = manifestEntry.Open())
            {
                entryStream.Write(signature, 0, signature.Length);
            }
        }

        private X509Certificate LoadCertificate(string path)
        {
            using (var fileStream = File.OpenRead(path))
            {
                return LoadCertificate(fileStream);
            }
        }

        private X509Certificate LoadCertificateFromString(string certificateData)
        {
            int startIndex = certificateData.IndexOf("-----BEGIN CERTIFICATE-----");
            if (startIndex == -1)
            {
                throw new ArgumentException("Invalid certificate format: BEGIN CERTIFICATE marker not found");
            }

            startIndex += "-----BEGIN CERTIFICATE-----".Length;
            int endIndex = certificateData.IndexOf("-----END CERTIFICATE-----", startIndex);
            if (endIndex == -1)
            {
                throw new ArgumentException("Invalid certificate format: END CERTIFICATE marker not found");
            }

            string base64Data = certificateData.Substring(startIndex, endIndex - startIndex)
                .Replace("\n", "")
                .Replace("\r", "")
                .Trim();

            byte[] certBytes = Convert.FromBase64String(base64Data);
            using (var memoryStream = new MemoryStream(certBytes))
            {
                return LoadCertificate(memoryStream);
            }
        }

        private X509Certificate LoadCertificate(Stream stream){
                var certParser = new X509CertificateParser();
                return certParser.ReadCertificate(stream);
        }

        private AsymmetricKeyParameter LoadPrivateKey(string path, string passphrase)
        {
            using (var reader = File.OpenText(path))
            {
                return LoadPrivateKey(reader, passphrase);
            }
        }

        private AsymmetricKeyParameter LoadPrivateKeyFromString(string privateKeyData, string passphrase)
        {
            using (var reader = new StringReader(privateKeyData))
            {
                return LoadPrivateKey(reader, passphrase);
            }
        }

        private AsymmetricKeyParameter LoadPrivateKey(TextReader reader, string passphrase)
        {
                var pemReader = new PemReader(reader, new PasswordFinder(passphrase));
                object keyObject = pemReader.ReadObject();

                if (keyObject is AsymmetricCipherKeyPair keyPair)
                {
                    var rsaPrivateParams = keyPair.Private as RsaPrivateCrtKeyParameters;
                    if (rsaPrivateParams != null)
                    {
                        return rsaPrivateParams;
                    }
                    else
                    {
                        throw new InvalidCastException("Private key is not of type RsaPrivateCrtKeyParameters");
                    }
                }
                else if (keyObject is AsymmetricKeyParameter keyParam)
                {
                    return keyParam;
                }
                else
                {
                    throw new InvalidCastException("Unknown key object type");
                }
        }

        private class PasswordFinder : IPasswordFinder
        {
            private readonly string _password;

            public PasswordFinder(string password)
            {
                _password = password;
            }

            public char[] GetPassword()
            {
                return _password.ToCharArray();
            }
        }
    }
}
