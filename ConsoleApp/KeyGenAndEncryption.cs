using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.Crypto;

namespace ConsoleApp
{
    class KeyGenAndEncryption
    {
        public static void Execute(BitcoinSecret privateKey)
        {
            // PRNG (Pseudo-Random-Number-Generator)
            // add entropy to the PRNG output that NBitcoin is using
            RandomUtils.AddEntropy("Hello");
            RandomUtils.AddEntropy(new byte[] { 1, 2, 3 });
            Key nsaProofKey = new Key();

            // KDF (Key Derivation Function)
            byte[] derived = SCrypt.BitcoinComputeDerivedKey("Hello", new byte[] { 1, 2, 3 });
            RandomUtils.AddEntropy(derived);

            string password = "password-1";
            BitcoinEncryptedSecret encryptedBitcoinPrivateKey = privateKey.Encrypt(password);
            BitcoinSecret decryptedBitcoinPrivateKey = encryptedBitcoinPrivateKey.GetSecret(password);

            Console.WriteLine("-------------------Key Derivation Function------------------");
            Console.WriteLine("BitcoinPrivateKey : " + privateKey);
            Console.WriteLine("BitcoinPrivateKey(encrypted) : " + encryptedBitcoinPrivateKey);
            Console.WriteLine("BitcoinPrivateKey(decrypted) : " + decryptedBitcoinPrivateKey);

            // BIP38 (Part 2)
            string mySecret = "Secret Phrase";
            BitcoinPassphraseCode passphraseCode = new BitcoinPassphraseCode(mySecret, Network.Main, null);
            EncryptedKeyResult encryptedKeyResult = passphraseCode.GenerateEncryptedSecret();
            var generatedAddress = encryptedKeyResult.GeneratedAddress;
            var encryptedKey = encryptedKeyResult.EncryptedKey;
            var confirmationCode = encryptedKeyResult.ConfirmationCode;
            var bitcoinPrivateKey = encryptedKey.GetSecret(mySecret);

            Console.WriteLine("---------------------BIP38 (Part 2)---------------------");
            Console.WriteLine("check generatedAddress: " + confirmationCode.Check(mySecret, generatedAddress)); // True
            Console.WriteLine("check generatedAddress: " + (bitcoinPrivateKey.GetAddress() == generatedAddress)); // True
            Console.WriteLine("bitcoinPrivateKey: " + bitcoinPrivateKey);
        }
    }
}
