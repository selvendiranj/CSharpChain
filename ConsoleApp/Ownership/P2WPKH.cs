using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace ConsoleApp.Ownership
{
    class P2WPKH
    {
        public static void Execute()
        {
            var key = new Key();
            Console.WriteLine("WitHash: " + key.PubKey.WitHash);
            Console.WriteLine("ScriptPubKey: " + key.PubKey.WitHash.ScriptPubKey);
        }
    }
}
