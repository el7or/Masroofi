using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.ViewModels.ParentWalletTransactions
{
    public class UpdateChargeByCreditCardViewModel
    {
        public string ParentWalletTransactionId { get; set; }
        public string TransactionReference { get; set; }
        public string TransactionDataJson { get; set; }
    }
}
