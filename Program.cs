using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using HtmlAgilityPack;
using TribalWars.Models;

namespace TribalWars
{
    class Program
    {
        static string[] alliesUrls = new string[]
        {
            "https://pl121.plemiona.pl/guest.php?screen=info_member&id=256", // ~~F~~
            "https://pl121.plemiona.pl/guest.php?screen=info_member&id=413", // --F--
            "https://pl121.plemiona.pl/guest.php?screen=info_member&id=997"  // ..F..
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Hello in tribal wars ally-members-defeated-table generator ;)");
            var data = GetParsedPlayersList();
            var culture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentCulture = culture;
            var currentDateString = DateTime.UtcNow.ToShortDateString().Replace('/', '-');

            StringBuilder sb = new StringBuilder();
            sb.Append($"[size=12][b]Ranking pokonanych z dnia {currentDateString}[/b][/size]\n");
            sb.Append("[spoiler]");
            sb.Append(GetHtmlAgressorsTable(data));
            sb.Append(GetHtmlDefendersTable(data));
            sb.Append(GetHtmlSupportersTable(data));
            sb.Append("[i]Wygenerowano za pomocą [url=https://github.com/l4n5ky/tribalwars]aplikacji[/url] napisanej przez [player]BabkaBezKlapka[/player].[/i]");
            sb.Append("[/spoiler]");

            // write to file
            var filePath = $"/home/marcin/Dokumenty/plemiona/{currentDateString}.txt";
            var writer = System.IO.File.CreateText(filePath);

            writer.Write(sb.ToString());
            writer.Dispose();

            Console.WriteLine($"\nAll data completely fetched, parsed and saved in {filePath}.\nHave a nice day!");
        }

        private static List<Player> GetParsedPlayersList()
        {
            var list = new List<Player>();

            foreach (string allyUrl in alliesUrls)
            {
                Console.WriteLine("Fetching ally members from: " + allyUrl);
                var allyMembersUrls = GetAllyMembersUrlsList(allyUrl);

                foreach(var playerUrl in allyMembersUrls)
                {
                    Console.WriteLine("Fetching player data from: " + playerUrl);
                    var player = new Player(playerUrl);
                    list.Add(player);
                }
            }

            return list;
        }

        private static List<string> GetAllyMembersUrlsList(string allyInfoUrl)
        {
            var htmlWeb = new HtmlWeb();
            var htmlDoc = htmlWeb.Load(allyInfoUrl);

            var list = new List<string>();
            var nodes = htmlDoc.DocumentNode.SelectNodes("//a");
            foreach(var node in nodes)
            {
                string uri = node.Attributes["href"].Value;
                if (uri.Contains("player"))
                {
                    string url = "https://pl121.plemiona.pl" + uri.Replace("amp;", "");
                    list.Add(url);
                }
            }

            return list;
        }

        private static string GetHtmlAgressorsTable(List<Player> players)
        {
            var sb = new StringBuilder();
            var list = players.OrderByDescending(x => x.Points_Aggresor).ToList();

            sb.Append($"[size=12][i][b]Agresorzy[/b][/i][/size]\n\n");
            sb.Append("[table]\n[**]Lp.[||]Gracz[||]Ranking globalny[||]Pokonanych jednostek[/**]\n");

            for (int i = 1; i <= 10; i++)
            {
                var p = list[i - 1];
                //var points = p.Points_Aggresor.ToString("N", CultureInfo.CreateSpecificCulture("es-ES")).Replace(",00", "");
                sb.Append($"[*][b]{i}.[/b][|][player]{p.Nick}[/player][|][b]{p.Rank_Aggresor}[/b][|][b]{p.Points_Aggresor}[/b]\n");
            }

            sb.Append("[/table]\n\n");

            return sb.ToString();
        }

        private static string GetHtmlDefendersTable(List<Player> players)
        {
            var list = players.OrderByDescending(x => x.Points_Defender).ToList();

            var sb = new StringBuilder();
            sb.Append($"[size=12][i][b]Obrońcy[/b][/i][/size]\n\n");
            sb.Append("[table]\n[**]Lp.[||]Gracz[||]Ranking globalny[||]Pokonanych jednostek[/**]\n");

            for (int i = 1; i <= 10; i++)
            {
                var p = list[i - 1];
                //var points = p.Points_Defender.ToString("N", CultureInfo.CreateSpecificCulture("es-ES")).Replace(",00", "");
                sb.Append($"[*][b]{i}.[/b][|][player]{p.Nick}[/player][|][b]{p.Rank_Defender}[/b][|][b]{p.Points_Defender}[/b]\n");
            }

            sb.Append("[/table]\n\n");

            return sb.ToString();
        }

        private static string GetHtmlSupportersTable(List<Player> players)
        {
            var list = players.OrderByDescending(x => x.Points_Supporter).ToList();

            var sb = new StringBuilder();
            sb.Append($"[size=12][i][b]Pomocnicy[/b][/i][/size]\n\n");
            sb.Append("[table]\n[**]Lp.[||]Gracz[||]Ranking globalny[||]Pokonanych jednostek[/**]\n");

            for (int i = 1; i <= 10; i++)
            {
                var p = list[i - 1];
                //var points = p.Points_Supporter.ToString("N", CultureInfo.CreateSpecificCulture("es-ES")).Replace(",00", "");
                sb.Append($"[*][b]{i}.[/b][|][player]{p.Nick}[/player][|][b]{p.Rank_Supporter}[/b][|][b]{p.Points_Supporter}[/b]\n");
            }

            sb.Append("[/table]\n\n");

            return sb.ToString();
        }
    }
}
