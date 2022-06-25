using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using Meadow.Foundation.Sensors.Motion;

namespace Impetuosity_Rover.ViewModels
{
    public class MagnometerViewModel : SensorBaseViewModel
    {
        private Hmc5883 _sensor;
        private Meadow.Foundation.Spatial.Vector _vector;

        public MagnometerViewModel(string name) : base(name)
        {

        }

        public override void Init(ref II2cBus i2CBus)
        {
            base.Init(ref i2CBus);

            _sensor = new Hmc5883(i2CBus);
            _sensor.Updated += _sensor_Updated;
            _sensor.StartUpdating(TimeSpan.FromMilliseconds(250));

            IsReady = true;
        }

        private void _sensor_Updated(
            object sender, 
            Meadow.IChangeResult<Meadow.Foundation.Spatial.Vector> e)
        {
            if (!IsReady)
            {
                return;
            }

            _vector = e.New;
        }

        public Meadow.Foundation.Spatial.Vector CurrentVector
        {
            get => _vector;
        }
    }
}
