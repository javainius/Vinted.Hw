namespace Vinted.Hw.Models
{
    public class TransactionDataModel
    {
        public DateOnly Date { get; set; }
        public PackageSize PackageSize { get; set; }
        public string CarrierCode { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
    }
}
