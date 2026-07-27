using BlockchainP.Models;
namespace BlockchainP.Services{
	public class HashingService{
		public string ComputeHash(Block block){
			var input = $"{block.Id}{block.TimeStamp.ToString("o")}{block.Author}{block.Data}{block.PrevHash}{block.Nonce}";
			return ComputeHash(input);
		}
		public string ComputeHash(string input){
			using(var sha = System.Security.Cryptography.SHA256.Create()){
				var bytes = System.Text.Encoding.UTF8.GetBytes(input);
				var hashBytes = sha.ComputeHash(bytes);
				return BitConverter.ToString(hashBytes).Replace("-","").ToLower();
			}
		}
	}
}
