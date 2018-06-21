using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin;
using NBitcoin.Stealth;

namespace ConsoleApp.Ownership
{
    class TxnBuilder
    {
        public static void Execute()
        {
            /*
            With the TransactionBuilder you can:
            -Spend any
            -P2PK, P2PKH,
            -multi - sig,
            -P2WPK, P2WSH.
            -Spend any P2SH on the previous redeem script.
            -Spend Stealth Coin(DarkWallet).
            -Issue and transfer Colored Coins(open asset, following chapter).
            -Combine partially signed transactions.
            -Estimate the final size of an unsigned transaction and its fees.
            -Verify if a transaction is fully signed.
            */

            //Goal: take Coins and Keys as input, and return back a signed or partially signed transaction

            /*
            The usage of the TransactionBuilder is done in four steps:
            -You gather the Coins that will be spent
            -You gather the Keys that you own
            -You enumerate how much Money you want to send to what scriptPubKey
            -You build and sign the transaction
            Optional: you give the transaction to somebody else, then he will sign or continue to build it
            */

            // Create a fake transaction
            Key bob = new Key();
            Key alice = new Key();

            PubKey[] multiSig = new PubKey[] { bob.PubKey, alice.PubKey };
            Script bobAliceMSig = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, multiSig);

            Transaction init = new Transaction();
            init.Outputs.Add(new TxOut(Money.Coins(1m), bob.PubKey)); // P2PK
            init.Outputs.Add(new TxOut(Money.Coins(1m), alice.PubKey.Hash)); // P2PKH
            init.Outputs.Add(new TxOut(Money.Coins(1m), bobAliceMSig));

            // Now let’s say they want to use the coins of this transaction to pay Satoshi.
            Key satoshi = new Key();

            // First they have to get their Coins.
            Coin[] coins = init.Outputs.AsCoins().ToArray();
            Coin bobCoin = coins[0];
            Coin aliceCoin = coins[1];
            Coin bobAliceCoin = coins[2];

            // let’s say bob wants to send 0.2 BTC, alice 0.3 BTC,
            // and they agree to use bobAlice to send 0.5 BTC
            TransactionBuilder builder = new TransactionBuilder();
            Transaction tx = builder.AddCoins(bobCoin)
                                    .AddKeys(bob)
                                    .Send(satoshi, Money.Coins(0.2m))
                                    .SetChange(bob)
                                    .Then()
                                    .AddCoins(aliceCoin)
                                    .AddKeys(alice)
                                    .Send(satoshi, Money.Coins(0.3m))
                                    .SetChange(alice)
                                    .Then()
                                    .AddCoins(bobAliceCoin)
                                    .AddKeys(bob, alice)
                                    .Send(satoshi, Money.Coins(0.5m))
                                    .SetChange(bobAliceMSig)
                                    .SendFees(Money.Coins(0.0001m))
                                    .BuildTransaction(sign: true);

            // you can verify it is fully signed and ready to send to the network.
            Console.WriteLine("Verify Txn is fully Signed: " + builder.Verify(tx)); // True

            // it works the same way for P2SH, P2WSH, P2SH(P2WSH), and P2SH(P2PKH)
            // except you need to create ScriptCoin
            init = new Transaction();
            init.Outputs.Add(new TxOut(Money.Coins(1.0m), bobAliceMSig.Hash));

            coins = init.Outputs.AsCoins().ToArray();
            ScriptCoin bobAliceScriptCoin = coins[0].ToScriptCoin(bobAliceMSig);

            // Then the signature:
            builder = new TransactionBuilder();
            tx = builder.AddCoins(bobAliceScriptCoin)
                        .AddKeys(bob, alice)
                        .Send(satoshi, Money.Coins(0.9m))
                        .SetChange(bobAliceMSig.Hash)
                        .SendFees(Money.Coins(0.0001m))
                        .BuildTransaction(true);

            Console.WriteLine("Verify Txn is fully Signed: " + builder.Verify(tx)); // True

            // Stealth Coin - you need a ScanKey to see the StealthCoin
            // create darkAliceBob stealth address as in previous chapter:
            Key scanKey = new Key();
            BitcoinStealthAddress darkAliceBob =
                new BitcoinStealthAddress
                    (
                        scanKey: scanKey.PubKey,
                        pubKeys: new[] { alice.PubKey, bob.PubKey },
                        signatureCount: 2,
                        bitfield: null,
                        network: Network.Main
                    );

            // Someone sent to darkAliceBob
            init = new Transaction();
            darkAliceBob.SendTo(init, Money.Coins(1.0m));
            // scanner will detect the StealthCoin:
            // Get the stealth coin with the scanKey
            StealthCoin stealthCoin = StealthCoin.Find(init, darkAliceBob, scanKey);

            // And forward it to bob and alice, who will sign:
            // Spend it
            tx = builder.AddCoins(stealthCoin)
                        .AddKeys(bob, alice, scanKey)
                        .Send(satoshi, Money.Coins(0.9m))
                        .SetChange(bobAliceMSig.Hash)
                        .SendFees(Money.Coins(0.0001m))
                        .BuildTransaction(true);

            Console.WriteLine("Verify Txn is fully Signed: " + builder.Verify(tx)); // True
            // Note: You need the scanKey for spending a StealthCoin
        }
    }
}
