using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving
{
    public interface IPersistable
    {
         void SaveNew(Utilities.ChildElement element);
        void SaveExisting(Utilities.ChildElement element, string oldName, Statistics.UncertainCurveDataCollection oldCurve);
         List<Utilities.ChildElement> Load();
    }
}
