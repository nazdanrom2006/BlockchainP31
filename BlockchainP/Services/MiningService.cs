using BlockchainP.Services;
using BlockchainP.Models;
namespace BlockchainP.Services {
	public class MiningService{
		private readonly HashingService _hashingService;

		public MiningService(HashingService hashingService){
			_hashingService = hashingService;
		}
		public long MineBlock(Block block, int difficulty){
			string target = new string('0', difficulty);
			block.Hash = _hashingService.ComputeHash(block);
			while(!block.Hash.StartsWith(target)){
				block.Nonce++;
				block.Hash = _hashingService.ComputeHash(block);

				if (block.Nonce%10000 == 0){
					Console.Write('.');
				}
			}
			return block.Nonce;
		}
	}
}
