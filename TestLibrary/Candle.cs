using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    public class Candle
    {
        public int ChannId { get; private set; }
        public string Pair { get; set; }
        public DateTimeOffset Time { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal HightPrice { get; set; }
        public decimal LowPrice { get; set; }

        public Candle(string candleData, string pair)
        {
            Pair = pair;

            candleData = candleData.Replace("[", "").Replace("]", "");

            string[] rawCandle = candleData.Split(',');
            if (rawCandle.Length == 7)
            {
                ChannId = Int32.Parse(rawCandle[0]);
                Time = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds( long.Parse(rawCandle[1]) );
                OpenPrice = (decimal)float.Parse(rawCandle[2].Replace('.', ','));
                ClosePrice = (decimal)float.Parse(rawCandle[3].Replace('.', ','));
                HightPrice = (decimal)float.Parse(rawCandle[4].Replace('.', ','));
                LowPrice = (decimal)float.Parse(rawCandle[5].Replace('.', ','));
            } else if(rawCandle.Length == 6)
            {
                ChannId = 0;
                Time = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds( long.Parse(rawCandle[0]));
                OpenPrice = (decimal)float.Parse(rawCandle[1].Replace('.', ','));
                ClosePrice = (decimal)float.Parse(rawCandle[2].Replace('.', ','));
                HightPrice = (decimal)float.Parse(rawCandle[3].Replace('.', ','));
                LowPrice = (decimal)float.Parse(rawCandle[4].Replace('.', ','));
            }
        }
    }
}
