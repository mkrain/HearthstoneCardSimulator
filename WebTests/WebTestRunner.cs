using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Colorful;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebTests.Models;
using WebTests.Repository;
using Console = Colorful.Console;

namespace WebTests
{
    public class WebTest
    {
        private static readonly string _javaScriptToExecute;
        private static readonly IWebDriver _driver;
        private static readonly IJavaScriptExecutor _jsExecutor;
        private static readonly Stopwatch _runStopwatch = new Stopwatch();

        static WebTest()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = assembly.GetManifestResourceNames().First(res => res.Contains("HearthstoneCard"));
            var stream = assembly.GetManifestResourceStream(resource);

            if(stream != null)
            {
                using(var streamReader = new StreamReader(stream))
                {
                    _javaScriptToExecute = streamReader.ReadToEnd();
                }
            }

            _driver = new ChromeDriver();
            _driver.Manage().Window.Size = new Size {Height = 450, Width = 1100};
            _jsExecutor = (IJavaScriptExecutor)_driver;
            _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
        }

        static void Main(string[] args)
        {
            int runs = 1;

            var options = new Options();

            CommandLine.Parser.Default.ParseArguments(args, options);

            if(options.Export)
            {
                Console.WriteLine("Exporting data...");
            }

            var container = DependencyConfig.Register(options);
            var fileService = container.GetInstance<IFileService>();

            Console.WriteLine("Checking for exiting records to import...");
            fileService.MergeExistingRunResults();

            Login(options);

            if(options.UseLeaderBoard)
            {
                var leaderBoard = GetLeaderBoard();
                var minimumLeaderScore = leaderBoard.Leaders.Last().Score;
                var maximumLeaderScore = leaderBoard.Leaders.First().Score;

                options.MinScore = minimumLeaderScore;
                options.MaxScore = maximumLeaderScore;
            }

            Console.WriteLine("Searching for min: {0} or max: {1}", options.MinScore, options.MaxScore);
            Console.WriteLine("From: {0}", options.PackUrl);
            Console.WriteLine();

            while(true)
            {
                var runResult = new CardRunResult();

                try
                {
                    _runStopwatch.Restart();
                    runResult = Run(options);
                    _runStopwatch.Stop();
                }
                catch(Exception e)
                {
                    var message = e.ToString();

                    if(message.Contains("no such window: target window already closed"))
                    {
                        Console.WriteLine("The windows was closed, exiting.");
                        break;
                    }
                    if(message.Contains("NullReference"))
                    {
                        Console.WriteLine("The windows was closed, exiting.");
                        break;
                    }
                }

                //Dont Add blank cards to the output
                if(runResult?.Cards == null
                //No cards
                || !runResult.Cards.Any()
                //Any empty cards
                || runResult.Cards.Any(c => c == null))
                {
                    continue;
                }

                runs++;
                int score = runResult.Score;
                runResult.Id = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                runResult.Timespan = _runStopwatch.ElapsedMilliseconds;

                fileService.SaveRuResult(runResult);

                string cardScore = $"{DateTime.Now}: Score: {score}, Run: {runs}, Elapsed: {runResult.Timespan}ms";

                var styleSheet = new StyleSheet(Color.White);
                styleSheet.AddStyle(
                    score.ToString(),
                    runResult.IsGoodResult
                        ? Color.GreenYellow
                        : Color.Red);

                Console.WriteLineStyled(
                    cardScore,
                    styleSheet);

                if(runs % 100 == 0)
                {
                    Console.Clear();
                }

                runResult.PrintToConsole();

                if(_runStopwatch.ElapsedMilliseconds < options.Delay)
                {
                    var delay = TimeSpan.FromMilliseconds(options.Delay - _runStopwatch.ElapsedMilliseconds);
                    Console.WriteLine("Waiting to match delay: {0}\n", delay.Milliseconds);
                    Thread.Sleep(delay);
                }

                if(//(options.EnableMin && score < options.MinScore) || 
                (options.EnableMax && score >= options.MaxScore))
                {
                    object[] fragments = { score.ToString(), runs.ToString() };

                    Console.WriteLineFormatted("Found score: {0}, in {1} runs", Color.Green, Color.White, fragments);
                    Console.WriteLine();
                    Console.WriteLine("Saving found pack...");

                    //SaveRunResult(runResult, options);
                    Console.WriteLine("Press enter to contine, any other key to exit.");
                    Console.ReadLine();

                    //var shouldContinue = Console.ReadLine();

                    //if(string.IsNullOrEmpty(shouldContinue))
                    //{
                    //    continue;
                    //}

                    //_driver.Quit();
                    //return;
                }
            }
        }

