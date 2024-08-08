namespace Vinted.Hw.Entities
{
    public class TransactionEntity
    {
        public string TransactionLine { get; set; }
        public bool IsIgnored { get; set; }
        public TransactionDataEntity TransactionData { get; set; } // in relational data bases it allows to avoid additional
                                                                   // columns if transaction line is ignored
    }
}
