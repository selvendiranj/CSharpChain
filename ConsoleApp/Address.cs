using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class Address
    {
        public static void Execute(Key privateKey)
        {
            // use a one-way cryptographic function, to generate a public key.
            PubKey publicKey = privateKey.PubKey;
            Console.WriteLine("My publicKey: " + publicKey);

            // get your bitcoin address from your public key and the network
            Console.WriteLine();
            Console.WriteLine("MainNet bitcoin address: " + publicKey.GetAddress(Network.Main));
            Console.WriteLine("TestNet bitcoin address: " + publicKey.GetAddress(Network.TestNet));

            var publicKeyHash = publicKey.Hash;
            var mainNetAddress = publicKeyHash.GetAddress(Network.Main);
            var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);

            Console.WriteLine();
            Console.WriteLine("publicKeyHash: " + publicKeyHash);
            Console.WriteLine("mainNetAddress: " + mainNetAddress);
            Console.WriteLine("testNetAddress: " + testNetAddress);
        }
    }
}
