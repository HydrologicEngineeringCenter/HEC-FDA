using HEC.FDA.ViewModel.ImpactAreaScenario;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioDamageSummaryVM : BaseViewModel
    {
        public List<ScenarioDamageRowItem> Rows { get; } = new List<ScenarioDamageRowItem>();
        public List<ScenarioPerformanceRowItem> PerformanceRows { get; } = new List<ScenarioPerformanceRowItem>();

        public DataTable DamCatTable { get; set; } = new DataTable();

        public ScenarioDamageSummaryVM(List<IASElement> scenarioElems)
        {
            List<ScenarioDamCatRowItem> damCatRows = new List<ScenarioDamCatRowItem>();
            foreach(IASElement element in scenarioElems)
            {
                Rows.Add(new ScenarioDamageRowItem(element));
                damCatRows.Add(new ScenarioDamCatRowItem(element));
                PerformanceRows.Add(new ScenarioPerformanceRowItem(element));
            }
            LoadDamCatDataTable(damCatRows);
        }

        private void LoadDamCatDataTable(List<ScenarioDamCatRowItem> rows)
        {
            DataColumn nameCol = new DataColumn("Name", typeof(string));
            DamCatTable.Columns.Add(nameCol);
            DataColumn yearCol = new DataColumn("Analysis Year", typeof(int));
            DamCatTable.Columns.Add(yearCol);
            List<string> allUniqueDamCats = GetAllDamCats(rows);
            foreach (string damCat in allUniqueDamCats)
            {
                DamCatTable.Columns.Add( new DataColumn(damCat, typeof(string)));
            }

            foreach(ScenarioDamCatRowItem row in rows)
            {
                AddDamCatRowToTable(row, allUniqueDamCats);
            }
        }

        private void AddDamCatRowToTable(ScenarioDamCatRowItem row, List<string> allDamCats)
        {
            DataRow myRow = DamCatTable.NewRow();
            myRow["Name"] = row.Name;
            myRow["Analysis Year"] = row.AnalysisYear;
            foreach(string damCat in allDamCats)
            {
                if(row.DamCatMap.ContainsKey(damCat))
                {
                    myRow[damCat] = row.DamCatMap[damCat];
                }
                else
                {
                    //this scenario doesn't have a value for that dam cat. Assign 0.
                    myRow[damCat] = 0;
                }
            }
            DamCatTable.Rows.Add(myRow);
        }

        private List<string> GetAllDamCats(List<ScenarioDamCatRowItem> rows)
        {
            HashSet<string> uniqueDamCats = new HashSet<string>();
            foreach(ScenarioDamCatRowItem row in rows)
            {
                foreach(string damCat in row.DamCatMap.Keys)
                {
                    uniqueDamCats.Add(damCat);
                }
            }
            return uniqueDamCats.ToList();
        }
    }
}
