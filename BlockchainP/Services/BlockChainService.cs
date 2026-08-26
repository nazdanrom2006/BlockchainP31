using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using BlockChain_P.Models;
using BlockChain_P.Services;
namespace BlockChain_P.Services{
    public class BlockChainService{
        public List<Block> Chain { get; set; }
        public List<Transaction> pendingTransactions { get; set; } = new List<Transaction>();
        private readonly HashingService _hashingService;
        private readonly MiningService _miningService;
        private readonly TransactionService _transactionService;
        public int Difficulty { get; set; }
        public readonly int _initialDifficulty = 4;
        private readonly int _targetTimePerBlock = 5000;
        private readonly int _adjustmentInterval = 3;
        private readonly int _halvingInterval = 210000;
        private readonly decimal _rewardAmount = 50;
        private readonly int _maxBlockSize = 2;
        private readonly string _chainFilePath = "chain.json";
        public BlockChainService(){
            Difficulty = _initialDifficulty;
            _hashingService = new HashingService();
            _miningService = new MiningService(_hashingService);
            _transactionService = new TransactionService();
            Chain = new List<Block>();
            CreateGenesisBlock();
        }

        private void CreateGenesisBlock(){
            var genesisBlock = new Block(0, DateTime.UnixEpoch, new List<Transaction>(), string.Empty, Difficulty, string.Empty);

            _miningService.MineBlock(genesisBlock, Difficulty);
            Chain.Add(genesisBlock);
        }
        public decimal GetBalance(string walletAddress){
            decimal balance = 0;
            foreach (var block in Chain){
                foreach (var transaction in block.Transactions){
                    if (transaction.From == walletAddress)
                    {
                        balance -= transaction.Amount + transaction.Fee;
                    }
                    else if (transaction.To == walletAddress){
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }
        public void AddTransaction(Transaction transaction){
            if (!_transactionService.ValidateTransaction(transaction).isValid){
                throw new Exception("Invalid Transaction");
            }
            if (pendingTransactions.Any(t => t.Id == transaction.Id)){
                Console.WriteLine("Transaction with this ID already exists");
                return;
            }
            var Fee = pendingTransactions.Sum(pendingTransaction => pendingTransaction.From == transaction.From? pendingTransaction.Amount + pendingTransaction.Fee : 0); //from pendingTransaction in pendingTransactions where pendingTransaction.From == transaction.From select pendingTransaction.Amount;
            var senderBalance = GetBalance(transaction.From);
            if (senderBalance < Fee + transaction.Amount + transaction.Fee){
                throw new Exception($"Insufficient balance for transaction from {transaction.From} to {transaction.To} with amount {transaction.Amount}");
            }
            pendingTransactions.Add(transaction);
        }
        public void MinePendingTransactions(string minerAddress){
            Dictionary<string, decimal> pendingBalances = new Dictionary<string, decimal>();
            /*foreach (Transaction transaction in pendingTransactions)
            {
                if (!pendingBalances.ContainsKey(transaction.From)){
                    pendingBalances.Add(transaction.From, GetBalance(transaction.From));
                }
                if (_transactionService.ValidateTransaction(transaction).isValid == false)
                {
                    throw new Exception("Invalid transaction");
                }
                if (pendingBalances[transaction.From] < transaction.Amount){
                    throw new Exception($"Insufficient balance for transaction from {transaction.From} to {transaction.To} with amount {transaction.Amount}");
                }
                pendingBalances[transaction.From] -= transaction.Amount;
            }*/
            var transactionCopy = pendingTransactions.OrderByDescending(t => t.Fee).Take(_maxBlockSize).ToList();
            

            var prevBlock = Chain.Last();
            var newIndex = prevBlock.Index + 1;
            var newTimeStamp = DateTime.UtcNow;
            var newPrevHash = prevBlock.Hash;


            var totalFee = transactionCopy.Sum(t => t.Fee);
            var transactionReward = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                From = "Coinbase",
                To = minerAddress,
                Amount = GetCurrentReward() + totalFee,
                TimeStamp = DateTime.UtcNow
            };
            transactionCopy.Add(transactionReward);

            string MerkleRoot = CalculateMerkleRoot(transactionCopy);
            var newBlock = new Block(newIndex, newTimeStamp, transactionCopy, newPrevHash, Difficulty, MerkleRoot);

            _miningService.MineBlock(newBlock, Difficulty);
            Chain.Add(newBlock);
            foreach(var i in transactionCopy){
                pendingTransactions.Remove(i);
            }
            if (newIndex %_adjustmentInterval == 0){
                AdjustDifficulty();
            }
        }
        private void AdjustDifficulty(){
            var recentBlocks = Chain.Skip(Chain.Count - _adjustmentInterval).Take(_adjustmentInterval).ToList();
            var avgTime = recentBlocks.Average(b => b.MiningDuration);
            var newCoefficient = avgTime / _targetTimePerBlock;
            Difficulty += (newCoefficient < 1 ? 1 : -1);
        }
        public bool IsValidChain(List<Block> chain){
            for (int i = 1; i < chain.Count; i++){
                var currentBlock = chain[i];
                var prevBlock = chain[i - 1];
                foreach (var transaction in currentBlock.Transactions){
                    if (transaction.From == "Coinbase"){
                        int pastHalvings = currentBlock.Index / _halvingInterval;
                        decimal expectedReward = _rewardAmount / (decimal)Math.Pow(2, pastHalvings);
                        decimal feeReward = chain[i].Transactions.Sum(t => t.Fee);
                        Console.WriteLine(feeReward);
                        if (transaction.Amount != expectedReward + feeReward){
                            Console.WriteLine("Coinbase");
                            return false;
                        }
                    }
                }
                if (currentBlock.Hash != _hashingService.ComputeHash(currentBlock)){
                    
                            Console.WriteLine("Wrong hash");
                            Console.WriteLine(currentBlock.Hash);
                            Console.WriteLine(_hashingService.ComputeHash(currentBlock));
                    return false;
                }
                if (currentBlock.PrevHash != prevBlock.Hash){

                            Console.WriteLine("Wrong prev hash");
                    return false;
                }
                if(!currentBlock.Hash.StartsWith(new String('0', currentBlock.Difficulty))){
                            Console.WriteLine("Wrong POW");
                    return false;
                }
                if(currentBlock.TimeStamp.CompareTo(prevBlock.TimeStamp) <= 0){

                            Console.WriteLine("Timestamp older than prev block");
                    return false;
                }
                if(currentBlock.TimeStamp.CompareTo(DateTime.UtcNow.AddMinutes(2)) > 0){
                    Console.WriteLine("Block from future");
                    return false;
                }
                if (currentBlock.MerkleRoot != CalculateMerkleRoot(currentBlock.Transactions)){
                    Console.WriteLine("Invalid Merkle Root");
                    return false;
                }
            }
            return true;
        }
        public bool isValid(){
            return IsValidChain(Chain);
        }
        public decimal GetTotalSupply(){
            decimal result = 0;
            foreach (var block in Chain)
            {
                foreach (var transaction in block.Transactions){
                    if (transaction.From == "Coinbase"){
                        result += transaction.Amount;
                    }
                }   
            }
            return result;
        }
        public decimal GetCurrentReward(){
            var halvingCount = Chain.Count / (int)_halvingInterval;
            return _rewardAmount / (decimal)Math.Pow(2, halvingCount);
        }
        public void SaveChain(){
            var jsonOption = new JsonSerializerOptions();
            var json = JsonSerializer.Serialize(Chain, jsonOption);

            File.WriteAllText(_chainFilePath, json);
        }
        public void LoadChain(){
            if (File.Exists(_chainFilePath)){
                var json = File.ReadAllText(_chainFilePath);
                if (String.IsNullOrEmpty(json)){
                    return;
                }
                var loadChain = JsonSerializer.Deserialize<List<Block>>(json) ?? new List<Block>();

                if (loadChain.Count == 0){
                    return;
                }
                if (IsValidChain(loadChain)){
                    Chain = loadChain;
                }
                else{
                    return;
                }
            }
        }
        public (bool accepted, string errorMessage) TryAddBlockFromNetwork(Block block){
            var latestBlock = Chain.Last();

            if (block.Index != latestBlock.Index + 1){
                return (false, $"Invalid Index: Expected {latestBlock.Index + 1}, Actual: {block.Index}");
            }
            if (block.PrevHash != latestBlock.Hash){
                return (false, $"Invalid PrevHash: Expected {latestBlock.Hash}, Actual: {block.PrevHash}");
            }
            if (block.Hash != _hashingService.ComputeHash(block)){
                return (false, $"Invalid hash: Expected {_hashingService.ComputeHash(block)}, Actual: {block.Hash}");
            }
            if(!block.Hash.StartsWith(new String('0', block.Difficulty))){
                return (false, "wrong POW");
            }
            if (Chain.Any(b => b.Hash == block.Hash)){
                return (false, $"Block with hash {block.Hash} already exists");
            }
            foreach (var transaction in block.Transactions){
                var validationResult = _transactionService.ValidateTransaction(transaction);
                if (!validationResult.isValid){
                    return (false, $"Invalid transaction in block: {validationResult.errorMessage}");
                }
            }

            if (block.Transactions.FindAll(t => t.From == "Coinbase").Count != 1){
                return (false, $"Block must contain exactly 1 reward transaction");
            }


            foreach(var transaction in block.Transactions){
                if(pendingTransactions.Contains(transaction)){
                    pendingTransactions.Remove(transaction);
                }
            }
            if (block.MerkleRoot != CalculateMerkleRoot(block.Transactions)){
                return (false, $"Invalid Merkle Root: Expected {CalculateMerkleRoot(block.Transactions)}, Actual: {block.MerkleRoot}");
            }
            Chain.Add(block);
            return (true, "Block is valid and accepted");
        }
        public (bool accepted, string message) CopyBlockChain(BlockChainService blockChain){
            if (!blockChain.isValid()){
                return (false, "Recived Blockchain is not valid");
            }
            if (blockChain.Chain.Count() <= Chain.Count()){
                return (false, "Recived Blockchain should be longer than local");
            }
            Chain = blockChain.Chain;
            return (true, "BlockChain Accepted");
        }
        public string CalculateMerkleRoot(List<Transaction> transactions){
            List<string> MerkleTree = new List<string>();
            foreach (var transaction in transactions){
                MerkleTree.Add(_hashingService.ComputeHash(transaction.ToString()));
            }
            while (MerkleTree.Count() != 1){
                List<string> newMerkleTree = new List<string>();
                for (int i = 0; i < MerkleTree.Count(); i += 2){
                    newMerkleTree.Add(_hashingService.ComputeHash(MerkleTree[i] + MerkleTree[i + 1]));
                }
                MerkleTree = newMerkleTree;
            }
            return MerkleTree[0];
        }
    }
}