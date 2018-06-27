using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin;
using NBitcoin.OpenAsset;

namespace ConsoleApp.Assets
{
    class UnitTests
    {
        public static void Execute()
        {
            // NBitcoin allows you either to depend on a web service, either to provide 
            // your own implementation for fetching the color of a Transaction

            // Let’s introduce two issuers: Silver and Gold. And three participants: Bob, Alice and Satoshi.
            // Let’s create a fake transaction that give some bitcoins to Silver, Gold and Satoshi.

            var gold = new Key();
            var silver = new Key();
            var goldId = gold.PubKey.ScriptPubKey.Hash.ToAssetId();
            var silverId = silver.PubKey.ScriptPubKey.Hash.ToAssetId();

            var bob = new Key();
            var alice = new Key();
            var satoshi = new Key();

            var init = new Transaction()
            {
                Outputs = { new TxOut("1.0", gold),
                            new TxOut("1.0", silver),
                            new TxOut("1.0", satoshi)}
            };

            // In NBitcoin, the summary of color transfers and issuances is described by a class called 
            // FetchColor, which will permit you to extract colored information
            // The trick when writing unit tests is to use an in memory IColoredTransactionRepository:
            var repo = new NoSqlColoredTransactionRepository();
            // we can put our init transaction inside.
            repo.Transactions.Put(init);
            // you can extract the color:
            ColoredTransaction color = ColoredTransaction.FetchColors(init, repo);
            Console.WriteLine("color: " + color);
            /*
            {
                "inputs": [],
                "issuances": [],
                "transfers": [],
                "destructions": []
            }
            */
            // let’s use the two coins sent to Silver and Gold as Issuance Coins.
            var issuanceCoins = init.Outputs.AsCoins()
                                    .Take(2).Select((c, i) => new IssuanceCoin(c))
                                    .OfType<ICoin>()
                                    .ToArray();
            // Gold is the first coin, Silver the second one.
            // From that you can send Gold to Satoshi with the TransactionBuilder,

            // imagine that Satoshi wants to send 4 gold to Alice.
            // First, he will fetch the ColoredCoin out of the transaction.
            var sendGoldToSatoshi = init;
            var goldCoin = ColoredCoin.Find(sendGoldToSatoshi, color).FirstOrDefault();
            // build a transaction like that:
            var builder = new TransactionBuilder();
            var sendToBobAndAlice = builder.AddKeys(satoshi)
                                           .AddCoins(goldCoin)
                                           .SendAsset(alice, new AssetMoney(goldId, 4))
                                           .SetChange(satoshi)
                                           .BuildTransaction(true);
            // you are out of 600 satoshi.
            // You can fix the problem by adding the last Coin of 1 BTC 
            // in the init transaction that belongs to satoshi.
            var satoshiBtc = init.Outputs.AsCoins().Last();
            builder = new TransactionBuilder();
            var sendToAlice = builder.AddKeys(satoshi)
                                     .AddCoins(goldCoin, satoshiBtc)
                                     .SendAsset(alice, new AssetMoney(goldId, 4))
                                     .SetChange(satoshi)
                                     .BuildTransaction(true);
            repo.Transactions.Put(sendToAlice);
            color = ColoredTransaction.FetchColors(sendToAlice, repo);

            // Let’s see the transaction and its colored part:
            Console.WriteLine("sendToAlice: " + sendToAlice);
            Console.WriteLine("color: " + color);
        }
    }
}
