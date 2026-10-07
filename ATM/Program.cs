using System;
using System.Collections.Generic;
using System.Text;

namespace ATM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ATM atm = new ATM();


            while (true)
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("            WELCOME TO THE ATM             ");
                Console.WriteLine("===========================================");
                Console.Write("Please enter your card number (or type 'exit' to quit): ");
                string cardNumber = Console.ReadLine().Trim();

                if (cardNumber?.ToLower() == "exit") break;

                BankCard currentCard = atm.ValidateCard(cardNumber);

                if (currentCard == null)
                {
                    Console.WriteLine("Error: Card not found or it is blocked.\n");
                    continue;
                }

                bool isAuthenticated = false;
                int pinAttempts = 3;

                while (pinAttempts > 0)
                {
                    Console.Write("Enter your PIN: ");
                    string enteredPin = Console.ReadLine()?.Trim();

                    if (enteredPin == currentCard.GetPin())
                    {
                        isAuthenticated = true;
                        break;
                    }
                    else
                    {
                        pinAttempts--;
                        if (pinAttempts > 0)
                        {
                            Console.WriteLine($"Incorrect PIN. Attempts remaining: {pinAttempts}");
                        }
                        else
                        {
                            currentCard.BlockCard();
                            Console.WriteLine("You have exceeded the maximum PIN attempts. Your card has been blocked!\n");
                        }
                    }
                }

                if (!isAuthenticated) continue;

                Console.WriteLine($"\nWelcome, {currentCard.GetOwnerName()}!");

                bool sessionActive = true;
                while (sessionActive)
                {
                    Console.WriteLine("\nPlease select an option:");
                    Console.WriteLine("1. Check Balance");
                    Console.WriteLine("2. Withdraw Cash");
                    Console.WriteLine("3. Exit & Retrieve Card");
                    Console.Write("Your choice: ");

                    int choice=Convert.ToInt32(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine($"\nYour current balance: {currentCard.GetBalance():F2} UAH");
                            break;

                        case 2:
                            Console.Write("\nEnter the amount to withdraw: ");
                            if (int.TryParse(Console.ReadLine(), out int amount))
                            {
                                if (atm.TryDispenseCash(currentCard, amount, out Dictionary<int, int> dispensed, out string error))
                                {
                                    Console.WriteLine("\nTransaction successful! Please take your cash:");
                                    foreach (var pair in dispensed)
                                    {
                                        Console.WriteLine($"- {pair.Key} UAH x {pair.Value} banknote(s)");
                                    }
                                    Console.WriteLine($"Remaining card balance: {currentCard.GetBalance():F2} UAH");
                                }
                                else
                                {
                                    Console.WriteLine($"\nTransaction declined: {error}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("\nInvalid amount entered.");
                            }
                            break;

                        case 3:
                            sessionActive = false;
                            Console.WriteLine("\nPlease take your card. Thank you for using our ATM!");
                            break;

                        default:
                            Console.WriteLine("\nInvalid option. Please try again.");
                            break;
                    }
                }

                Console.WriteLine("\nSession completed.\n");
                Console.WriteLine();
            }
        }
    }
}
