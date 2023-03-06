using System;

namespace HEC.FDA.ViewModel.ImpactAreaScenario
{
    public class EnterSeedVM:BaseViewModel
    {
        public int Seed { get; set; }
        public EnterSeedVM()
        {
            Seed = DateTime.Now.Millisecond;
        }

    }
}
