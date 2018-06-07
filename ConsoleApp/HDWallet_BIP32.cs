using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace ConsoleApp
{
    class HDWallet_BIP32
    {
        public static void Execute()
        {
            // HD Wallet (BIP 32)
            // from the master key, I can generate new keys:
            ExtKey masterKey = new ExtKey();
            Console.WriteLine("---------------------HD Wallet (BIP 32)---------------------");
            Console.WriteLine("Master key : " + masterKey.ToString(Network.Main));
            for (int i = 0; i < 5; i++)
            {
                ExtKey childKey = masterKey.Derive((uint)i);
                Console.WriteLine("Key " + i + " : " + childKey.ToString(Network.Main));
            }

            // go back from a Key to an ExtKey by supplying the Key and the ChainCode
            ExtKey extKey = new ExtKey();
            Key key = extKey.PrivateKey;
            byte[] chainCode = extKey.ChainCode;

            ExtKey newExtKey = new ExtKey(key, chainCode);
            Console.WriteLine("extKey==newExtKey: " +
                (extKey.ToString(Network.Main) == newExtKey.ToString(Network.Main)));

            // delegating address creation to a peer
            // third party can generate public keys without knowing the private key
            ExtPubKey masterPubKey = masterKey.Neuter();
            Console.WriteLine("Master Pubkey : " + masterPubKey.ToString(Network.Main));
            for (int i = 0; i < 5; i++)
            {
                ExtPubKey pubkey = masterPubKey.Derive((uint)i);
                Console.WriteLine("PubKey " + i + " : " + pubkey.ToString(Network.Main));
            }
        }
    }
}
