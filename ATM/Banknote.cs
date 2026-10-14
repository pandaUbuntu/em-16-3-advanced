using System;
using System.Text;

namespace ATM
{
    internal class Banknote
    {
        public int Denomination { get; private set; }
        public int Count { get; set; }

        public Banknote(int denomination)
        {
            Denomination = denomination;
            Count = 0;
        }
    }
}
