using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
{
    public class SensorsViewModel : ValleyBaseViewModel
    {
        private List<SensorBaseViewModel> _items;
        private II2cBus _i2CBus;

        public SensorsViewModel(string name) : base(name)
        {
            _items = new List<SensorBaseViewModel>();
        }

        public void Init(ref II2cBus i2CBus)
        {
            _i2CBus = i2CBus;
        }
    }
}
