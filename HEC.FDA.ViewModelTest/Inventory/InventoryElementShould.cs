using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.Model.structures;
using Statistics.Distributions;

namespace HEC.FDA.ViewModelTest.Inventory;
[Trait("RunsOn", "Remote")]
public class InventoryElementShould
{
    [Fact]
    public void CreateModelOccTypesThatMatchVMOccTypes()
    {
        ValueUncertainty valueUncertainty = new(Statistics.IDistributionEnum.Triangular, 50, 100);
        //Structures
        OccTypeAsset structures = new(OccTypeAsset.OcctypeAssetType.structure, true, new ViewModel.TableWithPlot.CurveComponentVM(isDepthPercentDamage: true), new Triangular(50,100,200));

        //Content
        OccTypeAssetWithRatio contents = new(OccTypeAsset.OcctypeAssetType.content, true, new ViewModel.TableWithPlot.CurveComponentVM(isDepthPercentDamage: true), new Triangular(50, 100, 200), new Triangular(.5,1,1.5),true);

        //Vehicle
        OccTypeAsset vehicles = new(OccTypeAsset.OcctypeAssetType.structure, true, new ViewModel.TableWithPlot.CurveComponentVM(isDepthPercentDamage: true), new Triangular(50, 100, 200));

        //Other
        OccTypeAssetWithRatio others = new(OccTypeAsset.OcctypeAssetType.content, true, new ViewModel.TableWithPlot.CurveComponentVM(isDepthPercentDamage: true), new Triangular(50, 100, 200), new Triangular(.5, 1, 1.5), true);

        //Foundation
        ViewModel.Inventory.OccupancyTypes.OccupancyType occupancyType = new("name","desc",0,"damCat",structures,contents,vehicles,others, new Triangular(0,1,10),0);

        var modelOccupancyType = InventoryElement.CreateModelOcctypeFromVMOcctype(occupancyType);

        //Assert that user options are recorded
        Assert.Equal(occupancyType.VehicleItem.IsChecked, modelOccupancyType.ComputeVehicleDamage);
        Assert.Equal(occupancyType.ContentItem.IsChecked, modelOccupancyType.ComputeContentDamage);
        Assert.Equal(occupancyType.ContentItem.IsByValue, !modelOccupancyType.UseContentToStructureValueRatio);
        Assert.Equal(occupancyType.OtherItem.IsChecked, modelOccupancyType.ComputeContentDamage);
        Assert.Equal(occupancyType.OtherItem.IsByValue, !modelOccupancyType.UseOtherToStructureValueRatio);
    }
}
