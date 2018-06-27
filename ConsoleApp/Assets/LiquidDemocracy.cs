using NBitcoin;
using NBitcoin.OpenAsset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConsoleApp.Assets
{
    class LiquidDemocracy
    {
        public static void Execute()
        {
            // Issuing voting power
            // Let’s say that three persons are interested, Satoshi, Alice and Bob. (Yes, them again)
            // So Boss decides to sell each Power Coin at 0.1 BTC each.
            // Let’s start funding some money to the powerCoin address, satoshi, alice and bob.
            var powerCoin = new Key();
            var alice = new Key();
            var bob = new Key();
            var satoshi = new Key();
            var init = new Transaction()
            {
                Outputs =
                    {
                        new TxOut(Money.Coins(1.0m), powerCoin),
                        new TxOut(Money.Coins(1.0m), alice),
                        new TxOut(Money.Coins(1.0m), bob),
                        new TxOut(Money.Coins(1.0m), satoshi),
                    }
            };

            var repo = new NoSqlColoredTransactionRepository();
            repo.Transactions.Put(init);

            // Alice buy 2 Power coins
            var issuance = GetCoins(init, powerCoin)
                .Select(c => new IssuanceCoin(c))
                .ToArray();

            var builder = new TransactionBuilder();

            // You can note that I am double spending the coin of Alice from the init transaction.
            // **Such thing would not be accepted on the Blockchain
            var toAlice = builder.AddCoins(issuance)
                                 .AddKeys(powerCoin)
                                 .IssueAsset(alice, new AssetMoney(powerCoin, 2))
                                 .SetChange(powerCoin)
                                 .Then()
                                 .AddCoins(GetCoins(init, alice))
                                 .AddKeys(alice)
                                 .Send(powerCoin, Money.Coins(0.2m))
                                 .SetChange(alice)
                                 .BuildTransaction(true);
            repo.Transactions.Put(toAlice);

            // Running a vote, create some funds for votingCoin.
            var votingCoin = new Key();
            var init2 = new Transaction()
            {
                Outputs =
                    {
                        new TxOut(Money.Coins(1.0m), votingCoin),
                    }
            };
            repo.Transactions.Put(init2);
            
            // issue the voting coins.
            issuance = GetCoins(init2, votingCoin).Select(c => new IssuanceCoin(c)).ToArray();
            builder = new TransactionBuilder();
            var toVoters = builder.AddCoins(issuance)
                                  .AddKeys(votingCoin)
                                  .IssueAsset(alice, new AssetMoney(votingCoin, 1))
                                  .IssueAsset(satoshi, new AssetMoney(votingCoin, 1))
                                  .SetChange(votingCoin)
                                  .BuildTransaction(true);
            repo.Transactions.Put(toVoters);

            // Vote delegation

        }

        private static IEnumerable<Coin> GetCoins(Transaction tx, Key owner)
        {
            return tx.Outputs.AsCoins().Where(c => c.ScriptPubKey == owner.ScriptPubKey);
        }
    }
}
