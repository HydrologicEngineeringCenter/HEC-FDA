using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving
{
    public interface IPersistable
    {
        
        void SaveNew(Utilities.ChildElement element);
        void Remove(Utilities.ChildElement element);
        void SaveExisting(Utilities.ChildElement oldElement, ChildElement elementToSave, int changeTableIndex);
         List<Utilities.ChildElement> Load();


       


    }
}
