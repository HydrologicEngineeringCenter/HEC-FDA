using System;

namespace HEC.MVVMFramework.ViewModel.Enumerations
{
    [Flags]
    public enum NavigationOptionsEnum : byte
    {
        Unassigned = 0x00,

        Scalable = 0x01,
        AsNewWindow = 0x02,
        AsDialog = 0x04,
        AsWizard = 0x08,

        NewScalableDialog = Scalable | AsNewWindow | AsDialog,
        NewScalableWizardDialog = Scalable | AsNewWindow | AsDialog | AsWizard,
        ScalableWizard = Scalable | AsWizard,
        NewScalable = Scalable | AsNewWindow,
    }
}
