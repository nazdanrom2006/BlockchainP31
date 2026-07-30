using BlockchainP.Services;
using BlockchainP.Models;
using System.Diagnostics;

namespace BlockchainP.Services {
	public class MiningService{
		private readonly HashingService _hashingService;

		public MiningService(HashingService hashingService){
			_hashingService = hashingService;
		}
		public long MineBlock(Block block, int difficulty){
			string target = new string('0', difficulty);
			var startTime = Stopwatch.StartNew();
			block.Hash = _hashingService.ComputeHash(block);
			while(!block.Hash.StartsWith(target)){
				block.Nonce++;
				block.Hash = _hashingService.ComputeHash(block);

				if (block.Nonce%10000 == 0){
					Console.Write('.');
				}
			}
			startTime.Stop();
			block.MiningDuration = startTime.ElapsedMilliseconds;
			return block.Nonce;
		}
	}
}
