using HEC.FDA.Model.compute;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public class ProgressRowItem:BaseViewModel
    {
        private int _Progress;
        private string _ProgressLabel;

        public int Progress
        {
            get { return _Progress; }
            set { _Progress = value; NotifyPropertyChanged(); }
        }

        public string ProgressLabel
        {
            get { return _ProgressLabel; }
            set { _ProgressLabel = value; NotifyPropertyChanged(); }
        }
        public ImpactAreaScenarioSimulation Simulation { get; }
        public ProgressRowItem(ImpactAreaScenarioSimulation sim)
        {
            Simulation = sim;
            ProgressLabel = "Impact Area: " + GetImpactAreaFromID(sim.ImpactAreaID);
        }

        private string GetImpactAreaFromID(int id)
        {
            string impactName = null;
            List<ImpactAreaElement> impactAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElems.Count > 0)
            {
                //there only ever be one or zero
                List<ImpactAreaRowItem> impactAreaRows = impactAreaElems[0].ImpactAreaRows;
                foreach (ImpactAreaRowItem row in impactAreaRows)
                {
                    if (row.ID == id)
                    {
                        impactName = row.Name;
                        break;
                    }
                }
            }
            return impactName;
        }

    }
}
