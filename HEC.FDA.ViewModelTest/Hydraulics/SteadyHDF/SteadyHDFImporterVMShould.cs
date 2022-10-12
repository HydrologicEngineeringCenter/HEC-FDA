using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.SteadyHDF;
using Xunit;

namespace HEC.FDA.ViewModelTest.Hydraulics.SteadyHDF;

public class SteadyHDFImporterVMShould
{
    private const string PathToSteadyResult = @"..\..\fda-model-test\Resources\MuncieSteadyResult\Muncie.p09.hdf";

    [Fact]
    public void PopulateRowsCorrectly()
    {
        SteadyHDFImporterVM vm = new SteadyHDFImporterVM(new EditorActionManager());
        vm.SelectedPath=PathToSteadyResult;
        Assert.Equal(8, vm.ListOfRows.Count);
    }

    [Fact]
    public void 
}
