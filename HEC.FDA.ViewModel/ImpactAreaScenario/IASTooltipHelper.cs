using HEC.FDA.ViewModel.Results;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.ImpactAreaScenario
{
    public static class IASTooltipHelper
    {

        public static void UpdateTooltip(IASElement elem)
        {
            StringBuilder sb = new StringBuilder();
            bool hasResults = elem.Results != null;

            //check if the elements that the scenario points to still exists.
            FdaValidationResult vr = elem.CanCompute();
            if (!vr.IsValid)
            {
                sb.AppendLine(vr.ErrorMessage);
                sb.AppendLine("Last Edited: " + elem.LastEditDate);
                if (hasResults)
                {
                    sb.AppendLine("Last Computed: " + GetComputeDate(elem));
                }
                elem.CustomTreeViewHeader.Decoration = ImageSources.ERROR_IMAGE;
            }
            else
            {
                //todo: put decorator icons on these options?
                sb.AppendLine("Last Edited: " + elem.LastEditDate);
                if (hasResults)
                {
                    //is it up to date.
                    if (hasResults)
                    {
                        string lastCompute = GetComputeDate(elem);
                        sb.AppendLine("Last Computed: " + lastCompute);

                        DateTime lastEditDate = DateTime.Parse(elem.LastEditDate);
                        if (lastCompute != "NA")
                        {
                            DateTime computeDate = DateTime.Parse(lastCompute);
                            elem.CustomTreeViewHeader.Decoration = ImageSources.GREEN_CHECKMARK_IMAGE;

                            if (lastEditDate > computeDate)
                            {
                                sb.AppendLine("Changes have been made since last compute.");
                                elem.CustomTreeViewHeader.Decoration = ImageSources.CAUTION_IMAGE;
                            }
                        }
                    }
                }

            }
            //remove last new line char
            string tooltipMsg = sb.ToString();

            elem.CustomTreeViewHeader.Tooltip = tooltipMsg.Trim();

        }

        public static void UpdateTooltip(SelectableChildElement selectableElement)
        {
            IASElement elem = selectableElement.Element;
            StringBuilder sb = new StringBuilder();
            bool hasResults = elem.Results != null;

            //check if the elements that the scenario points to still exists.
            FdaValidationResult vr = elem.CanCompute();
            if (!vr.IsValid)
            {
                sb.AppendLine(vr.ErrorMessage);
                sb.AppendLine("Last Edited: " + elem.LastEditDate);
                if (hasResults)
                {
                    sb.AppendLine("Last Computed: " + GetComputeDate(elem));
                }
                selectableElement.Decoration = ImageSources.ERROR_IMAGE;
            }
            else
            {
                //todo: put decorator icons on these options?
                sb.AppendLine("Last Edited: " + elem.LastEditDate);
                if (hasResults)
                {
                    //is it up to date.
                    if (hasResults)
                    {
                        string lastCompute = GetComputeDate(elem);
                        sb.AppendLine("Last Computed: " + lastCompute);

                        DateTime lastEditDate = DateTime.Parse(elem.LastEditDate);
                        DateTime computeDate = DateTime.Parse(lastCompute);
                        selectableElement.Decoration = ImageSources.GREEN_CHECKMARK_IMAGE;

                        if (lastEditDate > computeDate)
                        {
                            sb.AppendLine("Changes have been made since last compute.");
                            selectableElement.Decoration = ImageSources.CAUTION_IMAGE;
                        }

                    }
                }

            }
            //remove last new line char
            string tooltipMsg = sb.ToString();

            selectableElement.Tooltip = tooltipMsg.Trim();
        }

        private static string GetComputeDate(IASElement elem)
        {
            string computeDate = null;
            if (elem.Results != null)
            {
                computeDate = elem.Results.ComputeDate;
                if (computeDate == null)
                {
                    computeDate = "NA";
                }
            }
            return computeDate;
        }

    }
}
