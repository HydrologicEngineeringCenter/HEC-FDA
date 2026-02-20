# ImpactAreaScenarioSimulation.Compute() - Detailed Explanation

This document explains the computation flow of the `Compute()` method in `ImpactAreaScenarioSimulation.cs`, which is the core Monte Carlo simulation engine for flood damage and life loss analysis.

---

## Table of Contents

1. [Overview](#overview)
2. [Main Compute() Method](#main-compute-method)
3. [Validation Phase](#validation-phase)
4. [Random Number Preparation](#random-number-preparation)
5. [Threshold and Histogram Setup](#threshold-and-histogram-setup)
6. [Monte Carlo Iteration Loop](#monte-carlo-iteration-loop)
7. [Stage-Frequency Computation Paths](#stage-frequency-computation-paths)
8. [Consequence Computation](#consequence-computation)
9. [Performance Metrics](#performance-metrics)
10. [Flow Diagram](#flow-diagram)

---

## Overview

The `Compute()` method performs a Monte Carlo simulation to estimate:
- **Expected Annual Damage (EAD)** - Economic flood damages
- **Expected Annual Life Loss** - Potential fatalities
- **System Performance Metrics** - Probability of exceeding thresholds

The simulation samples from uncertain functions (frequency-discharge, stage-damage, system response, etc.) across many iterations to build statistical distributions of outcomes.

---

## Main Compute() Method

**Location:** Lines 103-165

```
Compute(convergenceCriteria, cancellationToken, computeIsDeterministic)
```

### High-Level Flow:

```
1. Validate configuration (CanCompute)
2. Generate random numbers for all uncertain functions
3. Set up consequence histograms and thresholds
4. Run Monte Carlo iterations until convergence
5. Return results
```

### Key Parameters:
- `convergenceCriteria` - Controls min/max iterations and convergence checks
- `cancellationToken` - Allows cancellation of long-running compute
- `computeIsDeterministic` - If true, uses median values instead of sampling

---

## Validation Phase

**Method:** `CanCompute()` (Lines 273-299)

Before any computation, the system validates that the configuration is runnable:

```
CanCompute() checks:
├── ErrorLevel < Fatal (no critical configuration errors)
├── ConvergenceCriteria is valid
├── ValidateNonFail() - if non-fail risk requested, levee must exist
└── Must have either:
    ├── Stage-Damage functions, OR
    ├── Stage-Life-Loss functions, OR
    └── A levee (system response function)
```

If validation fails, returns an empty results object with `isNull = true`.

---

## Random Number Preparation

**Method:** `PopulateRandomNumbers()` (Lines 168-218)

Generates pre-computed random numbers for each uncertain function. This enables:
- Reproducible results (same seed = same results)
- Parallel iteration (each iteration pulls from pre-generated arrays)

```
PopulateRandomNumbers() generates randoms for:
├── _FrequencyDischarge (seed: 1234)
├── _FrequencyDischargeGraphical (seed: 1234)
├── _UnregulatedRegulated (seed: 2345)
├── _DischargeStage (seed: 3456)
├── _FrequencyStage (seed: 1234)
├── _ChannelStageFloodplainStage (seed: 4567)  [currently disabled]
├── _SystemResponseFunction (seed: 5678)
├── _FailureStageDamageFunctions (seed: 6789)
├── _NonFailureStageDamageFunctions (seed: 6789)
├── _FailureStageLifeLossFunctions (seed: 7891)
└── _NonFailureStageLifeLossFunctions (seed: 7891)
```

Each seed is a constant ensuring reproducibility across runs.

---

## Threshold and Histogram Setup

**Location:** Lines 117-157

### Case 1: No Stage-Damage Functions (Performance-Only Compute)

```
if (_FailureStageDamageFunctions.Count == 0):
├── Add zero-consequence placeholder result
├── Determine threshold type:
│   ├── If SystemResponse has ≤2 points → TopOfLevee threshold
│   └── If SystemResponse has >2 points → LeveeSystemResponse threshold
└── Add threshold to results
```

### Case 2: Has Stage-Damage Functions (Full Compute)

```
if (_FailureStageDamageFunctions.Count > 0):
├── ComputeConsequenceFrequency() → Get damage-frequency curves
├── Save damage-frequency functions to results
├── CreateEAConsequenceHistograms() → Set up convergence tracking
└── ComputeDefaultThreshold() → Calculate 5% damage threshold stage
```

### Life Loss Setup (Lines 150-155)

```
if (_HasFailureStageLifeLoss):
├── ComputeConsequenceFrequency() → Get life-loss-frequency curves
├── Save to results
└── CreateEAConsequenceHistograms() for life loss
```

---

## Monte Carlo Iteration Loop

**Method:** `ComputeIterations()` (Lines 315-429)

This is the heart of the simulation - a convergence-driven loop that runs parallel iterations.

### Loop Structure:

```
while (computeIsNotConverged):
│
├── for each computeChunk (batch of iterations):
│   │
│   ├── Parallel.For(0, iterationsPerChunk):
│   │   │
│   │   ├── Determine frequency-stage relationship
│   │   │   ├── If _FrequencyStage exists → Use directly
│   │   │   └── Otherwise → Build from frequency-discharge + rating curve
│   │   │
│   │   └── ComputeFromStageFrequency(...)
│   │
│   ├── PutDataIntoHistograms() for consequences
│   ├── PutDataIntoHistograms() for each threshold
│   └── Report progress
│
├── Check convergence: ResultsAreConverged(0.95, 0.05)
│   ├── If not converged → Calculate remaining iterations, continue
│   └── If converged → Exit loop
```

### Building Frequency-Stage (Inside Parallel Loop)

Three possible paths to get `frequency_stage`:

```
Path A: Direct Stage-Frequency
─────────────────────────────
if _FrequencyStage exists:
    frequency_stage = _FrequencyStage.Sample()

Path B: Discharge-Frequency with Rating Curve (No Regulation)
─────────────────────────────────────────────────────────────
if _FrequencyDischargeGraphical exists:
    frequency_discharge = _FrequencyDischargeGraphical.Sample()
else:
    frequency_discharge = _FrequencyDischarge.Bootstrap()

discharge_stage = _DischargeStage.Sample()
frequency_stage = discharge_stage.compose(frequency_discharge)

Path C: Discharge-Frequency with Regulation Transform
─────────────────────────────────────────────────────
frequency_discharge = [same as Path B]
inflow_outflow = _UnregulatedRegulated.Sample()
transformed_frequency = inflow_outflow.compose(frequency_discharge)
discharge_stage = _DischargeStage.Sample()
frequency_stage = discharge_stage.compose(transformed_frequency)
```

---

## Stage-Frequency Computation Paths

**Method:** `ComputeFromStageFrequency()` (Lines 431-513)

Once we have a frequency-stage curve, this method branches based on whether a levee exists.

```
ComputeFromStageFrequency(frequency_stage, ...):
│
├── WITHOUT LEVEE (_SystemResponseFunction.IsNull):
│   │
│   ├── if computeWithDamage:
│   │   └── ComputeConsequencesFromStageFrequency(..., _FailureStageDamageFunctions)
│   │
│   ├── if computeWithLifeLoss:
│   │   └── ComputeConsequencesFromStageFrequency(..., _FailureStageLifeLossFunctions)
│   │
│   └── ComputePerformance(frequency_stage)
│
└── WITH LEVEE:
    │
    ├── systemResponse_sample = _SystemResponseFunction.Sample()
    │
    ├── if computeWithDamage:
    │   └── ComputeDamagesFromStageFrequency_WithLevee(..., ConsequenceType.Damage)
    │
    ├── if computeWithLifeLoss:
    │   └── ComputeDamagesFromStageFrequency_WithLevee(..., ConsequenceType.LifeLoss)
    │
    └── Performance:
        ├── if systemResponse ≤2 points → ComputePerformance()
        └── if systemResponse >2 points → ComputeLeveePerformance()
```

---

## Consequence Computation

### Without Levee: Simple Integration

**Method:** `ComputeConsequencesFromStageFrequency()` (Lines 515-526)

```
For each stage-consequence function:
│
├── stage_consequences = stageUncertainConsequences.Sample()
├── frequency_consequences = stage_consequences.compose(frequency_stage)
├── eaConsequences = frequency_consequences.integrate()
└── Store result in histogram
```

The integration computes Expected Annual Consequences by integrating under the consequence-frequency curve.

### With Levee: Failure and Non-Failure Paths

**Method:** `ComputeDamagesFromStageFrequency_WithLevee()` (Lines 528-576)

This is the most complex computation path, handling both failure and non-failure scenarios.

```
ComputeDamagesFromStageFrequency_WithLevee():
│
├── Select function lists based on ConsequenceType:
│   ├── LifeLoss → _FailureStageLifeLossFunctions, _NonFailureStageLifeLossFunctions
│   └── Damage → _FailureStageDamageFunctions, _NonFailureStageDamageFunctions
│
└── For each failure stage-consequence function:
    │
    ├── stageDamageFailSample = stageUncertainDamage.Sample()
    ├── validatedSystemResponse = EnsureBottomAndTopHaveCorrectProbabilities()
    │   └── Ensures fragility goes from 0 to 1
    │
    ├── FAILURE CONSEQUENCES:
    │   ├── stageDamageFailAdjusted = stageDamageFailSample × systemResponse
    │   ├── stageFreqFail = stageDamageFailAdjusted.compose(frequency_stage)
    │   ├── failEad = stageFreqFail.integrate()
    │   └── Store failEad in results
    │
    └── NON-FAILURE CONSEQUENCES (if both fail and non-fail functions exist):
        │
        ├── Find matching non-failure function (same damage/asset category)
        ├── inverseOfSystemResponse = 1 - systemResponse
        ├── stageDamNonFail = stageUncertainNonFailureDamage.Sample()
        ├── stageDamNonFailAdjusted = stageDamNonFail × inverseOfSystemResponse
        │
        ├── TOTAL = stageDamageFailAdjusted + stageDamNonFailAdjusted
        ├── frequency_damage = TOTAL.compose(frequency_stage)
        ├── eadEstimate = frequency_damage.integrate()
        └── Store eadEstimate in results
```

**Key Insight:** When a levee exists:
- **Failure consequences** are multiplied by the probability of failure (system response)
- **Non-failure consequences** are multiplied by (1 - probability of failure)
- Both are added together for total expected consequences

---

## Performance Metrics

### Without Levee (Simple Threshold)

**Method:** `ComputePerformance()` (Lines 604-614)

```
For each threshold:
├── thresholdValue = threshold stage
├── aep = 1 - frequency_stage.f_inverse(thresholdValue)
│   └── Annual Exceedance Probability of exceeding threshold
├── Store AEP for assurance calculation
└── GetStageForNonExceedanceProbability()
    └── Record stages at 0.9, 0.96, 0.98, 0.99, 0.996, 0.998 non-exceedance
```

### With Levee (Fragility-Weighted)

**Method:** `ComputeLeveePerformance()` (Lines 616-644)

```
levee_frequency_stage = levee_curve.compose(frequency_stage)
    └── Maps exceedance probability to failure probability

aep = 0

# Below fragility curve range
aep += (first_prob - 0) × (first_failure_prob + 0) / 2

# Within fragility curve range
for each segment:
    prob_in_range = prob[i] - prob[i-1]
    avg_failure_prob = (failure[i] + failure[i-1]) / 2
    aep += prob_in_range × avg_failure_prob

# Above fragility curve range
aep += (1 - last_prob) × last_failure_prob

Store aep for all thresholds
```

This computes the probability-weighted failure AEP considering the entire fragility curve.

---

## Flow Diagram

### Master Flow (Sequential Steps)

These steps execute **in order**, one after another:

```
═══════════════════════════════════════════════════════════════════════════════
 STEP 1: VALIDATION                                              [CanCompute()]
═══════════════════════════════════════════════════════════════════════════════
                                     │
                                     ▼
═══════════════════════════════════════════════════════════════════════════════
 STEP 2: RANDOM NUMBER GENERATION                     [PopulateRandomNumbers()]
═══════════════════════════════════════════════════════════════════════════════
                                     │
                                     ▼
═══════════════════════════════════════════════════════════════════════════════
 STEP 3: SETUP PHASE                                           [Multiple calls]
═══════════════════════════════════════════════════════════════════════════════
                                     │
                                     ▼
═══════════════════════════════════════════════════════════════════════════════
 STEP 4: MONTE CARLO LOOP                                 [ComputeIterations()]
═══════════════════════════════════════════════════════════════════════════════
                                     │
                                     ▼
═══════════════════════════════════════════════════════════════════════════════
 STEP 5: RETURN RESULTS
═══════════════════════════════════════════════════════════════════════════════
```

---

### STEP 3 Detail: Setup Phase (Sequential Sub-steps)

These sub-steps run **in order** within the Setup Phase:

```
STEP 3: SETUP PHASE
│
├─► 3.1  IF no stage-damage functions:
│        └─► Add zero-consequence placeholder
│        └─► Add system response threshold
│
├─► 3.2  ELSE (has stage-damage functions):
│        ├─► 3.2a  ComputeConsequenceFrequency() for damage
│        │           └─► Produces deterministic damage-frequency curves
│        ├─► 3.2b  CreateEAConsequenceHistograms() for damage
│        │           └─► Sets up histogram bins for convergence tracking
│        └─► 3.2c  ComputeDefaultThreshold()
│                    └─► Calculates 5% damage threshold stage
│
├─► 3.3  IF has life loss functions:
│        ├─► 3.3a  ComputeConsequenceFrequency() for life loss
│        └─► 3.3b  CreateEAConsequenceHistograms() for life loss
│
└─► 3.4  CreateHistogramsForAssuranceOfThresholds()
           └─► Sets up stage histograms at standard non-exceedance probs
```

---

### STEP 4 Detail: Monte Carlo Loop Structure

```
STEP 4: MONTE CARLO LOOP (ComputeIterations)
│
│   ┌─────────────────────────────────────────────────────────────────────┐
│   │  OUTER LOOP: while (not converged)                                  │
│   │  ════════════════════════════════════════════════════════════════   │
│   │      │                                                              │
│   │      ▼                                                              │
│   │  ┌─────────────────────────────────────────────────────────────┐   │
│   │  │  MIDDLE LOOP: for each compute chunk                        │   │
│   │  │  ───────────────────────────────────────────────────────    │   │
│   │  │      │                                                      │   │
│   │  │      ▼                                                      │   │
│   │  │  ┌───────────────────────────────────────────────────────┐ │   │
│   │  │  │ INNER LOOP: Parallel.For (iterations in chunk)        │ │   │
│   │  │  │ ─────────────────────────────────────────────────     │ │   │
│   │  │  │    ┌────────────────────────────────────────────┐    │ │   │
│   │  │  │    │  PER-ITERATION WORK (see detail below)     │    │ │   │
│   │  │  │    │  • Build frequency_stage                   │    │ │   │
│   │  │  │    │  • ComputeFromStageFrequency()             │    │ │   │
│   │  │  │    └────────────────────────────────────────────┘    │ │   │
│   │  │  └───────────────────────────────────────────────────────┘ │   │
│   │  │      │                                                      │   │
│   │  │      ▼                                                      │   │
│   │  │  AFTER PARALLEL BLOCK (sequential):                         │   │
│   │  │      ├─► PutDataIntoHistograms() for consequences           │   │
│   │  │      ├─► PutDataIntoHistograms() for each threshold         │   │
│   │  │      └─► Report progress                                    │   │
│   │  └─────────────────────────────────────────────────────────────┘   │
│   │      │                                                              │
│   │      ▼                                                              │
│   │  CHECK CONVERGENCE: ResultsAreConverged(0.95, 0.05)?               │
│   │      ├─► YES → Exit loop                                           │
│   │      └─► NO  → Recalculate chunks, continue                        │
│   └─────────────────────────────────────────────────────────────────────┘
```

---

### Per-Iteration Work (Inside Parallel.For)

Each iteration runs these steps **sequentially**, but iterations run in **parallel** with each other:

```
ONE ITERATION (runs in parallel with other iterations)
══════════════════════════════════════════════════════

SEQUENTIAL STEP A: Build frequency_stage curve
──────────────────────────────────────────────
    ┌─────────────────────────────────────────────────────────────┐
    │  CHOICE (pick one path):                                    │
    │                                                             │
    │  Path A: _FrequencyStage exists                             │
    │     └─► frequency_stage = _FrequencyStage.Sample()          │
    │                                                             │
    │  Path B: No regulation transform                            │
    │     ├─► frequency_discharge = Sample discharge-frequency    │
    │     ├─► discharge_stage = _DischargeStage.Sample()          │
    │     └─► frequency_stage = discharge_stage.compose(freq_Q)   │
    │                                                             │
    │  Path C: Has regulation transform                           │
    │     ├─► frequency_discharge = Sample discharge-frequency    │
    │     ├─► inflow_outflow = _UnregulatedRegulated.Sample()     │
    │     ├─► transformed = inflow_outflow.compose(freq_discharge)│
    │     ├─► discharge_stage = _DischargeStage.Sample()          │
    │     └─► frequency_stage = discharge_stage.compose(transform)│
    └─────────────────────────────────────────────────────────────┘
                                │
                                ▼
SEQUENTIAL STEP B: ComputeFromStageFrequency()
──────────────────────────────────────────────
    (See next diagram for internal structure)
```

---

### ComputeFromStageFrequency() Internal Structure

This shows what happens **inside** ComputeFromStageFrequency for ONE iteration:

```
ComputeFromStageFrequency(frequency_stage, ...)
═══════════════════════════════════════════════

BRANCH: Does levee exist?
─────────────────────────

        ┌──────────────── NO LEVEE ────────────────┐      ┌────────────── WITH LEVEE ──────────────┐
        │                                          │      │                                        │
        │  SEQUENTIAL STEPS:                       │      │  SEQUENTIAL STEPS:                     │
        │                                          │      │                                        │
        │  B.1  IF computeWithDamage:              │      │  B.1  Sample system response:          │
        │       └─► ComputeConsequences...()       │      │       └─► systemResponse.Sample()      │
        │           for _FailureStageDamage        │      │                                        │
        │                 │                        │      │  B.2  IF computeWithDamage:            │
        │                 ▼                        │      │       └─► ComputeDamages_WithLevee()   │
        │  B.2  IF computeWithLifeLoss:            │      │           (ConsequenceType.Damage)     │
        │       └─► ComputeConsequences...()       │      │                 │                      │
        │           for _FailureStageLifeLoss      │      │                 ▼                      │
        │                 │                        │      │  B.3  IF computeWithLifeLoss:          │
        │                 ▼                        │      │       └─► ComputeDamages_WithLevee()   │
        │  B.3  ComputePerformance()               │      │           (ConsequenceType.LifeLoss)   │
        │       └─► Calculate AEP for thresholds   │      │                 │                      │
        │                                          │      │                 ▼                      │
        │                                          │      │  B.4  Performance calculation:         │
        │                                          │      │       ├─► IF ≤2 pts: ComputePerf()    │
        │                                          │      │       └─► IF >2 pts: ComputeLeveePerf()│
        │                                          │      │                                        │
        └──────────────────────────────────────────┘      └────────────────────────────────────────┘
```

---

### ComputeDamagesFromStageFrequency_WithLevee() Detail

When a levee exists, this is the detailed flow for consequence computation:

```
ComputeDamagesFromStageFrequency_WithLevee()
════════════════════════════════════════════

FOR EACH failure stage-consequence function:      ◄─── LOOP
│
├─► W.1  Sample stage-damage: stageDamageFailSample = stageUncertainDamage.Sample()
│
├─► W.2  Validate system response: EnsureBottomAndTopHaveCorrectProbabilities()
│
├─► W.3  FAILURE PATH (always runs):
│        ├─► stageDamageFailAdjusted = stageDamageFailSample × systemResponse
│        ├─► stageFreqFail = stageDamageFailAdjusted.compose(frequency_stage)
│        ├─► failEad = stageFreqFail.integrate()
│        └─► Store failEad in results                    ◄─── STORES FAIL-ONLY EAD
│
└─► W.4  NON-FAILURE PATH (only if non-fail functions exist):
         │
         ├─► W.4a  Find matching non-failure function (same category)
         ├─► W.4b  inverseOfSystemResponse = 1 - systemResponse
         ├─► W.4c  stageDamNonFail = stageUncertainNonFailure.Sample()
         ├─► W.4d  stageDamNonFailAdjusted = stageDamNonFail × inverseOfSystemResponse
         ├─► W.4e  TOTAL = stageDamageFailAdjusted + stageDamNonFailAdjusted
         ├─► W.4f  frequency_damage = TOTAL.compose(frequency_stage)
         ├─► W.4g  eadEstimate = frequency_damage.integrate()
         └─► W.4h  Store eadEstimate in results          ◄─── STORES TOTAL EAD
```

---

### Visual Legend

```
═══════════════     Major sequential step (happens after previous ═══ block)
───────────────     Sub-step within a phase
        │
        ▼           Sequential flow (this happens AFTER what's above)

        ┌───┐
        │   │       Parallel or choice block
        └───┘

    ├─►             Sub-step (sequential within its parent)

    BRANCH:         Decision point - pick ONE path

    FOR EACH:       Loop - repeats for each item

    ◄───            Annotation / note
```

---

## Summary of Conditional Paths

| Condition | Consequence Computation | Performance Computation |
|-----------|------------------------|------------------------|
| No Levee, Has Damage | `ComputeConsequencesFromStageFrequency` | `ComputePerformance` |
| No Levee, Has Life Loss | `ComputeConsequencesFromStageFrequency` | `ComputePerformance` |
| Levee (≤2 pts), Has Damage | `ComputeDamagesFromStageFrequency_WithLevee` | `ComputePerformance` |
| Levee (>2 pts), Has Damage | `ComputeDamagesFromStageFrequency_WithLevee` | `ComputeLeveePerformance` |
| Levee, Has Life Loss | `ComputeDamagesFromStageFrequency_WithLevee` | Same as damage |
| Levee, Has NonFail Functions | Adds failure + non-failure weighted by P(fail) | N/A |

---

## Key Mathematical Operations

1. **compose()** - Function composition: Given f(x) and g(x), produces f(g(x))
2. **integrate()** - Computes area under curve (Expected Annual value)
3. **multiply()** - Point-wise multiplication of two curves
4. **SumYsForGivenX()** - Adds Y values at matching X coordinates
5. **f_inverse()** - Inverse interpolation (find X for given Y)

---

## Convergence Criteria

The simulation runs until:
- Minimum iterations completed, AND
- Results converged at 95% confidence with 5% tolerance, OR
- Maximum iterations reached

Convergence is checked after each batch of iterations via `ResultsAreConverged(0.95, 0.05)`.
