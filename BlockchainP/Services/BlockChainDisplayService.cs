using BlockchainP.Models;
using BlockchainP.Services;

namespace BlockchainP.Services{
	public class BlockChainDisplayService{
		public void ShowBlockChain(List<Block> chain){
			foreach (var block in chain){
				Console.WriteLine($"Index: {block.Id}");
				Console.WriteLine($"Timestamp: {block.TimeStamp}");
				Console.WriteLine($"Author: {block.Author}");
				
				Console.WriteLine($"Data: {block.Data}");
				Console.WriteLine($"Hash: {block.Hash}");
				Console.WriteLine($"Previous hash: {block.PrevHash}");
				Console.WriteLine(new string('-', 50));
			}
		}
		public void ShowValidationResult(bool isValid){
			if(isValid){
				Console.WriteLine("Blockchain is valid");
			}
			else{
				Console.WriteLine("Blovkchain is NOT valid");
			}
		}
	}
}
