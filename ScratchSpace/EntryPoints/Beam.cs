using Geospatial.Features;
using Geospatial.GDALAssist;
using Geospatial.GDALAssist.Vectors;
using Geospatial.IO;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.Spatial;
using RasMapperLib;
using Statistics;
using Statistics.Histograms;
using ScottPlot;
using HEC.FDA.Model.utilities;

namespace ScratchSpace.EntryPoints;
public static class Beam
{
    public static void EntryPoint()
    {
        WeirdPerfResults();
        //GapAnalysis.EntryPoint();
        //InvestigateGap.EntryPoint();
        //WeirdPerfResults();
        //string tiff = @"C:\Temp\_FDA\MySAC38_FragTest\MySAC38_FragTest\Hydraulic Data\Grids\80\WSE80Pct.tif";
        //string structureInv = @"C:\Temp\_FDA\MySAC38_FragTest\MySAC38_FragTest\Structure Inventories\Structs\Structure.shp";
        //string impactArea = @"C:\Temp\_FDA\MySAC38_FragTest\MySAC38_FragTest\Impact Areas\IA\Sac38_Zone_10N.shp";
        //Projection tiffProj = RASHelper.GetProjectionFromTerrain(tiff);
        //Projection siProj = RASHelper.GetVectorProjection(structureInv);
        //Projection iaProj = RASHelper.GetVectorProjection(impactArea);
        //bool same = tiffProj.Equals(iaProj);
        //PolygonFeatureLayer ias = new("",impactArea);
        //PolygonFeatureCollection pgsOut = new();
        //ShapefileWriter.TryReadShapefile(impactArea,out pgsOut);
        //var gon = pgsOut.Features[0];
        //var reprogon = gon.Reproject(iaProj, tiffProj);
        //Console.WriteLine(gon);
        //Console.WriteLine(reprogon);
        //List<Polygon> polygons = ias.Polygons().ToList();
        ////This reprojection is incorrect and failing. Move shapefile reader. 
        //List<Polygon> reproPolys = RASHelper.ReprojectPolygons(tiffProj, polygons, iaProj);
        //Console.WriteLine("garbage");

    }

