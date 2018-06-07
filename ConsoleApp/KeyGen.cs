using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class KeyGen
    {
        public static BitcoinSecret Execute(Key privateKey)
        {
            BitcoinSecret mySecret = privateKey.GetWif(Network.Main);
            Console.WriteLine("My BitcoinSecret: " + mySecret);

            return mySecret;
        }
    }
}
