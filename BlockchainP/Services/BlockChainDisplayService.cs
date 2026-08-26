using System.Collections.Generic;
using BlockChain_P.Models;

namespace BlockChain_P.Services{
    public class BlockChainDisplayService
    {
        public void ShowBlockChain(BlockChainService service)
        {
            var chain = service.Chain;
            foreach (var block in chain)
            {
                Console.WriteLine($"Index: {block.Index}");
                Console.WriteLine($"Timestamp: {block.TimeStamp}");
                Console.WriteLine($"Hash: {block.Hash}");
                Console.WriteLine($"Previous Hash: {block.PrevHash}");
                Console.WriteLine($"Nonce: {block.Nonce}");
                Console.WriteLine($"Difficulty: {block.Difficulty}");

                Console.WriteLine("Transanctions:");
                ShowTransactions(block.Transactions);
                Console.WriteLine(new string('-', 50));
            }
        }

        public void ShowTransactions(List<Transaction> transactions){
            foreach(var transaction in transactions){
                Console.WriteLine(new string('-', 30));
                Console.WriteLine($"Transaction ID: {transaction.Id}");
                Console.WriteLine($"From: {transaction.From}");
                Console.WriteLine($"To: {transaction.To}");
                Console.WriteLine($"Amount: {transaction.Amount}");
                Console.WriteLine($"Fee: {transaction.Fee}");
                Console.WriteLine($"Timestamp: {transaction.TimeStamp}");
            }
        }

        public void ShowValidationResult(bool isValid)
        {
            Console.WriteLine(isValid ? "The blockchain is valid." : "The blockchain is NOT valid.");
        }
        public void PrintAccountStatement(BlockChainService blockChain, string address){
            decimal moneyGot = 0;
            decimal moneySpent = 0;
            int transactionCount = 0;
            foreach (var block in blockChain.Chain){
                foreach (var transaction in block.Transactions){
                    if (transaction.From == address){
                        moneySpent += transaction.Amount + transaction.Fee;
                        transactionCount++;
                    }
                    else if (transaction.To == address){
                        moneyGot += transaction.Amount;
                        transactionCount++;
                    }
                }
            }
            Console.WriteLine($"Money Spent: {moneySpent}");
            Console.WriteLine($"Money Got: {moneyGot}");
            Console.WriteLine($"Balance: {moneyGot - moneySpent}");
            Console.WriteLine($"Overall transactions: {transactionCount}");
        }
    }
}