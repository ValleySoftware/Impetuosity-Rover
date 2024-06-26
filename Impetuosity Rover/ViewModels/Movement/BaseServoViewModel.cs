﻿using Impetuosity_Rover.ViewModels.Primary;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Servos;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Movement
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
        private float _defaultAngle;

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
            float centreAngle, 
            float defaultAngle)
        {
            try
            {
                _pca = pca;
                _alignmentModifier = alignmentModifier;
                _port = pca.CreatePwmPort(Convert.ToByte(servoPortIndex));
                _servo = new Servo(_port, servoConfig);
                _isReversed = isReversed;
                _minDegrees = minDegrees;
                _maxDegrees = maxDegrees;
                _centreAngle = centreAngle;
                _defaultAngle = defaultAngle;
                _position = defaultAngle;

                IsReady = true;

                RotateToPosition(true);

                return true;
            }
            catch (Exception ex)
            {
                mainViewModel.MasterStatus.ShowDebugMessage(this, "Error: " + ex.Message, ErrorLoggingThreshold.exception);
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

        public float CentreAngle
        {
            get => _centreAngle;
            set
            {
                _centreAngle = value;
            }
        }
        public float DefaultAngle
        {
            get => _defaultAngle;
            set
            {
                _defaultAngle = value;
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

                RotateToPosition(false);
            }
        }

        private void RotateToPosition(bool fastMove)
        {
            if (!IsReady)
            {
                return;
            }

            double adjustedAngle = 0;
            try
            {
                adjustedAngle = Position + AlignmentModifier;
                adjustedAngle = Math.Max(adjustedAngle, _minDegrees);
                adjustedAngle = Math.Min(adjustedAngle, _maxDegrees);

                if (!_position.Equals(adjustedAngle))
                {
                    mainViewModel.MasterStatus.ShowDebugMessage(this,
                        _name + " angle modified by " + AlignmentModifier + " from " + Position + " to " + adjustedAngle,
                        ErrorLoggingThreshold.debug);
                }

                if (_servo != null)
                {
                    if (fastMove)
                    {
                        _servo.RotateTo(new Meadow.Units.Angle(adjustedAngle, Meadow.Units.Angle.UnitType.Degrees));
                    }
                    else
                    {
                        mainViewModel.MasterStatus.ShowDebugMessage(this,
                            _name + " slow rotate to " + Position,
                            ErrorLoggingThreshold.important);

                        double incrementalAngle = Position;
                        double step = 0;

                        if (incrementalAngle < adjustedAngle)
                        {
                            step = 0.05f;
                        }
                        if (incrementalAngle > adjustedAngle)
                        {
                            step = -0.05f;
                        }

                        while (!incrementalAngle.Equals(adjustedAngle))
                        {
                            if (Math.Round(incrementalAngle, 1).Equals(Math.Round(adjustedAngle, 1)))
                            {
                                //To allow for rounding differences and minor final adjustments
                                incrementalAngle = adjustedAngle;
                            }
                            else
                            {
                                incrementalAngle = incrementalAngle + step;
                            }

                            //Console.WriteLine(_name + " Slow Rotation step " + incrementalAngle);
                            _servo.RotateTo(new Meadow.Units.Angle(incrementalAngle, Meadow.Units.Angle.UnitType.Degrees));

                            Task.Delay(TimeSpan.FromMilliseconds(500));
                        }

                        _servo.RotateTo(new Meadow.Units.Angle(Position, Meadow.Units.Angle.UnitType.Degrees));

                    }
                }
            }
            catch (Exception ex)
            {
                mainViewModel.MasterStatus.ShowDebugMessage(this, "Error: " + ex.Message + " - angle requested (adjusted) was : " + adjustedAngle.ToString(), ErrorLoggingThreshold.exception);
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
            if (!IsReady)
            {
                return;
            }

            Position = _centreAngle;
        }

        public void MoveToDefaultAngle()
        {
            if (!IsReady)
            {
                return;
            }

            Position = DefaultAngle;
        }


    }

}
