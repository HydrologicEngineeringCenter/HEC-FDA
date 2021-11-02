using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ViewModel.AggregatedStageDamage;
using ViewModel.Utilities;

namespace ViewModel.ImpactAreaScenario.Editor
{
    class OverlappingRangeHelper
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

            //assume that the big 3 exist by the time we get here (flow-freq, rating, stage-damage).
            //we do not care about the exterior interior curve, just skip it.

            bool inflowOutflowSelected = SelectedInflowOutflowElement.ChildElement != null;

            if (inflowOutflowSelected)
            {
                //check in-out flows with flow freq
                CheckRangeValues(SelectedInflowOutflowElement, SelectedFrequencyElement, true, false, INFLOW_OUTFLOW, FLOW, messageRows);
                //check outflows with rating flows
                CheckRangeValues(SelectedRatingCurveElement, SelectedInflowOutflowElement, true, false, RATING, FLOW, messageRows);
                //check rating stages with stage-damage stages
                CheckRangeWithStageDamage(StageDamageElement, SelectedRatingCurveElement, SelectedDamageCurve, messageRows);

            }
            else
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
            //this will always compare the x values of the stage-damage to the y values of the other element.
            double stageDamageMin = -1;
            double stageDamageMax = -1;
            double otherMin = -1;
            double otherMax = -1;

            IFdaFunction otherCurve = otherElem.ChildElement.Curve;

            stageDamageMin = selectedDamageCurve.Function.Coordinates.First().X.Value();
            stageDamageMax = selectedDamageCurve.Function.Coordinates.Last().X.Value();

            otherMin = otherCurve.YSeries.Range.Min;
            otherMax = otherCurve.YSeries.Range.Max;

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
            ChildElement elem1 = element1.ChildElement;
            ChildElement elem2 = element2.ChildElement;
            string name1 = elem1.Name;
            string name2 = elem2.Name;
            IParameterRange range1;
            IParameterRange range2;
            if (compareXAxisOnElem1)
            {
                range1 = elem1.Curve.XSeries;
            }
            else
            {
                range1 = elem1.Curve.YSeries;
            }
            if (compareXAxisOnElem2)
            {
                range2 = elem2.Curve.XSeries;
            }
            else
            {
                range2 = elem2.Curve.YSeries;
            }

            double min1 = range1.Range.Min;
            double max1 = range1.Range.Max;

            double min2 = range2.Range.Min;
            double max2 = range2.Range.Max;


            AddRecommendationForNonoverlappingRange(min1, max1, min2, max2, headerBase, axisLabel, name1, name2, messageRows);

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

            //todo: apply some min and max rule.
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
}
