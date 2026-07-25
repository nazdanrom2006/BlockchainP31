using BlockchainP.Models;
namespace BlockchainP.Services{
	public class BlockChainService{
		public List<Block> Chain{get; set;}
		private readonly HashingService _hashingService;

		public BlockChainService(){
			_hashingService = new HashingService();
			Chain = new List<Block>();
			CreateGenesisBlock();
		}
		private void CreateGenesisBlock(){
			var genesisBlock = new Block(0, "Blockchain", "Genesis block", string.Empty, DateTime.UtcNow);

			genesisBlock.Hash = _hashingService.ComputeHash(genesisBlock);
			Chain.Add(genesisBlock);
		}
		public void AddBlock(string author, string data){
			var prevBlock = Chain.Last();
			var newIndex = prevBlock.Id + 1;
			var newTimeStamp = DateTime.UtcNow;
			var newPrevHash = prevBlock.Hash;
			var newBlock = new Block(newIndex, author, data, newPrevHash, newTimeStamp);
			newBlock.Hash = _hashingService.ComputeHash(newBlock);
			Chain.Add(newBlock);
		}
		public bool IsValid(){
			for (int i = 1; i < Chain.Count; i++){
				var currentBlock = Chain[i];
				var prevBlock = Chain[i-1];
				if(currentBlock.Hash != _hashingService.ComputeHash(currentBlock)){
					return false;
				}
				if (currentBlock.PrevHash != prevBlock.Hash){
					return false;
				}
			}
			return true;
		}
	}
}
