using System;
using System.Collections.Generic;
using System.Text;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuous.Models
{
    public class SteeringMessageModel : MessageBaseModel
    {
        public SteeringRequestType RequestType { get; set; }
        public double SteeringValue { get; set; }
    }
}
