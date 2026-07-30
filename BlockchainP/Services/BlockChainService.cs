using BlockchainP.Models;
using BlockchainP.Services;
namespace BlockchainP.Services{
	public class BlockChainService{
		public List<Block> Chain{get; set;}
		private readonly HashingService _hashingService;
		private readonly MiningService _miningService;

		public int Difficulty {get; set;} = 2;
		private readonly int _targetTimePerBlock = 3000;
		private readonly int _adjustmentInterval = 4;

		public BlockChainService(){
			_hashingService = new HashingService();
			_miningService = new MiningService(_hashingService);
			Chain = new List<Block>();
			CreateGenesisBlock();
		}
		private void CreateGenesisBlock(){
			var genesisBlock = new Block(0, "Blockchain", "Genesis block", string.Empty, DateTime.UtcNow, Difficulty);

			_miningService.MineBlock(genesisBlock, Difficulty);
			Chain.Add(genesisBlock);
		}
		public void AddBlock(string author, string data){
			var prevBlock = Chain.Last();
			var newIndex = prevBlock.Id + 1;
			var newTimeStamp = DateTime.UtcNow;
			var newPrevHash = prevBlock.Hash;
			var newBlock = new Block(newIndex, author, data, newPrevHash, newTimeStamp, Difficulty);
			_miningService.MineBlock(newBlock, Difficulty);
			Chain.Add(newBlock);

			if (newIndex % _adjustmentInterval == 0){
				AdjustDifficulty();
			}
		}
		private void AdjustDifficulty(){
			var recentBlocks = Chain.Skip(Chain.Count - _adjustmentInterval).Take(_adjustmentInterval).ToList();
			var avgTime = recentBlocks.Average(b => b.MiningDuration);
			Difficulty += (avgTime < _targetTimePerBlock ? 1 : -1);
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
				if (!currentBlock.Hash.StartsWith(new string('0', currentBlock.Difficulty))){
					return false;
				}
			}
			return true;
		}
	}
}
