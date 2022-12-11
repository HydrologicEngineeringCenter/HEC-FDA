using HEC.FDA.Model.metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("Category", "Unit")]

    public class ProcessedConsequenceResultsShould
    {
        private static int impactAreaID = 87;



        [Theory]
        [InlineData("RES", "COM", 100,100)]
        public void ProcessedConsequenceResultsListShould(string resDamCat, string comDamCat, double structureDamage, double contentDamage)
        {
            ConsequenceResult resConsequenceResultONE = new ConsequenceResult(resDamCat, impactAreaID);
            resConsequenceResultONE.IncrementConsequence(structureDamage, contentDamage*2);
            ConsequenceResult commercialConsequenceResultONE = new ConsequenceResult(comDamCat, impactAreaID);
            commercialConsequenceResultONE.IncrementConsequence(structureDamage * 3, contentDamage * 4);
            ConsequenceResults consequenceResultsONE = new ConsequenceResults();
            consequenceResultsONE.AddExistingConsequenceResultObject(resConsequenceResultONE);
            consequenceResultsONE.AddExistingConsequenceResultObject(commercialConsequenceResultONE);

            ConsequenceResult resConsequenceResultTWO = new ConsequenceResult(resDamCat, impactAreaID);
            resConsequenceResultTWO.IncrementConsequence(structureDamage*5, contentDamage*6);
            ConsequenceResult commercialConsequenceResultTWO = new ConsequenceResult(comDamCat, impactAreaID);
            commercialConsequenceResultTWO.IncrementConsequence(structureDamage * 7, contentDamage * 8);
            ConsequenceResults consequenceResultsTWO = new ConsequenceResults();
            consequenceResultsTWO.AddExistingConsequenceResultObject(resConsequenceResultTWO);
            consequenceResultsTWO.AddExistingConsequenceResultObject(commercialConsequenceResultTWO);

            List<ConsequenceResults> consequenceResultsList = new List<ConsequenceResults>();
            consequenceResultsList.Add(consequenceResultsONE);
            consequenceResultsList.Add(consequenceResultsTWO);

            ProcessedConsequenceResultsList processedConsequenceResultsList = new ProcessedConsequenceResultsList(consequenceResultsList);

            int expectedDamageRealizationsLength = 2;
            int expectedResultsCount = 8;

            Assert.Equal(expectedResultsCount, processedConsequenceResultsList.Results.Count);
            foreach (ProcessedConsequenceResults processedConsequenceResults in processedConsequenceResultsList.Results)
            {
                if (processedConsequenceResults.DamageCategory.Equals(resDamCat))
                {
                    if (processedConsequenceResults.AssetCategory.Equals("Structure"))
                    {
                        Assert.Equal(expectedDamageRealizationsLength, processedConsequenceResults.DamageRealizations.Count);
                        Assert.Contains(structureDamage, processedConsequenceResults.DamageRealizations);
                        Assert.Contains(structureDamage * 5, processedConsequenceResults.DamageRealizations);
                    } 
                    else if (processedConsequenceResults.AssetCategory.Equals("Content"))
                    {
                        Assert.Equal(expectedDamageRealizationsLength, processedConsequenceResults.DamageRealizations.Count);
                        Assert.Contains(contentDamage * 2, processedConsequenceResults.DamageRealizations);
                        Assert.Contains(contentDamage * 6, processedConsequenceResults.DamageRealizations);
                    }
                }
                else if (processedConsequenceResults.DamageCategory.Equals(comDamCat))
                {
                    if (processedConsequenceResults.AssetCategory.Equals("Structure"))
                    {
                        Assert.Equal(expectedDamageRealizationsLength, processedConsequenceResults.DamageRealizations.Count);
                        Assert.Contains(structureDamage * 3, processedConsequenceResults.DamageRealizations);
                        Assert.Contains(structureDamage * 7, processedConsequenceResults.DamageRealizations);
                    }
                    else if (processedConsequenceResults.AssetCategory.Equals("Content"))
                    {
                        Assert.Equal(expectedDamageRealizationsLength, processedConsequenceResults.DamageRealizations.Count);
                        Assert.Contains(contentDamage * 4, processedConsequenceResults.DamageRealizations);
                        Assert.Contains(contentDamage * 8, processedConsequenceResults.DamageRealizations);
                    }
                }
                else
                {
                    //hack to make test fail
                    //if we got here, then something went wrong
                    bool falseBoolean = false;
                    Assert.True(falseBoolean);
                }
            }
        }
    }
}
