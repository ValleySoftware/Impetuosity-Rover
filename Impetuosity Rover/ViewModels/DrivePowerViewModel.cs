using Meadow.Foundation.Motors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
{
    public class DrivePowerViewModel : ValleyBaseViewModel
    {
        private HBridgeMotor _hBridge;
        bool _stopRequested = false;

        public DrivePowerViewModel(string name) : base(name)
        {

        }

        public bool Init(
            Meadow.Hardware.IPin HBridgePinA, 
            Meadow.Hardware.IPin HBridgePinB, 
            Meadow.Hardware.IPin HBridgePinEnable)
        {
            bool result = false;

            try
            {
                _hBridge = new HBridgeMotor(
                    _device.CreatePwmPort(HBridgePinA),
                    _device.CreatePwmPort(HBridgePinB), 
                    _device.CreateDigitalOutputPort(HBridgePinEnable),
                    0.05f);

                //_hBridge.IsNeutral = true;

                result = true;
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Error: " + ex.Message, true);
                result = false;
            }

            return result;
        }

        public void SetMotorPower(float power)
        {
            _stopRequested = false;
            _hBridge.Power = power;
        }

        public void Stop()
        {
            _stopRequested = true;

            //_hBridge.IsNeutral = true;

            _hBridge.Power = 0.0f;
        }

    }
}
