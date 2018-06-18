using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace ConsoleApp.Ownership
{
    class P2WSH
    {
        public static void Execute()
        {
            // what was previously in the scriptSig, and the scriptPubKey being modified.
            // From: OP_HASH160 10f400e996c34410d02ae76639cbf64f4bdf2def OP_EQUAL
            // To: 0 e4d3d21bab744d90cd857f56833252000ac0fade318136b713994b9319562467

            Key key = new Key();
            Script witnessScript = key.PubKey.ScriptPubKey.WitHash.ScriptPubKey;
            Console.WriteLine("witnessScript: " + witnessScript);

            // Imagine that the multi-sig P2WSH receives a coin in a transaction called received.
            Transaction received = new Transaction();
            // Pay to the witness script hash
            // Warning: The payment is sent to WitHash.ScriptPubKey and not to ScriptPubKey!
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), witnessScript));
            Console.WriteLine("receivedTxn: \n" + received);

            /* scriptSig (signature + redeem script), moved to the witness:
            "in": [
                {
                  "prev_out": {
                    "hash": "ffa2826ba2c9a178f7ced0737b559410364a62a41b16440beb299754114888c4",
                    "n": 0
                  },
                  "scriptSig": "",
                  "witness": "304402203a4d9f42c190682826ead3f88d9d87e8c47db57f5c272637441bafe11d5ad8a302206ac21b2bfe831216059ac4c91ec3e4458c78190613802975f5da5d11b55a69c601 210243b3760ce117a85540d88fa9d3d605338d4689bed1217e1fa84c78c22999fe08ac"
                }
              ]
             */
        }
    }
}
