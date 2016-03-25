using System.Collections.Generic;

namespace WebTests.Models
{
    public class LeaderBoard
    {
        public IEnumerable<Leader> Leaders { get; set; }

        public LeaderBoard()
        {
            this.Leaders = new List<Leader>();
        } 
    }
}