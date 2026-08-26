using BlockChain_P.Models;
namespace BlockChain_P.Services{
    public class TransactionService{
        private readonly WalletService walletService = new WalletService();
        public Transaction CreateTransaction(string from, string to, decimal amount, decimal fee, Wallet wallet){
            var id = Guid.NewGuid().ToString();
            var tx = new Transaction(from, to, amount)
            {
                Fee = fee,
                SenderPublicKey = wallet.PublicKey
            };
            tx.Signature = wallet.Sign(tx.GetDataToSign());
            return tx;
        }
        public (bool isValid, string errorMessage) ValidateTransaction(Transaction transaction)
        {
            if (transaction.From == "Coinbase"){
                return (true, string.Empty);
            }
            if (string.IsNullOrWhiteSpace(transaction.From))
            {
                return (false, "Sender address is required.");
            }
            if (string.IsNullOrWhiteSpace(transaction.To))
            {
                return (false, "Recepient address is required.");
            }
            if (transaction.Amount <= 0)
            {
                return (false, "Transaction amount should be greater than zero");
            }
            if (transaction.SenderPublicKey == null || transaction.SenderPublicKey.Length == 0)
            {
                return (false, "Sender public key is required");
            }
            if (transaction.Signature == null || transaction.Signature.Length == 0){
                return (false, "Transaction signature is required");
            }
            if (!walletService.VerifySignature(transaction.GetDataToSign(), transaction.Signature, transaction.SenderPublicKey)){
                return (false, "Invalid transaction signature");
            }
            return (true, string.Empty);
        }
    }
}