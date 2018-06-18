using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace ConsoleApp.Ownership
{
    class P2WAnyOverP2SH
    {
        public static void Execute()
        {
            // example of P2WPKH over P2SH
            var key = new Key();
            Console.WriteLine("P2SH scriptPubKey: " + key.PubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey);
            /*
             a signed transaction spending this output will look like:
            "in": [
                {
                  "prev_out": {
                    "hash": "674ece694e5e28956138efacab96fc0bffd7c6cc1af7bb2729943fedf8f0b8b9",
                    "n": 0
                  },
                  "scriptSig": "001404100ab485c95701bf0f4d73e3fe7d69ecc4f0ea",
                  "witness": "3045022100f4c14cf383c0c97bbdaf520ea06f7db6c61e0effbc4bd3dfea036a90272f6cce022055b0fc058759a7961e718d48a3dc4dd5580fffc310557925a0865dbe467a835901 0205b956a5afe8f34a01337f0949f5733b5e376caaea57c9624e40e739a0b1d16c"
                }
              ],
             */

            //In NBitcoin, signing a P2SH(P2WPKH) is exactly similar as signing a normal P2SH with ScriptCoin.
            // NBitcoin requires you to create a ScriptCoin by supplying the Redeem
            // (P2WSH redeem or P2SH redeem) and the ScriptPubKey, exactly as explained in the P2SH part.
            //Compatible for P2SH/P2WSH/P2SH(P2WSH)/P2SH(P2WPKH)
        }
    }
}
