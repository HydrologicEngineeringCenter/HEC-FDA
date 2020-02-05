using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using static System.Console;

namespace DbMemory
{
    public class EquivBenefitsList
    {
        #region Fields
        private EquivBenefits _EquivBenefits;
        private List<EquivBenefits> _EquivalentBenefitsList = new List<EquivBenefits>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        #endregion
        #region Voids
        public void Add(EquivBenefits theBenefits)
        {
            EquivBenefits aBenefit = DbMemory.ObjectCopier.Clone(theBenefits);
            _EquivalentBenefitsList.Add(aBenefit);
            WriteLine($"Add Equivalent Annual Damage and Benefits to SortList.  \tPlan ID: {aBenefit.IdPlan}\tReach ID: {aBenefit.IdReach}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aBenefit.Print();
            //aBenefit.PrintToFile();
        }
        public void Print()
        {
            EquivBenefits aFunc;
            Write("\n\n\n\n");
            //for (int j = 0; j < 100; j++) Write("-");
            WriteLine($"\nNumber of Equivalent Annual Damage and Benefit Functions {_EquivalentBenefitsList.Count}");
            for (int i = 0; i < _EquivalentBenefitsList.Count; i++)
            {
                aFunc = _EquivalentBenefitsList.ElementAt(i);
                aFunc.Print();
            }
        }
        public void PrintToFile()
        {
            EquivBenefits aFunc;
            //Write("\n\n\n\n");
            //for (int j = 0; j < 100; j++) Write("-");
            Study._StreamWriter.WriteLine($"\nNumber of Equivalent Annual Damage and Benefit Functions {_EquivalentBenefitsList.Count}");
            WriteLine($"\nNumber of Equivalent Annual Damage and Benefit Functions {_EquivalentBenefitsList.Count}");
            for (int i = 0; i < _EquivalentBenefitsList.Count; i++)
            {
                aFunc = _EquivalentBenefitsList.ElementAt(i);
                aFunc.PrintToFile();
            }
        }

        #endregion
        #region Functions
        #endregion

    }
}
