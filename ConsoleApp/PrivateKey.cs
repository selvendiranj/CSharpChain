using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class PrivateKey
    {
        public static void Print(Key privateKey)
        {
            // generate our Bitcoin secret(also known as Wallet Import Format or simply WIF)
            // from our private key for the mainnet & testnet
            BitcoinSecret mainNetPrivateKey = privateKey.GetBitcoinSecret(Network.Main);
            BitcoinSecret testNetPrivateKey = privateKey.GetBitcoinSecret(Network.TestNet);

            Console.WriteLine();
            Console.WriteLine("mainNetPrivateKey: " + mainNetPrivateKey);
            Console.WriteLine("testNetPrivateKey: " + testNetPrivateKey);

            BitcoinSecret mainNetPrivateKey2 = privateKey.GetWif(Network.Main);
            bool WifIsBitcoinSecret = mainNetPrivateKey == mainNetPrivateKey2;
            BitcoinSecret bitcoinSecret = privateKey.GetWif(Network.Main);
            Key samePrivateKey = bitcoinSecret.PrivateKey;

            Console.WriteLine("WifIsBitcoinSecret: " + WifIsBitcoinSecret); // True
            Console.WriteLine(samePrivateKey == privateKey); // True

            PubKey publicKey = privateKey.PubKey;
            BitcoinPubKeyAddress bitcoinPublicKey = publicKey.GetAddress(Network.Main); // 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
            //PubKey samePublicKey = bitcoinPublicKey.ItIsNotPossible;
        }
    }
}
