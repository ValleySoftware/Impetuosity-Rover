using Impetuosity_Rover.ViewModels.Primary;
using Impetuous.Models;
using Meadow.Foundation.Leds;
using Meadow.Gateways.Bluetooth;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Misc
{
    public class LightsViewModel : ValleyBaseViewModel
    {
        private bool _panTiltlightsOn = false;
        private bool _headlightsOn = false;
        private bool _floodlightsOn = false;
        private Led headLightsLed;
        private Led panTiltLightsLed;
        private Led floodLightsLed;

        public LightsViewModel(string name) : base(name)
        {
            mainViewModel.MasterStatus.LightsStatus = ComponentStatus.Uninitialised;

        }

        public void Init()
        {
            try
            {
                mainViewModel.MasterStatus.LightsStatus = ComponentStatus.Initialising;

                panTiltLightsLed = new Led(_device, _device.Pins.D04);
                headLightsLed = new Led(_device, _device.Pins.D05);
                //floodLightsLed = new Led((IDigitalOutputPort)_device.Pins.D05);

                IsReady = true;
                mainViewModel.MasterStatus.LightsStatus = ComponentStatus.Ready;
            }
            catch
            {
                IsReady = false;
                mainViewModel.MasterStatus.LightsStatus = ComponentStatus.Error;
            }
        }

        public bool SetLight(ref LightMessageModel model)
        {
            var result = false;

            try
            {
                switch (model.Light)
                {
                    case LightSelect.panTilt: PanTiltLightsOn = model.NewState; mainViewModel.MasterStatus.ShowDebugMessage(this, "pantiltlightrequested");  result = true; break;
                    case LightSelect.headlight: HeadlightsOn = model.NewState; result = true; ; break;
                    case LightSelect.flood: FloodlightsOn = model.NewState; result = true; ; break;
                    default:; break;
                }
                mainViewModel.MasterStatus.ShowDebugMessage(this, Convert.ToInt32(model.Light).ToString() + " requested " + model.NewState, ErrorLoggingThreshold.important);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public bool PanTiltLightsOn
        {
            get
            {
                return _panTiltlightsOn;
            }
            private set
            {
                _panTiltlightsOn = value;
                UpdatePanTiltlights();
            }
        }

        public bool HeadlightsOn
        {
            get
            {
                return _headlightsOn;
            }
            private set
            {
                _headlightsOn = value;
                UpdateHeadlights();
            }
        }

        public bool FloodlightsOn
        {
            get
            {
                return _floodlightsOn;
            }
            private set
            {
                _floodlightsOn = value;
                UpdateFloodlights();
            }
        }

        private void UpdatePanTiltlights()
        {
            panTiltLightsLed.IsOn = PanTiltLightsOn;
        }

        private void UpdateHeadlights()
        {
            headLightsLed.IsOn = HeadlightsOn;
            //((DigitalOutputPort)_device.Pins.D04).State = HeadlightsOn;
        }

        private void UpdateFloodlights()
        {
            //floodLightsLed.IsOn = FloodlightsOn;
            //((DigitalOutputPort)_device.Pins.D05).State = FloodlightsOn;
        }
    }
}
