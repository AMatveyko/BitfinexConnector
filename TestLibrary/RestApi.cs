using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace TestLibrary
{
    public class RestApi
    {

        public event Action<string> log;

        public async Task<IEnumerable<Trade>> GetTrades(string pair, int maxCount)
        {
            return await Task.Run(() =>
            {
                List<Trade> trades = new List<Trade>();
                maxCount = (maxCount > 5000) ? 5000 : maxCount;
                string command = "https://api.bitfinex.com/v2/trades/t" + pair + "/hist?limit=" + maxCount;
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(command);
                StreamReader sr = new StreamReader(stream);
                string newLine;
                string result = String.Empty;
                while ((newLine = sr.ReadLine()) != null)
                    result += newLine;
                if (result.Length > 0)
                {
                    result = result.Replace("[[", "").Replace("]]", "").Replace("],[", "|");
                    string[] rawTrades = result.Split('|');
                    foreach (var trd in rawTrades)
                    {
                        trades.Add(new Trade(trd, pair));
                    }
                }
                return trades;
            });
        }

        public async Task<IEnumerable<Candle>> GetCandles(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            return await Task.Run(() =>
            {
                List<Candle> candles = new List<Candle>();

                string period = TimeFrame.CandlePeriod(periodInSec);


                string command = "https://api.bitfinex.com/v2/candles/trade:"+period+":t"+pair+"/hist";
                if ((count != null) && (count != 0))
                {
                    count = (count > 5000) ? 5000 : count;
                    command += "&limit=" + count;
                }
                if (from != null)
                {
                    DateTimeOffset fr = from ?? default(DateTimeOffset);
                    DateTime nx = new DateTime(1970, 1, 1);
                    TimeSpan ts = fr - nx;
                    command += "&start=" + ts.TotalMilliseconds;
                }
                if (to != null)
                {
                    DateTimeOffset fr = to ?? default(DateTimeOffset);
                    DateTime nx = new DateTime(1970, 1, 1);
                    TimeSpan ts = fr - nx;
                    command += "&end=" + ts.TotalMilliseconds;
                }

                int index = command.IndexOf('&');
                if(index > 0)
                {
                    command = command.Remove(index, 1).Insert(index, "?");
                }

                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(command);
                StreamReader sr = new StreamReader(stream);
                string newLine;
                string result = String.Empty;
                while ((newLine = sr.ReadLine()) != null)
                    result += newLine;
                if (result.Length > 2)
                {
                    result = result.Replace("[[", "").Replace("]]", "").Replace("],[", "|");
                    string[] rawTrades = result.Split('|');
                    foreach (var trd in rawTrades)
                    {
                        candles.Add(new Candle(trd, pair));
                    }
                }
                return candles;
            });
        }

    }
}
