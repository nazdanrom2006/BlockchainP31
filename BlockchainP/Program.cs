using System.ComponentModel;
using System.Data;
using BlockChain_P.Models;
using BlockChain_P.Services;
var blockChainService = new BlockChainService();
var blockChainDisplayService = new BlockChainDisplayService();
var transactionService = new TransactionService();
var networkService = new BlockNetworkService();

var walletService = new WalletService();

var aliceWallet = walletService.CreateWallet("Alice");
var bobWallet = walletService.CreateWallet("Bob");

//var hashingService = new HashingService();


//var miningService = new MiningService(hashingService);
/*
blockChainService.AddBlock("Alice sent Bob 100 coins");
blockChainService.AddBlock("Alice sent Bob 200 coins");
blockChainService.AddBlock("Alice sent Bob 100 coins");
blockChainService.AddBlock("Alice sent Bob 400 coins");

blockChainDisplayService.ShowBlockChain(blockChainService);
blockChainDisplayService.ShowValidationResult(blockChainService.isValid());

//blockChainService.Chain[4].TimeStamp = blockChainService.Chain[4].TimeStamp.AddHours(-2);
blockChainService.Chain[4].TimeStamp = blockChainService.Chain[4].TimeStamp.AddHours(2);

miningService.MineBlock(blockChainService.Chain[4], blockChainService.Chain[4].Target);

blockChainDisplayService.ShowBlockChain(blockChainService);
blockChainDisplayService.ShowValidationResult(blockChainService.isValid());
*/
Console.WriteLine("Awaiting port: ");
int awaitingPort = int.Parse(Console.ReadLine());

Console.WriteLine("Sending port: ");
int sendingPort = int.Parse(Console.ReadLine());

bool exit = false;
while (!exit)
{
    Console.WriteLine("Menu");
    Console.WriteLine("0. Load Chain");
    Console.WriteLine("1. Add Block");
    Console.WriteLine("2. Show BlockChain");
    Console.WriteLine("3. Validate BlockChain");
    Console.WriteLine("4. Add Transaction Alice to Bob");
    Console.WriteLine("5. Show Transaction Buffer");
    Console.WriteLine("6. Show Total Supply");
    Console.WriteLine("7. Hack Chain");
    Console.WriteLine("8. Exit");
    Console.WriteLine("9. Await Network Block");
    Console.WriteLine("9.1. Await Network Transaction");
    Console.WriteLine("9.2. Await Network BlockChain");
    Console.WriteLine("10. Send Network Block");
    Console.WriteLine("10.1. Send Network Transation");
    Console.WriteLine("10.2. Send Network BlockChain");

    string choice = Console.ReadLine();

    switch(choice){
        case "0":
            blockChainService.LoadChain();
            break;
        case "1":
            Console.WriteLine("Block added");
            blockChainService.MinePendingTransactions(aliceWallet.Address);
            break;
        case "2":
            blockChainDisplayService.ShowBlockChain(blockChainService);
            break;
        case "3":
            blockChainDisplayService.ShowValidationResult(blockChainService.isValid());
            break;
        case "4":
            Console.Write("Fee: ");
            decimal fee = decimal.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.Write("Amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());
            var newTransaction = transactionService.CreateTransaction(aliceWallet.Address, bobWallet.Address, amount, fee, aliceWallet);
            blockChainService.AddTransaction(newTransaction);
            break;
        case "5":
            blockChainDisplayService.ShowTransactions(blockChainService.pendingTransactions);
            break;
        case "6":
            Console.WriteLine($"Total suply:  {blockChainService.GetTotalSupply()}");
            break;
        case "7":
            for (int i = 0; i < blockChainService.Chain.Count(); i++){
                for (int j = 0; j < blockChainService.Chain[i].Transactions.Count; j++){
                    if (blockChainService.Chain[i].Transactions[j].From == "Coinbase"){
                        blockChainService.Chain[i].Transactions[j].Amount = 10000;
                    }
                }
            }
            break;
        case "8":
            blockChainService.SaveChain();
            exit = true;
                    break;
        case "9":
            {
                Console.WriteLine();
                var recivedBlock = await networkService.ReciveBlockAsync(awaitingPort);
                var success = blockChainService.TryAddBlockFromNetwork(recivedBlock);
                Console.WriteLine(success.errorMessage);
            }
            break;
        case "9.1":
            {
                Console.WriteLine();
                var recivedTransation = await networkService.ReciveTransactionAsync(awaitingPort);
                blockChainService.AddTransaction(recivedTransation);
            }
            break;
        case "9.2":
            {
                Console.WriteLine();
                var recivedBlockChain = await networkService.ReciveBlockChainAsync(awaitingPort);
                var success = blockChainService.CopyBlockChain(recivedBlockChain);
                Console.WriteLine(success.message);
            }
            break;
        case "10":
            {
                Console.WriteLine();
                var lastBlock = blockChainService.Chain.Last();
                await networkService.SendBlockAsync(lastBlock, "127.0.0.1", sendingPort);
            }
            break;
        case "10.1":
            {
                Console.WriteLine();
                var lastTransaction = blockChainService.pendingTransactions.Last();
                await networkService.SendTransactionAsync(lastTransaction, "127.0.0.1", sendingPort);
            }
            break;
        case "10.2":
            {
                Console.WriteLine();
                await networkService.SendBlockChainAsync(blockChainService, "127.0.0.1", sendingPort);
            }
            break;
    }
        


    }