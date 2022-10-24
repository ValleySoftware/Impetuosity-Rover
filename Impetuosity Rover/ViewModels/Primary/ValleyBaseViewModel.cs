using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Leds;
using System;
using System.Collections.Generic;
using System.Text;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Primary
{
    public class ValleyBaseViewModel
    {
        public ErrorLoggingThreshold debugThreshhold = ErrorLoggingThreshold.important;

        private bool _isReady = false;

        protected MeadowApp _appRoot => MeadowApp.Current;
        public F7FeatherV2 _device => MeadowApp.Device;

        protected string _name;

        public ValleyBaseViewModel(string name)
        {
            _name = name;
        }

        public string Name
        {
            get => _name;
        }

        public bool IsReady
        {
            get => _isReady;
            set => _isReady = value;
        }

        public MainViewModel mainViewModel => MeadowApp.Current.mainViewModel;
    }
}
