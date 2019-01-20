using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Cmd;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using TestLibrary;
using ConnectorTest;

namespace test
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region localvariables
        //public ITestConnector Connector { get; private set; }
        public Connector Connector { get; private set; }
        #endregion

        #region View

        private List<string> _pairs = new List<string>() { "BTCUSD", "XRPBTC", "ETHBTC" };

        #region WebSocketAPI
        public string TradeCount { get; set; }
        public ObservableCollection<Trade> Trades { get; private set; }
        public string SelectTradePair { get; set; }
        public string SelectCandlePair { get; set; }
        public string CandleCount { get; set; }
        public ObservableCollection<Candle> Candles { get; private set; }
        public string CandleInterval { get; set; }
        public string CandleDateFrom { get; set; }
        public string CandleDateTo { get; set; }
        public List<string> TradePairs { get => _pairs; }
        public List<string> CandlePairs { get => _pairs; }

        #region Commands
        private SubscribeCommand _subscribeTradeCommand;
        public SubscribeCommand SubscribeTrade { get => _subscribeTradeCommand ?? (_subscribeTradeCommand = new SubscribeCommand(this)); }
        private UnSubscribeCommand _unSubscribeTradeCommand;
        public UnSubscribeCommand UnSubscribeTrade { get => _unSubscribeTradeCommand ?? (_unSubscribeTradeCommand = new UnSubscribeCommand(this)); }
        private SubscribeCandleCommand _subscribeCandleCommand;
        public SubscribeCandleCommand SubscribeCandleCommand { get => _subscribeCandleCommand ?? (_subscribeCandleCommand = new SubscribeCandleCommand(this)); }
        private UnSubscribeCandleCommand _unSubscribeCandleCommand;
        public UnSubscribeCandleCommand UnSubscribeCandleCommand { get => _unSubscribeCandleCommand ?? (_unSubscribeCandleCommand = new UnSubscribeCandleCommand(this)); }
        #endregion

        #endregion

        #region RESTAPI

        public ObservableCollection<Trade> RESTTrades { get; private set; }
        public ObservableCollection<Candle> RESTCandles { get; private set; }

        public List<string> TradePairsRest { get => _pairs; }
        public string TradeCountsRest { get; set; }
        public string TradeSelectedPairRest { get; set; }

        public List<string> CandlesPairRest { get => _pairs; }
        public string CandleSelectPairRest { get; set; }
        public string CandleIntervalSecRest { get; set; }
        public string CandleCountRest { get; set; }
        public string CandleFromRest { get; set; }
        public string CandleToRest { get; set; }

        #region Commands
        private GetTradesRest _getTradesRest;
        public GetTradesRest GetTradesRest { get => _getTradesRest ?? (_getTradesRest = new GetTradesRest(this)); }
        private GetCandlesRest _getCandlesRest;
        public GetCandlesRest GetCandlesRest { get => _getCandlesRest ?? (_getCandlesRest = new GetCandlesRest(this)); }
        #endregion

        #endregion

        #endregion

        public ViewModel()
        {
            Trades = new ObservableCollection<Trade>();
            Candles = new ObservableCollection<Candle>();
            RESTTrades = new ObservableCollection<Trade>();
            RESTCandles = new ObservableCollection<Candle>();
            TradeCount = "10";
            CandleCount = "0";
            CandleInterval = "60";
            SelectCandlePair = "BTCUSD";
            SelectTradePair = "BTCUSD";
            DateTime nx = new DateTime(1970, 1, 1);
            TimeSpan ts = DateTime.UtcNow - nx;
            //CandleDateFrom = ((long)ts.TotalMilliseconds + 60000).ToString(); //подписаться на "через минуту"
            CandleDateFrom = "";
            CandleDateTo = ((long)ts.TotalMilliseconds + 180000).ToString();  //отписаться в "через две минуты"

            TradeCountsRest = "100";
            TradeSelectedPairRest = "BTCUSD";
            CandleSelectPairRest = "BTCUSD";
            CandleIntervalSecRest = "60";
            CandleCountRest = "0";
            CandleFromRest = ((long)ts.TotalMilliseconds - 3000000).ToString(); //за последние 5 минут
            CandleToRest = "";

            Connector = new Connector();
            Connector.NewBuyTrade += AddTrade;
            Connector.NewSellTrade += AddTrade;
            Connector.CandleSeriesProcessing += AddCandle;

            Connector.log += (str) => { System.Windows.MessageBox.Show(str); };
        }

        private void AddTrade(Trade trade)
            => App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Trades.Insert(0, trade)));

        private void AddCandle(Candle candle)
            => App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Candles.Insert(0, candle)));
    }
}
