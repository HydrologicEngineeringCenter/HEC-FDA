using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;

namespace DbMemory
{
    [Serializable]
    public class EquivBenefits
    {
        #region Fields
        private long[] _IdCategory = null;
        private double[] _EquivAnnualDamage = null;

        private double[] _ProbBenefit = null;
        private double[] _Benefit = null;
        #endregion

        #region Properties
        public long IdPlan { get; set; }
        public long IdReach { get; set; }
        public string CalculationDate { get; set; }
        public int NumberOfEquivAnnualDamage { get; set; }
        public int NumberOfEquivAnnualDamageAllocated { get; set; }
        public int NumberOfBenefits { get; set; }
        public int NumberOfBenefitsAllocated { get; set; }
        #endregion
        #region Constructors
        public EquivBenefits()
        {
            Reset();
        }
        #endregion
        #region Voids
        public void Reset()
        {
            NumberOfBenefits = NumberOfBenefitsAllocated = 0;
            NumberOfEquivAnnualDamage = NumberOfEquivAnnualDamageAllocated = 0;

        }
        public void ReallocateEquivAnnualDamageWithCheckAndSave(int numOrds)
        {
            if(numOrds > NumberOfEquivAnnualDamageAllocated)
            {
                long[] idCat = new long[numOrds];
                double[] equivAnnualDamage = new double[numOrds];
                for(int i = 0; i < numOrds; i++)
                {
                    idCat[i] = -2;
                    equivAnnualDamage[i] = Study.badNumber;
                }
                for(int i = 0; i < NumberOfEquivAnnualDamage;i++)
                {
                    idCat[i] = _IdCategory[i];
                    equivAnnualDamage[i] = _EquivAnnualDamage[i];
                }
                NumberOfEquivAnnualDamageAllocated = numOrds;
                _IdCategory = idCat;
                _EquivAnnualDamage = equivAnnualDamage;
            }
            return;
        }
        public void ReallocateBenefitsWithCheckAndSave(int numOrds)
        {
            if (numOrds > NumberOfBenefitsAllocated)
            {
                double[] probBenefit = new double[numOrds];
                double[] benefits = new double[numOrds];
                for (int i = 0; i < numOrds; i++)
                {
                    probBenefit[i] = Study.badNumber;
                    benefits[i] = Study.badNumber;
                }
                for (int i = 0; i < NumberOfBenefits; i++)
                {
                    probBenefit[i] = _ProbBenefit[i];
                    benefits[i] = _Benefit[i];
                }
                NumberOfBenefitsAllocated = numOrds;
                _ProbBenefit = probBenefit;
                _Benefit = benefits;
            }
            return;
        }
        public void SetEquivAnnualDamgIdCategory(int numPoints, long[] idCat)
        {
            ReallocateEquivAnnualDamageWithCheckAndSave(numPoints);
            NumberOfEquivAnnualDamage = numPoints;
            for (int i = 0; i < numPoints; i++) _IdCategory[i] = idCat[i];
        }
        public void SetEquivAnnualDamg(int numPoints, double[] equivAnnualDamage)
        {
            ReallocateEquivAnnualDamageWithCheckAndSave(numPoints);
            NumberOfEquivAnnualDamage = numPoints;
            for (int i = 0; i < numPoints; i++) _EquivAnnualDamage[i] = equivAnnualDamage[i];
        }
        public void SetBenefitsProbability(int numPoints, double[] benefitsProb)
        {
            ReallocateBenefitsWithCheckAndSave(numPoints);
            NumberOfBenefits = numPoints;
            for (int i = 0; i < numPoints; i++) _ProbBenefit[i] = benefitsProb[i];
        }
        public void SetBenefits(int numPoints, double[] benefits)
        {
            ReallocateBenefitsWithCheckAndSave(numPoints);
            NumberOfBenefits = numPoints;
            for (int i = 0; i < numPoints; i++) _Benefit[i] = benefits[i];
        }
        public void Print()
        {
            //Basic Information
            WriteLine($"\n\n\tEquivalent Annual Damage and Benefits, Plan: {IdPlan}");
            WriteLine($"\t{GlobalVariables.mp_fdaStudy.GetPlanList().getName(IdPlan)}");
            WriteLine($"\tReach: {IdReach}");
            WriteLine($"\t{GlobalVariables.mp_fdaStudy.GetDamageReachList().getName(IdReach)}");
            WriteLine($"\tCalculation Date: {CalculationDate}");
            WriteLine($"\tNumber of Equiv. Annual Damage Ordinates: {NumberOfEquivAnnualDamage}");
            WriteLine($"\tNumber of Benefit Ordinates: {NumberOfBenefits}");

            //Equivalent Annual Damage
            WriteLine("\n\tEquivalent Annual Damage");
            for(int i = 0; i < NumberOfEquivAnnualDamage; i++)
            {
                string catName = GlobalVariables.mp_fdaStudy.GetDamageCategoryList().getName(_IdCategory[i]);
                Write($"\t{Convert.ToString(i).PadLeft(5)}");
                Write($"{_IdCategory[i].ToString("N0").PadLeft(8)}");
                Write($"  {catName.PadRight(33)}");
                Write($"  {_EquivAnnualDamage[i].ToString("F2").PadLeft(12)}");
                Write("\n");
            }

            //Benefits
            WriteLine("\n\tAnnual Benefits");
            for(int i = 0; i < NumberOfBenefits; i++)
            {
                Write($"\t{Convert.ToString(i).PadLeft(5)}");
                Write($"  {_ProbBenefit[i].ToString("F4").PadLeft(12)}");
                Write($"  {_Benefit[i].ToString("F1").PadLeft(12)}");
                Write("\n");
            }
        }
        public void PrintToFile()
        {
            //StreamWriter wr = GlobalVariables.mp_fdaStudy._StreamWriter;
            //GlobalVariables.mp_fdaStudy._StreamWriter.WriteLine("\nFirst Attempt to Write to text file.");
            StreamWriter wr = Study._StreamWriter;


            //Basic Information
            wr.WriteLine($"\n\nEquivalent Annual Damage and Benefits, Plan ID: {IdPlan}");
            wr.WriteLine($"\t{GlobalVariables.mp_fdaStudy.GetPlanList().getName(IdPlan)}");
            wr.WriteLine($"\tReach: {IdReach}");
            wr.WriteLine($"\t{GlobalVariables.mp_fdaStudy.GetDamageReachList().getName(IdReach)}");
            wr.WriteLine($"\tCalculation Date: {CalculationDate}");
            wr.WriteLine($"\tNumber of Equiv. Annual Damage Ordinates: {NumberOfEquivAnnualDamage}");
            wr.WriteLine($"\tNumber of Benefit Ordinates: {NumberOfBenefits}");

            //Equivalent Annual Damage
            wr.WriteLine("\n\tEquivalent Annual Damage");
            for (int i = 0; i < NumberOfEquivAnnualDamage; i++)
            {
                string catName = GlobalVariables.mp_fdaStudy.GetDamageCategoryList().getName(_IdCategory[i]);
                wr.Write($"\t{Convert.ToString(i).PadLeft(5)}");
                wr.Write($"\t{_IdCategory[i].ToString("N0").PadLeft(8)}");
                wr.Write($"  \t{catName.PadRight(33)}");
                wr.Write($"  \t{_EquivAnnualDamage[i].ToString("F2").PadLeft(12)}");
                wr.Write("\n");
            }

            //Benefits
            wr.WriteLine("\n\tAnnual Benefits");
            for (int i = 0; i < NumberOfBenefits; i++)
            {
                wr.Write($"\t{Convert.ToString(i).PadLeft(5)}");
                wr.Write($"  \t{_ProbBenefit[i].ToString("F4").PadLeft(12)}");
                wr.Write($"  \t{_Benefit[i].ToString("F1").PadLeft(12)}");
                wr.Write("\n");
            }
        }

        #endregion
        #region Functions
        #endregion


    }
}
