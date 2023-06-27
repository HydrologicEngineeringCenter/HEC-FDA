using System;
using System.Collections.Generic;

namespace HEC.FDA.Model.metrics
{
    public enum ThresholdEnum
    {
        [StoredProperty("NotSupported")]
        NotSupported = 0,
        [StoredProperty("DefaultExteriorStage")]
        DefaultExteriorStage = 1,
        [StoredProperty("TopOfLevee")]
        TopOfLevee = 2,
        [StoredProperty("LeveeSystemResponse")]
        LeveeSystemResponse = 3,
        [StoredProperty("AdditionalExteriorStage", AlsoKnownAs = new[] {"InteriorStage"})]
        AdditionalExteriorStage = 4,
    }
    

    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class StoredPropertyAttribute : Attribute
    {
        readonly string _serializedName;
        public string[] AlsoKnownAs { get; set; }
        public StoredPropertyAttribute(string serializedName)
        {
            this._serializedName = serializedName;
        }
        public string SerializedName
        {
            get { return _serializedName; }
        }
    }
}
