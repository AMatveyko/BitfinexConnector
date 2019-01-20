using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Cmd
{
    public class UnSubscribeCandleCommand : DelegateCommand
    {

        public UnSubscribeCandleCommand(ViewModel viewModel)
            : base (viewModel) { }

        public override void Execute(object parameter)
        {
            _viewModel.Connector.UnsubscribeCandles(_viewModel.SelectTradePair);
        }
    }
}
