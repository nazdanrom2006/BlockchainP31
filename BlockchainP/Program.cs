using BlockchainP.Services;

var blockChainService = new BlockChainService();
var blockChainDisplayService = new BlockChainDisplayService();
var hashingService = new HashingService();
/*
blockChainService.AddBlock("Alice", "sent Bob 100 coins");
blockChainService.AddBlock("Bob", "sent Martha 50 coins");
blockChainService.AddBlock("Jack", "sent Alice 20 coins");
blockChainService.AddBlock("Sam", "sent Bob 20 coins");
blockChainService.AddBlock("Alice", "sent Sam 40 coins");
blockChainService.AddBlock("Jack", "sent Frank 80 coins");
blockChainService.AddBlock("Frank", "sent Sam 20 coins");
*/
for (int i = 0; i < 18; i++){
	blockChainService.AddBlock("Alice", $"sent Bob {i} coins");
	Console.WriteLine($"block {i} mined");
}
blockChainDisplayService.ShowBlockChain(blockChainService.Chain);
blockChainDisplayService.ShowValidationResult(blockChainService.IsValid());

Console.WriteLine();

/*
blockChainService.Chain[0].Data = "Coined Genesis block";
blockChainService.Chain[0].Hash = hashingService.ComputeHash(blockChainService.Chain[0]);
for (int i = 1; i < blockChainService.Chain.Count; i++){
	blockChainService.Chain[i].PrevHash = blockChainService.Chain[i - 1].Hash;
	blockChainService.Chain[i].Hash = hashingService.ComputeHash(blockChainService.Chain[i]);
}
blockChainDisplayService.ShowBlockChain(blockChainService.Chain);
blockChainDisplayService.ShowValidationResult(blockChainService.IsValid());
*/
