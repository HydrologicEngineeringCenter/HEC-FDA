using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functions;
using Statistics;

namespace ViewModel.Inventory.DepthDamage
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
            List<double> Xs = new List<double>() { -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            List<double> Ys = new List<double>() { 0, 0, 0, 0, 6, 11, 15, 19, 25, 30, 35, 41, 46, 51, 57, 63, 70, 75, 79, 82, 84, 87, 89, 90, 92, 93, 95, 96, 96 };
            ICoordinatesFunction curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            string name = "Agricultural Structure";
            string desc = "Depth-damage curve for agricultural structures.";
            DepthDamageCurve agrStruct = new DepthDamageCurve(name, desc, curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Agriculture Structure", agrStruct);

            Ys = new List<double>() { 0, 0, 0, 0, 6, 20, 43, 58, 65, 66, 66, 67, 70, 75, 76, 76, 76, 77, 77, 77, 78, 78, 78, 79, 79, 79, 79, 80, 80 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            name = "Agricultural Content";
            desc = "Depth-damage curve for agricultural content.";
            DepthDamageCurve agrCont = new DepthDamageCurve(name, desc, curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Agriculture Content", agrCont);


            Ys = new List<double>() { 0, 0, 0, 0, 1, 9, 14, 16, 18, 20, 23, 26, 30, 34, 38, 42, 47, 51, 55, 58, 61, 64, 67, 69, 71, 74, 76, 78, 80 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            name = "Retail Structure";
            desc = "Depth-damage curve for retail structure.";
            DepthDamageCurve retailStruct = new DepthDamageCurve(name, desc, curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Retail Structure", retailStruct);

            Ys = new List<double>() { 0, 0, 0, 0, 2, 26, 42, 56, 68, 78, 83, 85, 87, 88, 89, 90, 91, 92, 92, 92, 93, 93, 94, 94, 94, 94, 94, 94, 94 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            DepthDamageCurve retailCont = new DepthDamageCurve("Retail Content", "Depth-damage curve for retail content.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Retail Content", retailCont);

            Ys = new List<double>() { 0, 0, 0, 0, 0, 5, 8, 11, 13, 16, 19, 22, 25, 29, 32, 37, 41, 45, 49, 52, 55, 58, 61, 63, 66, 68, 70, 71, 73 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            DepthDamageCurve wholeSaleStruct = new DepthDamageCurve("Wholesale Trade Structure", "Depth-damage curve for wholesale trade structure.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Wholesale Trade Structure", wholeSaleStruct);

            Ys = new List<double>() { 0, 0, 0, 0, 3, 16, 27, 36, 49, 57, 63, 69, 72, 76, 80, 82, 84, 86, 87, 87, 88, 89, 89, 89, 89, 89, 89, 89, 89 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            DepthDamageCurve wholeSaleCont = new DepthDamageCurve("Wholesale Trade Content", "Depth-damage curve for wholesale trade content.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Wholesale Trade Content", wholeSaleCont);

            Ys = new List<double>() { 0, 0, 0, 0, 3, 16, 27, 36, 49, 57, 63, 69, 72, 76, 80, 82, 84, 86, 87, 87, 88, 89, 89, 89, 89, 89, 89, 89, 89 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            DepthDamageCurve pAndRStruct = new DepthDamageCurve("Personal and Repair Services Structure", "Depth-damage curve for personal and repair services structure.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Personal and Repair Services Structure", pAndRStruct);

            Ys = new List<double>() { 0, 0, 0, 0, 4, 29, 46, 67, 79, 85, 91, 92, 92, 93, 94, 96, 96, 97, 97, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            DepthDamageCurve pAndRCont = new DepthDamageCurve("Personal and Repair Services Content", "Depth-damage curve for personal and repair services content.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Personal and Repair Services Content", pAndRCont);

            Ys = new List<double>() { 0, 0, 0, 0, 4, 29, 46, 67, 79, 85, 91, 92, 92, 93, 94, 96, 96, 97, 97, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            DepthDamageCurve vehicle = new DepthDamageCurve("Vehicular", "Depth-damage curve for vehicles.", curve, DepthDamageCurve.DamageTypeEnum.Vehicle);
            CurveDictionary.Add("Vehicular", vehicle);

            Ys = new List<double>() { 0, 0, 0, 0, 1, 10, 12, 15, 19, 22, 26, 30, 35, 39, 42, 48, 50, 51, 53, 54, 55, 55, 56, 56, 57, 57, 57, 58, 58 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            DepthDamageCurve heavyInd = new DepthDamageCurve("Heavy Industrial Structure", "Depth-damage curve for heavy industrial structures.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Heavy Industrial Structure", heavyInd);

            Ys = new List<double>() { 0, 0, 0, 0, 0, 15, 24, 34, 41, 47, 52, 57, 60, 63, 64, 66, 68, 69, 72, 73, 73, 73, 74, 74, 74, 74, 75, 75, 75 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            DepthDamageCurve heavyIndCont = new DepthDamageCurve("Heavy Industrial Content", "Depth-damage curve for heavy industrial content.", curve, DepthDamageCurve.DamageTypeEnum.Content);
            CurveDictionary.Add("Heavy Industrial Content", heavyIndCont);

            Xs = new List<double>() { -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            Ys = new List<double>() { 0, 2.5, 13.4, 23.3, 32.1, 40.1, 47.1, 53.2, 58.6, 63.2, 67.2, 70.5, 73.2, 75.4, 77.2, 78.5, 79.5, 80.2, 80.7 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
            DepthDamageCurve res1SNB = new DepthDamageCurve("Residential 1SNB structure", "Depth-damage curve for residential structure with 1 story and no basement.", curve, DepthDamageCurve.DamageTypeEnum.Structural);
            CurveDictionary.Add("Residential 1SNB structure", res1SNB);

            Xs = new List<double>() { -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            Ys = new List<double>() { 0, 2.4, 8.1, 13.3, 17.9, 22, 25.7, 28.8, 31.5, 33.8, 35.7, 37.2, 38.4, 39.2, 39.7, 40, 40, 40, 40 };
            curve = ICoordinatesFunctionsFactory.Factory(Xs, Ys);
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
