using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLibrary;

namespace test.Cmd
{
    public class GetCandlesRest : DelegateCommand
    {

        public GetCandlesRest(ViewModel viewModel)
            : base (viewModel) { }

        public async override void Execute(object parameter)
        {
            string error = "";
            long? count = 0;
            int periodInSec = 60;
            string pair = _viewModel.CandleSelectPairRest;
            DateTimeOffset? from = null;
            DateTimeOffset? to = null;
            try { count = long.Parse(_viewModel.CandleCountRest); }
            catch { error += "не верное количество\n"; }
            try { periodInSec = Int32.Parse(_viewModel.CandleIntervalSecRest); }
            catch { error += "не верный интервал\n"; }
            try
            {
                if (_viewModel.CandleFromRest != "")
                {
                    long i = long.Parse(_viewModel.CandleFromRest);
                    from = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(i);
                }
            }
            catch { error += "не верная дата ОТ\n"; }
            try
            {
                if (_viewModel.CandleToRest != "")
                {
                    long i = long.Parse(_viewModel.CandleToRest);
                    to = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(i);
                }
            }
            catch { error += "не верная дата ДО\n"; }
            if (error != "")
                System.Windows.MessageBox.Show(error);
            var candles = await _viewModel.Connector.GetCandleSeriesAsync(pair, periodInSec, from, to, count);
            _viewModel.RESTCandles.Clear();
            foreach (var cnd in candles)
                _viewModel.RESTCandles.Add(cnd);
        }
    }
}
