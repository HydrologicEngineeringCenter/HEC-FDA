using ImpactArea.Elevations;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace ImpactArea.Validation
{
    class AssetElevationValidator
    {
        public bool IsValid(AssetElevation obj, IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max() < IMessageLevels.Error;
        }
        public IEnumerable<IMessage> ReportErrors(AssetElevation obj)
        {
            List<IMessage> messages = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentException("The composite elevation can not be validated because it is null.");
            if (!obj.Ground.Messages.IsNullOrEmpty()) messages.AddRange(obj.Ground.Messages);
            if (!obj.Height.Messages.IsNullOrEmpty()) messages.AddRange(obj.Height.Messages);
            if (!obj.Composite.Messages.IsNullOrEmpty()) messages.AddRange(obj.Composite.Messages);
            return messages;            
        }
        public bool IsConstructable(IElevation[] elements, out string msg)
        {
            msg = "";
            if (elements.IsNullOrEmpty()) throw new ArgumentNullException("The composite elevation cannot be constructed because one or more of its elevations are null.");
            if (elements.Length != 2 || !ContainsExpectedElements(elements)) msg += $"The composite elevation can not be constructed because it does not contain the required: 1 {ElevationEnum.Ground} and 1 {ElevationEnum.Height} elevation elements.";
            return msg.Length == 0;
        }
        private bool ContainsExpectedElements(IElevation[] elements)
        {
            bool ground = false, height = false;
            foreach (var ele in elements)
            {
                if (ele.Type == ElevationEnum.Ground) ground = true;
                if (ele.Type == ElevationEnum.Height) height = true;
            }
            return ground && height;
        }
    }
}
