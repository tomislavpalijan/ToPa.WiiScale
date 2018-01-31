using System;
using WiiScale.Logic.UI.BaseClasses;

namespace WiiScale.Logic.UI.Model
{
    public class Weight : BaseModel
    {
        public float Value { get; set; }
        public DateTime MeasureTime { get; set; }
        public bool IsManualAdded { get; set; }
    }
}