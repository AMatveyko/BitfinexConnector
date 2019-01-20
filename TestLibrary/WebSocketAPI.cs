using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using System.Threading;

namespace TestLibrary
{
    public class WebSocketAPI
    {
        private WebSocket _client;

        public event Action<Trade> TradeSend;
        public event Action<Candle> CandleSeriesProcessing;

        public event EventHandler ConnectOpen;

        public event Action<string> log;

        private List<Subscribe> subscribes;

        private enum Type
        {
            trades,
            candles
        }

        public void SubTrade(string pair, int maxCount)
        {
            Type type = Type.trades;
            if (subscribes.Count == 0 || (subscribes.FirstOrDefault(x => ((x.Pair == pair) && (x.Type == type)))) == null)
            {
                subscribes.Add(new SubscribeTrade(pair, maxCount, type));
                _client.Send("{\"event\":\"subscribe\", \"channel\":\"trades\", \"pair\":\"t" + pair + "\"}");
            } else
                log?.Invoke("такая пара уже есть");
        }

        public void SubCandles(string pair, int periodInSec, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
        {
            Type type = Type.candles;
            if (subscribes.Count == 0 || (subscribes.FirstOrDefault(x => ((x.Pair == pair) && (x.Type == type)))) == null)
            {
                string period = TimeFrame.CandlePeriod(periodInSec);
                string key = "trade:" + period + ":t" + pair;
                subscribes.Add(new SubscribeCandle(pair, periodInSec, from, to, count, key, type));
                _client.Send("{\"event\":\"subscribe\", \"channel\":\"candles\", \"key\":\"" + key + "\"}");
            } else
                log?.Invoke("такая пара уже есть");
        }
        
        public void UnSubTrade(string pair)
        {
            UnSub(pair, Type.trades);
        }

        public void UnSubCandles(string pair)
        {
            UnSub(pair, Type.candles);
        }

        private void UnSub(string pair, Type type)
        {
            Subscribe subscribe = subscribes.FirstOrDefault(x=>((x.Pair == pair) && (x.Type == type)));
            if (subscribes.Contains(subscribe))
                _client.Send("{\"event\":\"unsubscribe\",\"chanId\":\"" + subscribe.ChanId + "\"}");
            else
                log?.Invoke("вы не подписанны на эту пару");
        }

        public WebSocketAPI()
        {

            subscribes = new List<Subscribe>();
            _client = new WebSocket("wss://api.bitfinex.com/ws/2");
            _client.OnMessage += (sender, e) =>
            {
                if (e.Data.Contains("\"hb\""))
                    return;
                else if (e.Data.Contains("\"event\":\"subscribed\""))
                    IfSubscribe(e.Data);
                else if (e.Data.Contains("\"event\":\"unsubscribed\""))
                    IfUnSubscribe(e.Data);
                else if (e.Data.Contains("\"info\""))
                    return;
                else
                    IfTradeMessage(e.Data);

            };
            _client.OnOpen += (sender, e) =>
            {
                ConnectOpen?.Invoke(sender, e);
            };
            _client.Connect();
        }

        private void IfSubscribe(string packet)
        {
            Dictionary<string, string> param = GetParam(packet);
            Type type;
            Subscribe subscribe;
            switch (param["channel"])
            {
                case ("trades"):
                    type = Type.trades;
                    subscribe = subscribes.FirstOrDefault(x => (x.Pair == param["pair"])&&( x.Type == type));
                    subscribe.ChanId = param["chanId"];
                    log?.Invoke("Trade: Sub pair: "+subscribe.Pair+", chanId: "+subscribe.ChanId);
                    break;
                case ("candles"):
                    type = Type.candles;
                    subscribe = subscribes.FirstOrDefault(x => (x.Key == param["key"]) && (x.Type == type));
                    subscribe.ChanId = param["chanId"];
                    log?.Invoke("Candle: Sub pair: " + subscribe.Pair + ", chanId: " + subscribe.ChanId);
                    break;
                default:
                    break;
            }
        }

        private void IfUnSubscribe(string packet)
        {
            Dictionary<string, string> param = GetParam(packet);
            Subscribe subscribe = subscribes.FirstOrDefault(x => x.ChanId == param["chanId"]);
            if (subscribe != null)
            {
                subscribes.Remove(subscribe);
                log?.Invoke(subscribe.Type.ToString() + " UnSub pair: " + subscribe.Pair + ", chainId: " + subscribe.ChanId);
            }
        }

        private void IfTradeMessage(string packet)
        {
            string channId = (packet.Split(','))[0].TrimStart('[');
            Subscribe subscribe = subscribes.FirstOrDefault(x=>x.ChanId == channId);
            if(subscribe != null)
            {
                if ((packet.Split(',')).Length > 7) return; //( первые ответы после подписки ) не ясно нужны ли в данном случае
                switch (subscribe.Type)
                {
                    case (Type.trades):
                        TradeMessage(packet, subscribe);
                        break;
                    case (Type.candles):
                        CandleMessage(packet, subscribe);
                        break;
                    default:
                        break;
                }
            }
        }

        private void CandleMessage(string packet, Subscribe subscribe)
        {
            SubscribeCandle subscribeCandle = (SubscribeCandle)subscribe;
            Candle candle = new Candle(packet,subscribeCandle.Pair);

            //log?.Invoke(candle.HumDate.ToString() + " >= " + subscribeCandle.From.ToString() + " = " + (candle.HumDate >= subscribeCandle.From).ToString()+"  packet >= from\n" +
            //            candle.HumDate.ToString() + " <= " + subscribeCandle.To.ToString() + " = " + (candle.HumDate <= subscribeCandle.To).ToString() + "  packet >= To");

            if (   ((subscribeCandle.From == null) || (candle.Time >= subscribeCandle.From))
                &&((subscribeCandle.To == null) || (candle.Time <= subscribeCandle.To))
                &&((subscribeCandle.Count == 0) || (subscribeCandle.CurrentCount < subscribeCandle.Count)))
            {
                CandleSeriesProcessing?.Invoke(candle);
            }
            if (       ((subscribeCandle.To != null) && (candle.Time > subscribeCandle.To))
                    || ((subscribeCandle.Count!=0)&&(++subscribeCandle.CurrentCount >= subscribeCandle.Count)))
                UnSub(subscribe.Pair, subscribe.Type);
        }

        private void TradeMessage(string packet, Subscribe subscribe)
        {
            SubscribeTrade subscribeTrade = (SubscribeTrade)subscribe;
            if (packet.Contains("\"tu\"")) //
            {
                Trade trade = new Trade(packet, subscribe.Pair);
                subscribeTrade.CurrentCount++;
                if (subscribeTrade.CurrentCount <= subscribeTrade.maxCount)
                {
                    TradeSend?.Invoke(trade);
                }
                if (subscribeTrade.CurrentCount >= subscribeTrade.maxCount)
                    UnSub(subscribe.Pair,subscribe.Type);
            }
        }

        private Dictionary<string, string> GetParam(string subscribeInfo)
        {
            //subscribe Trade example:
            //{"event":"subscribed","channel":"trades","chanId":1,"symbol":"tBTCUSD","pair":"BTCUSD"}
            //unsubscribe Trade example:
            //{"event":"unsubscribed","status":"OK","chanId":"106"}
            //subscribe Scale example:
            //{"event":"subscribed","channel":"candles","chanId":55608,"key":"trade:1m:tBTCUSD"}
            subscribeInfo = subscribeInfo.TrimEnd('}');
            subscribeInfo = subscribeInfo.TrimStart('}');
            List<string> parameters = subscribeInfo.Split(',').ToList();
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            foreach (var str in parameters)
            {
                int index = str.IndexOf(':');
                string strModif = str.Remove(index, 1).Insert(index, "|");
                string[] pair = strModif.Split('|');
                string key = pair[0].TrimEnd('"').TrimStart('"');
                string value = pair[1].TrimEnd('"').TrimStart('"');
                pairs.Add(key, value);
            }
            return pairs;
        }

        private abstract class Subscribe
        {
            public Type Type { get; protected set; }
            public string ChanId { get; set; }
            public long CurrentCount { get; set; } = 0;
            public string Pair { get; protected set; }
            public string Key { get; protected set; }

            public Subscribe( string pair, Type type)
            {
                Pair = pair;
                Type = type;
            }
        }

        private class SubscribeCandle : Subscribe
        {
            public long? Count { get; private set; }
            public int PeriodInSec { get; private set; }
            public DateTimeOffset? From { get; private set; }
            public DateTimeOffset? To { get; private set; }
            public SubscribeCandle(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to, long? count, string key, Type type)
                : base (pair, type)
            {
                Key = key;
                Count = count;
                From = from;
                To = to;
                PeriodInSec = periodInSec;
            }
        }

        private class SubscribeTrade : Subscribe
        {
            public int maxCount { get; set; }
            public SubscribeTrade(string pair, int maxCount, Type type)
                :base (pair, type)
            {
                this.maxCount = maxCount;
            }
        }
    }
}
