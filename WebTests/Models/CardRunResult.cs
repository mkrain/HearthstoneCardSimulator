using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Colorful;
using ProtoBuf;
using Console = Colorful.Console;

namespace WebTests.Models
{
    [ProtoContract]
    public class CardRunResult
    {
        [ProtoMember(1)]
        public long Id { get; set; }
        [ProtoMember(2)]
        public bool IsGoodResult { get; set; }
        [ProtoMember(3)]
        public int Score { get; set; }
        [ProtoMember(4)]
        public long Timespan { get; set; }
        [ProtoMember(5)]
        public IEnumerable<Card> Cards { get; set; }

        public void PrintToConsole()
        {
            bool first = true;

            if(this.Cards == null || !this.Cards.Any())
            {
                return;
            }
               
            foreach(var card in this.Cards)
            {
                if(card == null)
                {
                    continue;
                }

                var cardName = card.Name;
                var cardColor = this.GetRarityColor(card.Rarity);

                if(card.IsGolden)
                {
                    cardName += " [G]";
                    cardColor = this.GetRarityColor(Rarity.Legendary);
                }

                var rarity = card.Rarity.ToString();
                var cardData = "{0], {1}, {2}";

                if(first)
                {
                    first = false;
                    Console.WriteLine(new string('=', 60), Color.White);
                }

                var formatters =
                    new[]
                    {
                        new Formatter($"{rarity,9}", this.GetRarityColor(card.Rarity)),
                        new Formatter(cardName, cardColor),
                    };

                Console.WriteLineFormatted(cardData, Color.White, formatters);
            }
            Console.WriteLine();
        }

        private Color GetRarityColor(Rarity rarity)
        {
            switch(rarity)
            {
                case Rarity.Common:
                    return Color.White;
                case Rarity.Classic:
                    return Color.Brown;
                case Rarity.Epic:
                    return Color.Purple;
                case Rarity.Legendary:
                    return Color.Gold;
                case Rarity.Rare:
                    return Color.Blue;
                default:
                    return Color.White;
            }
        }
    }
}