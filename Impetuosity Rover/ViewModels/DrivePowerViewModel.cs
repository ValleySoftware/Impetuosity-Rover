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

            try
            {
                _hBridge = new HBridgeMotor(
                    device : _device,
                    a1Pin :HBridgePinA,
                    a2Pin : HBridgePinB, 
                    enablePin : HBridgePinEnable);
                //a1Port :_device.CreatePwmPort(HBridgePinA),
                //a2Port : _device.CreatePwmPort(HBridgePinB), 
                //enablePort : _device.CreateDigitalOutputPort(HBridgePinEnable),
                //pwmFrequency : 0.05f);

                _hBridge.IsNeutral = true;

                return true;
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Error: " + ex.Message, true);
                return false;
            }

        }

        public void SetMotorPower(float power)
        {
            _stopRequested = false;
            _hBridge.Speed = power;
        }

        public void Stop()
        {
            _stopRequested = true;

            _hBridge.IsNeutral = true;

            _hBridge.Speed = 0.0f;
        }

    }
}
