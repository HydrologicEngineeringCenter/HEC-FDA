using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FdaViewModel.Inventory.DamageCategory;
using Functions;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    internal class OccupancyType : IOccupancyType
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IDamageCategory DamageCategory { get; set; }

        public bool CalculateStructureDamage { get; set; }

        public bool CalcualateContentDamage { get; set; }

        public bool CalculateVehicleDamage { get; set; }

        public bool CalculateOtherDamage { get; set; }

        public ICoordinatesFunction StructureDepthDamageFunction 
        { 
            get; 
            set; 
        }
        public ICoordinatesFunction ContentDepthDamageFunction { get; set; }
        public ICoordinatesFunction VehicleDepthDamageFunction { get; set; }
        public ICoordinatesFunction OtherDepthDamageFunction { get; set; }
        public IDistributedOrdinate StructureValueUncertainty { get; set; }
        public IDistributedOrdinate ContentValueUncertainty { get; set; }
        public IDistributedOrdinate VehicleValueUncertainty { get; set; }
        public IDistributedOrdinate OtherValueUncertainty { get; set; }
        public IDistributedOrdinate FoundationHeightUncertainty { get; set; }
        public string StructureDepthDamageName { get; set; }
        public string ContentDepthDamageName { get; set; }
        public string VehicleDepthDamageName { get; set; }
        public string OtherDepthDamageName { get; set; }

        public OccupancyType()
        {

        }
        public OccupancyType(string name, string damageCategoryName)
        {

        }

        public OccupancyType(XElement Xel)
        {
            //Name = Xel.Attribute("Name").Value;
            //if (Xel.Elements("Description").Any())
            //    Description = Xel.Element("Description").Value;
            //else
            //    Description = "";
            //if (Xel.Elements("DamageCategory").Any())
            //    DamageCategory = new DamageCategory(Xel.Element("DamageCategory"));
            //else
            //    DamageCategory = new DamageCategory();
            //if (Xel.Elements("FoundationHeightUncertainty").Any)
            //    FndHeightAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("FoundationHeightUncertainty").Descendants.First);
            //else
            //    _FndHeightAsPcntOfMean = new None();
            //// 
            //if (Xel.Elements("StructureUncertainty").Any)
            //    _StructureValAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("StructureUncertainty").Descendants.First);
            //else
            //    _StructureValAsPcntOfMean = new None();
            //// 
            //if (Xel.Elements("ContentUncertainty").Any)
            //    _ContentValAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("ContentUncertainty").Descendants.First);
            //else
            //    _ContentValAsPcntOfMean = new None();
            //// 
            //if (Xel.Elements("OtherUncertainty").Any)
            //    _OtherValAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("OtherUncertainty").Descendants.First);
            //else
            //    _OtherValAsPcntOfMean = new None();
            //// 
            //if (Xel.Elements("VehicleUncertainty").Any)
            //    _VehicleValAsPcntOfMean = ContinuousDistribution.readfromXElement(Xel.Element("VehicleUncertainty").Descendants.First);
            //else
            //    _VehicleValAsPcntOfMean = new None();
            //// 
            //XElement DDCurve = Xel.Element("StructureDD");
            //_CalcStructDamage = System.Convert.ToBoolean(DDCurve.Attribute("CalculateDamage").Value);
            //if (DDCurve.Elements.Count > 0)
            //    _StructureDDPercent = new MonotonicCurveUSingle(DDCurve.Element("MonotonicCurveUSingle"));
            //else
            //    _StructureDDPercent = new MonotonicCurveUSingle();
            //// 
            //DDCurve = Xel.Element("ContentDD");
            //_CalcContentDamage = System.Convert.ToBoolean(DDCurve.Attribute("CalculateDamage").Value);
            //if (DDCurve.Elements.Count > 0)
            //    _ContentDDPercent = new MonotonicCurveUSingle(DDCurve.Element("MonotonicCurveUSingle"));
            //else
            //    _ContentDDPercent = new MonotonicCurveUSingle();
            //// 
            //DDCurve = Xel.Element("OtherDD");
            //_CalcOtherDamage = System.Convert.ToBoolean(DDCurve.Attribute("CalculateDamage").Value);
            //if (DDCurve.Elements.Count > 0)
            //    _OtherDDPercent = new MonotonicCurveUSingle(DDCurve.Element("MonotonicCurveUSingle"));
            //else
            //    _OtherDDPercent = new MonotonicCurveUSingle();
            //// 
            //DDCurve = Xel.Element("VehicleDD");
            //_CalcVehicleDamage = System.Convert.ToBoolean(DDCurve.Attribute("CalculateDamage").Value);
            //if (DDCurve.Elements.Count > 0)
            //    _VehicleDDPercent = new MonotonicCurveUSingle(DDCurve.Element("MonotonicCurveUSingle"));
            //else
            //    _VehicleDDPercent = new MonotonicCurveUSingle();
        }

        public IOccupancyType Clone()
        {
            throw new NotImplementedException();
        }

        //    public void LoadFromFDAInformation(StringBuilder occtype, int startdata, int parameter)
        //    {
        //        string[] lines = occtype.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        //        string[] line = new string[0];
        //        string[,] tmparray = new string[0,1];
        //        int stagecount = 0;
        //        string separator = "\t";
        //        List<float> stages = new List<float>();
        //        string curvetype = "";
        //        float val = 0;
        //        for (var i = 0; i <= lines.Count() - 1; i++)
        //        {
        //            if (i == 0)
        //            {
        //                line = lines[i].Split(separator.ToCharArray());
        //                if (i == 0)
        //                {
        //                    Name = line[0];
        //                    Description = line[1];
        //                    DamageCategory = new DamageCategory(line[2]);
        //                }
        //            }

        //            switch (line[parameter])
        //            {
        //                case "Stage":
        //                    {
        //                        if (stages.Count > 0)
        //                        {
        //                            stages.Clear();
        //                        }
        //                        for (var j = parameter + 1; j <= line.Count() - 1; j++)
        //                        {
        //                            if (float.TryParse(line[j], out val))
        //                            {
        //                                stages.Add(val);
        //                            }
        //                        }
        //                        i += 1;
        //                        line = lines[i].Split(separator.ToCharArray());
        //                        curvetype = line[parameter][0].ToString();
        //                        while (line[parameter] == "Stage" | line[parameter] == "Struct" | curvetype != line[parameter][0].ToString())
        //                        {
        //                            if (tmparray == null)
        //                                tmparray = new String[0,1];
        //                            else
        //                            {
        //                                var oldTmparray = tmparray;
        //                                tmparray = new String()[tmparray.Count() + 1];
        //                                if (oldTmparray != null)
        //                                    Array.Copy(oldTmparray, tmparray, Math.Min(tmparray.Count() + 1, oldTmparray.Length));
        //                            }
        //                            tmparray[tmparray.Count() - 1] = line;
        //                            i += 1;
        //                            line = lines[i].Split(separator.ToCharArray());
        //                        }
        //                        switch (curvetype)
        //                        {
        //                            case "S":
        //                                {
        //                                    _CalcStructDamage = true;
        //                                    _StructureDDPercent = ConvertFDAStringToCurve(tmparray, stages, parameter);
        //                                    break;
        //                                }

        //                            case "C":
        //                                {
        //                                    _CalcContentDamage = true;
        //                                    _ContentDDPercent = ConvertFDAStringToCurve(tmparray, stages, parameter);
        //                                    break;
        //                                }

        //                            case "O":
        //                                {
        //                                    _CalcOtherDamage = true;
        //                                    _OtherDDPercent = ConvertFDAStringToCurve(tmparray, stages, parameter);
        //                                    break;
        //                                }

        //                            default:
        //                                {
        //                                    break;
        //                                }
        //                        }
        //                        tmparray = null;
        //                        i -= 1;
        //                        break;
        //                    }

        //                case "Struct":
        //                    {
        //                        // parse struct line
        //                        ConvertFDAStructLineToContDists(line, parameter);
        //                        break;
        //                    }

        //                default:
        //                    {
        //                        // could cause potential issues

        //                        while (!line[parameter] == "Stage" | line[parameter] == "Struct" | curvetype != line[parameter].Chars[0])
        //                        {
        //                            if (Information.IsNothing(tmparray))
        //                            {
        //                                tmparray = new String()[1];
        //                                curvetype = line[parameter].Chars[0];
        //                            }
        //                            else
        //                            {
        //                                var oldTmparray = tmparray;
        //                                tmparray = new String()[tmparray.Count() + 1];
        //                                if (oldTmparray != null)
        //                                    Array.Copy(oldTmparray, tmparray, Math.Min(tmparray.Count() + 1, oldTmparray.Length));
        //                            }
        //                            tmparray[tmparray.Count() - 1] = line;
        //                            i += 1;
        //                            line = Strings.Split(lines[i], Constants.vbTab);
        //                        }

        //                        break;
        //                    }
        //            }
        //        }
        //    }



        //    public IOccupancyType Clone()
        //    {
        //        throw new NotImplementedException();
        //    }
    }
}
