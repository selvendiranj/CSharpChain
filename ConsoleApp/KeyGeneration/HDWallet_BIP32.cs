using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace ConsoleApp.KeyGeneartion
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

            // payment server generates pubkey1
            // we can get the corresponding private key with our private master key
            masterKey = new ExtKey();
            masterPubKey = masterKey.Neuter();

            //The payment server generate pubkey1
            ExtPubKey pubkey1 = masterPubKey.Derive((uint)1);

            //You get the private key of pubkey1
            ExtKey key1 = masterKey.Derive((uint)1);

            //Check it is legit
            Console.WriteLine("Generated address : " + pubkey1.PubKey.GetAddress(Network.Main));
            Console.WriteLine("Expected address : " + key1.PrivateKey.PubKey.GetAddress(Network.Main));
            // Generated address : 1Jy8nALZNqpf4rFN9TWG2qXapZUBvquFfX
            // Expected address: 1Jy8nALZNqpf4rFN9TWG2qXapZUBvquFfX

            // “hierarchical”:  Parent Key + KeyPath => Child Key (and so on..)
            // derivate Child(1,1) from parent in two different way
            ExtKey parent = new ExtKey();
            ExtKey child1 = parent.Derive(1);
            ExtKey child11;

            child11 = child1.Derive(1); //Or
            child11 = parent.Derive(1).Derive(1); //Or
            child11 = parent.Derive(new KeyPath("1/1"));

            // generate one hierarchy for each department.
            // non-hardened key, accounting department can “climb” the hierarchy
            // hardened key, so the marketing department will not be able to climb the hierarchy.
            ExtKey ceoKey = new ExtKey();
            ExtKey accountingKey = ceoKey.Derive(0, hardened: false);
            ExtKey marketingKey = ceoKey.Derive(0, hardened: true);
            ExtPubKey ceoPubkey = ceoKey.Neuter();

            //Recover ceo key with accounting private key and ceo public key
            ExtKey ceoKeyRecovered = accountingKey.GetParentExtKey(ceoPubkey);
            ExtKey ceoKeyNotRecovered = marketingKey.GetParentExtKey(ceoPubkey); //Crash

            Console.WriteLine();
            Console.WriteLine("CEO Key: " + ceoKey.ToString(Network.Main));
            Console.WriteLine("CEO recovered: " + ceoKeyRecovered.ToString(Network.Main));
            Console.WriteLine("CEO Not recovered: " + ceoKeyNotRecovered.ToString(Network.Main));

            //Or
            KeyPath nonHardened = new KeyPath("1/2/3");
            KeyPath hardened = new KeyPath("1/2/3'");
            
            ceoKey = new ExtKey();
            string accounting = "1'";
            int customerId = 5;
            int paymentId = 50;
            KeyPath path = new KeyPath(accounting + "/" + customerId + "/" + paymentId);
            ExtKey paymentKey = ceoKey.Derive(path); //Path : "1'/5/50"
        }
    }
}
