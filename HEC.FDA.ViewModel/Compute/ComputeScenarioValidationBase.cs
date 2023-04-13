using HEC.FDA.Model.compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Compute
{
    public class ComputeScenarioValidationBase:ComputeWithProgressAndMessagesBase
    {

        public FdaValidationResult CanScenarioCompute(IASElement element)
        {
            
            FdaValidationResult canComputeVr = new FdaValidationResult();
            foreach (SpecificIAS ias in element.SpecificIASElements)
            {
                FdaValidationResult canComputeScenario = ias.CanComputeScenario();
                if (!canComputeScenario.IsValid)
                {              
                    canComputeVr.AddErrorMessage(canComputeScenario.ErrorMessage);
                }
            }
            return canComputeVr;
        }

       

    }
}
