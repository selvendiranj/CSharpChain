using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace ConsoleApp.KeyGeneartion
{
    class MnemonicCode_HD_BIP39
    {
        public static void Execute()
        {
            string password = "my password";
            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            ExtKey hdRoot = mnemo.DeriveExtKey(password);
            Console.WriteLine(mnemo);
            
            // Now, if you have the mnemonic and the password, you can recover the hdRoot key.
            string mnemonicPhrase = "minute put grant neglect anxiety case globe win famous correct turn link";
            mnemo = new Mnemonic(mnemonicPhrase, Wordlist.English);
            hdRoot = mnemo.DeriveExtKey(password);
        }
    }
}
