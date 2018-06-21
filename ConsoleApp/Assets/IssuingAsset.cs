﻿using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.OpenAsset;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Assets
{
    class IssuingAsset
    {
        public static void Execute()
        {
            // use the following coin for issuing assets.
            /*
             {
                "transactionId": "eb49a599c749c82d824caf9dd69c4e359261d49bbb0b9d6dc18c59bc9214e43b",
                "index": 0,
                "value": 2000000,
                "scriptPubKey": "76a914c81e8e7b7ffca043b088a992795b15887c96159288ac",
                "redeemScript": null
             }
             */
            Network btcNetwork = Network.TestNet;
            string txnId = "a416cc07134b6049aebfc36712fe3385b57325ae8c0dc1218adc2bda839ae319";
            string scriptPubKey = "OP_DUP OP_HASH160 761165aeb773479007b4bad25dc594980b0deb68 OP_EQUALVERIFY OP_CHECKSIG";
            var ownerScript = new Script(scriptPubKey);

            string receiverAddrHex = "mki1B6VAi2xaUo7GiFjwBuX7f6urgCPqoU";
            string ownerPrivateKeyHex = "cNFXPwduvGRWJ89Wu5rtb3M32hC7j5x15E8F4WPof6qBh5Jrb7eQ";

            BitcoinAddress receiverAddress = BitcoinAddress.Create(receiverAddrHex, btcNetwork);
            BitcoinSecret ownerPrivateKey = new BitcoinSecret(ownerPrivateKeyHex);

            Coin coin = new Coin(fromTxHash: new uint256(txnId),
                                 fromOutputIndex: 0,
                                 amount: Money.Satoshis(490000),
                                 scriptPubKey: ownerScript);
            var issuance = new IssuanceCoin(coin);

            // build transaction and sign the transaction using TransactionBuilder
            TransactionBuilder builder = new TransactionBuilder();

            var tx = builder.AddKeys(ownerPrivateKey)
                            .AddCoins(issuance)
                            .IssueAsset(receiverAddress, new AssetMoney(issuance.AssetId, quantity: 10))
                            .SendFees(Money.Coins(0.0001m))
                            .SetChange(ownerPrivateKey.GetAddress())
                            .BuildTransaction(sign: true);
            
            // After transaction verifications it is ready to be sent to the network.
            Console.WriteLine(tx);
            Console.WriteLine("Verify Txn: " + builder.Verify(tx));

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
        }
    }
}

