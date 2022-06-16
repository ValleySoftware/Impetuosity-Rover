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

    public partial class BaseServoViewModel : ValleyBaseViewModel
    {
        IPwmPort _port;
        Servo _servo;
        double _position;
        private double _alignmentModifier = 0;
        private bool _isReversed = false;
        private Pca9685 _pca;

        private float _minDegrees;
        private float _maxDegrees;
        private float _centreAngle;

        public BaseServoViewModel(string name) : base(name)
        {

        }

        public virtual bool Init(
            ref Pca9685 pca, 
            int servoPortIndex, 
            ref ServoConfig servoConfig,
            double alignmentModifier,
            bool isReversed,
            float minDegrees,
            float maxDegrees,
            float centreAngle)
        { 
            try
            {
                _pca = pca;
                _alignmentModifier = alignmentModifier;
                _port = pca.CreatePwmPort(Convert.ToByte(servoPortIndex));
                _servo = new Servo(_port, servoConfig);
                _position = 90;
                _isReversed = isReversed;
                _minDegrees = minDegrees;
                _maxDegrees = maxDegrees;
                _centreAngle = centreAngle;

                RotateToPosition();


                return true;
            }
            catch (Exception ex)
            {
                mainViewModel.ShowDebugMessage(this, "Error: " + ex.Message, ErrorLoggingThreshold.exception);
                return false;
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
                adjustedAngle = Math.Max(adjustedAngle, _minDegrees);
                adjustedAngle = Math.Min(adjustedAngle, _maxDegrees);

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

        public float CentreAngle = 90f;
    }
}
