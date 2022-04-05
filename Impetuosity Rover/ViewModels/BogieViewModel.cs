using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Servos;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
{
    public class BogieViewModel : ValleyBaseViewModel
    {
        IPwmPort _steeringPort;
        Servo _servo;
        double _position;

        public BogieViewModel() : base()
        {

        }

        public bool Init(Pca9685 bus, int servoPortIndex, ServoConfig servoConfig)
        {
            bool result = false;

            try
            {
                _steeringPort = bus.CreatePwmPort(Convert.ToByte(servoPortIndex));
                _servo = new Servo(_steeringPort, servoConfig);
                Position = 90;
                result = true;
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Error: " + ex.Message, true);
                result = false;
            }

            return result;
        }

        public double Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    _servo.RotateTo(new Meadow.Units.Angle(_position, Meadow.Units.Angle.UnitType.Degrees));
                }
            }
        }
    }
}
