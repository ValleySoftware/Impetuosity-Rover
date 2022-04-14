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

        public BogieViewModel(string name) : base(name)
        {

        }

        public bool Init(Pca9685 pca, int servoPortIndex, ref ServoConfig servoConfig)
        {
            try
            {
                _steeringPort = pca.CreatePwmPort(Convert.ToByte(servoPortIndex));
                _servo = new Servo(_steeringPort, servoConfig);
                _position = 40;
                _servo.RotateTo(new Meadow.Units.Angle(_position));
                return true;
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Error: " + ex.Message, true);
                return  false;
            }

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
                    try
                    {
                        _servo.RotateTo(new Meadow.Units.Angle(_position, Meadow.Units.Angle.UnitType.Degrees));
                    }
                    catch (Exception ex)
                    {
                        ShowDebugMessage("Error: " + ex.Message, true);
                    }
                }
            }
        }
    }
}
