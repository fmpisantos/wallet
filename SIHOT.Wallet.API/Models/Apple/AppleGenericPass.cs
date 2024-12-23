using Newtonsoft.Json;

namespace SIHOT.Wallet.API.Models
{
    public class Pass
    {
        [JsonProperty("formatVersion")]
        public int FormatVersion { get; set; } = 1;

        [JsonProperty("passTypeIdentifier")]
        public string? PassTypeIdentifier { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; } = "";

        [JsonProperty("teamIdentifier")]
        public string TeamIdentifier { get; set; } = "";

        [JsonProperty("organizationName")]
        public string OrganizationName { get; set; } = "";

        [JsonProperty("webServiceURL")]
        public string WebServiceURL { get; set; } = "";

        [JsonProperty("authenticationToken")]
        public string AuthenticationToken { get; set; } = "";

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("logoText")]
        public string LogoText { get; set; } = "";

        [JsonProperty("foregroundColor")]
        public string ForegroundColor { get; set; } = "";

        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; } = "";

        [JsonProperty("labelColor")]
        public string LabelColor { get; set; } = "";

        [JsonProperty("generic")]
        public Generic? Generic { get; set; }

        [JsonProperty("barcodes")]
        public List<Barcode>? Barcodes { get; set; }
    }

    public class Generic
    {
        [JsonProperty("primaryFields")]
        public List<Field>? PrimaryFields { get; set; }

        [JsonProperty("secondaryFields")]
        public List<Field>? SecondaryFields { get; set; }

        [JsonProperty("auxiliaryFields")]
        public List<Field>? AuxiliaryFields { get; set; }

        [JsonProperty("backFields")]
        public List<Field>? BackFields { get; set; }
    }

    public class Field
    {
        [JsonProperty("key")]
        public string Key { get; set; } = "";

        [JsonProperty("label")]
        public string Label { get; set; } = "";

        [JsonProperty("value")]
        public string Value { get; set; } = "";
    }

    public class Barcode
    {
        [JsonProperty("format")]
        public string Format { get; set; } = "";

        [JsonProperty("message")]
        public string Message { get; set; } = "";

        [JsonProperty("messageEncoding")]
        public string MessageEncoding { get; set; } = "";
    }
}
