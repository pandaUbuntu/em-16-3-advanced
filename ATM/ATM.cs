using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATM
{
    internal class ATM
    {
        private BankCardManager cardManager;
        private List<Banknote> vault;
        private const int MaxBanknotesPerTransaction = 40;

        public ATM()
        {
            cardManager = new BankCardManager();
            vault = new List<Banknote>();
            InitializeVault();
        }

        private void InitializeVault()
        {
            Random rnd = new Random();
            int[] availableDenominations = { 1000, 500, 200, 100 };

            foreach (var denomination in availableDenominations)
            {
                vault.Add(new Banknote(denomination, rnd.Next(0, 101)));
            }
        }

        public BankCard ValidateCard(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length > 19 || !cardNumber.All(char.IsDigit))
            {
                return null;
            }

            BankCard card = cardManager.GetBankCard(cardNumber);

            if (card == null || card.IsBlocked)
            {
                return null;
            }

            return card;
        }

        public bool TryDispenseCash(BankCard card, int amount, out Dictionary<int, int> dispensedBanknotes, out string errorMessage)
        {
            dispensedBanknotes = new Dictionary<int, int>();
            errorMessage = string.Empty;

            if (amount <= 0 || amount % 100 != 0)
            {
                errorMessage = "Amount must be a multiple of 100 UAH.";
                return false;
            }

            if (card.GetBalance() < amount)
            {
                errorMessage = "Insufficient funds in your account.";
                return false;
            }

            int remainingAmount = amount;
            int totalBanknotesCount = 0;

            foreach (var banknote in vault.OrderByDescending(b => b.Denomination))
            {
                if (remainingAmount <= 0) break;

                int needed = remainingAmount / banknote.Denomination;
                int toTake = Math.Min(needed, banknote.Count);

                if (toTake > 0)
                {
                    dispensedBanknotes[banknote.Denomination] = toTake;
                    remainingAmount -= toTake * banknote.Denomination;
                    totalBanknotesCount += toTake;
                }
            }

            if (remainingAmount > 0)
            {
                errorMessage = "The ATM does not have the required banknotes to dispense this amount.";
                dispensedBanknotes.Clear();
                return false;
            }

            if (totalBanknotesCount > MaxBanknotesPerTransaction)
            {
                errorMessage = $"The ATM cannot dispense more than {MaxBanknotesPerTransaction} banknotes per transaction (required: {totalBanknotesCount}). Please request a smaller amount.";
                dispensedBanknotes.Clear();
                return false;
            }

            if (card.WithdrawMoney(amount))
            {
                foreach (var pair in dispensedBanknotes)
                {
                    var banknote = vault.First(b => b.Denomination == pair.Key);
                    banknote.Count -= pair.Value;
                }
                return true;
            }

            errorMessage = "Transaction failed.";
            return false;
        }

        public void DisplayVaultStatus()
        {
            Console.WriteLine("\n--- Current ATM Vault Status ---");
            foreach (var b in vault.OrderByDescending(x => x.Denomination))
            {
                Console.WriteLine($"Denomination {b.Denomination} UAH: {b.Count} pcs.");
            }
            Console.WriteLine("--------------------------------\n");
        }
    }
}
