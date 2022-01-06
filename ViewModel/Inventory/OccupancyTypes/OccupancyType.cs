using FdaLogging;
using paireddata;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HEC.FDA.ViewModel.Inventory.DamageCategory;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    internal class OccupancyType : BaseViewModel,  IOccupancyType
    {

        private string _Name;
        private string _Description;
        //todo: what about damage category changing name?
        private bool _CalculateStructureDamage;
        private bool _CalculateContentDamage;
        private bool _CalculateVehicleDamage;
        private bool _CalculateOtherDamage;
        private bool _IsModified;

        /// <summary>
        /// This is used by the occtype editor to determine if this occtype
        /// was edited. This value should be set to false every time the editor
        /// is opened.
        /// </summary>
        public bool IsModified 
        {
            get{ return _IsModified;}
            set{_IsModified = value; NotifyPropertyChanged();}
        }

        public string Name
        {
            get  { return _Name;}
            set { _Name = value; IsModified = true; NotifyPropertyChanged(); }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; IsModified = true; }
        }

        public string DamageCategory { get; set; }

        public bool CalculateStructureDamage
        {
            get { return _CalculateStructureDamage; }
            set { _CalculateStructureDamage = value; IsModified = true; }
        }

        public bool CalculateContentDamage
        {
            get { return _CalculateContentDamage; }
            set { _CalculateContentDamage = value; IsModified = true; }
        }

        public bool CalculateVehicleDamage
        {
            get { return _CalculateVehicleDamage; }
            set { _CalculateVehicleDamage = value; IsModified = true; }
        }

        public bool CalculateOtherDamage
        {
            get { return _CalculateOtherDamage; }
            set { _CalculateOtherDamage = value; IsModified = true; }
        }

        public UncertainPairedData StructureDepthDamageFunction { get; set; }
        public UncertainPairedData ContentDepthDamageFunction { get; set; }
        public UncertainPairedData VehicleDepthDamageFunction { get; set; }
        public UncertainPairedData OtherDepthDamageFunction { get; set; }
        public IDistribution StructureValueUncertainty { get; set; }
        public IDistribution ContentValueUncertainty { get; set; }
        public IDistribution VehicleValueUncertainty { get; set; }
        public IDistribution OtherValueUncertainty { get; set; }
        public IDistribution FoundationHeightUncertainty { get; set; }

        public IOrdinate ContentToStructureValueUncertainty { get; set; }
        public double ContentToStructureValue { get; set; }
        public IOrdinate OtherToStructureValueUncertainty { get; set; }
        public double OtherToStructureValue { get; set; }

        //These booleans determine if the content/vehicle/other curves are a ratio of structure value or not
        public bool IsContentRatio { get; set; }
        public bool IsVehicleRatio { get; set; }
        public bool IsOtherRatio { get; set; }

        public int GroupID { get; set; }
        public int ID { get; set; }

        public OccupancyType()
        {

        }
        public OccupancyType(string name, string damageCategoryName)
        {
            Name = name;
            DamageCategory = damageCategoryName;
        }


        #region messages section
        public LoggingLevel SaveStatusLevel => throw new NotImplementedException();

        public bool IsExpanded { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ObservableCollection<LogItem> MessageRows { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int MessageCount => throw new NotImplementedException();

        public List<LogItem> TempErrors { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /// <summary>
        /// Gets rid of any temperary messages from the list of messages and adds the new list of temp
        /// messages from "TempErrors" property.
        /// </summary>
        public void UpdateMessages(bool saving = false)
        {
            ////there are three places that messages come from.
            //// 1.) The sqlite database
            //// 2.) Temp messages from the validation of the "rules" (ie. Name cannot be blank)
            //// 3.) Temp messages from any object that implements IValidate. These messages come out of the model, stats, functions

            ////get rid of any temp logs
            //ObservableCollection<LogItem> tempList = new ObservableCollection<LogItem>();
            //foreach (LogItem li in MessageRows)
            //{
            //    //exclude any temp logs
            //    if (!li.IsTempLog())
            //    {
            //        tempList.Add(li);
            //    }

            //}
            
            ////get IMessages from the coord func editor
            ////and convert them into temp log messages
            //List<LogItem> funcLogs = GetTempLogsFromCoordinatesFunctionEditor();
            ////add them to the temp errors so that they will be included
            //TempErrors.AddRange(funcLogs);

            ////i want all of these messages to be put on the top of the list, but i want to respect their order. This 
            ////means i need to insert at 0 and start with the last in the list
            //for (int i = TempErrors.Count - 1; i >= 0; i--)
            //{
            //    tempList.Insert(0, TempErrors[i]);
            //}
            //MessageRows = tempList;
            //TempErrors.Clear();
            ////if we are saving then we want the save status to be visible
            //if (saving)
            //{
            //    UpdateSaveStatusLevel();
            //}
            //else
            //{
            //    SaveStatusLevel = LoggingLevel.Debug;
            //}
        }

        public void FilterRowsByLevel(LoggingLevel level)
        {
            throw new NotImplementedException();
        }

        public void DisplayAllMessages()
        {
            throw new NotImplementedException();
        }

        #endregion

        //public OccupancyType(XElement Xel)
        //{
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
        //}

        //public IOccupancyType Clone()
        //{
        //    //todo: you need to actually craete a "new" for each sub element here.
        //    OccupancyType ot = new OccupancyType();
        //    ot.Name = Name;
        //    ot.Description = Description;
        //    ot.DamageCategory = DamageCategory;

        //    ot.CalculateStructureDamage = CalculateStructureDamage;
        //    ot.CalculateContentDamage = CalculateContentDamage;
        //    ot.CalculateVehicleDamage = CalculateVehicleDamage;
        //    ot.CalculateOtherDamage = CalculateOtherDamage;

        //    ot.StructureDepthDamageFunction = StructureDepthDamageFunction;
        //    ot.ContentDepthDamageFunction = ContentDepthDamageFunction;
        //    ot.VehicleDepthDamageFunction = VehicleDepthDamageFunction;
        //    ot.OtherDepthDamageFunction = OtherDepthDamageFunction;

        //    ot.StructureValueUncertainty = StructureValueUncertainty;
        //    ot.ContentValueUncertainty = ContentValueUncertainty;
        //    ot.VehicleValueUncertainty = VehicleValueUncertainty;
        //    ot.OtherValueUncertainty = OtherValueUncertainty;
        //    ot.FoundationHeightUncertainty = FoundationHeightUncertainty;

        //    ot.StructureUncertaintyType = StructureUncertaintyType;
        //    ot.ContentUncertaintyType = ContentUncertaintyType;
        //    ot.VehicleUncertaintyType = VehicleUncertaintyType;
        //    ot.OtherUncertaintyType = OtherUncertaintyType;



        //    ot.GroupID = GroupID;
        //    ot.ID = ID;

        //    return ot;
        //}

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
