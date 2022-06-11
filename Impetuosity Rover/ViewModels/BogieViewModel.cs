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
        private bool _isReversed = false;
        private float minDegrees = 10;
        private float maxDegrees = 170;

        public BogieViewModel(string name) : base(name)
        {

        }

        public bool Init(ref Pca9685 pca, int servoPortIndex, ref ServoConfig servoConfig, double alignmentModifier = 0, bool isReversed = false)
        {
            try
            {
                AlignmentModifier = alignmentModifier;
                _steeringPort = pca.CreatePwmPort(Convert.ToByte(servoPortIndex));
                _servo = new Servo(_steeringPort, servoConfig);
                _position = 90;
                _isReversed = isReversed;

                RotateToPosition();


                return true;
            }
            catch (Exception ex)
            {
                mainViewModel.ShowDebugMessage(this, "Error: " + ex.Message, ErrorLoggingThreshold.exception);
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
                //if (_isReversed)
                //{
                 //   ShowDebugMessage(
                 //       _name + " (Reversed) old = " + _position + " new = 180 - " + value + " =  " + (180 - value),
                 //       ErrorLoggingThreshold.important);;
                //    _position = 180 - value;
                //}
                //else
                //{
                    _position = value;
                //}

                RotateToPosition();
            }
        }
        
        private void RotateToPosition()
        {
            double adjustedAngle = 0;
            try
            {
                adjustedAngle = Position + AlignmentModifier;
                adjustedAngle = Math.Max(adjustedAngle, minDegrees);
                adjustedAngle = Math.Min(adjustedAngle, maxDegrees);

                if (!_position.Equals(adjustedAngle))
                {
                    mainViewModel.ShowDebugMessage(this, 
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
                mainViewModel.ShowDebugMessage(this, "Error: " + ex.Message + " - angle requested (adjusted) was : " + adjustedAngle.ToString(), ErrorLoggingThreshold.exception);
            }
        }

        public void Test()
        {
            Position = 10;
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            CentreBogie();
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            Position = 170;
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            CentreBogie();
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
        }

        public void CentreBogie()
        {
            Position = CentreAngle;
        }

        private static float CentreAngle = 90f;
    }
}
