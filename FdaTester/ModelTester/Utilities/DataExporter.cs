

namespace FdaTester.ModelTester.Utilities
{
    public static class DataExporter
    {
        #region Voids
        public static void ExportDelimitedColumns(string fullFilePath, object[][] exportData, string[] exportColumnNames, char delimiter = '\t')
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(new System.IO.FileStream(fullFilePath, System.IO.FileMode.Create)))
            {
                //1. Find longest sublist
                int n = 0;
                for (int k = 0; k < exportData.Length; k++)
                {
                    if (exportData[k].Length > n)
                    {
                        n = exportData[k].Length;
                    }
                }
                //2. Column Names
                for (int s = 0; s < exportColumnNames.Length; s++)
                {
                    writer.Write(exportColumnNames[s]);
                    writer.Write(delimiter);
                }
                writer.WriteLine();

                //3. Loop across sublist elements
                for (int j = 0; j < n; j++)
                {
                    //4. Loop across list number
                    for (int i = 0; i < exportData.Length; i++)
                    {
                        if (j < exportData[i].Length)
                        {
                            writer.Write(exportData[i][j]);
                            writer.Write(delimiter);
                        }
                        else
                        {
                            writer.Write(delimiter);
                        }
                    }
                    writer.WriteLine();
                }
            }
        }
        public static void ExportData(string fullFilePath, object[][] exportData, string[] exportColumnNames, char delimiter = '\t')
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(new System.IO.FileStream(fullFilePath, System.IO.FileMode.Create));

            for (int j = 0; j < exportData.Length; j++)
            {
                for (int i = 0; i < exportData[j].Length; i++)
                {
                    writer.Write(exportData[j][i]);

                }
                writer.WriteLine();

            }
        }

        public static void ExportDelimitedColumns(string fullFilePath, string[] parameterList, float[] probabilities, float[] cSharpFlows, float[] fortranFlows, double[] diff, double[] percDiff, char delimiter = '\t')
        {

            System.IO.StreamWriter writer = new System.IO.StreamWriter(new System.IO.FileStream(fullFilePath, System.IO.FileMode.Append));


            for (int i = 0; i < parameterList.Length; i++)
            {
                writer.Write(parameterList[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();

            writer.Write("Probabilities: ");
            writer.Write(delimiter);

            for (int i = 0; i < probabilities.Length; i++)
            {
                writer.Write(probabilities[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();

            writer.Write("CSharp Flows: ");
            writer.Write(delimiter);
            for (int i = 0; i < cSharpFlows.Length; i++)
            {
                writer.Write(cSharpFlows[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();

            writer.Write("Fortran Flows: ");
            writer.Write(delimiter);
            for (int i = 0; i < fortranFlows.Length; i++)
            {
                writer.Write(fortranFlows[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();

            writer.Write("Abs Diff: ");
            writer.Write(delimiter);
            for (int i = 0; i < diff.Length; i++)
            {
                writer.Write(diff[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();

            bool testPassed = true;

            writer.Write("% Diff: ");
            writer.Write(delimiter);
            for (int i = 0; i < percDiff.Length; i++)
            {
                if (System.Math.Abs(percDiff[i]) > .1) { testPassed = false; }
                writer.Write(percDiff[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();
            writer.WriteLine();

            writer.Write("Test Passed? " + testPassed);
            writer.WriteLine();
            writer.WriteLine();



            writer.Close();

        }


        public static void ExportFlowData(string fullFilePath, string[] parameterList, double[] probabilities, double[] cSharpFlows, double[] fortranFlows,  char delimiter = '\t')
        {

            System.IO.StreamWriter writer = new System.IO.StreamWriter(new System.IO.FileStream(fullFilePath, System.IO.FileMode.Append));


            for (int i = 0; i < parameterList.Length; i++)
            {
                writer.Write(parameterList[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();

            writer.Write("Probabilities: ");
            writer.Write(delimiter);

            for (int i = 0; i < probabilities.Length; i++)
            {
                writer.Write(probabilities[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();

            writer.Write("Flows 1: ");
            writer.Write(delimiter);
            for (int i = 0; i < cSharpFlows.Length; i++)
            {
                writer.Write(cSharpFlows[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();

            writer.Write("Flows 2: ");
            writer.Write(delimiter);
            for (int i = 0; i < fortranFlows.Length; i++)
            {
                writer.Write(fortranFlows[i]);
                writer.Write(delimiter);

            }
            writer.WriteLine();

           
            writer.WriteLine();
            writer.WriteLine();



            writer.Close();

        }

        //public static void ExportDelimitedColumns(string fullFilePath, System.Collections.Generic.List<FdaTester.ModelTester.UnitTests.LogPearsonIIIFunctionTester> listOfLogIII, char delimiter = '\t')

        //{
        //    System.IO.StreamWriter writer = new System.IO.StreamWriter(new System.IO.FileStream(fullFilePath, System.IO.FileMode.Append));

        //    foreach (FdaTester.ModelTester.UnitTests.LogPearsonIIIFunctionTester log3 in listOfLogIII)
        //    {

        //        writer.Write("Mean: " + log3.parameters[0]);
        //        writer.Write(delimiter);
        //        writer.Write("St Dev: " + log3.parameters[1]);
        //        writer.Write(delimiter);
        //        writer.Write("Skew: " + log3.parameters[2]);
        //        writer.Write(delimiter);
        //        writer.Write("POR: " + log3.parameters[3]);
        //        writer.Write(delimiter);

        //        writer.WriteLine();

        //        writer.Write("Probabilities: ");
        //        writer.Write(delimiter);

        //        for (int i = 0; i < log3.probabilities.Length; i++)
        //        {
        //            writer.Write(log3.probabilities[i]);
        //            writer.Write(delimiter);

        //        }
        //        writer.WriteLine();

        //        writer.Write("CSharp Flows: ");
        //        writer.Write(delimiter);
        //        for (int i = 0; i < log3.cSharpFlowValues.Length; i++)
        //        {
        //            writer.Write(log3.cSharpFlowValues[i]);
        //            writer.Write(delimiter);

        //        }
        //        writer.WriteLine();

        //        writer.Write("Fortran Flows: ");
        //        writer.Write(delimiter);
        //        for (int i = 0; i < log3.fortranFlowValues.Length; i++)
        //        {
        //            writer.Write(log3.fortranFlowValues[i]);
        //            writer.Write(delimiter);

        //        }
        //        writer.WriteLine();

        //        writer.Write("Abs Diff: ");
        //        writer.Write(delimiter);
        //        for (int i = 0; i < log3.absoluteDiffValues.Length; i++)
        //        {
        //            writer.Write(log3.absoluteDiffValues[i]);
        //            writer.Write(delimiter);

        //        }
        //        writer.WriteLine();

        //        bool testPassed = true;

        //        writer.Write("% Diff: ");
        //        writer.Write(delimiter);
        //        for (int i = 0; i < log3.percentDiffValues.Length; i++)
        //        {
        //            if (System.Math.Abs(log3.percentDiffValues[i]) > .1) { testPassed = false; }
        //            writer.Write(log3.percentDiffValues[i]);
        //            writer.Write(delimiter);

        //        }
        //        writer.WriteLine();
        //        writer.WriteLine();

        //        writer.Write("Test Passed? " + testPassed);
        //        writer.WriteLine();
        //        writer.WriteLine();


        //    }
        //    writer.Close();
        //}

        //public static void writeTestResultsToXML(string fullFilePath, System.Collections.Generic.List<FdaTester.ModelTester.UnitTests.LogPearsonIIIFunctionTester> listOfLogIII)
        //{
        //    System.Xml.XmlWriterSettings mySettings = new System.Xml.XmlWriterSettings();
        //    mySettings.Indent = true;
        //    System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(fullFilePath, mySettings);
        //    writer.WriteStartDocument();
        //    writer.WriteStartElement("Test_Objects");
        //    //foreach (UnitTesters.LogPearsonIIIFunctionTester log3 in listOfLogIII)
        //    for (int k = 0; k < listOfLogIII.Count; k++)
        //    {
        //        writer.WriteStartElement("testObject" + (k + 1).ToString());
        //        writer.WriteAttributeString("Mean", listOfLogIII[k].parameters[0].ToString());
        //        writer.WriteAttributeString("St_Dev", listOfLogIII[k].parameters[1].ToString());
        //        writer.WriteAttributeString("Skew", listOfLogIII[k].parameters[2].ToString());
        //        writer.WriteAttributeString("POR", listOfLogIII[k].parameters[3].ToString());
        //        writer.WriteAttributeString("TestStatus", listOfLogIII[k].testStatus.ToString());


        //        writer.WriteStartElement("Probabilities");
        //        for (int i = 0; i < listOfLogIII[k].probabilities.Length; i++)
        //        {
        //            writer.WriteAttributeString("prob" + (i + 1).ToString(), listOfLogIII[k].probabilities[i].ToString());
        //        }
        //        writer.WriteEndElement();

        //        writer.WriteStartElement("CSharpFlows");
        //        for (int i = 0; i < listOfLogIII[k].cSharpFlowValues.Length; i++)
        //        {
        //            writer.WriteAttributeString("flow" + (i + 1).ToString(), listOfLogIII[k].cSharpFlowValues[i].ToString());
        //        }
        //        writer.WriteEndElement();

        //        writer.WriteStartElement("FortranFlows");
        //        for (int i = 0; i < listOfLogIII[k].fortranFlowValues.Length; i++)
        //        {
        //            writer.WriteAttributeString("flow" + (i + 1).ToString(), listOfLogIII[k].fortranFlowValues[i].ToString());
        //        }
        //        writer.WriteEndElement();


        //        writer.WriteStartElement("AbsDiff");
        //        for (int i = 0; i < listOfLogIII[k].absoluteDiffValues.Length; i++)
        //        {
        //            writer.WriteAttributeString("absDiff" + (i + 1).ToString(), listOfLogIII[k].absoluteDiffValues[i].ToString());
        //        }
        //        writer.WriteEndElement();


        //        writer.WriteStartElement("PercDiff");
        //        for (int i = 0; i < listOfLogIII[k].percentDiffValues.Length; i++)
        //        {
        //            writer.WriteAttributeString("percDiff" + (i + 1).ToString(), listOfLogIII[k].percentDiffValues[i].ToString());
        //        }
        //        writer.WriteEndElement();

        //        writer.WriteEndElement(); //end of object


        //    }
        //    writer.WriteEndElement(); //end of xml doc
        //    writer.WriteEndDocument();
        //    writer.Close();
        //}
        #endregion
    }
}
