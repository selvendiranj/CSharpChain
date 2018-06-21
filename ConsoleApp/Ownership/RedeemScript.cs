using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin;
using NBitcoin.Crypto;

namespace ConsoleApp.Ownership
{
    class RedeemScript
    {
        public static void Execute()
        {
            // we can create your own definition of what “ownership” means.
            // build the RedeemScript, Add Reference... -> Find System.Numerics
            
            // receiver (address): mrHEsU8AfJTrsjYMLdHmeUyWMW5hWegdKu
            // bitcoinPrivateKey: cNFXPwduvGRWJ89Wu5rtb3M32hC7j5x15E8F4WPof6qBh5Jrb7eQ
            string receiverWalletAddress = "mrHEsU8AfJTrsjYMLdHmeUyWMW5hWegdKu";
            BitcoinAddress receiverAddress = BitcoinAddress.Create(receiverWalletAddress, Network.TestNet);
            var birth = Encoding.UTF8.GetBytes("18/07/1988");
            var birthHash = Hashes.Hash256(birth);
            string script = "OP_IF " + "OP_HASH256 {0} OP_EQUAL " +
                            "OP_ELSE " + "{1} " +
                            "OP_ENDIF";
            script = string.Format(script, Op.GetPushOp(birthHash.ToBytes()), receiverAddress.ScriptPubKey);
            Script redeemScript = new Script(script);

            // there are 2 ways of spending such ScriptCoin:
            // 1. you know the data that gives birthHash (birthdate) or
            // 2. you own the bitcoin address.
            
            // send money to such redeemScript:
            Transaction tx = new Transaction();
            tx.Outputs.Add(new TxOut(Money.Parse("0.0001"), redeemScript.Hash));
            ScriptCoin scriptCoin = tx.Outputs.AsCoins().First().ToScriptCoin(redeemScript);

            //Create spending transaction
            Transaction spending = new Transaction();
            spending.AddInput(new TxIn(new OutPoint(tx, 0)));

            //Option 1 : Spender knows my birthdate and prove it in the ScriptSig
            Op pushBirthdate = Op.GetPushOp(birth);
            Op selectIf = OpcodeType.OP_1; //go to if
            Op redeemBytes = Op.GetPushOp(redeemScript.ToBytes());
            Script scriptSig = new Script(pushBirthdate, selectIf, redeemBytes);
            spending.Inputs[0].ScriptSig = scriptSig;

            //Verify the script pass
            var result = spending.Inputs.AsIndexedInputs().First()
                                 .VerifyScript(tx.Outputs[0].ScriptPubKey);
            Console.WriteLine(result); // True

            //Option 2 : Spender knows privateKey of the bitcoin address used
            BitcoinSecret secret = new BitcoinSecret("cNFXPwduvGRWJ89Wu5rtb3M32hC7j5x15E8F4WPof6qBh5Jrb7eQ");
            var sig = spending.SignInput(secret, scriptCoin);
            var p2pkhProof = PayToPubkeyHashTemplate.Instance.GenerateScriptSig(sig, secret.PrivateKey.PubKey);
            selectIf = OpcodeType.OP_0; //go to else
            scriptSig = p2pkhProof + selectIf + redeemBytes;
            spending.Inputs[0].ScriptSig = scriptSig;

            //Verify the script pass
            result = spending.Inputs.AsIndexedInputs().First()
                             .VerifyScript(tx.Outputs[0].ScriptPubKey);
            Console.WriteLine(result); // True
        }
    }
}
