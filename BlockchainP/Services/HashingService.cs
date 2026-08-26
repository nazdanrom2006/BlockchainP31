using System;
using System.Numerics;
using System.Text;
using BlockChain_P.Models;

namespace BlockChain_P.Services{
    public class HashingService
    {
        public string ComputeHash(Block block)
        {
            string transactionsRow = string.Concat(block.Transactions.Select(t => t.ToRowString()));
            var input = $"{block.Index}{block.TimeStamp.ToString("o")}{transactionsRow}{block.PrevHash}{block.Nonce}{block.Difficulty}";
            return ComputeHash(input);
        }

        public string ComputeHash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-","").ToLower();
            }
        }
    }
}