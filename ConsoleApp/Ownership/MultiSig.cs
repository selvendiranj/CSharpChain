using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin;

namespace ConsoleApp.Ownership
{
    class MultiSig
    {
        public static void Execute()
        {
            // m-of-n multisig
            // scriptPubkey  = <sigsRequired> <pubkeys…> <pubKeysCount> OP_CHECKMULTISIG
            Key bob = new Key();
            Key alice = new Key();
            Key satoshi = new Key();

            var scriptPubKey = PayToMultiSigTemplate
                                .Instance
                                .GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey });

            Console.WriteLine("scriptPubKey: " + scriptPubKey);

            // multisig scriptPubKey received a coin in a transaction
            var received = new Transaction();
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), scriptPubKey));

            // Bob and Alice agree to pay Nico 1.0 BTC
            // First they get the Coin they received from the transaction:
            // Then, with the TransactionBuilder, they create an unsigned transaction.
            Coin coin = received.Outputs.AsCoins().First();
            BitcoinAddress nico = new Key().PubKey.GetAddress(Network.Main);
            TransactionBuilder builder = new TransactionBuilder();
            Transaction unsigned = builder.AddCoins(coin)
                                          .Send(nico, Money.Coins(1.0m))
                                          .BuildTransaction(sign: false);
            // The transaction is not yet signed. Here is how Alice signs it, then Bob
            Transaction aliceSigned = builder.AddCoins(coin)
                                             .AddKeys(alice)
                                             .SignTransaction(unsigned);
            //At this line, SignTransaction(unSigned) has the identical functionality with the SignTransaction(aliceSigned).
            //It's because unsigned transaction has already been signed by Alice privateKey from above.
            Transaction bobSigned = builder.AddCoins(coin)
                                           .AddKeys(bob)
                                           .SignTransaction(aliceSigned);
            // CombineSignatures() method is needless in this case because
            // the transaction got signed properly without the CombineSignatures() method.
            Transaction fullySigned = builder.AddCoins(coin)
                                             .CombineSignatures(aliceSigned, bobSigned);
            Console.WriteLine("fullySigned: " + fullySigned);

            //a case where CombineSignatures() is required:
            TransactionBuilder builderNew = new TransactionBuilder();
            TransactionBuilder builderForAlice = new TransactionBuilder();
            TransactionBuilder builderForBob = new TransactionBuilder();

            Transaction unsignedNew = builderNew
                                        .AddCoins(coin)
                                        .Send(nico, Money.Coins(1.0m))
                                        .BuildTransaction(sign: false);

            Transaction aliceSignedNew = builderForAlice
                                        .AddCoins(coin)
                                        .AddKeys(alice)
                                        .SignTransaction(unsignedNew);

            Transaction bobSignedNew = builderForBob
                                        .AddCoins(coin)
                                        .AddKeys(bob)
                                        .SignTransaction(unsignedNew);

            //In this case, the CombineSignatures() method is essentially needed.
            Transaction fullySignedNew = builderNew
                                        .AddCoins(coin)
                                        .CombineSignatures(aliceSignedNew, bobSignedNew);
        }
    }
}
