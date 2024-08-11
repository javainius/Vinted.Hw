using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vinted.Hw.Entities;
using Vinted.Hw.Persistence.Interfaces;

namespace Vinted.Hw.Persistence
{
    public class TransactionRepository : BaseRepository<TransactionEntity>, ITransactionRepository
    {
        public TransactionRepository(string filePath) : base(filePath) {}

        public TransactionEntity SaveTransaction(TransactionEntity transaction)
        {
            List<TransactionEntity> transactions = GetEntities();
            transactions.Add(transaction);
            SaveEntities(transactions);
            return transaction;
        }

        public List<TransactionEntity> GetTransactions() => GetEntities();
    }
}
