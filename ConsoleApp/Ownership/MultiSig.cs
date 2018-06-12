using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace ConsoleApp.Ownership
{
    class MultiSig
    {
        public static void Execute()
        {
            Key bob = new Key();
            Key alice = new Key();
            Key satoshi = new Key();

            var scriptPubKey = PayToMultiSigTemplate
                                .Instance
                                .GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey });

            Console.WriteLine("scriptPubKey: " + scriptPubKey);
        }
    }
}
