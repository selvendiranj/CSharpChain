using System;
using NBitcoin;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            BitcoinSecret mySecret = new Key().GetWif(Network.Main);
            Console.WriteLine("My BitcoinSecret: " + mySecret);

            // generate a random private key
            Key privateKey = new Key();
            
            // use a one-way cryptographic function, to generate a public key.
            PubKey publicKey = privateKey.PubKey;
            Console.WriteLine("My publicKey: " + publicKey);

            // get your bitcoin address from your public key and the network
            Console.WriteLine("MainNet bitcoin address: " + publicKey.GetAddress(Network.Main)); 
            Console.WriteLine("TestNet bitcoin address: " + publicKey.GetAddress(Network.TestNet));

            var publicKeyHash = publicKey.Hash;
            Console.WriteLine(publicKeyHash);
            var mainNetAddress = publicKeyHash.GetAddress(Network.Main);
            var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);
            Console.WriteLine("mainNetAddress: " + mainNetAddress);
            Console.WriteLine("testNetAddress: " + testNetAddress);
        }
    }
}