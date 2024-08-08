using Vinted.Hw.Entities;

namespace Vinted.Hw.Persistence
{
    public interface ITransactionRepository
    {
        public TransactionEntity SaveTransaction(TransactionEntity transaction);
        public List<TransactionEntity> GetTransactions();
    }
}
