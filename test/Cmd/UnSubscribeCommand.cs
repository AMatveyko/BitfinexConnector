using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Cmd
{
    public class UnSubscribeCommand : DelegateCommand
    {

        public UnSubscribeCommand(ViewModel viewModel)
            : base (viewModel) { }

        public override void Execute(object parameter)
        {
            _viewModel.Connector.UnsubscribeTrades(_viewModel.SelectTradePair);
        }
    }
}
