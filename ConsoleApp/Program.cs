using System;
using NBitcoin;
using ConsoleApp.KeyGeneartion;
using ConsoleApp.Ownership;
using ConsoleApp.Transfer;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Generate a random private key
            Key privateKey = new Key();
            var bitcoinPrivateKey = KeyGen.GerNewPair();

            //Address.Execute(privateKey);
            //ScriptPubKey.Execute(privateKey);
            //PrivateKey.Execute(privateKey);
            //TransactionVerifier.Execute();
            //SpendCoins.Execute();
            //ProofOfOwnership.Execute();

            KeyGenAndEncryption.Execute(bitcoinPrivateKey.Key);
            //HDWallet_BIP32.Execute();
            //MnemonicCode_HD_BIP39.Execute();
            //DarkWallet.Execute();

            //P2PKH.Execute();
            //P2WPKH.Execute();
            //MultiSig.Execute();
            //P2SH.Execute();
            //P2WSH.Execute();
            //P2WAnyOverP2SH.Execute();
            Arbitrary.Execute();
        }
    }
}