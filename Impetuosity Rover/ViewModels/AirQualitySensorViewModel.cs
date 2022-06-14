using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
{
    public class AirQualitySensorViewModel : SensorBaseViewModel
    {
        public AirQualitySensorViewModel(string name) : base(name)
        {

        }

        public override void Init(ref II2cBus i2CBus)
        {
            base.Init(ref i2CBus);
        }
    }
}
