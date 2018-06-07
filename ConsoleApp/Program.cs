using System;
using NBitcoin;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Generate a random private key
            Key privateKey = new Key();
            BitcoinSecret bitcoinPrivateKey = KeyGen.Execute(privateKey);

            //Address.Execute(privateKey);
            //ScriptPubKey.Execute(privateKey);
            //PrivateKey.Execute(privateKey);
            //TransactionVerifier.Execute();
            //SpendCoins.Execute();
            //ProofOfOwnership.Execute();
            //KeyGenAndEncryption.Execute(bitcoinPrivateKey);

            HDWallet_BIP32.Execute();
        }
    }
}