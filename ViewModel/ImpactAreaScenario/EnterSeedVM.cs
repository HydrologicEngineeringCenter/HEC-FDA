using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario
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
