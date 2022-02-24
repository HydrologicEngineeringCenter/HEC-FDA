using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Inventory.DamageCategory;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    internal class OccupancyTypeGroup : IOccupancyTypeGroup
    {
        public string Name { get; set; }
        public List<IOccupancyType> OccupancyTypes { get; set; }
        public List<IDamageCategory> DamageCategories { get; set; }
        public int ID { get; set; }

        public OccupancyTypeGroup(string selectedPath)
        {
            //this is a path to the occtype file. I need to read it and load the occtypes from it
            LoadFromFile(selectedPath);
        }

        public void LoadFromFile(string inputfile)
        {
            //if (OccupancyTypes == null)
            //{
            //    OccupancyTypes = new List<IOccupancyType>();
            //}
            //switch (System.IO.Path.GetExtension(inputfile).ToLower())
            //{
            //    case ".txt":
            //        {
            //            // fda text reader
            //            System.IO.FileStream fs = new System.IO.FileStream(inputfile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //            System.IO.StreamReader sr = new System.IO.StreamReader(fs);
            //            System.Text.StringBuilder occtypestring;
            //            string[] tabbedline;
            //            string tmpline = "";
            //            int startdata = 0;
            //            int parameter = 0;
            //            string separator = "\t";
            //            tabbedline = sr.ReadLine().Split(separator.ToCharArray());
            //            while (!tabbedline[0].Equals("Occ_Name"))
            //            {
            //                if (!sr.EndOfStream)
            //                    tabbedline = sr.ReadLine().Split(separator.ToCharArray());
            //                else
            //                    throw new Exception("This file did not contain the correct format for FDA occupancy types");
            //            }
            //            startdata = Array.IndexOf(tabbedline, "Start_Data");
            //            parameter = Array.IndexOf(tabbedline, "Parameter");
            //            while (!sr.EndOfStream)
            //            {
            //                occtypestring = new System.Text.StringBuilder();
            //                tmpline = sr.ReadLine();
            //                if (tmpline == "" || tmpline == Environment.NewLine || tmpline.Substring(0, 7) == "Struc_N" || tmpline.Substring(0, 8) == "Mod_Name")
            //                    break; // what if the last line is a return or a blank line...
            //                occtypestring.AppendLine(tmpline);
            //                tabbedline = tmpline.Split(separator.ToCharArray());
            //                try
            //                {
            //                    while (tabbedline.Count() - 1 < parameter || tabbedline[parameter] == "Struct")
            //                    {
            //                        if (sr.EndOfStream)
            //                            throw new Exception("Encountered an occupancy type without a 'Struct' line");
            //                        else
            //                        {
            //                            tmpline = sr.ReadLine();
            //                            occtypestring.AppendLine(tmpline);
            //                            tabbedline = tmpline.Split(separator.ToCharArray());
            //                        }
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    int i = 0;
            //                }
            //                if (tabbedline.Count() - 1 < parameter)
            //                {
            //                }
            //                else
            //                {
            //                    //I am using the occtype reader Occ Type which is really the Consequences Assist
            //                    //occupancy type. I just didn't want to re write all that code.
            //                    OccTypeReader.ComputableObjects.OccupancyType ot = new OccTypeReader.ComputableObjects.OccupancyType();

            //                    //ot.ReportMessage += OnReportMessage;
            //                    ot.LoadFromFDAInformation(occtypestring, startdata, parameter);
            //                    //translate the consequences assist occtype into our occtype
            //                    OccupancyTypes.Add(OccupancyTypeFactory.Factory(ot));
            //                }
            //            }
            //            sr.Close(); sr.Dispose();
            //            fs.Close(); fs.Dispose();
            //            break;
            //        }

            //    case ".xml":
            //        {
            //            // create a reader for xml.
            //            byte[] ba = System.IO.File.ReadAllBytes(inputfile);
            //            System.IO.MemoryStream ms = new System.IO.MemoryStream(ba);
            //            XDocument xdoc = new XDocument();
            //            xdoc = XDocument.Load(ms);
            //            ms.Dispose();
            //            XElement xel = xdoc.Root;
            //            foreach (XElement ele in xel.Elements())
            //            {
            //                OccupancyTypes.Add(new OccupancyType(ele));
            //            }

            //            break;
            //        }
            //}
        }


        public IOccupancyType GetOcctypeByNameAndDamCat(string name, string damCatName)
        {
            throw new NotImplementedException();
        }

        public int GetOccTypeIndex(string name)
        {
            throw new NotImplementedException();
        }

        public List<IOccupancyType> GetOccypesByName(string name)
        {
            throw new NotImplementedException();
        }

        public string WriteToXml(string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}
