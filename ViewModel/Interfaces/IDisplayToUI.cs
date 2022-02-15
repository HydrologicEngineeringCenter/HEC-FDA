using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.MVVMFramework.ViewModel.Interfaces
{
    public interface IDisplayToUI : INamed
    {
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }
    }
}
