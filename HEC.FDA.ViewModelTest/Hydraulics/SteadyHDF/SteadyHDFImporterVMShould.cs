﻿using System;
using System.Diagnostics;
using System.IO;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.SteadyHDF;
using Xunit;

namespace HEC.FDA.ViewModelTest.Hydraulics.SteadyHDF;
[Trait("RunsOn", "Remote")]
[Collection("Serial")]
public class SteadyHDFImporterVMShould
{
    private const string PathToSteadyResult = @"..\..\..\HEC.FDA.ModelTest\Resources\MuncieSteadyResult\Muncie.p10.hdf";

    [Fact]
    public void PopulateRowsCorrectly()
    {
        string fullPath = Path.GetFullPath(PathToSteadyResult);
        SteadyHDFImporterVM vm = new SteadyHDFImporterVM(new EditorActionManager());
        vm.SelectedPath=PathToSteadyResult;
        Assert.Equal(8, vm.ListOfRows.Count);
    }
}
