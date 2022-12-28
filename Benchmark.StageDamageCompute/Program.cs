using HEC.FDA.Model.compute;
using HEC.FDA.ModelTest.integrationtests;
using HEC.FDA.ModelTest.unittests;
using Statistics;

//Testing TractableStageDamage
//var stageDamageTestClass = new TractableStageDamageTests();
//stageDamageTestClass.impactAreaStageDamage.Compute(new MedianRandomProvider());
//Console.WriteLine("compute complete");

var testClass = new StageDamageShould();
for( int i = 0; i < 5; i++)
{
    testClass.ComputeDamageOneCoordinateShouldComputeCorrectly(560, 504, 690, 621);
}
