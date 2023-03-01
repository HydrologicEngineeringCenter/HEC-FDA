using System;
using System.Collections.Generic;
using System.Text;

namespace HEC.MVVMFramework.Model.Messaging
{
    public interface IContainValidationGroups
    {
        List<ValidationGroup> ValidationGroups { get; }
    }
}