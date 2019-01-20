using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Cmd
{
    public class SubscribeCommand : DelegateCommand
    {

        public SubscribeCommand(ViewModel viewModel)
            : base (viewModel) { }

        public override void Execute(object parameter)
        {
            int maxCount;
            try
            {
                maxCount = Int32.Parse(_viewModel.TradeCount);
                _viewModel.Connector.SubscribeTrades(_viewModel.SelectTradePair, maxCount);
            }
            catch
            {
                System.Windows.MessageBox.Show("не верное количество");
            }
        }
    }
}
