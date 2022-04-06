using Meadow.Foundation.Motors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
{
    public class DrivePowerViewModel : ValleyBaseViewModel
    {
        private HBridgeMotor _hBridge;
        private bool _reverseMotorOrientation = false;
        private int _reverseMotorOrientationMultiplier = 1; //This is changed in the public property if the motor controller is backwards.
        bool _stopRequested = false;

        public DrivePowerViewModel() : base()
        {

        }

        public bool Init(Meadow.Hardware.IPin HBridge1PinA, Meadow.Hardware.IPin HBridge1PinB, Meadow.Hardware.IPin HBridge1PinEnable)
        {
            bool result = false;

            try
            {
                _hBridge = new HBridgeMotor(MeadowApp.Device,
                    a1Pin: HBridge1PinA,
                    a2Pin: HBridge1PinB,
                    enablePin: HBridge1PinEnable);

                _hBridge.IsNeutral = true;

                ReverseMotorOrientation = false;

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

        public bool ReverseMotorOrientation
        {
            get => _reverseMotorOrientation;
            set
            {
                _reverseMotorOrientation = value;
                if (value)
                {
                    _reverseMotorOrientationMultiplier = -1;
                }
                else
                {
                    _reverseMotorOrientationMultiplier = 1;
                }
            }
        }

        public void Stop()
        {
            _stopRequested = true;

            _hBridge.IsNeutral = true;

            _hBridge.Power = 0;
        }

    }
}
