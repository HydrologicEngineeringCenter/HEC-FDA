using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.IO;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class HydraulicPersistenceManager : SavingBase<HydraulicElement>
    {
        
        public HydraulicPersistenceManager(Study.FDACache studyCache, string tableName):base(studyCache, tableName)
        {
        }
      
        private void RemoveWaterSurfElevFiles(HydraulicElement element)
        {
            try
            {
                Directory.Delete(Connection.Instance.HydraulicsDirectory + "\\" + element.Name,true);
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        public override void Remove(ChildElement element)
        {
            RemoveElementFromTable(element);
            RemoveWaterSurfElevFiles((HydraulicElement)element);
            StudyCacheForSaving.RemoveElement((HydraulicElement)element);
        }

    }
}
