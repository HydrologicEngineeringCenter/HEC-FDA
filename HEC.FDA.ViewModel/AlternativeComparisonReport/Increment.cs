﻿using System.Collections.Generic;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport
{
    public class Increment
    {
        public List<string> Plans { get; set; }
        public string SelectedPlan1 { get; set; }
        public string SelectedPlan2 { get; set; }
        public string Name { get; set; }
        public Increment(string name, List<string> plans)
        {
            Name = name;
            Plans = plans;
        }

    }
}
