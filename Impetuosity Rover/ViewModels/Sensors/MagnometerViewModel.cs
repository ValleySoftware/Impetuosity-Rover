using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using Meadow.Foundation.Sensors.Motion;

namespace Impetuosity_Rover.ViewModels
{
    public class MagnometerViewModel : SensorBaseViewModel
    {
        Hmc5883 _sensor;

        public MagnometerViewModel(string name) : base(name)
        {

        }

        public override void Init(ref II2cBus i2CBus)
        {
            base.Init(ref i2CBus);

            _sensor = new Hmc5883(i2CBus);
            _sensor.Updated += _sensor_Updated;
            _sensor.StartUpdating(TimeSpan.FromMilliseconds(250));
        }

        private void _sensor_Updated(
            object sender, 
            Meadow.IChangeResult<Meadow.Foundation.Spatial.Vector> e)
        {
            throw new NotImplementedException();
        }
    }
}
