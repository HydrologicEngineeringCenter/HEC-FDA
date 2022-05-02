using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Saving
{
    public interface IElementManager
    {    
        void SaveNew(ChildElement element);
        void Remove(ChildElement element);
        void SaveExisting( ChildElement elementToSave);
        void Load();
    }
}
