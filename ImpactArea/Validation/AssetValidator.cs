using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace ImpactArea.Validation
{
    internal class AssetValidator: IValidator<IAsset>
    {
        public bool IsValid(IAsset obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max() < IMessageLevels.Error;
        }
        public IEnumerable<IMessage> ReportErrors(IAsset obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            return msgs;
        }
        
    }
}
