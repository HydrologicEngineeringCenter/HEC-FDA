using HEC.MVVMFramework.ViewModel.Implementations;
using System;

namespace HEC.MVVMFramework.ViewTest.ViewModel
{
    internal class CounterVM: BaseViewModel
    {
        private Model.Counter FastCounter = new Model.Counter(0, 25);
        private Model.Counter SlowCounter = new Model.Counter(0, 2);
        private Model.Counter SuperFastCounter = new Model.Counter(0, 2);
        private Model.Counter SuperSlowCounter = new Model.Counter(0, 2);

        private NamedAction _countUp;
        public NamedAction CountUp
        {
            get { return _countUp; }
            set { _countUp = value; NotifyPropertyChanged(); }
        }
        private int _countInterval = 1;
        public int CountInterval
        {
            get { return _countInterval; }
            set { _countInterval = value; }
        }

        public SubscriberMessageViewModel MySubscriberMessageViewModel { get; } = new SubscriberMessageViewModel();
        public CounterVM()
        {
            CountUp = new NamedAction();
            CountUp.Name = "CountUp";
            CountUp.Action = CountUpAction;
            MySubscriberMessageViewModel.InstanceHash.Add(FastCounter.GetHashCode());
        }

        private void CountUpAction(object arg1, EventArgs arg2)
        {
            FastCounter.DoCounting();
            SlowCounter.DoCounting();
            SuperFastCounter.DoCounting();
            SuperSlowCounter.DoCounting();
        }
    }
}
