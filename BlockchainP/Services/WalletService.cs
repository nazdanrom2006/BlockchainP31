using System.Security.Cryptography;
using BlockChain_P.Models;

namespace BlockChain_P.Services{
    public class WalletService{
        public Wallet CreateWallet(string name){
            using (var ecdsa = System.Security.Cryptography.ECDsa.Create()){
                var privateKey = ecdsa.ExportECPrivateKey();
                var publicKey = ecdsa.ExportSubjectPublicKeyInfo();
                var base64 = Convert.ToBase64String(publicKey);
                var address = base64[^10..];
                return new Wallet(name, address, publicKey, privateKey);
            }
        }
        public bool VerifySignature(byte[] data, byte[] signature, byte[] publicKey){
            using (var ecdsa = System.Security.Cryptography.ECDsa.Create()){
                ecdsa.ImportSubjectPublicKeyInfo(publicKey, out _);
                return ecdsa.VerifyData(data, signature, System.Security.Cryptography.HashAlgorithmName.SHA256);
            }
        }
    }
}