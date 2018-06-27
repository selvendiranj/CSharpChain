using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using NBitcoin;
using NBitcoin.Protocol;
using Newtonsoft.Json;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace ConsoleApp.Transfer
{
    class SpendCoins
    {
        public static void Execute()
        {
            // Step 1: Generate KeyPair and note down privateKey & walletAddress
            // Send some bitcoins from testnet/mainnet
            // Note down the transaction Id. or we can get the TxId later using wallet address
            var network = Network.TestNet;

            var privateKey = new Key();
            var bitcoinPrivateKey = privateKey.GetWif(network);
            BitcoinAddress aliceAddress = bitcoinPrivateKey.GetAddress();

            Console.WriteLine();
            Console.WriteLine("bitcoinPrivateKey: " + bitcoinPrivateKey);
            Console.WriteLine("address: " + aliceAddress);

            // faucet sends money to alice
            // sender  (address) : https://testnet.manu.backend.hamburg/faucet
            // TxId              : 4d08b8d2ae8083924b8e87b1c0a3abdee6336f7d4b9419fa8b942405b189bb27

            // alice send money to bob

            // Step 2:
            // Import your private key (replace the "cN5Y...K2RS" string with yours)
            // get the wallet address from privatekey
            Keys keys = JsonConvert.DeserializeObject<Keys>(File.ReadAllText(@"Keys.json"));
            BitcoinSecret alicePrivateKey = new BitcoinSecret(keys.alice.PrivateKey);
            network = alicePrivateKey.Network;
            aliceAddress = alicePrivateKey.GetAddress();

            Console.WriteLine();
            Console.WriteLine("alicePrivateKey: " + alicePrivateKey);
            Console.WriteLine("aliceAddress: " + aliceAddress);

            // Step 3:
            // And finally get the transaction info from your wallet software 
            // or blockchain explorer after you sent the coins
            var client = new QBitNinjaClient(network);
            var transactionId = uint256.Parse("4d08b8d2ae8083924b8e87b1c0a3abdee6336f7d4b9419fa8b942405b189bb27");
            var transactionResponse = client.GetTransaction(transactionId).Result;

            Console.WriteLine();
            Console.WriteLine("TransactionId: " + transactionResponse.TransactionId);
            Console.WriteLine("Confirmations: " + transactionResponse.Block.Confirmations);

            // Step 4:
            // create our transactions. from where, to where and how much?
            // spend the second outpoint. Here's how we have figured this out
            List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = null;
            ICoin coinToSpend = null;
            foreach (ICoin coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == alicePrivateKey.ScriptPubKey)
                {
                    coinToSpend = coin;
                    outPointToSpend = coin.Outpoint;
                }
            }
            if (outPointToSpend == null)
            {
                throw new Exception("TxOut doesn't contain our ScriptPubKey");
            }
            Console.WriteLine();
            Console.WriteLine("We want to spend {0}{1}. outpoint: {2}",
                coinToSpend.Amount, MoneyUnit.BTC, outPointToSpend);

            // Step 5:
            // For the payment you will need to reference this outpoint in the transaction
            // from where: Constructing the TxIn and adding it to the transaction
            // to where: Constructing the TxOut and adding it to the transaction
            // how much: 0.005
            // you must spend all amount in your input Txn
            // your transaction output specifies 0.005 BTC to receiverAddress and 0.269 BTC back to you.
            // What happens to the remaining 0.001 BTC? This is the miner fee.

            string bobAddress = keys.bob.Address;
            BitcoinAddress receiverAddress = BitcoinAddress.Create(bobAddress, Network.TestNet);
            Transaction newTxn = new Transaction();

            // calculate the change based on the miner fee, How much you want to spend
            Money receiverAddressAmount = new Money(0.005m, MoneyUnit.BTC);

            /* How much miner fee you want to pay? Depending on the market price and
             * the currently advised mining fee, you may consider to increase or decrease it.
             */
            Money minerFee = new Money(0.0001m, MoneyUnit.BTC);

            // How much you want to get back as change
            Money txInAmount = (Money)receivedCoins[(int)outPointToSpend.N].Amount;
            Money changeAmount = txInAmount - receiverAddressAmount - minerFee;

            // refer your own transaction
            TxIn myownTxIn = new TxIn()
            {
                PrevOut = outPointToSpend
            };

            // Let's add our calculated values to our TxOuts:
            TxOut receiverAddressTxOut = new TxOut()
            {
                Value = receiverAddressAmount,
                ScriptPubKey = receiverAddress.ScriptPubKey
            };

            TxOut changeTxOut = new TxOut()
            {
                Value = changeAmount,
                ScriptPubKey = alicePrivateKey.ScriptPubKey
            };

            // And add them to our transaction:
            newTxn.Inputs.Add(myownTxIn);
            newTxn.Outputs.Add(receiverAddressTxOut);
            newTxn.Outputs.Add(changeTxOut);

            // Step 6: 
            // Message on The Blockchain, must be <= 80 bytes or your transaction will get rejected
            var message = "Long live NBitcoin and its makers!";
            var bytes = Encoding.UTF8.GetBytes(message);
            newTxn.Outputs.Add(new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
            });

            // ScriptPubKey is empty since we haven't signed the txn yet
            Console.WriteLine();
            Console.WriteLine("newTxn (not signed): " + newTxn);

            // Step 7: Sign your transaction
            // prove that you own the TxOut that you referenced in the input
            // two options to fill the ScriptSig with the ScriptPubKey of our address

            // 1. Get it from the public address
            aliceAddress = BitcoinAddress.Create(keys.alice.Address, Network.TestNet);
            newTxn.Inputs[0].ScriptSig = aliceAddress.ScriptPubKey;

            // 2. OR we can also use the private key 
            alicePrivateKey = new BitcoinSecret(keys.alice.PrivateKey);
            newTxn.Inputs[0].ScriptSig = alicePrivateKey.ScriptPubKey;

            newTxn.Sign(alicePrivateKey, false);

            // ScriptPubKey is filled since we signed the txn
            Console.WriteLine();
            Console.WriteLine("newTxn (signed): " + newTxn);

            // Step 8: Propagate your transactions
            // With QBitNinja:
            BroadcastResponse broadcastResponse = client.Broadcast(newTxn).Result;
            if (!broadcastResponse.Success)
            {
                Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success! You can check out the hash of the transaciton in any block explorer:");
                Console.WriteLine(newTxn.GetHash());
            }

            // With your own Bitcoin Core:
            /*
            using (Node node = Node.ConnectToLocal(network)) //Connect to the node
            {
                node.VersionHandshake(); //Say hello
                                         //Advertize your transaction (send just the hash)
                node.SendMessage(new InvPayload(InventoryType.MSG_TX, newTxn.GetHash()));
                //Send it
                node.SendMessage(new TxPayload(newTxn));
                Thread.Sleep(500); //Wait a bit
            }*/

            Console.ReadLine();
        }
    }
}
