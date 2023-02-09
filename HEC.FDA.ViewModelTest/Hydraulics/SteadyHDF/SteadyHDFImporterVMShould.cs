using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.SteadyHDF;
using Xunit;

namespace HEC.FDA.ViewModelTest.Hydraulics.SteadyHDF;
[Trait("Category", "Local")]
public class SteadyHDFImporterVMShould
{
    private const string PathToSteadyResult = @"..\..\fda-model-test\Resources\MuncieSteadyResult\Muncie.p10.hdf";

    [Fact]
    public void PopulateRowsCorrectly()
    {
        SteadyHDFImporterVM vm = new SteadyHDFImporterVM(new EditorActionManager());
        vm.SelectedPath=PathToSteadyResult;
        Assert.Equal(8, vm.ListOfRows.Count);
    }
}
