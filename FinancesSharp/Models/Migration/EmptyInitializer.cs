using System.Data.Entity;

namespace FinancesSharp.Models.Migration
{
    public class EmptyInitializer : IDatabaseInitializer<FinanceDb>
    {
        public void InitializeDatabase(FinanceDb context)
        {
            //do nothing
        }
    }
}