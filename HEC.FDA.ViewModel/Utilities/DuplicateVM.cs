using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Utilities;

public partial class DuplicateVM : BaseEditorVM
{
    private int _selectedNumDuplicates = 1;
    private const int MAX_NUM_DUPLICATES = 10; // the max nunmber of duplicates we allow the user to create at once
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
        // _clonedElement is a direct clone of the object which the user clicked "Duplicate" on
        // we have to update its name and ID to make it distinct from its original element
        string baseName = _clonedElement.Name.TrimEnd();

        List<ChildElement> siblings = StudyCache.GetChildElementsOfType(_clonedElement.GetType());
        string duplicateName = GetDuplicateName(siblings, baseName, out int lastIndex);

        _clonedElement.Name = duplicateName;
        _clonedElement.UpdateTreeViewHeader(duplicateName);

        IElementManager savingManager = PersistenceFactory.GetElementManager(_clonedElement);
        _clonedElement.ID = savingManager.GetNextAvailableId();
        savingManager.SaveNew(_clonedElement);

        for (int i = lastIndex; i < lastIndex + SelectedNumDuplicates - 1; i++)
        {
            // create a NEW clone of the original element for each duplicate so that it has its own associated ChildElement instance
            // as with the first duplicate, each of these will also get a unique name and ID
            ChildElement nextDup = _clonedElement.CloneElement();
            string name = $"{baseName} ({i})";
            nextDup.Name = name;
            nextDup.UpdateTreeViewHeader(name);
            nextDup.ID = savingManager.GetNextAvailableId();
            savingManager.SaveNew(nextDup);
        }
    }

    private static string GetDuplicateName(List<ChildElement> siblings, string baseName, out int lastIndex)
    {
        int i = 1;
        string candidate;
        // keep incrementing the number in the name until the first available (i) is found
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
