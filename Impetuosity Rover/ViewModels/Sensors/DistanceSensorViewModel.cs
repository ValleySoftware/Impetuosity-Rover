using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using Meadow.Foundation.Sensors.Distance;

namespace Impetuosity_Rover.ViewModels
{
    public class DistanceSensorViewModel : SensorBaseViewModel
    {
        private Vl53l0x _distanceSensor;
        public double LatestReadingInCM = -1;

        public DistanceSensorViewModel(string name) : base(name)
        {

        }

        public override void Init(ref II2cBus i2CBus)
        {
            base.Init(ref i2CBus);

            _distanceSensor = new Vl53l0x(MeadowApp.Device, i2CBus);
            _distanceSensor.DistanceUpdated += _distanceSensor_DistanceUpdated;
            _distanceSensor.StartUpdating(TimeSpan.FromSeconds(1));

        }

        private void _distanceSensor_DistanceUpdated(object sender, Meadow.IChangeResult<Meadow.Units.Length> e)
        {
            LatestReadingInCM = e.New.Centimeters;
        }
    }
}
