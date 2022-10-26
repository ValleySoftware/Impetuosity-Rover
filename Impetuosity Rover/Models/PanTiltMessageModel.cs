using Impetuous.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuous.Models
{
    public class PanTiltMessageModel : MessageBaseModel
    {
        public SteeringRequestType RequestType { get; set; }
        public double RequestValue { get; set; }
        public PanTiltSelect PanOrTilt { get; set; }
    }
}