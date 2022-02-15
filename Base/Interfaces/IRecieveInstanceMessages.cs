namespace HEC.MVVMFramework.Base.Interfaces
{
    interface IRecieveInstanceMessages : IRecieveMessages
    {
        int InstanceHash { get; }
    }
}
