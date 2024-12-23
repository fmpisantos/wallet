using Google.Apis.Walletobjects.v1.Data;
using GoogleBarcode = Google.Apis.Walletobjects.v1.Data.Barcode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIHOT.Wallet.API.Configs;

namespace SIHOT.Wallet.API.Models.Google
{
    public class GoogleGuestPass : GuestPass
    {
        private GenericClass? genericClass { set; get; }
        private GenericObject? genericObject { set; get; }

        private JsonSerializerSettings excludeNulls = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private TemplateItem CreateItem(string fieldPath)
        {
            return new TemplateItem
            {
                FirstValue = new FieldSelector
                {
                    Fields = new List<FieldReference>
                {
                    new FieldReference { FieldPath = fieldPath }
                }
                }
            };
        }

        public GenericClass CreateGenericClass(string issuerId, string classSuffix)
        {
            return CreateGenericClass($"{issuerId}.{classSuffix}");
        }

        public GenericClass CreateGenericClass(string classId)
        {
            if (genericClass == null)
            {
                genericClass = new GenericClass
                {
                    Id = classId,
                    ClassTemplateInfo = new ClassTemplateInfo
                    {
                        CardTemplateOverride = new CardTemplateOverride
                        {
                            CardRowTemplateInfos = new List<CardRowTemplateInfo>
                    {
                        new CardRowTemplateInfo
                        {
                            ThreeItems = new CardRowThreeItems
                            {
                                StartItem = CreateItem("object.textModulesData['hotel']"),
                                MiddleItem = CreateItem("object.textModulesData['reservation']"),
                                EndItem = CreateItem("object.textModulesData['room']")
                            }
                        },
                        new CardRowTemplateInfo
                        {
                            TwoItems = new CardRowTwoItems
                            {
                                StartItem = CreateItem("object.textModulesData['from']"),
                                EndItem = CreateItem("object.textModulesData['to']")
                            }
                        }
                    }
                        }
                    }
                };
            }
            return genericClass;
        }

        public JObject SerializeGenericClass(string classID)
        {
            return JObject.Parse(JsonConvert.SerializeObject(CreateGenericClass(classID), excludeNulls));
        }

        public GenericObject CreateGenericObject(string issuerId, string classId)
        {
            if (genericObject == null)
            {
                genericObject = new GenericObject
                {
                    Id = $"{issuerId}.{SerialNumber}",
                    ClassId = $"{classId}",
                    State = "ACTIVE",
                    Logo = GoogleWalletConfig.Logo,
                    Header = new LocalizedString
                    {
                        DefaultValue = new TranslatedString
                        {
                            Language = "en-US",
                            Value = $"{FirstName} {LastName}"
                        }
                    },
                    Barcode = new GoogleBarcode
                    {
                        Type = "QR_CODE",
                        Value = Qrcodecontent,
                    },
                    CardTitle = new LocalizedString
                    {
                        DefaultValue = new TranslatedString
                        {
                            Language = "en-US",
                            Value = GoogleWalletConfig.OrganizationName
                        }
                    },
                    TextModulesData = AddSIHOTFields(),
                    HexBackgroundColor = GoogleWalletConfig.HexBackgroundColor
                };
            }

            return genericObject;
        }

        public JObject SerializeGenericObject(string issuerId, string classId)
        {
            return JObject.Parse(JsonConvert.SerializeObject(CreateGenericObject(issuerId, classId), excludeNulls));
        }

        private IList<TextModuleData> AddSIHOTFields()
        {
            List<TextModuleData> modData =
            [
                NewModData("hotel", HotelName),
                NewModData("reservation", RESUNR.ToString()),
                NewModData("room", Room.ToString()),
                NewModData("from", From),
                NewModData("to", To),
            ];

            return modData;
        }

        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        private TextModuleData NewModData(string name, string value)
        {
            return new TextModuleData
            {
                Id = name.ToLower(),
                Header = CapitalizeFirstLetter(name),
                Body = value
            };
        }
    }
}
