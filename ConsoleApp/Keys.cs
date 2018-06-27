using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class Pair
    {
        string address;
        string privateKey;

        public string Address { get => address; set => address = value; }
        public string PrivateKey { get => privateKey; set => privateKey = value; }
    }
    class Keys
    {
        public Pair bob;
        public Pair alice;
        public Pair satoshi;
        public Pair selven;
    }
}
