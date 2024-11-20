using HEC.CS.Collections;
using HEC.FDA.ViewModel.Results;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HEC.FDA.ViewModel.Alternatives.Results.BatchCompute
{


    public class AlternativeSummaryVM : BaseViewModel
    {
        public List<AlternativeDamageRowItem> Rows { get; } = new List<AlternativeDamageRowItem>();

        public DataTable DamCatTable { get; set; } = new DataTable();

        public AlternativeSummaryVM(List<AlternativeElement> altElems)
        {
            List<AlternativeDamCatRowItem> damCatRows = new List<AlternativeDamCatRowItem>();
            foreach (AlternativeElement element in altElems)
            {
                Rows.AddRange( AlternativeDamageRowItem.CreateAlternativeDamageRowItems(element));
                damCatRows.AddRange( AlternativeDamCatRowItem.CreateAlternativeDamCatRowItems(element));
            }
            LoadDamCatDataTable(damCatRows);
        }

        private void LoadDamCatDataTable(List<AlternativeDamCatRowItem> rows)
        {
            DataColumn nameCol = new DataColumn("Name", typeof(string));
            DamCatTable.Columns.Add(nameCol);
            DataColumn impactAreaCol = new DataColumn("Impact Area", typeof(string));
            DamCatTable.Columns.Add(impactAreaCol);
            List<string> allUniqueDamCats = GetAllDamCats(rows);
            foreach (string damCat in allUniqueDamCats)
            {
                DamCatTable.Columns.Add(new DataColumn(damCat, typeof(string)));
            }

            foreach (AlternativeDamCatRowItem row in rows)
            {
                AddDamCatRowToTable(row, allUniqueDamCats);
            }
        }

        private void AddDamCatRowToTable(AlternativeDamCatRowItem row, List<string> allDamCats)
        {
            DataRow myRow = DamCatTable.NewRow();
            myRow["Name"] = row.Name;
            myRow["Impact Area"] = row.ImpactArea;
            foreach (string damCat in allDamCats)
            {
                if (row.DamCatMap.ContainsKey(damCat))
                {
                    myRow[damCat] = row.DamCatMap[damCat];
                }
                else
                {
                    //this alternative doesn't have a value for that dam cat. Assign 0.
                    myRow[damCat] = 0;
                }
            }
            DamCatTable.Rows.Add(myRow);
        }

        private List<string> GetAllDamCats(List<AlternativeDamCatRowItem> rows)
        {
            HashSet<string> uniqueDamCats = new HashSet<string>();
            foreach (AlternativeDamCatRowItem row in rows)
            {
                foreach (string damCat in row.DamCatMap.Keys)
                {
                    uniqueDamCats.Add(damCat);
                }
            }
            return uniqueDamCats.ToList();
        }


    }
}
