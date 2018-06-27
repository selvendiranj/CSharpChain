using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Assets
{
    class RicardianContracts
    {
        public static void Execute()
        {
            // AssetId is specified by OpenAsset in such way :
            // AssetId = Hash160(ScriptPubKey)
            // Let’s make such ScriptPubKey a P2SH as:
            // ScriptPubKey = OP_HASH160 Hash(RedeemScript) OP_EQUAL
            // Where:
            // RedeemScript = HASH160(RicardianContract) OP_DROP IssuerScript

            /*
            let’s create the following Asset Definition File:
            {
                "IssuerScript" : IssuerScript,
                "name" : "MyAsset",
                "contract_url" : "http://issuer.com/readableContract",
                "contract_hash" : "DKDKocezifefiouOIUOIUOIufoiez980980",
                "Type" : "Vote",
                "Candidates" : ["A","B","C"],
                "Validity" : "10 jan 2015"
            }
            */

            // define the RicardianContract:
            // RicardianContract = AssetDefinitionFile
        }
    }
}
