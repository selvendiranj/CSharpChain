using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace ConsoleApp
{
    class ProofOfOwnership
    {
        public static void Execute()
        {
            var bitcoinSecret = new BitcoinSecret("cUyeaNoyfYQYC43sxozcyUvwhP6AHQK9ajBqtV23MbdEDxTuxgkT");

            string message = "I am Craig Wright";
            string signature = bitcoinSecret.PrivateKey.SignMessage(message);
            Console.WriteLine(signature);
            // IN5v9+3HGW1q71OqQ1boSZTm0/DCiMpI8E4JB1nD67TCbIVMRk/e3KrTT9GvOuu3NGN0w8R2lWOV2cxnBp+Of8c=

            // first ever bitcoin address, associated with the 
            // genesis block: 1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa and verify 'Craig Wright' claim
            message = "I am Craig Wright";
            signature = "IN5v9+3HGW1q71OqQ1boSZTm0/DCiMpI8E4JB1nD67TCbIVMRk/e3KrTT9GvOuu3NGN0w8R2lWOV2cxnBp+Of8c=";

            var address = new BitcoinPubKeyAddress("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa");
            bool isCraigWrightSatoshi = address.VerifyMessage(message, signature);

            Console.WriteLine("Is Craig Wright Satoshi? " + isCraigWrightSatoshi);

            /*
            prove you are the owner of an address without moving coins
            Address:1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB
            Message:Nicolas Dorier Book Funding Address
            Signature:H1jiXPzun3rXi0N9v9R5fAWrfEae9WPmlL5DJBj1eTStSvpKdRR8Io6/uT9tGH/3OnzG6ym5yytuWoA9ahkC3dQ=
            */

            var address2 = "1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB";
            var message2 = "Nicolas Dorier Book Funding Address";
            var signature2 = "H1jiXPzun3rXi0N9v9R5fAWrfEae9WPmlL5DJBj1eTStSvpKdRR8Io6/uT9tGH/3OnzG6ym5yytuWoA9ahkC3dQ=";

            //Verify that Nicolas sensei is not lying!
            var bcAddress = new BitcoinPubKeyAddress(address2);
            bool isNicolasDorier = bcAddress.VerifyMessage(message2, signature2);
            

            Console.WriteLine("Is isNicolasDorier Signature? " + isNicolasDorier);
        }
    }
}
