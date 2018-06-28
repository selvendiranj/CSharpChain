using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.RPC;

namespace ConsoleApp.Wallet
{
    class FullNode
    {
        public static void Execute()
        {
            // After Initial Blockchain Downloading (IBD,) which can take days, start bitcoind.
            // Then use NBitcoin's RPCClient class to manage your wallet.
            RPCClient client = new RPCClient(Network.Main);
            Console.WriteLine(client.GetNewAddress()); // Generate a new address
            Console.WriteLine(client.GetBalance()); // Get the balance
        }
    }
}
