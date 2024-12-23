namespace SIHOT.Wallet.API.Models
{
    public class GuestPass : BasePass
    {
        public required long PCIID { get; set; }
        public required long RESNO { get; set; }
        public required long RESUNR { get; set; }
        public required long GRPNO { get; set; }
        public required short Room { get; set; }
        public required string From { get; set; }
        public required string To { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Qrcodecontent { get; set; }
        protected override string GetHashInput() => $"{base.GetHashInput()}_{PCIID}_{RESNO}_{RESUNR}_{GRPNO}";
    }
}
