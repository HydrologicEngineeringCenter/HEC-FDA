using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Utilities;

public partial class DuplicateVM : BaseEditorVM
{
    private int _selectedNumDuplicates = 1;
    private const int MAX_NUM_DUPLICATES = 10;
    private readonly ChildElement _clonedElement;

    public int SelectedNumDuplicates
    {
        get { return _selectedNumDuplicates; }
        set { _selectedNumDuplicates = value; NotifyPropertyChanged(); }
    }

    public int[] NumberOfDuplicateOptions
    {
        get
        {
            int[] result = new int[MAX_NUM_DUPLICATES];
            for (int i = 0; i < MAX_NUM_DUPLICATES; i++)
                result[i] = i + 1;
            return result;
        }
    }

    public DuplicateVM(ChildElement clonedElement) : base(clonedElement, null)
    {
        _clonedElement = clonedElement;
    }

    public override void Save()
    {
        string originalName = _clonedElement.Name;

        List<ChildElement> siblings = StudyCache.GetChildElementsOfType(_clonedElement.GetType());
        string duplicateName = GetDuplicateName(siblings, originalName, out int lastIndex);

        _clonedElement.Name = duplicateName;
        _clonedElement.UpdateTreeViewHeader(duplicateName);

        IElementManager savingManager = PersistenceFactory.GetElementManager(_clonedElement);
        _clonedElement.ID = savingManager.GetNextAvailableId();
        savingManager.SaveNew(_clonedElement);

        for (int i = lastIndex; i < lastIndex + SelectedNumDuplicates - 1; i++)
        {
            string name = $"{originalName} ({i})";
            ChildElement nextDup = _clonedElement.CloneElement();
            nextDup.Name = name;
            nextDup.UpdateTreeViewHeader(name);
            nextDup.ID = savingManager.GetNextAvailableId();
            savingManager.SaveNew(nextDup);
        }
    }

    private static string GetDuplicateName(List<ChildElement> siblings, string originalName, out int lastIndex)
    {
        string baseName = originalName.TrimEnd();
        int i = 1;
        string candidate;
        do
        {
            candidate = $"{baseName} ({i})";
            i++;
        } while (SiblingNameExists(candidate, siblings));
        lastIndex = i;
        return candidate;
    }

    private static bool SiblingNameExists(string name, List<ChildElement> siblings)
    {
        foreach (var sibling in siblings)
        {
            if (sibling.Name == name)
            {
                return true;
            }
        }
        return false;
    }
}
