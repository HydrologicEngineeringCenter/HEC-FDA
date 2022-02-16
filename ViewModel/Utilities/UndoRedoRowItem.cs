namespace HEC.FDA.ViewModel.Utilities
{
    public class UndoRedoRowItem
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public UndoRedoRowItem(string name, string date)
        {
            Name = name + " " + date;
        }
    }
}
