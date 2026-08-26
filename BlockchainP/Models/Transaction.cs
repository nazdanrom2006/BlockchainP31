using System.Text;

namespace BlockChain_P.Models{
    public class Transaction : IEquatable<Transaction>
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public DateTime TimeStamp { get; set; }
        public byte[] SenderPublicKey { get; set; }
        public byte[] Signature { get; set; }
        public Transaction(string from, string to, decimal amount){
            Id = Guid.NewGuid().ToString();
            From = from;
            To = to;
            Amount = amount;
            TimeStamp = DateTime.UtcNow;
        }
        public Transaction(){}
        public string ToRowString(){
            return $"{Id}\t{From}\t{To}\t{Amount}\t{Fee}\t{TimeStamp}";
        }

        public byte[] GetDataToSign(){
            var data = ToRowString();
            return Encoding.UTF8.GetBytes(data);
        }
        public bool Equals(Transaction transaction)
        {
            return this.Id == transaction.Id;
        }

    }
}