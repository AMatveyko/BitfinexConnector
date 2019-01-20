using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Cmd
{
    public class SubscribeCandleCommand : DelegateCommand
    {

        public SubscribeCandleCommand(ViewModel viewModel)
            : base (viewModel) { }

        public override void Execute(object parameter)
        {
            string error = "";
            long? count = 0;
            int candleIntervalInSec = 60;
            string pair = _viewModel.SelectCandlePair;
            DateTimeOffset? from = null;
            DateTimeOffset? to = null;
            try { count = long.Parse(_viewModel.CandleCount); }
            catch { error += "не верное количество\n"; }
            try { candleIntervalInSec = Int32.Parse(_viewModel.CandleInterval); }
            catch { error += "не верный интервал\n"; }
            try
            {
                if (_viewModel.CandleDateFrom != "")
                {
                    long i = long.Parse(_viewModel.CandleDateFrom);
                    from = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(i);
                }
            }
            catch { error += "не верная дата ОТ\n"; }

            try
            {
                if (_viewModel.CandleDateTo != "")
                {
                    long i = long.Parse(_viewModel.CandleDateTo);
                    to = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(i);
                }
            }
            catch { error += "не верная дата ДО\n"; }

            

            if (error != "")
                System.Windows.MessageBox.Show(error);
            _viewModel.Connector.SubscribeCandles(pair, candleIntervalInSec, from, to, count);
        }
    }
}
