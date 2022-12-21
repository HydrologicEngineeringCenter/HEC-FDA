using HEC.FDA.Model.compute;
using HEC.FDA.ModelTest.integrationtests;
var stageDamageTestClass = new TractableStageDamageTests();
stageDamageTestClass.impactAreaStageDamage.Compute(new MedianRandomProvider());
Console.WriteLine("compute complete");

