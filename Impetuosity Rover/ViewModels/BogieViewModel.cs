using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Servos;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Impetuosity_Rover.ViewModels
{
    public class BogieViewModel : ValleyBaseViewModel
    {
        IPwmPort _steeringPort;
        Servo _servo;
        double _position;
        private double _alignmentModifier = 0;

        public BogieViewModel(string name) : base(name)
        {

        }

        public bool Init(Pca9685 pca, int servoPortIndex, ref ServoConfig servoConfig, double alignmentModifier = 0)
        {
            try
            {
                AlignmentModifier = alignmentModifier;
                _steeringPort = pca.CreatePwmPort(Convert.ToByte(servoPortIndex));
                _servo = new Servo(_steeringPort, servoConfig);
                _position = 90;

                if (!MainViewModel.QuietStartup)
                {
                    _servo.RotateTo(new Meadow.Units.Angle(_position));
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Error: " + ex.Message, true);
                return  false;
            }
        }

        public double AlignmentModifier
        {
            get => _alignmentModifier;
            set
            {
                _alignmentModifier = value;
                Position = _position; //this is to adjust immediatly.
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
                _position = value;
                try
                {
                    var adjustedAngle = _position + AlignmentModifier;
                    adjustedAngle = Math.Max(adjustedAngle, 0);
                    adjustedAngle = Math.Min(adjustedAngle, 180);

                    if (!_position.Equals(adjustedAngle))
                    {
                        ShowDebugMessage(_name + " angle modified by " + AlignmentModifier + " from " + _position + " to " + adjustedAngle, true);
                    }

                    if (_servo != null)
                    {
                        _servo.RotateTo(new Meadow.Units.Angle(adjustedAngle, Meadow.Units.Angle.UnitType.Degrees));
                    }
                }
                catch (Exception ex)
                {
                    ShowDebugMessage("Error: " + ex.Message, true);
                }
            }
        }

        public void Test()
        {
            Position = 10;
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            Position = 90;
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            Position = 170;
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            Position = 90;
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }
    }
}
