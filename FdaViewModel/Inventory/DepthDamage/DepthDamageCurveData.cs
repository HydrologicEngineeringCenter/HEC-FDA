using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using Statistics;

namespace FdaViewModel.Inventory.DepthDamage
{
    //[Author(q0heccdm, 7 / 18 / 2017 9:40:26 AM)]
    public class DepthDamageCurveData
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/18/2017 9:40:26 AM
        #endregion
        #region Fields
        //private static Dictionary<string, DepthDamageCurve> _CurveDictionary;
        #endregion
        #region Properties
        public int Count
        {
            get { return CurveDictionary.Count; }
        }
        public static Dictionary<string, DepthDamageCurve> CurveDictionary
        {
            get;set;
        }
        #endregion
        #region Constructors
        public DepthDamageCurveData()
        {
            CurveDictionary = new Dictionary<string, DepthDamageCurve>();
            CreateDefaultDepthDamageTable();
        }
        #endregion
        #region Voids
        public void CreateDefaultDepthDamageTable()
        {
            UncertainCurveIncreasing curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            double[] Xs = { -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            double[] Ys = { 0, 0, 0, 0, 6, 11, 15, 19, 25, 30, 35, 41, 46, 51, 57, 63, 70, 75, 79, 82, 84, 87, 89, 90, 92, 93, 95, 96, 96 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve agrStruct = new DepthDamageCurve("Agricultural Structure", "Depth-damage curve for agricultural structures.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Agriculture Structure", agrStruct);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 6, 20, 43, 58, 65, 66, 66, 67, 70, 75, 76, 76, 76, 77, 77, 77, 78, 78, 78, 79, 79, 79, 79, 80, 80 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve agrCont = new DepthDamageCurve("Agricultural Content", "Depth-damage curve for agricultural content.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Agriculture Content", agrCont);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 1, 9, 14, 16, 18, 20, 23, 26, 30, 34, 38, 42, 47, 51, 55, 58, 61, 64, 67, 69, 71, 74, 76, 78, 80 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve retailStruct = new DepthDamageCurve("Retail Structure", "Depth-damage curve for retail structure.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Retail Structure", retailStruct);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 2, 26, 42, 56, 68, 78, 83, 85, 87, 88, 89, 90, 91, 92, 92, 92, 93, 93, 94, 94, 94, 94, 94, 94, 94 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve retailCont = new DepthDamageCurve("Retail Content", "Depth-damage curve for retail content.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Retail Content", retailCont);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 0, 5, 8, 11, 13, 16, 19, 22, 25, 29, 32, 37, 41, 45, 49, 52, 55, 58, 61, 63, 66, 68, 70, 71, 73 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve wholeSaleStruct = new DepthDamageCurve("Wholesale Trade Structure", "Depth-damage curve for wholesale trade structure.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Wholesale Trade Structure", wholeSaleStruct);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 3, 16, 27, 36, 49, 57, 63, 69, 72, 76, 80, 82, 84, 86, 87, 87, 88, 89, 89, 89, 89, 89, 89, 89, 89 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve wholeSaleCont = new DepthDamageCurve("Wholesale Trade Content", "Depth-damage curve for wholesale trade content.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Wholesale Trade Content", wholeSaleCont);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 3, 16, 27, 36, 49, 57, 63, 69, 72, 76, 80, 82, 84, 86, 87, 87, 88, 89, 89, 89, 89, 89, 89, 89, 89 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve pAndRStruct = new DepthDamageCurve("Personal and Repair Services Structure", "Depth-damage curve for personal and repair services structure.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Personal and Repair Services Structure", pAndRStruct);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 4, 29, 46, 67, 79, 85, 91, 92, 92, 93, 94, 96, 96, 97, 97, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve pAndRCont = new DepthDamageCurve("Personal and Repair Services Content", "Depth-damage curve for personal and repair services content.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Personal and Repair Services Content", pAndRCont);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 4, 29, 46, 67, 79, 85, 91, 92, 92, 93, 94, 96, 96, 97, 97, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve vehicle = new DepthDamageCurve("Vehicular", "Depth-damage curve for vehicles.", curve, DepthDamageCurve.DamageTypeEnum.Vehicle);
            CurveDictionary.Add("Vehicular", vehicle);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 1, 10, 12, 15, 19, 22, 26, 30, 35, 39, 42, 48, 50, 51, 53, 54, 55, 55, 56, 56, 57, 57, 57, 58, 58 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve heavyInd = new DepthDamageCurve("Heavy Industrial Structure", "Depth-damage curve for heavy industrial structures.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Heavy Industrial Structure", heavyInd);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Ys = new double[] { 0, 0, 0, 0, 0, 15, 24, 34, 41, 47, 52, 57, 60, 63, 64, 66, 68, 69, 72, 73, 73, 73, 74, 74, 74, 74, 75, 75, 75 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve heavyIndCont = new DepthDamageCurve("Heavy Industrial Content", "Depth-damage curve for heavy industrial content.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Heavy Industrial Content", heavyIndCont);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Xs = new double[] { -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            Ys = new double[] { 0, 2.5, 13.4, 23.3, 32.1, 40.1, 47.1, 53.2, 58.6, 63.2, 67.2, 70.5, 73.2, 75.4, 77.2, 78.5, 79.5, 80.2, 80.7 };
            //double[] stdDevs = new double[] { 0, 0.3, 1.2, 1.6, 1.6, 1.8, 1.9, 2, 2.1, 2.2, 2.3, 2.3, 2.35, 2.39, 2.4, 2.41, 2.42, 2.43, 2.43 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve res1SNB = new DepthDamageCurve("Residential 1SNB structure", "Depth-damage curve for residential structure with 1 story and no basement.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Residential 1SNB structure", res1SNB);

            curve = new UncertainCurveIncreasing(UncertainCurveDataCollection.DistributionsEnum.None, true, false);
            Xs = new double[] { -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            Ys = new double[] { 0, 2.4, 8.1, 13.3, 17.9, 22, 25.7, 28.8, 31.5, 33.8, 35.7, 37.2, 38.4, 39.2, 39.7, 40, 40, 40, 40 };
            //double[] stdDevs = new double[] { 0, 0.3, 1.2, 1.6, 1.6, 1.8, 1.9, 2, 2.1, 2.2, 2.3, 2.3, 2.35, 2.39, 2.4, 2.41, 2.42, 2.43, 2.43 };
            for (int i = 0; i < Xs.Count(); i++)
            {
                curve.Add(Xs[i], new None(Ys[i]));
            }
            DepthDamageCurve res1SNBCont = new DepthDamageCurve("Residential 1SNB Contents", "Depth-damage curve for residential structure with 1 story and no basement contents.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Residential 1SNB Contents", res1SNBCont);

        }
        #endregion
        #region Functions
        public static bool HasDepthDamageCurve(string curveName)
        {
            return CurveDictionary.ContainsKey(curveName);
        }
        public static DepthDamageCurve GetDepthDamageCurve(string curveName)
        {
            return CurveDictionary[curveName];
        }
        #endregion
    }
}
