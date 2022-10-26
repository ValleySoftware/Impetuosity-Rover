using System;
using System.Collections.Generic;
using System.Text;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuous.Models
{
    public class MovementMessageModel : MessageBaseModel
    {
        public TimeSpan RequestedPowerDuration { get; set; }
        public float LeftPower { get; set; }
        public float RightPower { get; set; }
        public bool RequestStop { get; set; }

    }
}
