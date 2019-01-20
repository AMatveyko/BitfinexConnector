using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using ConnectorTest;

namespace TestLibrary
{
    public class Connector : ITestConnector
    {

        public event Action<string> log;

        #region ITestConnector

        #region Rest
        public Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
        {
            return _restApi.GetTrades(pair, maxCount);
        }

        public Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            return _restApi.GetCandles(pair, periodInSec, from, to, count);
        }
        #endregion

        #region Socket
        public event Action<Trade> NewBuyTrade;
        public event Action<Trade> NewSellTrade;

        public void SubscribeTrades(string pair, int maxCount = 100)
        {
            _webSocketAPI.SubTrade(pair, maxCount);
        }
        public void UnsubscribeTrades(string pair)
        {
            _webSocketAPI.UnSubTrade(pair);
        }

        public event Action<Candle> CandleSeriesProcessing;
        public void SubscribeCandles(string pair, int periodInSec, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
        {
            _webSocketAPI.SubCandles(pair, periodInSec, from, to, count);
        }
        public void UnsubscribeCandles(string pair)
        {
            _webSocketAPI.UnSubCandles(pair);
        }
        #endregion

        #endregion

        private WebSocketAPI _webSocketAPI;
        private RestApi _restApi;

        public Connector()
        {

            _restApi = new RestApi();
            _restApi.log += (str) => { log?.Invoke(str); };

            _webSocketAPI = new WebSocketAPI();
            _webSocketAPI.ConnectOpen += (sender, e) =>
            {
                
            };
            _webSocketAPI.TradeSend += (trade) =>
            {
                switch (trade.Side)
                {
                    case "Buy":
                        NewBuyTrade?.Invoke(trade);
                        break;
                    case "Sell":
                        NewSellTrade?.Invoke(trade);
                        break;
                }
            };

            _webSocketAPI.CandleSeriesProcessing += (candle) =>
            {
                CandleSeriesProcessing?.Invoke(candle);
            };

            _webSocketAPI.log += (str) => { log?.Invoke(str); };
        }
    }
}
