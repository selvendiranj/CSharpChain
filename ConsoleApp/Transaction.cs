using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class TransactionVerifier
    {
        public static void Print()
        {
            string TxnHex = "f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94";
            //Transaction tx = new Transaction(TxnHex);

            // Query the transaction by id:
            // Create a client
            QBitNinjaClient client = new QBitNinjaClient(Network.Main);
            // Parse transaction id to NBitcoin.uint256 so the client can eat it
            var transactionId = uint256.Parse(TxnHex);
            // Query the transaction
            GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;

            Transaction transaction = transactionResponse.Transaction;
            Console.WriteLine("TransactionId: " + transactionResponse.TransactionId);
            Console.WriteLine("Transaction Hash: " + transaction.GetHash());

            // RECEIVED COINS using QBitNinja
            List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
            foreach (var coin in receivedCoins)
            {
                Money amount = (Money)coin.Amount;
                Script paymentScript = coin.TxOut.ScriptPubKey;
                BitcoinAddress address = paymentScript.GetDestinationAddress(Network.Main);

                Console.WriteLine();
                Console.WriteLine("amount: " + amount.ToDecimal(MoneyUnit.BTC));
                Console.WriteLine("paymentScript: " + paymentScript);  // It's the ScriptPubKey
                Console.WriteLine("address: " + address); // 1HfbwN6Lvma9eDsv7mdwp529tgiyfNr7jc
            }
        }
    }
}
