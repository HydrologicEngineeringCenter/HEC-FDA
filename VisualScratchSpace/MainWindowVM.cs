using VisualScratchSpace.ViewModel;

namespace VisualScratchSpace
{
    public class MainWindowVM
    {
        public RunButtonVM RunButtonViewModel { get; } = new RunButtonVM();
        public LifeLossImporterVM LifeLossImporterViewModel { get; } = new LifeLossImporterVM();
    }
}
