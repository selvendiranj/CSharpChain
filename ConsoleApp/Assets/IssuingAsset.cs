using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.OpenAsset;
using NBitcoin.Protocol;
using Newtonsoft.Json;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ConsoleApp.Assets
{
    class IssuingAsset
    {
        public static void Execute()
        {
            // use the following coin for issuing assets.
            /*
             {
                "transactionId": "a416cc07134b6049aebfc36712fe3385b57325ae8c0dc1218adc2bda839ae319",
                "index": 0,
                "value": 2000000,
                "scriptPubKey": "OP_DUP OP_HASH160 761165aeb773479007b4bad25dc594980b0deb68 OP_EQUALVERIFY OP_CHECKSIG",
                "redeemScript": null
             }
             */
            Keys keys = JsonConvert.DeserializeObject<Keys>(File.ReadAllText(@"Keys.json"));
            Network btcTestNet = Network.TestNet;
            string txnId = "a416cc07134b6049aebfc36712fe3385b57325ae8c0dc1218adc2bda839ae319";
            string scriptPubKey = "OP_DUP OP_HASH160 761165aeb773479007b4bad25dc594980b0deb68 OP_EQUALVERIFY OP_CHECKSIG";

            string aliceAddressHex = keys.alice.Address;
            string bobPrivateKeyHex = keys.bob.PrivateKey;
            BitcoinAddress aliceAddress = BitcoinAddress.Create(aliceAddressHex, btcTestNet);
            BitcoinSecret bobPrivateKey = new BitcoinSecret(bobPrivateKeyHex);

            Coin coin = new Coin(fromTxHash: new uint256(txnId),
                                 fromOutputIndex: 0,
                                 amount: Money.Satoshis(490000),
                                 scriptPubKey: new Script(scriptPubKey));
            IssuanceCoin issuance = new IssuanceCoin(coin);

            // build transaction and sign the transaction using TransactionBuilder
            TransactionBuilder builder = new TransactionBuilder();
            Transaction tx = builder.AddKeys(bobPrivateKey)
                                    .AddCoins(issuance)
                                    .IssueAsset(aliceAddress, new AssetMoney(issuance.AssetId, quantity: 10))
                                    .SendFees(Money.Coins(0.0001m))
                                    .SetChange(bobPrivateKey.GetAddress())
                                    .BuildTransaction(sign: true);

            // After transaction verifications it is ready to be sent to the network.
            Console.WriteLine(tx);
            Console.WriteLine("Verify Txn: " + builder.Verify(tx));

            /*
            // With QBitNinja
            var client = new QBitNinjaClient(btcNetwork);
            BroadcastResponse broadcastResponse = client.Broadcast(tx).Result;

            if (!broadcastResponse.Success)
            {
                Console.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Txn Broadcast Success!");
            }

            // Or with local Bitcoin core
            using (var node = Node.ConnectToLocal(Network.Main)) //Connect to the node
            {
                node.VersionHandshake(); //Say hello
                //Advertize your transaction (send just the hash)
                node.SendMessage(new InvPayload(InventoryType.MSG_TX, tx.GetHash()));
                //Send it
                node.SendMessage(new TxPayload(tx));
                Thread.Sleep(500); //Wait a bit
            }
            */

            // preventing a user from sending Colored Coin to a wallet that do not support it,
            // Open Asset have its own address format, that only colored coin wallets understand
            Console.WriteLine("receiverAddress: " + aliceAddress);
            Console.WriteLine("ColoredCoinAddress: " + aliceAddress.ToColoredAddress());

            // Asset ID is derived from the issuer’s ScriptPubKey, here is how to get it in code
            var assetId = (new AssetId(aliceAddress)).GetWif(btcTestNet);
            Console.WriteLine("assetId: " + assetId); // oNRXXFo48zQ5AMtTMuW5Ss1NtMoSe39Cek
        }
    }
}

