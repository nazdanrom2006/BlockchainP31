namespace BlockchainP.Models {
	public class Block{
		public int Id{ get; set; }
		public string Author {get; set;}
		public string Data{ get; set; }
		public string Hash{ get; set; }
		public string PrevHash{ get; set; }
		public DateTime TimeStamp{ get; set; }
		public int Nonce { get; set; }
		public int Difficulty { get; set; }
		public double MiningDuration { get; set; }

		public Block(int id, string author, string data, string prevHash, DateTime timeStamp, int difficulty){
			Id = id;
			Author = author;
			Data = data;
			PrevHash = prevHash;
			TimeStamp = timeStamp;
			Nonce = 0;
			Difficulty = difficulty;
		}
	}
}
