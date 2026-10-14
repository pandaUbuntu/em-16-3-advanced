using System;
using System.Text;

namespace ATM
{
    internal class BankCard
    {
        private string cardNumber;
        private string pin;
        private decimal balance;
        private string ownerName;
        private bool isBlocked;

        public BankCard(string cardNumber, string pin, decimal balance, string ownerName)
        {
            this.cardNumber = cardNumber;
            this.pin = pin;
            this.balance = balance;
            this.ownerName = ownerName;
            this.isBlocked = false;
        }

        public string GetCardNumber() => cardNumber;
        public string GetPin() => pin;
        public decimal GetBalance() => balance;
        public string GetOwnerName() => ownerName;

        public void BlockCard()
        {
            isBlocked = true;
        }

        public bool WithdrawMoney(decimal amount)
        {
            if (amount <= balance)
            {
                balance -= amount;
                return true;
            }
            return false;
        }
    }
}
