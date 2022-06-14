using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
{
    public class SensorBaseViewModel : ValleyBaseViewModel
    {
        private II2cBus _i2CBus;

        public SensorBaseViewModel(string name) : base(name)
        {

        }

        public virtual void Init(ref II2cBus i2CBus)
        {
            _i2CBus = i2CBus;
        }
    }
}
