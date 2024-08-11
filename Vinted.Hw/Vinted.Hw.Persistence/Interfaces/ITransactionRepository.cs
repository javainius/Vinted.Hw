using Vinted.Hw.Entities;

namespace Vinted.Hw.Persistence.Interfaces
{
    public interface ITransactionRepository
    {
        public TransactionEntity SaveTransaction(TransactionEntity transaction);
        public List<TransactionEntity> GetTransactions();
    }
}
