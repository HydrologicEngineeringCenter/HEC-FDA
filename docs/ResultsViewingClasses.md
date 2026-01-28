# Results Viewing Classes Reference

This document lists all classes related to results viewing in HEC-FDA.

## ViewModels - Main Results Containers

| Class | Path |
|-------|------|
| `ScenarioResultsVM` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\` |
| `AlternativeResultsVM` | `HEC.FDA.ViewModel\Alternatives\Results\` |
| `AltCompReportResultsVM` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |
| `SpecificIASResultVM` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\` |
| `SpecificAltCompReportResultsVM` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |

## ViewModels - Damage & Uncertainty

| Class | Path |
|-------|------|
| `DamageWithUncertaintyVM` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\` |
| `DamageWithUncertaintyVM` | `HEC.FDA.ViewModel\Alternatives\Results\` |
| `DamageByImpactAreaVM` | `HEC.FDA.ViewModel\Alternatives\Results\` |
| `DamageByDamCatVM` | `HEC.FDA.ViewModel\Alternatives\Results\` |

## ViewModels - Performance

| Class | Path |
|-------|------|
| `PerformanceAEPVM` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\` |
| `PerformanceAssuranceOfThresholdVM` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\` |
| `PerformanceLongTermRiskVM` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\` |
| `PerformanceVMBase` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\` |

## ViewModels - Summaries

| Class | Path |
|-------|------|
| `ScenarioDamageSummaryVM` | `HEC.FDA.ViewModel\Results\` |
| `SummaryVM` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |
| `EADSummaryVM` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |
| `EqadSummaryVM` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |
| `AlternativeSummaryVM` | `HEC.FDA.ViewModel\Alternatives\Results\BatchCompute\` |

## Row Items - Scenario Results

| Class | Path |
|-------|------|
| `ScenarioDamageRowItem` | `HEC.FDA.ViewModel\Results\` |
| `ScenarioDamCatRowItem` | `HEC.FDA.ViewModel\Results\` |
| `ScenarioLifeLossRowItem` | `HEC.FDA.ViewModel\Results\` |
| `ScenarioPerformanceRowItem` | `HEC.FDA.ViewModel\Results\` |
| `AssuranceOfAEPRowItem` | `HEC.FDA.ViewModel\Results\` |

## Row Items - ImpactAreaScenario

| Class | Path |
|-------|------|
| `EadRowItem` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\RowItems\` |
| `EqadRowItem` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\RowItems\` |
| `LifeLossRowItem` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\RowItems\` |
| `DamageCategoryRowItem` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\RowItems\` |
| `ResultRowItem` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\RowItems\` |
| `PerformanceFrequencyRowItem` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\RowItems\` |
| `PerformancePeriodRowItem` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\RowItems\` |
| `IQuartileRowItem` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\RowItems\` |
| `IPerformanceRowItem` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\RowItems\` |

## Row Items - Alternative Comparison Report

| Class | Path |
|-------|------|
| `EADSummaryRowItem` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |
| `AggregatedEADSummaryRowItem` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |
| `EqadSummaryRowItem` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |
| `AggregatedEqadSummaryRowItem` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |
| `AggregatedEALLSummaryRowItem` | `HEC.FDA.ViewModel\AlternativeComparisonReport\Results\` |

## Row Items - Batch Compute

| Class | Path |
|-------|------|
| `AlternativeDamageRowItem` | `HEC.FDA.ViewModel\Alternatives\Results\BatchCompute\` |
| `AlternativeDamCatRowItem` | `HEC.FDA.ViewModel\Alternatives\Results\BatchCompute\` |

## Supporting Classes

| Class | Path |
|-------|------|
| `UncertaintyControlConfigs` | `HEC.FDA.ViewModel\ImpactAreaScenario\Results\` |
| `ImpactAreaRowItems` | `HEC.FDA.ViewModel\Alternatives\Results\` |
| `ScenarioProgressVM` | `HEC.FDA.ViewModel\Results\` |
| `ScenarioSelectorVM` | `HEC.FDA.ViewModel\Results\` |
| `ChildSelectorVM` | `HEC.FDA.ViewModel\Results\` |
| `SelectableChildElement` | `HEC.FDA.ViewModel\Results\` |

## Result Object Classes

| Class | Path |
|-------|------|
| `AlternativeResult` | `HEC.FDA.ViewModel\Alternatives\Results\ResultObject\` |
| `EADResult` | `HEC.FDA.ViewModel\Alternatives\Results\ResultObject\` |
| `EqadResult` | `HEC.FDA.ViewModel\Alternatives\Results\ResultObject\` |
| `YearResult` | `HEC.FDA.ViewModel\Alternatives\Results\ResultObject\` |
| `IAlternativeResult` | `HEC.FDA.ViewModel\Alternatives\Results\` |

## Model Layer - Results Data Models

| Class | Path |
|-------|------|
| `AlternativeResults` | `HEC.FDA.Model\metrics\` |
| `ImpactAreaScenarioResults` | `HEC.FDA.Model\metrics\` |
| `ScenarioResults` | `HEC.FDA.Model\metrics\` |
| `SystemPerformanceResults` | `HEC.FDA.Model\metrics\` |
| `AlternativeComparisonReportResults` | `HEC.FDA.Model\metrics\` |

## View Layer - XAML Views

### ImpactAreaScenario Results
- `IASResults.xaml`
- `SpecificIASResultsControl.xaml`
- `DamageWithUncertainty.xaml`
- `DamageByDamageCategory.xaml`
- `PerformanceAEP.xaml`
- `PerformanceAssuranceOfThreshold.xaml`
- `PerformanceLongTermRisk.xaml`

### Alternative Results
- `AlternativeResults.xaml`
- `DamageWithUncertainty.xaml`
- `DamageByImpactArea.xaml`
- `AlternativeSelector.xaml`
- `AlternativeSummaryTable.xaml`

### Alternative Comparison Report Results
- `AltCompReportResults.xaml`
- `SpecificAltCompReportResults.xaml`
- `SummaryResults.xaml`
- `EADSummary.xaml`
- `EqadSummary.xaml`

### General Results
- `ScenarioDamageSummary.xaml`
- `ScenarioProgressControl.xaml`
- `ScenarioChildSelector.xaml`
