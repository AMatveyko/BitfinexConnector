using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLibrary;

namespace test.Cmd
{
    public class GetTradesRest : DelegateCommand
    {

        public GetTradesRest(ViewModel viewModel)
            : base (viewModel) { }

        public async override void Execute(object parameter)
        {
            string pair = _viewModel.TradeSelectedPairRest;
            int maxCount = 10;
            try
            {
                maxCount = Int32.Parse(_viewModel.TradeCountsRest);
            }
            catch { }
            var trades = await _viewModel.Connector.GetNewTradesAsync(pair,maxCount);
            _viewModel.RESTTrades.Clear();
            foreach (var trd in trades)
                _viewModel.RESTTrades.Add(trd);
        }
    }
}
