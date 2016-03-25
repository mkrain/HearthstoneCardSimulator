using CommandLine;
using WebTests.Models;

namespace WebTests
{
    public class Options
    {
        [Option('n', "min", DefaultValue = 62, Required = false)]
        public int MinScore { get; set; }

        [Option('x', "max", DefaultValue = 100000, Required = false)]
        public int MaxScore { get; set; }

        [Option('u', "url", DefaultValue = "http://www.hearthpwn.com/packs/simulator/1-hearthpwn-wild-pack", Required = false)]
        public string PackUrl { get; set; }

        [Option('b', "leader", DefaultValue = false, Required = false)]
        public bool UseLeaderBoard { get; set; }

        [Option('e', "emin", DefaultValue = false, Required = false)]
        public bool EnableMin { get; set; }

        [Option('m', "emax", DefaultValue = false, Required = false)]
        public bool EnableMax { get; set; }

        [Option('d', "delay", DefaultValue = 500, Required = false)]
        public int Delay { get; set; }

        [Option('s', "user", DefaultValue = "", Required = true)]
        public string UserName { get; set; }

        [Option('p', "password", DefaultValue = "", Required = true)]
        public string Password { get; set; }

        [Option('l', "loginUrl", DefaultValue = "http://www.hearthpwn.com/", Required = false)]
        public string LoginUrl { get; set; }

        [Option('o', "logfileName", DefaultValue = "run", Required = false)]
        public string LogFileName { get; set; }

        [Option('a', "dbLocation", DefaultValue = @".\db\cards", Required = false)]
        public string DbLocation { get; set; }

        [Option('r', "csv", DefaultValue = false, Required = false)]
        public bool WriteCsv { get; set; }

        [Option('f', "dataFilesDir", DefaultValue = @".\data", Required = false)]
        public string DataFilesDirectory { get; set; }

        [Option('g', "mergeFileName", DefaultValue = @"merge", Required = false)]
        public string MergeFileName { get; set; }

        [Option("export", DefaultValue = false, Required = false)]
        public bool Export { get; set; }

        [Option("exportFileName", DefaultValue = @"export.csv", Required = false)]
        public string ExportFileName { get; set; }

        [Option("cardSet", DefaultValue = CardSet.Tgt, Required = false)]
        public CardSet CardSet { get; set; }
    }
}