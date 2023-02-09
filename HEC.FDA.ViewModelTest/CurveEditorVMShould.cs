using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("Category", "Remote")]
    public class CurveEditorVMShould
    {

        [Fact]
        public void UpdateUPDNameForInflowOutflow()
        {
            //when a curve editor is opened the table with plot gets a default internal name. When the user
            //saves the editor, we want to replace the default name with the user defined name.
            EditorActionManager actionManager = new EditorActionManager();
            CurveComponentVM curveComponentVM = DefaultData.UnregulatedRegulatedComputeComponent();
            InflowOutflowEditorVM vm = new InflowOutflowEditorVM(curveComponentVM, actionManager);
            string testName = "TestName";
            vm.Name = testName;
            //test that the name doesn't equal the test name yet
            string updName = vm.TableWithPlot.CurveComponentVM.SelectedItemToPairedData().CurveMetaData.Name;
            Assert.True(!updName.Equals(testName));
            //this line updates the name
            vm.UpdateCurveMetaData();
            string updName2 = vm.TableWithPlot.CurveComponentVM.SelectedItemToPairedData().CurveMetaData.Name;
            Assert.True(updName2.Equals(testName));
        }
       
    }
}