        private static void Login(Options options)
        {
            _driver.Navigate().GoToUrl(options.LoginUrl);

            var loginElement = _driver.FindElement(By.Id("login-link"));

            loginElement.Click();

            var userNameElement = _driver.FindElement(By.Id("field-username"));
            var passwordElement = _driver.FindElement(By.Id("field-loginFormPassword"));
            var loginButtonElement = _driver.FindElement(By.CssSelector("input[type='submit'].cta-button"));

            userNameElement.SendKeys(options.UserName);
            passwordElement.SendKeys(options.Password);

            loginButtonElement.Click();
        }

        private static LeaderBoard GetLeaderBoard()
        {
            var leaders = _driver.FindElements(By.CssSelector(".pack-leaderboard-widget .b-list_comments .b-list-item"));

            var leaderBoardResult = new LeaderBoard();
            var foundLeaders =
                from leader in leaders
                select leader.FindElements(By.TagName("span")).ToList()
                into elements
                let rank = elements.First().Text
                let score = elements.Last().Text.Replace(",", "")
                select new Leader
                       {
                           Score = int.Parse(score),
                           Rank = int.Parse(rank)
                       };

            leaderBoardResult.Leaders = foundLeaders.OrderBy(l => l.Rank);

            return leaderBoardResult;
        }

        private static void SaveRunResult(CardRunResult runResult, Options options)
        {
            const string titleJavaScript = "$('#field-title').val('{0}')";
            const string submitJs = "$('#pack-save').click()";
            string bestWorst = runResult.Score <= options.MinScore ? "Worst" : "Best";

            var scoreString = $"{runResult.Score}-{bestWorst}PackEver";

            //Send the title to the pack title input
            _jsExecutor.ExecuteScript(string.Format(titleJavaScript, scoreString));
            //Save the pack using the specified title
            _jsExecutor.ExecuteScript(submitJs);
        }

        private static CardRunResult Run(Options options)
        {
            _driver.Navigate().GoToUrl(options.PackUrl);

            _jsExecutor.ExecuteScript("$('.pack-stats')[0].scrollIntoView()");
            _jsExecutor.ExecuteScript(_javaScriptToExecute);

            var scoreElement = _driver.FindElement(By.CssSelector(".pack-score"), 10);
            var score = scoreElement.GetAttribute("data-score");
            var selectElements =
                _driver.FindElements(By.CssSelector(".pack-results .pack-slot"));
            var foundCards = new List<Card>();
            var runResult = new CardRunResult();

            foreach(var foundCard in selectElements)
            {
                var cardFront = foundCard.FindElement(By.CssSelector(".card-front"));
                var name = cardFront.GetAttribute("href");
                var cardId = foundCard.GetAttribute("data-id");
                var classes = foundCard.GetAttribute("class");
                var isGolden = !string.IsNullOrWhiteSpace(classes) && classes.Contains("is-gold");
                var cardRarity = (Rarity)int.Parse(foundCard.GetAttribute("data-rarity"));

                name = name.Substring(name.IndexOf("-", StringComparison.Ordinal)).Replace("-", " ");
                name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name);

                var card =
                    new Card
                    {
                        IsLegendary = cardRarity == Rarity.Legendary,
                        Name = name,
                        Id = cardId,
                        IsGolden = isGolden,
                        Rarity = cardRarity,
                        CardSet = options.CardSet
                    };
                foundCards.Add(card);
            }

            runResult.Cards = foundCards;
            runResult.Score = int.Parse(score);
            runResult.IsGoodResult = runResult.Score >= options.MaxScore;

            return runResult;
        }
    }
}