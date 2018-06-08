using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.Stealth;

namespace ConsoleApp.KeyGeneartion
{
    class DarkWallet
    {
        public static void Execute()
        {
            // Dark Wallets are not in use anymore
            var scanKey = new Key();
            var spendKey = new Key();
            BitcoinStealthAddress stealthAddress
                = new BitcoinStealthAddress(
                    scanKey: scanKey.PubKey,
                    pubKeys: new[] { spendKey.PubKey },
                    signatureCount: 1,
                    bitfield: null,
                    network: Network.Main);

            var ephemKey = new Key();
            Transaction transaction = new Transaction();
            stealthAddress.SendTo(transaction, Money.Coins(1.0m), ephemKey);
            Console.WriteLine(transaction);

            //Transaction transaction = new Transaction();
            stealthAddress.SendTo(transaction, Money.Coins(1.0m));
            Console.WriteLine(transaction);
        }
    }
}
