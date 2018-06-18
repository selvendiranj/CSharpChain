using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class KeyGen
    {
        public static KeyValuePair<BitcoinSecret, BitcoinAddress> GerNewPair()
        {
            Network network = Network.TestNet;

            Key privateKey = new Key();
            BitcoinSecret bitcoinPrivateKey = privateKey.GetWif(network);
            BitcoinAddress address = bitcoinPrivateKey.GetAddress();

            Console.WriteLine();
            Console.WriteLine("bitcoinPrivateKey: " + bitcoinPrivateKey);
            Console.WriteLine("address: " + address);

            return new KeyValuePair<BitcoinSecret, BitcoinAddress>(bitcoinPrivateKey, address);
        }
    }
}
