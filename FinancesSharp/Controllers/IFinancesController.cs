using FinancesSharp.Models;

namespace FinancesSharp.Controllers
{
    public interface IFinancesController
    {
        FinanceDb Db { get; }
    }
}
