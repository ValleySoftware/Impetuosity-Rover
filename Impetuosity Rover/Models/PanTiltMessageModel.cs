using System;
using System.Collections.Generic;
using System.Text;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.Models
{
    public class PanTiltMessageModel : MessageBaseModel
    {
        public SteeringRequestType RequestType { get; set; }
        public double SteeringValue { get; set; }
    }