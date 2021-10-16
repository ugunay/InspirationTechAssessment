using System.Collections.Generic;

namespace InspirationTechAssessment.Persistence.Entities
{
    public class Account
    {
        public long Id { get; set; }
        public decimal Balance { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}