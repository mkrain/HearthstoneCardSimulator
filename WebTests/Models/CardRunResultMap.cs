using System.Collections.Generic;
using ProtoBuf;

namespace WebTests.Models
{
    [ProtoContract]
    public class CardRunResultMap
    {
        [ProtoMember(1)]
        public IEnumerable<CardRunResultRecord> Records { get; set; }
    }
}