using FunctionsView.ViewModel;
using paireddata;
using System;
using System.Collections.Generic;
using Utilities;

namespace FunctionsView.Validation
{
    class CoordinatesFunctionEditorVMValidator : IValidator<CoordinatesFunctionEditorVM>
    {
        public CoordinatesFunctionEditorVMValidator()
        {

        }
        public bool IsValid(CoordinatesFunctionEditorVM entity, out IEnumerable<IMessage> errors)
        {
            errors = ReportErrors(entity);
            bool isValid = true;
            foreach(IMessage msg in errors)
            {
                if( msg.Level == IMessageLevels.FatalError)
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        public IEnumerable<IMessage> ReportErrors(CoordinatesFunctionEditorVM entity)
        {
            List<IMessage> errors = new List<IMessage>();
            try
            {
                //todo: Cody to Cody, is this the best way to validate the function?
                UncertainPairedData func = entity.CreateFunctionFromTables();
                //if(!func.IsValid)
                //{
                //    errors.AddRange(func.Messages);
                //}
            }
            catch(Exception ex)
            {
                errors.Add(IMessageFactory.Factory(IMessageLevels.FatalError, ex.Message));
            }
            return errors;
        }

        IMessageLevels IValidator<CoordinatesFunctionEditorVM>.IsValid(CoordinatesFunctionEditorVM entity, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }
    }
}
