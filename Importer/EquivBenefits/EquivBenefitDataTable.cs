using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Importer
{
    public class EquivBenefitDataTable
    {
        public void ProcessEquivBenefitDataTable(DataTable dataTable)
        {
            EquivBenefitsList theFuncList = GlobalVariables.mp_fdaStudy.GetEquivBenefitsList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new prob function and Add
            for (int irow = 0; irow < dataTable.Rows.Count; irow++)
            {
                EquivBenefits theFunc = new EquivBenefits();

                for (int icol = 0; icol < dataTable.Columns.Count; icol++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[icol].ColumnName;

                    if (colName == "ID_PLAN")
                        theFunc.IdPlan = (int)dataTable.Rows[irow][icol];
                    else if (colName == "ID_IMPAREA")
                        theFunc.IdReach = (int)dataTable.Rows[irow][icol];
                    else if (colName == "DATE")
                        theFunc.CalculationDate = (string)dataTable.Rows[irow][icol];
                    else if (colName == "NUORDEQEAD")
                        ;// theFunc.NumberOfEquivAnnualDamage = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NUORDBENEF")
                        ;// theFunc.NumberOfBenefits = (int)dataTable.Rows[irow][icol];
                    else if(colName == "EQUIVADTB")
                    {
                        theMemoFieldStr = dataTable.Rows[irow][icol].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc.ReallocateEquivAnnualDamageWithCheckAndSave(numRowsMemo);  //rdc check; Need to clear ordinates?
                        long[] idCat = new long[numRowsMemo];
                        double[] equivDmg = new double[numRowsMemo];
                        for(int jr = 0; jr < numRowsMemo; jr++)
                        {
                            idCat[jr] = (int)(Math.Round(valuesFunc[jr,0]));
                            string catName = GlobalVariables.mp_fdaStudy.GetDamageCategoryList().getName(idCat[jr]);
                            equivDmg[jr] = valuesFunc[jr, 1];
                        }
                        theFunc.SetEquivAnnualDamgIdCategory(numRowsMemo, idCat);
                        theFunc.SetEquivAnnualDamg(numRowsMemo, equivDmg);
                    }
                    else if(colName == "BENEFITTB")
                    {
                        theMemoFieldStr = dataTable.Rows[irow][icol].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc.ReallocateBenefitsWithCheckAndSave(numRowsMemo);  //rdc check; Need to clear ordinates?
                        double[] prob = new double[numRowsMemo];
                        double[] benefits = new double[numRowsMemo];
                        for (int jr = 0; jr < numRowsMemo; jr++)
                        {
                            prob[jr] = valuesFunc[jr, 0];
                            benefits[jr] = valuesFunc[jr, 1];
                        }
                        theFunc.SetBenefitsProbability(numRowsMemo, prob);
                        theFunc.SetBenefits(numRowsMemo, benefits);
                    }
                }
                theFuncList.Add(theFunc);

            }
        }
    }
}
