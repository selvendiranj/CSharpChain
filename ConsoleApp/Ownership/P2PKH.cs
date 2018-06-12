using System;
using System.Collections.Generic;
using System.Linq;
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
            Script scriptPubKey = bitcoinAddress.ScriptPubKey;
            BitcoinAddress sameBitcoinAddress = scriptPubKey.GetDestinationAddress(Network.Main);

            Console.WriteLine();
            Console.WriteLine("publicKeyHash: " + publicKeyHash);
            Console.WriteLine("bitcoinAddress: " + bitcoinAddress);
            Console.WriteLine("scriptPubKey: " + scriptPubKey);
            Console.WriteLine("sameBitcoinAddress: " + sameBitcoinAddress);

            // Not all ScriptPubKey represent a Bitcoin Address.
            Block genesisBlock = Network.Main.GetGenesis();
            Transaction firstTransactionEver = genesisBlock.Transactions.First();
            var firstOutputEver = firstTransactionEver.Outputs.First();
            var firstScriptPubKeyEver = firstOutputEver.ScriptPubKey;
            var firstBitcoinAddressEver = firstScriptPubKeyEver.GetDestinationAddress(Network.Main);
            var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First();
            
            Console.WriteLine();
            Console.WriteLine("firstBitcoinAddressEver==null: " + (firstBitcoinAddressEver == null)); // True
            Console.WriteLine("firstTransactionEver: " + firstTransactionEver);
            Console.WriteLine("firstScriptPubKeyEver: " + firstScriptPubKeyEver);
            Console.WriteLine("firstPubKeyEver: " + firstPubKeyEver);

            // P2PK (pay to public key) and P2PKH (pay to public key hash)
            Key key = new Key();
            Console.WriteLine();
            Console.WriteLine("Pay to public key : " + key.PubKey.ScriptPubKey);
            Console.WriteLine("Pay to public key hash : " + key.PubKey.Hash.ScriptPubKey);
        }
    }
}
