using System;
using System.Numerics;
using BlockChain_P.Models;
namespace BlockChain_P.Models{
	public class Block
	{
		public int Index {get; set;}
	public DateTime TimeStamp { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public string Hash { get; set; }
		public string PrevHash { get; set; }
		public long Nonce { get; set; }

		public double MiningDuration{ get; set; }

		public int Difficulty { get; set; }
        public string MerkleRoot { get; set; }
        public Block(int index, DateTime timeStamp, List<Transaction> transactions, string prevHash, int difficulty, string merkleRoot){
            Index = index;
            TimeStamp = timeStamp;
            Transactions = transactions;
            PrevHash = prevHash;
            Difficulty = difficulty;
            MerkleRoot = merkleRoot;
        }
        public Block() { }
    }
}
