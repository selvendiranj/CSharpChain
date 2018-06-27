using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.OpenAsset;
using Newtonsoft.Json;

namespace ConsoleApp.Assets
{
    class TransferAsset
    {
        public static void Execute()
        {
            // spend the 10 assets we received on the address “nico”. (IssuingAsset.cs)
            // Here is the coin I want to spend, we need to build a ColoredCoin.
            /*
            {
              "transactionId": "ba6218ca0c900edd03103c68121721bc966e247626e630b33811b0d977aa9613",
              "index": 0,
              "value": 600,
              "scriptPubKey": "76a914356facdac5f5bcae995d13e667bb5864fd1e7d5988ac",
              "redeemScript": null,
              "assetId": "AVAVfLSb1KZf9tJzrUVpktjxKUXGxUTD4e",
              "quantity": 10
            }
            */
            Keys keys = JsonConvert.DeserializeObject<Keys>(File.ReadAllText(@"Keys.json"));
            //  instantiate such Colored Coin in code:
            Network btcTestNet = Network.TestNet;
            string txnId = "ba6218ca0c900edd03103c68121721bc966e247626e630b33811b0d977aa9613";
            string txnFee = "733e124863e88c6d94ae67d306cd7a0bdb2c010a4720bf290a06965c53d51a8c";
            string scriptPubKey_0 = "OP_DUP OP_HASH160 38ef9bce9b02fb8da15926c9a3f6a1b00be949f0 OP_EQUALVERIFY OP_CHECKSIG";
            string scriptPubKey_1 = "OP_DUP OP_HASH160 38ef9bce9b02fb8da15926c9a3f6a1b00be949f0 OP_EQUALVERIFY OP_CHECKSIG";

            BitcoinAddress bobAddress = BitcoinAddress.Create(keys.bob.Address, btcTestNet);
            BitcoinSecret alicePrivateKey = new BitcoinSecret(keys.alice.PrivateKey);

            Coin coin = new Coin(fromTxHash: new uint256(txnId),
                                 fromOutputIndex: 0,
                                 amount: Money.Satoshis(546),
                                 scriptPubKey: new Script(scriptPubKey_0));

            Coin forFees = new Coin(fromTxHash: new uint256(txnFee),
                                    fromOutputIndex: 0,
                                    amount: Money.Satoshis(479454),
                                    scriptPubKey: new Script(scriptPubKey_1));

            BitcoinAssetId assetId = (new AssetId(bobAddress)).GetWif(btcTestNet);
            ColoredCoin colored = coin.ToColoredCoin(assetId, 10);

            TransactionBuilder builder = new TransactionBuilder();
            Transaction tx = builder.AddKeys(alicePrivateKey)
                                    .AddCoins(colored, forFees)
                                    .SendAsset(bobAddress, new AssetMoney(assetId, 10))
                                    .SendFees(Money.Coins(0.0001m))
                                    .SetChange(alicePrivateKey.GetAddress())
                                    .BuildTransaction(sign: true);

            Console.WriteLine("Transaction: " + tx);
            Console.WriteLine("Verify Txn: " + builder.Verify(tx));
        }
    }
}
