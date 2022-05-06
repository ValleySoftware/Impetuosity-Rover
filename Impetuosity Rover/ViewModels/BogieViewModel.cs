using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Servos;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static Impetuosity_Rover.Enumerations.Enumerations;

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

        public bool Init(ref Pca9685 pca, int servoPortIndex, ref ServoConfig servoConfig, double alignmentModifier = 0)
        {
            try
            {
                AlignmentModifier = alignmentModifier;
                _steeringPort = pca.CreatePwmPort(Convert.ToByte(servoPortIndex));
                _servo = new Servo(_steeringPort, servoConfig);
                _position = 90;
                RotateToPosition();

                return true;
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Error: " + ex.Message, ErrorLoggingThreshold.exception);
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
                RotateToPosition();
            }
        }

        private void RotateToPosition()
        {
            try
            {
                var adjustedAngle = Position + AlignmentModifier;
                adjustedAngle = Math.Max(adjustedAngle, 0);
                adjustedAngle = Math.Min(adjustedAngle, 180);

                if (!_position.Equals(adjustedAngle))
                {
                    ShowDebugMessage(
                        _name + " angle modified by " + AlignmentModifier + " from " + Position + " to " + adjustedAngle,
                        ErrorLoggingThreshold.debug);
                }

                if (_servo != null)
                {
                    _servo.RotateTo(new Meadow.Units.Angle(adjustedAngle, Meadow.Units.Angle.UnitType.Degrees));
                }
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Error: " + ex.Message, ErrorLoggingThreshold.exception);
            }
        }

        public void Test()
        {
            Position = 10;
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            Position = 90;
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            Position = 170;
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            Position = 90;
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
        }
    }
}
