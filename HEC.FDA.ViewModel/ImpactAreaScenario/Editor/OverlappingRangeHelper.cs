using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor;

public static class OverlappingRangeHelper
{
    private const double OVERLAP_RULE_PERCENT = .05;
    private const string INFLOW_OUTFLOW = "Inflow-Outflow";
    private const string RATING = "Rating-Curve";
    private const string STAGE_DAMAGE = "Stage-Damage";
    private const string FLOW = "Flow";
    private const string STAGE = "Stage";

    public static void CheckForOverlappingRanges(ChildElementComboItem SelectedFrequencyElement, ChildElementComboItem SelectedInflowOutflowElement,
        ChildElementComboItem SelectedRatingCurveElement, ChildElementComboItem SelectedExteriorInteriorElement, AggregatedStageDamageElement StageDamageElement,
        StageDamageCurve SelectedDamageCurve, ObservableCollection<RecommendationRowItem> messageRows)
    {
        //Note: the following axis determinations are based on this table:
        //       flow freq:  frequency,       inflow
        //inflow - outflow:  inflow,          outflow
        //          Rating:  Outflow,         Exterior Stage
        //       Ext - Int:  Exterior Stage,  Interior Stage
        //    Stage Damage:  Interior Stage,  Damage

        //We can assume that by the time we get here flow-freq,  stage-damage exist. You do not need a rating curve
        //if the frequency function is a stage-frequency
        //we do not care about the exterior interior curve, just skip it.

        bool inflowOutflowSelected = SelectedInflowOutflowElement.ChildElement != null;
        bool ratingSelected = SelectedRatingCurveElement.ChildElement != null;

        if (inflowOutflowSelected)
        {
            //check in-out flows with flow freq
            CheckRangeValues(SelectedInflowOutflowElement, SelectedFrequencyElement, true, false, INFLOW_OUTFLOW, FLOW, messageRows);
            if (ratingSelected)
            {
                //check outflows with rating flows
                CheckRangeValues(SelectedRatingCurveElement, SelectedInflowOutflowElement, true, false, RATING, FLOW, messageRows);
                //check rating stages with stage-damage stages
                CheckRangeWithStageDamage(StageDamageElement, SelectedRatingCurveElement, SelectedDamageCurve, messageRows);
            }
        }
        else if(ratingSelected)
        {
            //check rating flows with flow-freq flows
            CheckRangeValues(SelectedRatingCurveElement, SelectedFrequencyElement, true, false, RATING, FLOW, messageRows);

            //check rating stages with stage-damage stages
            CheckRangeWithStageDamage(StageDamageElement, SelectedRatingCurveElement, SelectedDamageCurve, messageRows);
        }
    }

    /// <summary>
    /// Dealing with the stage damage element is a little more complicated. The stage damage element has a list of curves. We have to find 
    /// the correct curve based on what damage category is selected by the user.
    /// </summary>
    /// <param name="stageDamElem"></param>
    /// <param name="otherElem"></param>
    /// <param name="compareXAxis"></param>
    private static void CheckRangeWithStageDamage(AggregatedStageDamageElement stageDamElem, ChildElementComboItem otherElem, 
        StageDamageCurve selectedDamageCurve, ObservableCollection<RecommendationRowItem> messageRows)
    {

        double minProb = .00001;
        double maxProb = .99999;

        //this will always compare the x values of the stage-damage to the y values of the other element.
        double stageDamageMin = -1;
        double stageDamageMax = -1;
        double otherMin = -1;
        double otherMax = -1;

        UncertainPairedData otherCurve = ((CurveChildElement)otherElem.ChildElement).CurveComponentVM.SelectedItemToPairedData();

        stageDamageMin = selectedDamageCurve.ComputeComponent.SelectedItemToPairedData().Xvals.Min();
        stageDamageMax = selectedDamageCurve.ComputeComponent.SelectedItemToPairedData().Xvals.Last();

        IDistribution firstDistribution = otherCurve.Yvals.First();
        IDistribution lastDistribution = otherCurve.Yvals.Last();

        otherMin = firstDistribution.InverseCDF(minProb);
        otherMax = lastDistribution.InverseCDF(maxProb);

        AddRecommendationForNonoverlappingRange(stageDamageMin, stageDamageMax, otherMin, otherMax, STAGE_DAMAGE, STAGE,
            stageDamElem.Name, otherElem.ChildElement.Name, messageRows);

    }

