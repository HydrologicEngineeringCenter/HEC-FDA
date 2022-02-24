namespace HEC.FDA.ViewModel.Editors
{
    public class EditorActionManager
    {
        public bool HasSiblingRules { get; set; }

        //this can be a parent element or a sibling
        public BaseFdaElement SiblingElement { get; set; }

        public EditorActionManager()
        {         
        }

        public EditorActionManager WithSiblingRules(BaseFdaElement element)
        {
            HasSiblingRules = true;
            SiblingElement = element;
            return this;
        }

    }
}
