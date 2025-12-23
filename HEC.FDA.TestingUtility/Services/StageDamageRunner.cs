using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.TestingUtility.Services;

public class StageDamageRunner
{
    public AggregatedStageDamageElement GetStageDamageElement(string elementName)
    {
        Console.WriteLine($"    Retrieving stage damage element '{elementName}'...");

        // Find element by name
        AggregatedStageDamageElement element = FindElement<AggregatedStageDamageElement>(elementName);

        Console.WriteLine($"    Found stage damage element with {element.Curves.Count} curves.");

        return element;
    }

    private static T FindElement<T>(string elementName) where T : ChildElement
    {
        var elements = BaseViewModel.StudyCache.GetChildElementsOfType<T>();

        var match = elements.FirstOrDefault(e =>
            e.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase));

        if (match == null)
        {
            string availableNames = string.Join(", ", elements.Select(e => e.Name));
            throw new InvalidOperationException(
                $"Element '{elementName}' of type {typeof(T).Name} not found. Available: {availableNames}");
        }

        return match;
    }
}
