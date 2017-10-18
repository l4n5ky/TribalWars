using System;
using HtmlAgilityPack;

namespace TribalWars.Models
{
    public class Player
    {
        public string Nick {get; set;}
        public string Ally {get; set;}
        public int Villages {get; set;}
        public long Points { get; set; }
        public ulong Points_Aggresor { get; set; }
        public ulong Points_Defender { get; set; }
        public ulong Points_Supporter { get; set; }
        public int Rank_Points { get; set; }
        public int Rank_Aggresor { get; set; }
        public int Rank_Defender { get; set; }
        public int Rank_Supporter { get; set; }

        public Player(string infoPageUrl)
        {
            ParseFromUrl(infoPageUrl);
        }

        private void ParseFromUrl(string infoPageUrl)
        {
            var htmlWeb = new HtmlWeb();
            var htmlDoc = htmlWeb.Load(infoPageUrl);

            Nick = htmlDoc.DocumentNode.SelectSingleNode("//h2[@class='']").InnerHtml;
            Nick = Nick.Remove(0, 6);
            Nick = Nick.Remove(Nick.Length - 1, 1);

            string tmpVills = htmlDoc.DocumentNode.SelectSingleNode("//table[@id='villages_list']//th").InnerText;
            tmpVills = tmpVills.Replace("Wioski (", "").Replace(")", "");
            Villages = Convert.ToInt32(tmpVills);

            var infoTable = htmlDoc.DocumentNode.SelectNodes("//table[@id='player_info']//tr");
            string tmpPoints = infoTable[1].InnerText;
            tmpPoints = tmpPoints.Replace("Punkty:", "").Replace(".", "");
            string tmpRank = infoTable[2].InnerText;
            tmpRank = tmpRank.Replace("Ranking:", "");
            string tmpAlly = "";
            if (infoTable.Count > 4)
            {
                tmpAlly = infoTable[4].InnerText;
                ForkOutDefeated(infoTable[3].InnerHtml);
            }
            else // nie ma pokonanych przeciwników
            {
                tmpAlly = infoTable[3].InnerText;
                Points_Aggresor = 0; Rank_Aggresor = 0;
                Points_Defender = 0; Rank_Defender = 0;
                Points_Supporter = 0; Rank_Supporter = 0;
            }

            tmpAlly = tmpAlly.Replace("Plemię:", "").Replace("\n", "").Replace("\t", "");

            Points = Convert.ToInt64(tmpPoints);
            Rank_Points = Convert.ToInt32(tmpRank);
            Ally = tmpAlly;
        }

        private void ForkOutDefeated(string innerHtml)
        {
            string tmp = innerHtml;
            bool hasAgro = innerHtml.Contains("agresor");
            bool hasDeff = innerHtml.Contains("obrońca");
            bool hasSupp = innerHtml.Contains("wspierający");
            int index = 0;

            index = tmp.IndexOf(":");
            tmp = tmp.Remove(0, index + 1);
            index = tmp.IndexOf(":");
            tmp = tmp.Remove(0, index + 2);
            tmp = tmp.Replace("&lt;span class=&quot;grey&quot;&gt;.&lt;/span&gt;", "");
            tmp = tmp.Replace("::", "");
            tmp = tmp.Replace(" Jako obrońca: ", "");
            tmp = tmp.Replace("<br/>Jako wspierający: ", "");
            index = tmp.LastIndexOf(")");
            tmp = tmp.Remove(index, tmp.Length - index);
            index = tmp.LastIndexOf(")");
            tmp = tmp.Remove(index, tmp.Length - index);
            tmp = tmp.Remove(tmp.Length - 1, 1);
            tmp = tmp.Replace(" ", "").Replace("(", ".").Replace(".)", ".");

            string[] data = tmp.Split('.');
            
            if (hasAgro && hasDeff && hasSupp)
            {
                Points_Aggresor = Convert.ToUInt64(data[0]);
                Rank_Aggresor = Convert.ToInt32(data[1]);
                Points_Defender = Convert.ToUInt64(data[2]);
                Rank_Defender = Convert.ToInt32(data[3]);
                Points_Supporter = Convert.ToUInt64(data[4]);
                Rank_Supporter = Convert.ToInt32(data[5]);
            }

            if (!hasAgro && hasDeff && hasSupp)
            {
                Points_Aggresor = 0;
                Rank_Aggresor = 0;
                Points_Defender = Convert.ToUInt64(data[0]);
                Rank_Defender = Convert.ToInt32(data[1]);
                Points_Supporter = Convert.ToUInt64(data[2]);
                Rank_Supporter = Convert.ToInt32(data[3]);
            }

            if (hasAgro && !hasDeff && hasSupp)
            {
                Points_Aggresor = Convert.ToUInt64(data[0]);
                Rank_Aggresor = Convert.ToInt32(data[1]);
                Points_Defender = 0;
                Rank_Defender = 0;
                Points_Supporter = Convert.ToUInt64(data[2]);
                Rank_Supporter = Convert.ToInt32(data[3]);
            }

            if (hasAgro && hasDeff && !hasSupp)
            {
                Points_Aggresor = Convert.ToUInt64(data[0]);
                Rank_Aggresor = Convert.ToInt32(data[1]);
                Points_Defender = Convert.ToUInt64(data[2]);
                Rank_Defender = Convert.ToInt32(data[3]);
                Points_Supporter = 0;
                Rank_Supporter = 0;
            }

            if (!hasAgro && !hasDeff && hasSupp)
            {
                Points_Aggresor = 0;
                Rank_Aggresor = 0;
                Points_Defender = 0;
                Rank_Defender = 0;
                Points_Supporter = Convert.ToUInt64(data[0]);
                Rank_Supporter = Convert.ToInt32(data[1]);
            }

            if (!hasAgro && hasDeff && !hasSupp)
            {
                Points_Aggresor = 0;
                Rank_Aggresor = 0;
                Points_Defender = Convert.ToUInt64(data[0]);
                Rank_Defender = Convert.ToInt32(data[1]);
                Points_Supporter = 0;
                Rank_Supporter = 0;
            }

            if (hasAgro && !hasDeff && !hasSupp)
            {
                Points_Aggresor = Convert.ToUInt64(data[0]);
                Rank_Aggresor = Convert.ToInt32(data[1]);
                Points_Defender = 0;
                Rank_Defender = 0;
                Points_Supporter = 0;
                Rank_Supporter = 0;
            }

            if (!hasAgro && !hasDeff && !hasSupp)
            {
                Points_Aggresor = 0;
                Rank_Aggresor = 0;
                Points_Defender = 0;
                Rank_Defender = 0;
                Points_Supporter = 0;
                Rank_Supporter = 0;
            }
        }
    }
}