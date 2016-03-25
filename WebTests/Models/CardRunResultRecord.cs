using ProtoBuf;

namespace WebTests.Models
{
    [ProtoContract]
    public class CardRunResultRecord
    {
        private string _cardName;
        private string _cardId;

        [ProtoMember(1)]
        public long Id { get; set; }
        [ProtoMember(2)]
        public string CardId
        {
            get { return _cardId; }
            set { _cardId = value?.Trim(); }
        }
        [ProtoMember(3)]
        public int Score { get; set; }
        [ProtoMember(4)]
        public bool IsGolden { get; set; }
        [ProtoMember(5)]
        public bool IsLegendary { get; set; }
        [ProtoMember(6)]
        public Rarity Rarity { get; set; }
        [ProtoMember(7)]
        public long Timespan { get; set; }
        [ProtoMember(8)]
        public string CardName
        {
            get { return _cardName; }
            set { _cardName = value?.Trim(); }
        }
        [ProtoMember(9)]
        public CardSet CardSet { get; set; }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((CardRunResultRecord)obj);
        }

        private bool Equals(CardRunResultRecord other)
        {
            return this.Id == other.Id 
                && this.Score == other.Score 
                && string.Equals(this.CardId, other.CardId) 
                && string.Equals(this.CardName, other.CardName) 
                && this.IsGolden == other.IsGolden 
                && this.IsLegendary == other.IsLegendary 
                && this.Rarity == other.Rarity 
                && this.Timespan == other.Timespan;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Id.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Score;
                hashCode = (hashCode * 397) ^ (this.CardId?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.CardName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ this.IsGolden.GetHashCode();
                hashCode = (hashCode * 397) ^ this.IsLegendary.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.Rarity;
                hashCode = (hashCode * 397) ^ this.Timespan.GetHashCode();
                return hashCode;
            }
        }
    }
}