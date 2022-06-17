using Impetuosity_Rover.ViewModels.Primary;
using Meadow.Foundation.Motors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Movement
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
                    device: _device,
                    a1Pin: HBridgePinA,
                    a2Pin: HBridgePinB,
                    enablePin: HBridgePinEnable);

                _hBridge.IsNeutral = true;

                return true;
            }
            catch (Exception ex)
            {
                mainViewModel.ShowDebugMessage(this, "Error: " + ex.Message, ErrorLoggingThreshold.exception);
                return false;
            }

        }

        public void SetMotorPower(float power)
        {
            _stopRequested = false;
            _hBridge.Power = power;
        }

        public bool Test()
        {
            var result = false;

            try
            {
                var duration = TimeSpan.FromMilliseconds(500);

                SetMotorPower(0.5f);

                Thread.Sleep(duration);
                Stop();

                SetMotorPower(-0.5f);

                Thread.Sleep(duration);
                Stop();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public void Stop()
        {
            _stopRequested = true;

            _hBridge.IsNeutral = true;

            _hBridge.Power = 0.0f;
        }

    }
}