    private static void WeirdPerfResults()
    {
        double[] exprobs = [0.5, 0.2, 0.1, 0.05, 0.02, 0.01, 0.005, 0.002];
        double[] stages = [13.388120651245117, 15.484384536743164, 16.70232582092285, 17.97307586669922, 18.84888458251953, 19.523529052734375, 20.172605514526367, 20.97872543334961];
        double interestThreshold = 18.81;
        double equivRecordLength = 48;

        UncertainPairedData upd = GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(exprobs, stages, true, 48);
        GraphicalUncertainPairedData gupd = new(exceedanceProbabilities: exprobs, flowOrStageValues: stages, equivalentRecordLength: 48, curveMetaData: new(), usingStagesNotFlows: true);
        Random seed = new(1234);
        DynamicHistogram histo = new(0.0002, new());
        DynamicHistogram althisto = new(0.0002, new());

        // Store some sampled paired data for visualization
        List<PairedData> sampledCurves = new();
        int numCurvesToPlot = 100; // Plot first 10 curves for visualization

        for (int i = 0; i < 500000; i++)
        {
            double rando = seed.NextDouble();
            PairedData pd = gupd.SamplePairedData(rando);
            PairedData altpd = upd.SamplePairedDataRaw(rando);
            histo.AddObservationToHistogram(1 - pd.f_inverse(interestThreshold));
            althisto.AddObservationToHistogram(1 - altpd.f_inverse(interestThreshold));

            // Store some curves for plotting
            if (i < numCurvesToPlot)
            {
                sampledCurves.Add(altpd);
            }
        }

        // ===== ASCII VISUALIZATION - Quick Debug in Console =====
        Console.WriteLine("\n===== QUICK DEBUG: ASCII Histogram =====");
        histo.PrintToConsole("Performance Results - Exceedance Probability", maxWidth: 100, maxBins: 30);
        althisto.PrintToConsole("ALT Performance Results - Exceedance Probability", maxWidth: 100, maxBins: 30);

        // ===== SCOTTPLOT VISUALIZATION - High Quality Chart =====
        Console.WriteLine("\n===== Creating ScottPlot Chart =====");
        Plot plt = new();

        // Prepare data for plotting as individual bars
        long[] binCounts = histo.BinCounts;
        List<ScottPlot.Bar> bars = [];

        for (int i = 0; i < binCounts.Length; i++)
        {
            double binCenter = histo.Min + (i + 0.5) * histo.BinWidth;
            double count = binCounts[i];

            // Create a bar at the bin center with the bin width
            var bar = new ScottPlot.Bar
            {
                Position = binCenter,
                Value = count,
                Size = histo.BinWidth * 0.95, // 95% of bin width to show separation
                FillColor = ScottPlot.Colors.Blue.WithAlpha(0.7)
            };
            bars.Add(bar);
        }

        // Add all bars to the plot
        plt.Add.Bars(bars);

        // Customize plot appearance
        plt.Title("Exceedance Probability Distribution");
        plt.XLabel("Exceedance Probability");
        plt.YLabel("Frequency");
        plt.Axes.SetLimitsX(histo.Min, histo.Max);

        // Add vertical line at threshold
        var thresholdLine = plt.Add.VerticalLine(interestThreshold);
        thresholdLine.Color = ScottPlot.Colors.Red;
        thresholdLine.LineWidth = 2;
        thresholdLine.LinePattern = LinePattern.Dashed;

        // Add statistics annotation
        string statsText = $"Sample Size: {histo.SampleSize:N0}\n" +
                          $"Mean: {histo.SampleMean:F4}\n" +
                          $"Std Dev: {histo.StandardDeviation:F4}\n" +
                          $"Min: {histo.Min:F4}\n" +
                          $"Max: {histo.Max:F4}\n" +
                          $"Threshold: {interestThreshold:F2}";
        plt.Add.Annotation(statsText, Alignment.UpperRight);

        // Save the histogram plot
        string outputPath = Path.Combine(Path.GetTempPath(), "FDA_Performance_Results.png");
        plt.SavePng(outputPath, 1400, 900);
        Console.WriteLine($"ScottPlot histogram saved to: {outputPath}");

        // ===== SCOTTPLOT PAIRED DATA LINE SERIES =====
        Console.WriteLine("\n===== Creating ScottPlot Paired Data Line Series =====");
        Plot pltCurves = new();

        // Plot each sampled paired data as a line series
        Console.WriteLine($"Plotting {sampledCurves.Count} sampled curves...");

        for (int i = 0; i < sampledCurves.Count; i++)
        {
            PairedData pd = sampledCurves[i];

            // Convert arrays to double[] for ScottPlot
            double[] xVals = [.. pd.Xvals.Select((x) => 1 - x)];
            double[] yVals = pd.Yvals;

            // Add line plot with semi-transparent color
            var linePlot = pltCurves.Add.Scatter(xVals, yVals);
            linePlot.LineWidth = 1;
            linePlot.MarkerSize = 0; // No markers, just lines
            linePlot.Color = ScottPlot.Colors.Blue.WithAlpha(0.15); // Very transparent for overlays
        }

        // Add the mean/original curve in a different color for reference
        var meanCurve = pltCurves.Add.Scatter(exprobs, stages);
        meanCurve.LineWidth = 3;
        meanCurve.Color = ScottPlot.Colors.Red;
        meanCurve.LegendText = "Mean Curve";
        meanCurve.MarkerSize = 6;

        // Add horizontal line at threshold
        var thresholdLineHoriz = pltCurves.Add.HorizontalLine(interestThreshold);
        thresholdLineHoriz.Color = ScottPlot.Colors.Green;
        thresholdLineHoriz.LineWidth = 2;
        thresholdLineHoriz.LinePattern = LinePattern.Dashed;

        // Customize appearance
        pltCurves.Title("Stage-Frequency Curves (Monte Carlo Samples)");
        pltCurves.XLabel("Exceedance Probability");
        pltCurves.YLabel("Stage (ft)");
        pltCurves.ShowLegend(Alignment.UpperRight);

        // Add annotation with sample info
        string curveStatsText = $"Samples Plotted: {sampledCurves.Count}\n" +
                               $"Total Samples: 500,000\n" +
                               $"Threshold: {interestThreshold:F2} ft";
        pltCurves.Add.Annotation(curveStatsText, Alignment.LowerLeft);

        // Save the curves plot
        string curvesOutputPath = Path.Combine(Path.GetTempPath(), "FDA_Stage_Frequency_Curves.png");
        pltCurves.SavePng(curvesOutputPath, 1400, 900);
        Console.WriteLine($"ScottPlot stage-frequency curves saved to: {curvesOutputPath}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
    private static void WeirdPerfResults4()
    {
        double[] exprobs = [0.5, 0.2, 0.1, 0.05, 0.02, 0.01, 0.005, 0.002];
        double[] stages = [13.388120651245117, 15.484384536743164, 16.70232582092285, 17.97307586669922, 18.84888458251953, 19.523529052734375, 20.172605514526367, 20.97872543334961];
        double interestThreshold = 18.81;

        GraphicalUncertainPairedData gupd = new(exceedanceProbabilities: exprobs, flowOrStageValues: stages, equivalentRecordLength: 48, curveMetaData: new(), usingStagesNotFlows: true);
        Random seed = new(1234);
        DynamicHistogram histo = new(0.0002, new());

        // Store some sampled paired data for visualization
        List<PairedData> sampledCurves = new();
        int numCurvesToPlot = 10; // Plot first 10 curves for visualization

        for (int i = 0; i < 500000; i++)
        {
            double rando = seed.NextDouble();
            PairedData pd = gupd.SamplePairedData(rando);
            histo.AddObservationToHistogram(1-pd.f_inverse(interestThreshold));

            // Store some curves for plotting
            if (i < numCurvesToPlot)
            {
                sampledCurves.Add(pd);
            }
        }

        // ===== ASCII VISUALIZATION - Quick Debug in Console =====
        Console.WriteLine("\n===== QUICK DEBUG: ASCII Histogram =====");
        histo.PrintToConsole("Performance Results - Exceedance Probability", maxWidth: 100, maxBins: 30);

        // ===== SCOTTPLOT VISUALIZATION - High Quality Chart =====
        Console.WriteLine("\n===== Creating ScottPlot Chart =====");
        Plot plt = new();

        // Prepare data for plotting as individual bars
        long[] binCounts = histo.BinCounts;
        List<ScottPlot.Bar> bars = [];

        for (int i = 0; i < binCounts.Length; i++)
        {
            double binCenter = histo.Min + (i + 0.5) * histo.BinWidth;
            double count = binCounts[i];

            // Create a bar at the bin center with the bin width
            var bar = new ScottPlot.Bar
            {
                Position = binCenter,
                Value = count,
                Size = histo.BinWidth * 0.95, // 95% of bin width to show separation
                FillColor = ScottPlot.Colors.Blue.WithAlpha(0.7)
            };
            bars.Add(bar);
        }

        // Add all bars to the plot
        plt.Add.Bars(bars);

        // Customize plot appearance
        plt.Title("Exceedance Probability Distribution");
        plt.XLabel("Exceedance Probability");
        plt.YLabel("Frequency");
        plt.Axes.SetLimitsX(histo.Min, histo.Max);

        // Add vertical line at threshold
        var thresholdLine = plt.Add.VerticalLine(interestThreshold);
        thresholdLine.Color = ScottPlot.Colors.Red;
        thresholdLine.LineWidth = 2;
        thresholdLine.LinePattern = LinePattern.Dashed;

        // Add statistics annotation
        string statsText = $"Sample Size: {histo.SampleSize:N0}\n" +
                          $"Mean: {histo.SampleMean:F4}\n" +
                          $"Std Dev: {histo.StandardDeviation:F4}\n" +
                          $"Min: {histo.Min:F4}\n" +
                          $"Max: {histo.Max:F4}\n" +
                          $"Threshold: {interestThreshold:F2}";
        plt.Add.Annotation(statsText, Alignment.UpperRight);

        // Save the histogram plot
        string outputPath = Path.Combine(Path.GetTempPath(), "FDA_Performance_Results.png");
        plt.SavePng(outputPath, 1400, 900);
        Console.WriteLine($"ScottPlot histogram saved to: {outputPath}");

        // ===== SCOTTPLOT PAIRED DATA LINE SERIES =====
        Console.WriteLine("\n===== Creating ScottPlot Paired Data Line Series =====");
        Plot pltCurves = new();

        // Plot each sampled paired data as a line series
        Console.WriteLine($"Plotting {sampledCurves.Count} sampled curves...");

        for (int i = 0; i < sampledCurves.Count; i++)
        {
            PairedData pd = sampledCurves[i];

            // Convert arrays to double[] for ScottPlot
            double[] xVals = [..pd.Xvals.Select((x) => 1-x)];
            double[] yVals = pd.Yvals;

            // Add line plot with semi-transparent color
            var linePlot = pltCurves.Add.Scatter(xVals, yVals);
            linePlot.LineWidth = 1;
            linePlot.MarkerSize = 0; // No markers, just lines
            linePlot.Color = ScottPlot.Colors.Blue.WithAlpha(0.15); // Very transparent for overlays
        }

        // Add the mean/original curve in a different color for reference
        var meanCurve = pltCurves.Add.Scatter(exprobs, stages);
        meanCurve.LineWidth = 3;
        meanCurve.Color = ScottPlot.Colors.Red;
        meanCurve.LegendText = "Mean Curve";
        meanCurve.MarkerSize = 6;

        // Add horizontal line at threshold
        var thresholdLineHoriz = pltCurves.Add.HorizontalLine(interestThreshold);
        thresholdLineHoriz.Color = ScottPlot.Colors.Green;
        thresholdLineHoriz.LineWidth = 2;
        thresholdLineHoriz.LinePattern = LinePattern.Dashed;

        // Customize appearance
        pltCurves.Title("Stage-Frequency Curves (Monte Carlo Samples)");
        pltCurves.XLabel("Exceedance Probability");
        pltCurves.YLabel("Stage (ft)");
        pltCurves.ShowLegend(Alignment.UpperRight);

        // Add annotation with sample info
        string curveStatsText = $"Samples Plotted: {sampledCurves.Count}\n" +
                               $"Total Samples: 500,000\n" +
                               $"Threshold: {interestThreshold:F2} ft";
        pltCurves.Add.Annotation(curveStatsText, Alignment.LowerLeft);

        // Save the curves plot
        string curvesOutputPath = Path.Combine(Path.GetTempPath(), "FDA_Stage_Frequency_Curves.png");
        pltCurves.SavePng(curvesOutputPath, 1400, 900);
        Console.WriteLine($"ScottPlot stage-frequency curves saved to: {curvesOutputPath}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static void WeirdPerfResults2()
    {
        double[] exprobs = [0.5, 0.2, 0.1, 0.05, 0.02, 0.01, 0.005, 0.002];
        double[] stages = [13.388120651245117, 15.484384536743164, 16.70232582092285, 17.97307586669922, 18.84888458251953, 19.523529052734375, 20.172605514526367, 20.97872543334961];
        double interestThreshold = 18.81;

        // Create a single paired data instance
        GraphicalUncertainPairedData gupd = new(exceedanceProbabilities: exprobs, flowOrStageValues: stages, equivalentRecordLength: 48, curveMetaData: new(), usingStagesNotFlows: true);

        // Sample a single curve
        PairedData pd = gupd.SamplePairedData(.85);

        // ===== SCOTTPLOT VISUALIZATION - Single Paired Data =====
        Console.WriteLine("\n===== Creating ScottPlot for Single Paired Data =====");
        Plot plt = new();

        // Transform X values (1 - x) to match the analysis
        double[] xVals = pd.Xvals.Select(x => 1 - x).ToArray();
        double[] yVals = pd.Yvals;

        // Plot the single curve
        var curve = plt.Add.Scatter(xVals, yVals);
        curve.LineWidth = 2;
        curve.Color = ScottPlot.Colors.Blue;
        curve.LegendText = "Sampled Stage-Frequency Curve";
        curve.MarkerSize = 5;

        // Add horizontal line at threshold
        var thresholdLine = plt.Add.HorizontalLine(interestThreshold);
        thresholdLine.Color = ScottPlot.Colors.Red;
        thresholdLine.LineWidth = 2;
        thresholdLine.LinePattern = LinePattern.Dashed;

        // Find the exceedance probability at the threshold
        double exceedanceAtThreshold = pd.f_inverse(interestThreshold);
        double nonExceedanceAtThreshold = 1 - exceedanceAtThreshold;

        // Add vertical line at the intersection
        var verticalLine = plt.Add.VerticalLine(nonExceedanceAtThreshold);
        verticalLine.Color = ScottPlot.Colors.Green;
        verticalLine.LineWidth = 2;
        verticalLine.LinePattern = LinePattern.Dotted;

        // Add intersection point
        var intersectionPoint = plt.Add.Marker(nonExceedanceAtThreshold, interestThreshold);
        intersectionPoint.Size = 15;
        intersectionPoint.Color = ScottPlot.Colors.Red;

        // Customize plot appearance
        plt.Title("Stage-Frequency Curve with Threshold");
        plt.XLabel("Non-Exceedance Probability");
        plt.YLabel("Stage (ft)");
        plt.ShowLegend(Alignment.UpperLeft);

        // Add annotation with key values
        string statsText = $"Threshold: {interestThreshold:F2} ft\n" +
                          $"Non-Exceedance Prob: {nonExceedanceAtThreshold:F4}\n" +
                          $"Exceedance Prob: {exceedanceAtThreshold:F4}";
        plt.Add.Annotation(statsText, Alignment.LowerRight);

        // Save the plot
        string outputPath = Path.Combine(Path.GetTempPath(), "FDA_Single_Stage_Frequency.png");
        plt.SavePng(outputPath, 1200, 800);
        Console.WriteLine($"ScottPlot saved to: {outputPath}");

        // Open the plot automatically
        Console.WriteLine("Opening plot...");
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = outputPath,
            UseShellExecute = true
        });

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static void WeirdPerfResults3()
    {
        double[] exprobs = [0.5, 0.2, 0.1, 0.05, 0.02, 0.01, 0.005, 0.002];
        double[] stages = [13.388120651245117, 15.484384536743164, 16.70232582092285, 17.97307586669922, 18.84888458251953, 19.523529052734375, 20.172605514526367, 20.97872543334961];
        double interestThreshold = 18.81;

        Console.WriteLine("===== USING GraphicalFrequencyUncertaintyCalculators =====\n");

        // Use the new static calculator to create uncertain paired data
        var uncertainPairedData = HEC.FDA.Model.utilities.GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
            exceedanceProbabilities: exprobs,
            stagesOrFlows: stages,
            usingStagesNotFlows: true,
            equivalentRecordLength: 48,
            curveMetaData: new CurveMetaData("Stage", "Test Curve", "Test Category"),
            frequentEventThreshold: 0.99,  // Using the updated default from the file
            rareEventThreshold: 0.01       // Using the updated default from the file
        );

        Console.WriteLine("Uncertain Paired Data Created Successfully!");
        Console.WriteLine($"Number of coordinates: {uncertainPairedData.Xvals.Length}");
        Console.WriteLine($"Using stages (not flows): true");
        Console.WriteLine($"Distribution type: {uncertainPairedData.Yvals[0].GetType().Name}\n");

        // Print the coordinates
        Console.WriteLine("===== UNCERTAIN PAIRED DATA COORDINATES =====");
        Console.WriteLine("Index | Exceedance Prob | Distribution Mean | Distribution Std Dev");
        Console.WriteLine("------|-----------------|-------------------|--------------------");

        for (int i = 0; i < uncertainPairedData.Xvals.Length; i++)
        {
            double exceedanceProb = uncertainPairedData.Xvals[i];
            var distribution = uncertainPairedData.Yvals[i];

            // Get mean and standard deviation from the distribution
            double mean = distribution.InverseCDF(0.5);  // Median as proxy for mean
            double stdDev = (distribution.InverseCDF(0.84134) - distribution.InverseCDF(0.15866)) / 2.0; // ~1 std dev

            Console.WriteLine($"  {i,3} | {exceedanceProb,15:F6} | {mean,17:F6} | {stdDev,18:F6}");
        }

        Console.WriteLine("\n===== NOTE ON STANDARD DEVIATIONS =====");
        Console.WriteLine("All standard deviations are the same because:");
        Console.WriteLine("1. The Less Simple Method holds standard errors constant at the tails");
        Console.WriteLine("2. With thresholds at 0.99 (frequent) and 0.01 (rare), and data from 0.5 to 0.002,");
        Console.WriteLine("   all points fall within the 'hold constant' range");
        Console.WriteLine("3. This is the expected behavior of the Less Simple Method for this data\n");

        Console.WriteLine("===== SAMPLING FROM UNCERTAIN PAIRED DATA =====");
        Console.WriteLine("Demonstrating sampling at different quantiles:\n");

        double[] testQuantiles = [0.05, 0.25, 0.5, 0.75, 0.95];
        Console.WriteLine("Quantile | Sampled Curve Values at Input Exceedance Probabilities");
        Console.WriteLine("---------|----------------------------------------------------------");
        Console.WriteLine("         |  EP=0.5    EP=0.2    EP=0.1    EP=0.05   EP=0.02   EP=0.01");
        Console.WriteLine("---------|----------------------------------------------------------");

        foreach (double quantile in testQuantiles)
        {
            var sampledCurve = uncertainPairedData.SamplePairedData(quantile);
            Console.Write($"  {quantile,5:F2}  |");

            // Sample at the INPUT exceedance probabilities (not arbitrary ones)
            foreach (double ep in exprobs.Take(6))  // Take first 6 for display
            {
                double stage = sampledCurve.f(1 - ep);  // Convert exceedance to non-exceedance
                Console.Write($"  {stage,7:F3}");
            }
            Console.WriteLine();
        }

        Console.WriteLine("\n===== COMPARING WITH GraphicalUncertainPairedData =====");
        Console.WriteLine("Original implementation for reference:\n");

        GraphicalUncertainPairedData originalImplementation = new(
            exceedanceProbabilities: exprobs,
            flowOrStageValues: stages,
            equivalentRecordLength: 48,
            curveMetaData: new(),
            usingStagesNotFlows: true
        );

        Console.WriteLine("Sampling at 0.5 quantile from both implementations:");
        Console.WriteLine("Exceedance Prob | New Calculator | Original Implementation");
        Console.WriteLine("----------------|----------------|------------------------");

        var newSample = uncertainPairedData.SamplePairedData(0.5);
        var originalSample = originalImplementation.SamplePairedData(0.5);

        foreach (double ep in exprobs)
        {
            double newValue = newSample.f(1 - ep);
            double originalValue = originalSample.f(1 - ep);
            Console.WriteLine($"     {ep,6:F3}    |    {newValue,9:F3}   |       {originalValue,9:F3}");
        }

        Console.WriteLine("\n===== ANALYSIS AT THRESHOLD =====");
        Console.WriteLine($"Threshold: {interestThreshold:F2} ft\n");

        Console.WriteLine("Quantile | Exceedance Prob at Threshold");
        Console.WriteLine("---------|-----------------------------");

        for (double q = 0.1; q <= 0.9; q += 0.1)
        {
            var sample = uncertainPairedData.SamplePairedData(q);
            double exceedanceAtThreshold = 1 - sample.f_inverse(interestThreshold);
            Console.WriteLine($"  {q,5:F1}  |         {exceedanceAtThreshold,10:F6}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static void TestIntegrate()
    {
        double[] xs = [.000001, 0.25, 0.5, 0.75, .999999];
        double[] ys = [0, 6000, 8600, 136000, 500000];
        PairedData pairedData = new(xs, ys);
        double integral = pairedData.integrate();
        Console.WriteLine(integral);
        Console.Read();
    }

    private static void Whatever()
    {
        string tiffile = @"C:\Temp\FDA Bugs\NewLondonOrleans\NewLondonOrleans\Hydraulic Data\FWOP\FWOP 0.002 AEP\WSE (Max).Northern_Gulf_of_Mexico_Topobathy_DEM_14.tif";
        string shapefile = @"C:\Temp\FDA Bugs\NewLondonOrleans\NewLondonOrleans\Structure Inventories\SELA Structures\SELA_Struc_Final.shp";

        PointMs pts = RASHelper.GetPointMsFromShapefile(shapefile);
        float[] wses = RASHelper.SamplePointsOnTiff(pts, tiffile);

        int i = 8968;

        Console.WriteLine($"WSEs: {wses.Length}");
        Console.WriteLine($"Structure {i + 1}| WSE: {wses[i]} X: {pts[i].X} Y: {pts[i].Y}");
        Console.Read();
    }
}
