using System.IdentityModel.Tokens.Jwt;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Walletobjects.v1;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIHOT.Wallet.API.Configs;
using SIHOT.Wallet.API.Models.Google;

namespace SIHOT.Wallet.API.Services
{
    public class GooglePassService
    {
        private ServiceAccountCredential credentials;
        private WalletobjectsService service;

        public GooglePassService(ServiceAccountCredential credentials, WalletobjectsService service)
        {
            this.credentials = credentials;
            this.service = service;
        }

        private JObject CreateClassId(string issuerId, string classSuffix, GoogleGuestPass obj, Boolean update)
        {
            string classId = $"{issuerId}.{classSuffix}";
            Stream responseStream = service.Genericclass
                .Get(classId)
                .ExecuteAsStream();

            StreamReader responseReader = new StreamReader(responseStream);
            JObject jsonResponse = JObject.Parse(responseReader.ReadToEnd());

            if (!jsonResponse.ContainsKey("error"))
            {
                if (update)
                {
                    responseStream = service.Genericclass
                        .Update(obj.CreateGenericClass(issuerId, classSuffix), classId)
                        .ExecuteAsStream();

                    responseReader = new StreamReader(responseStream);
                    jsonResponse = JObject.Parse(responseReader.ReadToEnd());
                }
                return jsonResponse;
            }
            else if (jsonResponse?["error"]?.Value<int>("code") != 404)
            {
                Console.WriteLine(jsonResponse?.ToString());
            }

            responseStream = service.Genericclass
                .Insert(obj.CreateGenericClass(issuerId, classSuffix))
                .ExecuteAsStream();

            responseReader = new StreamReader(responseStream);
            jsonResponse = JObject.Parse(responseReader.ReadToEnd());

            return jsonResponse;
        }

        private JObject CreateObject(string issuerId, string classSuffix, GoogleGuestPass obj, Boolean update)
        {
            string objId = $"{issuerId}.{obj.SerialNumber}";
            string classId = $"{issuerId}.{classSuffix}";
            Stream responseStream = service.Genericobject
                .Get(objId)
                .ExecuteAsStream();

            StreamReader responseReader = new StreamReader(responseStream);
            JObject jsonResponse = JObject.Parse(responseReader.ReadToEnd());

            if (!jsonResponse.ContainsKey("error"))
            {
                if (update || jsonResponse.GetValue("classId")?.ToString() != classId)
                {
                    responseStream = service.Genericobject
                        .Update(obj.CreateGenericObject(issuerId, classId), objId)
                        .ExecuteAsStream();
                    responseReader = new StreamReader(responseStream);
                    jsonResponse = JObject.Parse(responseReader.ReadToEnd());
                }
                return jsonResponse;
            }
            else if (jsonResponse?["error"]?.Value<int>("code") != 404)
            {
                Console.WriteLine(jsonResponse?.ToString());
            }

            responseStream = service.Genericobject
                .Insert(obj.CreateGenericObject(issuerId, classId))
                .ExecuteAsStream();

            responseReader = new StreamReader(responseStream);
            jsonResponse = JObject.Parse(responseReader.ReadToEnd());

            return jsonResponse;
        }

        private string GetToken(string issuerId, string classSuffix, GoogleGuestPass obj, Boolean update)
        {
            JObject jwtPayload = JObject.Parse(JsonConvert.SerializeObject(new
            {
                iss = credentials?.Id,
                aud = "google",
                origins = new List<string>
                  {
                      GoogleWalletConfig.Origin
                  },
                typ = "savetowallet",
                payload = JObject.Parse(JsonConvert.SerializeObject(new
                {
                    genericClasses = new List<JObject>
                    {
                        CreateClassId(issuerId, classSuffix, obj, update)
                    },
                    genericObjects = new List<JObject>
                    {
                        CreateObject(issuerId, classSuffix, obj, update)
                    }
                }))
            }));

            string temp = jwtPayload.ToString();

            JwtPayload claims = JwtPayload.Deserialize(jwtPayload.ToString());

            RsaSecurityKey key = new RsaSecurityKey(credentials?.Key);
            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            JwtSecurityToken jwt = new JwtSecurityToken(new JwtHeader(signingCredentials), claims);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string BuildPass(string issuerId, string classId, GoogleGuestPass obj, Boolean update)
        {
            return $"https://pay.google.com/gp/v/save/{GetToken(issuerId, classId, obj, update)}";
        }
    }
}

