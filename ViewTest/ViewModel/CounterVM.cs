using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.ViewModel.Implementations;
using System;

namespace ViewTest.ViewModel
{
    internal class CounterVM: BaseViewModel
    {
        private Model.Counter FastCounter = new Model.Counter(0, 25);
        private Model.Counter SlowCounter = new Model.Counter(0, 2);

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

        private int _instanceHash;
        public int InstanceHash
        {
            get { return _instanceHash; }
            set
            {
                _instanceHash = value;
                NotifyPropertyChanged();
            }
        }

        public CounterVM()
        {
            CountUp = new NamedAction();
            CountUp.Name = "CountUp";
            CountUp.Action = CountUpAction;
            InstanceHash = FastCounter.GetHashCode();
        }

        private void CountUpAction(object arg1, EventArgs arg2)
        {
            InstanceHash = FastCounter.GetHashCode();
            FastCounter.DoCounting();
            SlowCounter.DoCounting();
        }
    }
}
