using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin;

namespace ConsoleApp.Assets
{
    class ProofOfBurn
    {
        public static void Execute()
        {
            // here is an example of Proof of Burn.
            var alice = new Key();

            //Giving some money to alice
            var init = new Transaction()
            {
                Outputs =
                    {
                        new TxOut(Money.Coins(1.0m), alice),
                    }
            };

            var coin = init.Outputs.AsCoins().First();

            //Burning the coin
            var burn = new Transaction();
            burn.Inputs.Add(new TxIn(coin.Outpoint)
            {
                ScriptSig = coin.ScriptPubKey
            }); //Spend the previous coin

            var message = "Burnt for \"Alice Bakery\"";
            var opReturn = TxNullDataTemplate.Instance
                                             .GenerateScriptPubKey(Encoding.UTF8.GetBytes(message));
            burn.Outputs.Add(new TxOut(Money.Coins(1.0m), opReturn));
            burn.Sign(alice, false);

            Console.WriteLine("burn: " + burn);
            /*
             Once in the Blockchain, this transaction is undeniable proof that Alice invested money for her bakery.
             The Coin with ScriptPubKey OP_RETURN 4275726e7420666f722022416c6963652042616b65727922
             do not have any way to be spent, so those coins are lost forever.
             */
        }
    }
}
