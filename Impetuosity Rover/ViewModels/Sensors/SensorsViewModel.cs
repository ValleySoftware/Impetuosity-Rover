using Impetuosity_Rover.ViewModels.Primary;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels
{
    public class SensorsViewModel : ValleyBaseViewModel
    {
        private List<SensorBaseViewModel> _items;
        private II2cBus _i2CBus;
        private MagnometerViewModel _compass;
        private DistanceSensorViewModel _panTiltDistance;
        private AirQualitySensorViewModel _airQual;

        public SensorsViewModel(string name) : base(name)
        {
            mainViewModel.MasterStatus.SensorsStatus = ComponentStatus.Uninitialised;
        }

        public void Init(ref II2cBus i2CBus)
        {
            mainViewModel.MasterStatus.SensorsStatus = ComponentStatus.Initialising;

            _items = new List<SensorBaseViewModel>();

            _i2CBus = i2CBus;

            //_compass = new MagnometerViewModel("Compass");
            //_compass.Init(ref i2CBus);
            //_items.Add(_compass);

            //_panTiltDistance = new DistanceSensorViewModel("Pan Tilt Distance");
            //_panTiltDistance.Init(ref i2CBus);
            //_items.Add(_panTiltDistance);

            //_airQual = new AirQualitySensorViewModel("Air Quality Sensor");
            //_airQual.Init(ref i2CBus);
            //_items.Add(_airQual);

            IsReady = true;
            mainViewModel.MasterStatus.SensorsStatus = ComponentStatus.Ready;
        }

        public MagnometerViewModel Compass
        {
            get => _compass;
        }

        public DistanceSensorViewModel PanTiltDistance
        {
            get => _panTiltDistance;
        }

        public AirQualitySensorViewModel AirQual
        {
            get => _airQual;
        }
    }
}
