using System.ComponentModel;
using ProtoBuf;

namespace WebTests.Models
{
    [ProtoContract]
    public class Card
    {
        private string _name;
        private string _id;

        [ProtoMember(1)]
        public string Id
        {
            get { return _id; }
            set { _id = value?.Trim(); }
        }

        [ProtoMember(2)]
        public bool IsGolden { get; set; }
        [ProtoMember(3)]
        public bool IsLegendary { get; set; }

        [ProtoMember(4)]
        public Rarity Rarity { get; set; }

        [ProtoMember(5)]
        public string Name
        {
            get { return _name; }
            set { _name = value?.Trim(); }
        }

        [ProtoMember(6)]
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

            return this.Equals((Card)obj);
        }

        private bool Equals(Card other)
        {
            return string.Equals(this.Id, other.Id)
                   && this.IsGolden == other.IsGolden
                   && this.IsLegendary == other.IsLegendary
                   && string.Equals(this.Name, other.Name)
                   && this.Rarity == other.Rarity;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Id?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ this.IsGolden.GetHashCode();
                hashCode = (hashCode * 397) ^ this.IsLegendary.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (int)this.Rarity;
                return hashCode;
            }
        }
    }

    public enum CardSet
    {
        None = 0,
        [Description("The Grand Tournament")]
        Tgt,
        [Description("Classic")]
        Wild,
        [Description("Old Gods")]
        OldGods,
        [Description("Mean Streets of Gadgetzan")]
        Msog
    }
}