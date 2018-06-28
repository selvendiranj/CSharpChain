using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Wallet
{
    class Rules
    {
        public static void Execute()
        {
            // A Bitcoin wallet must do the following:
            /*
            1. Generate addresses.
            2. Recognize transactions spent to these addresses.
            3. Detect transactions, those are spending from these addresses.
            4. Show the history of the transactions involving this wallet.
            5. Handle reorgs.
            6. Handle conflicts.
            7. Dynamically calculate transaction fees.
            8. Build and sign transactions.
            9. Broadcast transactions.
            */
        }
    }
}
