using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace ConsoleApp
{
    class TransactionVerifier
    {
        public static void Execute()
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
            foreach (ICoin coin in receivedCoins)
            {
                Money amount = (Money)coin.Amount;
                Script paymentScript = coin.TxOut.ScriptPubKey;
                BitcoinAddress address = paymentScript.GetDestinationAddress(Network.Main);

                Console.WriteLine();
                Console.WriteLine("amount: " + amount.ToDecimal(MoneyUnit.BTC));
                Console.WriteLine("paymentScript: " + paymentScript);  // It's the ScriptPubKey
                Console.WriteLine("address: " + address); // 1HfbwN6Lvma9eDsv7mdwp529tgiyfNr7jc
            }

            // Each input shows you which previous out has been spent
            TxInList inputs = transaction.Inputs;
            foreach (TxIn input in inputs)
            {
                OutPoint previousOutpoint = input.PrevOut;

                Console.WriteLine();
                Console.WriteLine("previousOutpoint hash: " + previousOutpoint.Hash); // hash of prev tx
                Console.WriteLine("previousOutpoint indx: " + previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx
            }

            // TxOut represents an amount of bitcoin and a ScriptPubKey. (Recipient)
            Money twentyOneBtc = new Money(21, MoneyUnit.BTC);
            Script scriptPubKey = transaction.Outputs.First().ScriptPubKey;
            TxOut txOut = new TxOut(twentyOneBtc, scriptPubKey);

            // The Outpoint of the TxOut with 13.19683492 BTC in our transaction is 
            OutPoint firstOutPoint = receivedCoins.First().Outpoint;
            Console.WriteLine("firstOutPoint hash: " + firstOutPoint.Hash); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
            Console.WriteLine("firstOutPoint indx: " + firstOutPoint.N); // 0

            // TxIn is composed of the Outpoint of the TxOut being spent and 
            // of the ScriptSig (Proof of Ownership)
            OutPoint firstPrevOutPt = transaction.Inputs.First().PrevOut;
            Transaction firstPrevTxn = client.GetTransaction(firstPrevOutPt.Hash).Result.Transaction;

            Console.WriteLine("Txn input count: "+ transaction.Inputs.Count); // 9
            Console.WriteLine("IsCoinBase Txn: " + firstPrevTxn.IsCoinBase); // False

            // the outputs were for a total of 13.19703492 BTC
            Money spentAmount = Money.Zero;
            List<ICoin> spentCoins = transactionResponse.SpentCoins;
            foreach (var spentCoin in spentCoins)
            {
                spentAmount = (Money)spentCoin.Amount.Add(spentAmount);
            }
            Console.WriteLine("spentAmount: " + spentAmount.ToDecimal(MoneyUnit.BTC)); // 13.19703492

            // Get Transaction Fees or Miner’s Fees
            Money fee = transaction.GetFee(spentCoins.ToArray());
            Console.WriteLine("Txn fee: " + fee.ToString());

            // coinbase Txn rule
            // sum of output's value = (transaction fees in the block + the mining reward)

            Console.ReadLine();
        }
    }
}
