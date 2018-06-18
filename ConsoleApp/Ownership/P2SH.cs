using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin;

namespace ConsoleApp.Ownership
{
    class P2SH
    {
        public static void Execute()
        {
            // This P2SH scriptPubKey represents the hash of the 
            // multi - sig script: redeemScript.Hash.ScriptPubKey
            Key bob = new Key();
            Key alice = new Key();
            Key satoshi = new Key();

            PubKey[] pubKeys = new[] { bob.PubKey, alice.PubKey, satoshi.PubKey };
            Script redeemScript = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, pubKeys);
            Script paymentScript = redeemScript.PaymentScript;

            Console.WriteLine("RedeemScript : " + redeemScript);
            Console.WriteLine("PaymentScript: " + paymentScript);
            Console.WriteLine("ScriptPubKey : " + redeemScript.Hash.ScriptPubKey);
            Console.WriteLine("WalletAddress: " + redeemScript.Hash.GetAddress(Network.Main));

            // Imagine that the multi-sig P2SH receives a coin in a transaction called received.
            Transaction received = new Transaction();
            // Pay to the script hash
            // Warning: The payment is sent to redeemScript.Hash and not to redeemScript!
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), redeemScript.Hash));
            Console.WriteLine("receivedTxn: \n" + received);

            // any 2 of 3 owner that control the multi-sig address want to spend
            // instead of creating a Coin they will need to create a ScriptCoin
            // Give the redeemScript to the coin for Transaction construction and signing
            ScriptCoin coin = received.Outputs.AsCoins().First().ToScriptCoin(redeemScript);
            Console.WriteLine("ScriptCoin: " + coin);
            
            // The rest of the code concerning transaction generation and signing is exactly the same
            // as in the previous section about native multi sig
        }
    }
}
