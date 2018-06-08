using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Transfer
{
    class ScriptPubKey
    {
        public static void Execute(Key privateKey)
        {
            // use a one-way cryptographic function, to generate a public key.
            PubKey publicKey = privateKey.PubKey;
            KeyId publicKeyHash = publicKey.Hash;

            var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);
            var mainNetAddress = publicKeyHash.GetAddress(Network.Main);

            Console.WriteLine();
            Console.WriteLine("mainNetAddress.ScriptPubKey: " + mainNetAddress.ScriptPubKey);
            Console.WriteLine("testNetAddress.ScriptPubKey: " + testNetAddress.ScriptPubKey);

            // generate a bitcoin address from the ScriptPubKey and the network identifier.
            var paymentScript = publicKeyHash.ScriptPubKey;
            var sameMainNetAddress = paymentScript.GetDestinationAddress(Network.Main);
            // retrieve the hash from the ScriptPubKey and generate a Bitcoin Address from it
            var samePublicKeyHash = (KeyId)paymentScript.GetDestination();
            var sameMainNetAddress2 = new BitcoinPubKeyAddress(samePublicKeyHash, Network.Main);

            Console.WriteLine();
            Console.WriteLine(mainNetAddress == sameMainNetAddress); // True
            Console.WriteLine(publicKeyHash == samePublicKeyHash); // True
            Console.WriteLine(mainNetAddress == sameMainNetAddress2); // True

            Console.ReadLine();
        }
    }
}
