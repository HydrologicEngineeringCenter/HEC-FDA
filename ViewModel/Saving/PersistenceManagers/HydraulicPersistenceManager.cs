using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Hydraulics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class HydraulicPersistenceManager : SavingBase<HydraulicElement>
    {
        public override string TableName
        {
            get { return "hydraulics"; }
        }



        public HydraulicPersistenceManager(Study.FDACache studyCache)
        {
            StudyCacheForSaving = studyCache;
        }

        #region utilities

        //private void SavePathAndProbabilitiesTable(HydraulicElement element)
        //{
        //    //gets called if savestotable is true
        //    if (!Connection.Instance.IsConnectionNull)
        //    {
        //        if (Connection.Instance.TableNames().Contains(PATH_AND_PROB_TABLE + element.Name))
        //        {
        //            //already exists... delete?
        //            Connection.Instance.DeleteTable(PATH_AND_PROB_TABLE + element.Name);
        //        }

        //        string[] colNames = new string[] { "Name", "Probability", "LastEdited" };
        //        Type[] colTypes = new Type[] { typeof(string), typeof(string), typeof(string) };

        //        Connection.Instance.CreateTable(PATH_AND_PROB_TABLE + element.Name, colNames, colTypes);
        //        DatabaseManager.DataTableView tbl = Connection.Instance.GetTable(PATH_AND_PROB_TABLE + element.Name);

        //        object[][] rows = new object[element.RelativePathAndProbability.Count][];
        //        int i = 0;
        //        foreach (PathAndProbability p in element.RelativePathAndProbability)
        //        {
        //            rows[i] = new object[] { p.Path, p.Probability, DateTime.Now.ToString() };
        //            i++;
        //        }
        //        for (int j = 0; j < rows.Count(); j++)
        //        {
        //            tbl.AddRow(rows[j]);
        //        }
        //        tbl.ApplyEdits();
        //    }
        //}

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

        #endregion

        //public override void SaveNew(ChildElement element)
        //{
        //    if (element.GetType() == typeof(HydraulicElement))
        //    {
        //        string editDate = DateTime.Now.ToString("G");
        //        element.LastEditDate = editDate;

        //        SaveNewElementToTable(GetRowDataFromElement((HydraulicElement)element), TableName, TableColumnNames, TableColumnTypes);
        //        SavePathAndProbabilitiesTable((HydraulicElement)element);
        //        //save files to the study directory
        //        StudyCacheForSaving.AddElement((HydraulicElement)element);
        //    }
        //}

        //public override void Remove(ChildElement element)
        //{
        //    RemoveElementFromTable(element, TableName);
        //    RemoveTable(PATH_AND_PROB_TABLE + element.Name);
        //    //if the wse was imported from old fda, then it won't have associated files.
        //    HydraulicElement elem = (HydraulicElement)element;
        //    if (elem.HasAssociatedFiles)
        //    {
        //        RemoveWaterSurfElevFiles((HydraulicElement)element);
        //    }
        //    StudyCacheForSaving.RemoveElement((HydraulicElement)element);
        //}

        //public void SaveExisting(ChildElement element, string oldName)
        //{
        //    base.SaveExisting(element);
        //    //delete the old table and create a new one
        //    Connection.Instance.DeleteTable(PATH_AND_PROB_TABLE + oldName);
        //    SavePathAndProbabilitiesTable((HydraulicElement)element);
        //}

        //public void RenamePathAndProbabilitesTableName(string oldName, string newName)
        //{
        //    Connection.Instance.RenameTable(PATH_AND_PROB_TABLE + oldName, PATH_AND_PROB_TABLE + newName);
        //}


    }
}
