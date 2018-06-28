using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.RPC;
using NBXplorer;
using NBXplorer.DerivationStrategy;

namespace ConsoleApp.Wallet
{
    class BlockExplorer
    {
        public static void Execute()
        {
            // To setup NBXplorer's, you need a fully synced bitcoind node with default parameters.
            // Then clone and run NBXplorer with default parameters.
            // Reference the NBXplorer.Client nuget package then you need to notify the NBXplorer 
            // to track the user wallet:
            var network = new NBXplorerNetworkProvider(NetworkType.Mainnet).GetBTC();
            var userExtKey = new ExtKey();
            var userDerivationScheme = network.DerivationStrategyFactory.CreateDirectDerivationStrategy(userExtKey.Neuter(), new DerivationStrategyOptions()
            {
                // Use non-segwit
                Legacy = true
            });

            ExplorerClient client = new ExplorerClient(network);
            client.Track(userDerivationScheme);

            // query the UTXOs of your user and spend them the following way
            var utxos = client.GetUTXOs(userDerivationScheme, null, false);

            // If you want to spend those UTXOs:
            var coins = utxos.GetUnspentCoins();
            var keys = utxos.GetKeys(userExtKey);
            var changeAddress = new Key();
            TransactionBuilder builder = new TransactionBuilder();
            builder.AddCoins(coins)
                   .AddKeys(keys)
                   .Send(new Key(), Money.Coins(0.5m))
                   .SetChange(changeAddress.ScriptPubKey);

            // Set the fee rate
            var fallbackFeeRate = new FeeRate(Money.Satoshis(100), 1);
            var feeRate = client.GetFeeRate(1, fallbackFeeRate).FeeRate;
            builder.SendEstimatedFees(feeRate);
            /////

            var tx = builder.BuildTransaction(sign: true);
            Console.WriteLine(client.Broadcast(tx));

            // To prevent this problem of broadcasting the same transaction twice,
            // you need to make sure to not spend twice the same coins.
            // A way to solve the problem is by simply retrying:
            while (true)
            {
                coins = utxos.GetUnspentCoins();
                keys = utxos.GetKeys(userExtKey);
                builder = new TransactionBuilder();
                builder.AddCoins(coins)
                       .AddKeys(keys)
                       .Send(new Key(), Money.Coins(0.5m))
                       .SetChange(changeAddress.ScriptPubKey);

                // Set the fee rate
                fallbackFeeRate = new FeeRate(Money.Satoshis(100), 1);
                feeRate = client.GetFeeRate(1, fallbackFeeRate).FeeRate;
                builder.SendEstimatedFees(feeRate);
                /////

                tx = builder.BuildTransaction(true);
                var result = client.Broadcast(tx);
                if (result.Success)
                {
                    Console.WriteLine("Success!");
                    break;
                }
                else if (result.RPCCode.HasValue && result.RPCCode.Value == RPCErrorCode.RPC_TRANSACTION_REJECTED)
                {
                    Console.WriteLine("We probably got a conflict, let's try again!");
                    continue;
                }
                else
                {
                    Console.WriteLine($"Something is really wrong {result.RPCCode} {result.RPCCodeMessage} {result.RPCMessage}");
                    // Do something!!!
                }
            }

            // Another common way is to have a global list of already used outpoint that you can check against.
        }
    }
}
