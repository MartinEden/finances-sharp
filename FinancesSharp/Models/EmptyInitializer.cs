using System.Data.Entity;

namespace FinancesSharp.Models
{
    public class EmptyInitializer : IDatabaseInitializer<FinanceDb>
    {
        public void InitializeDatabase(FinanceDb context)
        {
            //do nothing
        }
    }
}