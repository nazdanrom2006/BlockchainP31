namespace BlockchainP.Models {
	public class Block{
		public int Id{ get; set; }
		public string Author {get; set;}
		public string Data{ get; set; }
		public string Hash{ get; set; }
		public string PrevHash{ get; set; }
		public DateTime TimeStamp{ get; set; }

		public Block(int id, string author, string data, string prevHash, DateTime timeStamp){
			Id = id;
			Author = author;
			Data = data;
			PrevHash = prevHash;
			TimeStamp = timeStamp;
		}
	}
}
