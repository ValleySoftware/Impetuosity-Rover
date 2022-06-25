using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using Meadow.Foundation.Sensors.Atmospheric;

namespace Impetuosity_Rover.ViewModels
{
    public class AirQualitySensorViewModel : SensorBaseViewModel
    {
        private Ccs811 _sensor;
        private Meadow.Units.Concentration? _Co2;
        private Meadow.Units.Concentration? _Voc;

        public AirQualitySensorViewModel(string name) : base(name)
        {

        }

        public override void Init(ref II2cBus i2CBus)
        {
            base.Init(ref i2CBus);

            _sensor = new Ccs811(i2CBus);
            _sensor.Updated += _sensor_Updated; 
            _sensor.StartUpdating(TimeSpan.FromSeconds(5));

            IsReady = true;
        }

        private void _sensor_Updated(
            object sender, 
            Meadow.IChangeResult<(
                Meadow.Units.Concentration? Co2, 
                Meadow.Units.Concentration? Voc)> e)
        {
            if (!IsReady)
            {
                return;
            }

            _Co2 = e.New.Co2;
            _Voc = e.New.Voc;
        }

        public Meadow.Units.Concentration? CurrentCo2
        {
            get => _Co2;
        }

        public Meadow.Units.Concentration? CurrentVoc
        {
            get => _Voc;
        }
    }
}
