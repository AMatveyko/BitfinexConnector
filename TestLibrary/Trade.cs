using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    public class Trade
    {
        public int ChannelID { get; private set; }
        public DateTimeOffset Time { get; set; }
        public int Period { get; set; }
        public string Pair { get; set; }
        public string Side { get; set; } = "Buy";
        public decimal Amount { get; set; }
        public decimal Price { get; set; }

        public Trade(string tradeData, string pair)
        {
            Pair = pair;

            string checkStr = "\"tu\",[";
            if (tradeData.Contains(checkStr))
            {
                tradeData = tradeData.Replace(checkStr, "").Replace("]]", "").TrimStart('[');
            }
            else
                tradeData = "0," + tradeData;

            string[] rawTrade = tradeData.Split(',');
            ChannelID = Int32.Parse(rawTrade[0]);
            Period = Int32.Parse(rawTrade[1]);
            Time = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds( long.Parse(rawTrade[2]) );
            Amount = (decimal)float.Parse(rawTrade[3].Replace(".", ","));
            Price = (decimal)float.Parse(rawTrade[4].Replace(".", ","));

            if (Amount < 0)
            {
                Side = "Sell";
                Amount = -Amount;
            }
        }
    }
}
