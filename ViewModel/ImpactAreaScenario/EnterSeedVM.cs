using System;

namespace HEC.FDA.ViewModel.ImpactAreaScenario
{
    public class EnterSeedVM:BaseViewModel
    {
        public int Seed { get; set; }
        public EnterSeedVM()
        {
            SetDimensions(450, 130, 450, 130);
            Seed = DateTime.Now.Millisecond;
        }

    }
}
