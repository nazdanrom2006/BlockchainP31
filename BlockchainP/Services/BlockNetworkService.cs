using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using BlockChain_P.Models;

namespace BlockChain_P.Services{
    public class BlockNetworkService{
        public async Task<BlockChainService> ReciveBlockChainAsync(int port){
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            var client = await listener.AcceptTcpClientAsync();

            using var reader = new StreamReader(client.GetStream());
            var jsonData = await reader.ReadToEndAsync();

            listener.Stop();
            var blockchain = JsonSerializer.Deserialize<BlockChainService>(jsonData);
            return await Task.FromResult(blockchain);
        }

        public async Task SendBlockChainAsync(BlockChainService blockchain, string ipAdress, int port){
            var client = new TcpClient();
            await client.ConnectAsync(ipAdress, port);
            using var writer = new StreamWriter(client.GetStream());
            var jsonData = JsonSerializer.Serialize(blockchain);
            await writer.WriteAsync(jsonData);
            await writer.FlushAsync();
            client.Close();
        }
        public async Task<Block> ReciveBlockAsync(int port){
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            var client = await listener.AcceptTcpClientAsync();

            using var reader = new StreamReader(client.GetStream());
            var jsonData = await reader.ReadToEndAsync();

            listener.Stop();
            var block = JsonSerializer.Deserialize<Block>(jsonData);
            return await Task.FromResult(block);
        }

        public async Task SendBlockAsync(Block block, string ipAdress, int port){
            var client = new TcpClient();
            await client.ConnectAsync(ipAdress, port);
            using var writer = new StreamWriter(client.GetStream());
            var jsonData = JsonSerializer.Serialize(block);
            await writer.WriteAsync(jsonData);
            await writer.FlushAsync();
            client.Close();
        }

        public async Task<Transaction> ReciveTransactionAsync(int port){
            
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            var client = await listener.AcceptTcpClientAsync();

            using var reader = new StreamReader(client.GetStream());
            var jsonData = await reader.ReadToEndAsync();

            listener.Stop();
            var transaction = JsonSerializer.Deserialize<Transaction>(jsonData);
            return await Task.FromResult(transaction);
        
        }
        public async Task SendTransactionAsync(Transaction transaction, string ipAdress, int port){
            var client = new TcpClient();
            await client.ConnectAsync(ipAdress, port);
            using var writer = new StreamWriter(client.GetStream());
            var jsonData = JsonSerializer.Serialize(transaction);
            await writer.WriteAsync(jsonData);
            await writer.FlushAsync();
            client.Close();
        }
    }
}