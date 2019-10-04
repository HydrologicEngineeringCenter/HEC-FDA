using System;

namespace FdaModel.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = true)]
    [Author("William Lehman", "06/08/2016", "06/10/2016")]
    public class Author : Attribute
    {
        #region Fields
        private string _Name;
        private string _DateCreated;
        private string _DateLastEdited;
        #endregion

        #region Constructor
        /// <summary>
        /// This creates a class, struct or enum level attribute that documents who created the class, struct or enum and when it was created.
        /// </summary>
        /// <param name="createdBy"> Who created the class, struct or enum. </param>
        /// <param name="createdOn"> When the class, struct or enum was created. </param>
        public Author(string createdBy, string createdOn)
        {
            _Name = createdBy;
            _DateCreated = createdOn;
            _DateLastEdited = createdOn;
        }
        /// <summary>
        /// This creates a class, struct or enum level attribute that documents who created the class, struct or enum, when it was created and the date of its last significant edit.
        /// </summary>
        /// <param name="createdBy"> Who created the class, struct or enum. </param>
        /// <param name="createdOn"> When the class, struct or enum was created. </param>
        /// <param name="editedOn"> Latest date of any edits that may influence the validity of the class, struct or enum unit tests. </param>
        public Author(string createdBy, string createdOn, string editedOn)
        {
            _Name = createdBy;
            _DateCreated = createdOn;
            _DateLastEdited = editedOn;
        }
        #endregion
    }
}

