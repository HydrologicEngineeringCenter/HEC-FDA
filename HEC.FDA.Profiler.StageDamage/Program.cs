var testClass = new HEC.FDA.ModelTest.unittests.PerformanceTest();
for (int i = 0; i < 10000; i++)
{
testClass.ConvergenceTest();

}


//TODO: THIS TEST NO LONGER EXISTS. A REPLACEMENT MUST BE WRITTEN.
//testClass.ComputeDamageOneCoordinateShouldComputeCorrectly(560, 504, 690, 621);
    