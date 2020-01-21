using System;
using System.Text;
using System.Collections.Generic;
using Model.Inputs.Functions;

namespace Model.Inputs.Functions.ComputationPoint
{
    internal sealed class ComputationPointFunctionRegistry: IFunctionRegistry
    {
        #region Notes
        //Needs: Reading, Writing, Unit Tests, Add Multiple items (implment IColleciton or IList?)
        //1. This may help most if I don't use Entity.
        /* Entity Notes
        1. public int Id { get; private set; }
        2. public sealed class DecoratorList: Database.IEntityData
        3. A badly written Write method for testing...
        
        public void Write()
        {
            string dbPath = String.Format("Data Source={0};Version=3;", Project.Instance.GetFilePathWithoutExtension() + ".sqlite");
            var sqLiteConnection = new System.Data.SQLite.SQLiteConnection(dbPath);
            using (var context = new DataBase.DataContext(sqLiteConnection, false))
            {
                context.Set<DecoratorList>().Add(this);
                context.SaveChanges();
            }
        }
        */
        #endregion

        #region Fields and Properties
        private int NameCounter;
        public static ComputationPointFunctionRegistry Instance { get; private set; }
        public IReadOnlyCollection<string> NamedFunctions
        {
            get
            {
                return GetNames();
            }
        }
        public IList<Tuple<string, ComputationPointFunctionBase>> CompleteList { get; }
        public IList<Tuple<string, ITransformFunction>> TransformFunctions
        {
            get
            {
                return GetTransformFunctions();
            }
        }
        public IList<Tuple<string, IFunctionCompose>> FrequencyFunctions
        {
            get
            {
                return GetFrequencyFunctions();
            }
        }
        public IList<Tuple<string, ComputationPointFunctionBase>> UnUsedFunctions
        {
            get
            {
                return GetUnUsableFunctions();
            }
        }
        #endregion

        #region Constructors
        private ComputationPointFunctionRegistry()
        {
            NameCounter = 0;
            CompleteList = new List<Tuple<string, ComputationPointFunctionBase>>();
        }
        #endregion

        #region Methods
        internal static ComputationPointFunctionRegistry CreateNew()
        {
            if (Instance == null) Instance = new ComputationPointFunctionRegistry();
            return Instance;
        }
        internal static void AddToRegistry(string name, ComputationPointFunctionBase function)
        {
            if (Instance == null) Instance = CreateNew();
            Instance.CompleteList.Add(new Tuple<string, ComputationPointFunctionBase>(Instance.GetValidName(name), function));
        }
        internal static void AddToRegistry(ComputationPointFunctionBase function)
        {
            if (Instance == null) Instance = CreateNew();
            Instance.CompleteList.Add(new Tuple<string, ComputationPointFunctionBase>(Instance.CreateName(function), function));
        }        
        private string CreateName(ComputationPointFunctionBase function)
        {
            Instance.NameCounter++;
            return new StringBuilder(function.GetType().ToString()).Append(Instance.NameCounter).ToString();
        }
        private string GetValidName(string name)
        {
            foreach (var item in CompleteList)
            {
                if (name == item.Item1)
                {
                    Instance.NameCounter++;
                    return new StringBuilder(name).Append(Instance.NameCounter).ToString(); 
                }
            }
            return name;
        }
        private List<Tuple<string, ITransformFunction>> GetTransformFunctions()
        {
            List<Tuple<string, ITransformFunction>> transformList = new List<Tuple<string, ITransformFunction>>();
            foreach (var implementation in CompleteList)
            {
                if (implementation.Item2.Type != ComputationPointFunctionEnum.NotSet &&
                   (int)implementation.Item2.Type % 2 == 0)
                {
                    transformList.Add(new Tuple<string, ITransformFunction>(implementation.Item1, (ITransformFunction)implementation.Item2));
                }
            }
            return transformList;
        }
        private List<Tuple<string, IFunctionCompose>> GetFrequencyFunctions()
        {
            List<Tuple<string, IFunctionCompose>> frequencyList = new List<Tuple<string, IFunctionCompose>>();
            foreach (var implementation in CompleteList)
            {
                if (!((int)implementation.Item2.Type % 2 == 0)) frequencyList.Add(new Tuple<string, IFunctionCompose>(implementation.Item1, (IFunctionCompose)implementation.Item2));
            }
            return frequencyList;
        }
        private List<Tuple<string, ComputationPointFunctionBase>> GetUnUsableFunctions()
        {
            List<Tuple<string, ComputationPointFunctionBase>> unUsableList = new List<Tuple<string, ComputationPointFunctionBase>>();
            foreach (var implementation in CompleteList)
            {
                if (implementation.Item2.Type == ComputationPointFunctionEnum.NotSet) unUsableList.Add(implementation);
            }
            return unUsableList;
        }
        private IReadOnlyCollection<string> GetNames()
        {
            List<string> names = new List<string>();
            foreach (var implementation in CompleteList)
            {
                names.Add(implementation.Item1);
            }
            return names;
        }
        #endregion

    }
}
