using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreasOwnerElement : ParentElement
    {

        public ImpactAreasOwnerElement() : base()
        {
            Name = StringConstants.IMPACT_AREAS;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
        }

        public void AddBaseElements(Study.FDACache cache)
        {
            ImpactAreaOwnerElement impactAreaOwnerElem = new ImpactAreaOwnerElement();
            AddElement(impactAreaOwnerElem);
            IndexPointsOwnerElement indexPointsOwnerElem = new IndexPointsOwnerElement();
            AddElement(indexPointsOwnerElem);
            cache.ImpactAreaParent = impactAreaOwnerElem;
        }

    }
}