    /// <summary>
    /// This method finds the non-overlapping regions and creates a message object that gets displayed in the UI. 
    /// </summary>
    /// <param name="element1">This needs to be the curve that is associated with that node in the warnings tree.</param>
    /// <param name="element2"></param>
    /// <param name="compareXAxis"></param>
    /// <param name="headerBase"></param>
    /// <param name="axisLabel"></param>
    private static void CheckRangeValues(ChildElementComboItem element1, ChildElementComboItem element2, bool compareXAxisOnElem1, bool compareXAxisOnElem2,
        string headerBase, string axisLabel, ObservableCollection<RecommendationRowItem> messageRows)
    {
        double minProb = .00001;
        double maxProb = .99999;

        IPairedDataProducer func1 = GetUncertainPairedData(element1);
        IPairedDataProducer func2 = GetUncertainPairedData(element2);


        PairedData function1Max = func1.SamplePairedData(maxProb, true);
        PairedData function1Min = func1.SamplePairedData(minProb, true);
        PairedData function2Max = func2.SamplePairedData(maxProb, true);
        PairedData function2Min = func2.SamplePairedData(minProb, true);

        string name1 = element1.Name;
        string name2 = element2.Name;

        double x1Min = function1Max.Xvals.First();
        double x1Max = function1Max.Xvals.Last();

        double x2Min = function2Max.Xvals.First();
        double x2Max = function2Max.Xvals.Last();

        double y1Min = function1Min.Yvals.First();
        double y1Max = function1Max.Yvals.Last();

        double y2Min = function2Min.Yvals.First();
        double y2Max = function2Max.Yvals.Last();

        //these will be the min and max for the axes that we care about
        double min1;
        double max1;

        double min2;
        double max2;
        if (compareXAxisOnElem1)
        {
            min1 = x1Min;
            max1 = x1Max;
        }
        else
        {
            min1 = y1Min;
            max1 = y1Max;
        }
        if (compareXAxisOnElem2)
        {
            min2 = x2Min;
            max2 = x2Max;
        }
        else
        {
            min2 = y2Min;
            max2 = y2Max;
        }

        AddRecommendationForNonoverlappingRange(min1, max1, min2, max2, headerBase, axisLabel, name1, name2, messageRows);
    }

    private static IPairedDataProducer GetUncertainPairedData(ChildElementComboItem elementComboItem)
    {
        ChildElement elem = elementComboItem.ChildElement;

        CurveChildElement curveElem = elem as CurveChildElement;

        IPairedDataProducer data;

        if (elem is FrequencyElement freqElem)
        {
            if (freqElem.IsAnalytical)
            {
                data = freqElem.LPIIIasUPD;
            }
            else
            {
                data = freqElem.GraphicalUncertainPairedData;
            }
        }
        else
        {
            data = curveElem.CurveComponentVM.SelectedItemToPairedData();
        }

        return data;
    }


    /// <summary>
    /// The values here should either be both mins or both maxes.
    /// </summary>
    /// <param name="val1"></param>
    /// <param name="val2"></param>
    /// <param name="totalRange"></param>
    /// <returns></returns>
    private static bool passesRangeTest(double val1, double val2, double totalRange)
    {
        double nonOverlap = Math.Abs(val1 - val2);
        double FivePercentOfTotal = totalRange * OVERLAP_RULE_PERCENT;
        return nonOverlap < FivePercentOfTotal;
    }

    private static double getTotalRange(double min1, double min2, double max1, double max2)
    {
        List<double> values = new List<double>() { min1, min2, max1, max2 };
        double min = values.Min();
        double max = values.Max();
        return max - min;
    }

    private static void AddRecommendationForNonoverlappingRange(double min1, double max1, double min2, double max2, string headerBase,
        string axisLabel, string name1, string name2, ObservableCollection<RecommendationRowItem> messageRows)
    {
        RecommendationRowItem ri = new RecommendationRowItem(headerBase + ": " + name1);
        bool nonOverlapMin = false;
        bool nonOverlapMax = false;
        string minRange = "";
        string maxRange = "";

        //if a max is less than a min then there is no overlap at all.
        if (min1 > max2 || min2 > max1)
        {
            ri.Messages.Add(name1 + " has no overlapping range with " + name2);
        }
        else
        {
            double totalRange = getTotalRange(min1, min2, max1, max2);
            if (!passesRangeTest(min1, min2, totalRange))
            {
                nonOverlapMin = true;
                minRange = getRangeString(min1, min2);
            }
            if (!passesRangeTest(max1, max2, totalRange))
            {
                nonOverlapMax = true;
                maxRange = getRangeString(max1, max2);
            }

            if (nonOverlapMin && nonOverlapMax)
            {
                ri.Messages.Add("Non-overlapping " + axisLabel + " with " + name2 + ": " + minRange + " and " + maxRange);
            }
            else if (nonOverlapMin)
            {
                ri.Messages.Add("Non-overlapping " + axisLabel + " with " + name2 + ": " + minRange);
            }
            else if (nonOverlapMax)
            {
                ri.Messages.Add("Non-overlapping " + axisLabel + " with " + name2 + ": " + maxRange);
            }
        }
        if (ri.Messages.Count > 0)
        {
            messageRows.Add(ri);
        }

    }

    private static string getRangeString(double val1, double val2)
    {
        string retval;
        //i want to display the lowest value first
        bool val1IsLowest = false;
        if (val1 < val2)
        {
            val1IsLowest = true;
        }

        //only dispaly 2 decimal places
        val1 = Math.Round(val1, 2);
        val2 = Math.Round(val2, 2);

        if (val1IsLowest)
        {
            retval = "[" + val1 + " - " + val2 + "]";
        }
        else
        {
            retval = "[" + val2 + " - " + val1 + "]";
        }
        return retval;
    }

}
