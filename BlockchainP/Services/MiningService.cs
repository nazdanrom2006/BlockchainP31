using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using BlockChain_P.Models;

namespace BlockChain_P.Services
{
    public class MiningService{
        private readonly HashingService _hashingService;

        public MiningService(HashingService hashingService){
            _hashingService = hashingService;
        }
        public long MineBlock(Block block, int difficulty){
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                block.Nonce++;
                block.Hash = _hashingService.ComputeHash(block);
                
                if (block.Nonce % 50000 == 0){
                    Console.Write($"\rSpeed: {block.Nonce/stopwatch.Elapsed.TotalSeconds} H/s");
                }
                if (block.Hash.StartsWith(new String('0', difficulty))){
                    Console.WriteLine(BigInteger.Parse(block.Hash, NumberStyles.AllowHexSpecifier));
                    break;
                }
            }
            stopwatch.Stop();
            block.MiningDuration = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"\nNonce: {block.Nonce}, Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
            return block.Nonce;
        }
        /*public long MineBlockParallel(Block block, string targetPrefix){
            Parallel.For(0, long.MaxValue, nonce =>
            {
                block.Nonce = nonce;
                block.Hash = _hashingService.ComputeHash(block);
                if (block.Hash.StartsWith(targetPrefix)){
                    // ParallelLoopState.Stop();
                }
            });
            return block.Nonce;
        }
        */
    }
}