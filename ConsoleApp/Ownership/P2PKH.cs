using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace ConsoleApp.Ownership
{
    // P2PK[H] (Pay to Public Key [Hash])
    class P2PKH
    {
        public static void Execute()
        {
            // Bitcoin Address was the hash of a public key
            KeyId publicKeyHash = new Key().PubKey.Hash;
            BitcoinAddress bitcoinAddress = publicKeyHash.GetAddress(Network.Main);
            Console.WriteLine("publicKeyHash: " + publicKeyHash);
            Console.WriteLine("bitcoinAddress: " + bitcoinAddress);

            Script scriptPubKey = bitcoinAddress.ScriptPubKey;
            Console.WriteLine("scriptPubKey: " + scriptPubKey);
            // OP_DUP OP_HASH160 41e0d7ab8af1ba5452b824116a31357dc931cf28 OP_EQUALVERIFY OP_CHECKSIG
        }
    }
}
