﻿using Impetuosity_Rover.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Servos;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels
{
    public class MovementViewModel : ValleyBaseViewModel
    {
        private List<BogieViewModel> _bogies = new List<BogieViewModel>();
        private BogieViewModel leftFrontBogie;
        private BogieViewModel leftRearBogie;
        private BogieViewModel rightFrontBogie;
        private BogieViewModel rightRearBogie;
        
        private DrivePowerViewModel leftMotorPower;
        private DrivePowerViewModel rightMotorPower;

        public MovementViewModel(string name) :base(name)
        {

        }

        public void Init(ref Pca9685 pca, TestMethodology requestedTesting = TestMethodology.none)
        {
            ShowDebugMessage("Prepare Servo Conf");

            ServoConfig SG51Conf = new ServoConfig
            (
                minimumAngle : new Meadow.Units.Angle(0, Meadow.Units.Angle.UnitType.Degrees),
                maximumAngle : new Meadow.Units.Angle(180, Meadow.Units.Angle.UnitType.Degrees),
                minimumPulseDuration : 1000,
                //maximumPulseDuration : 2750,
                maximumPulseDuration: 2000,
                frequency : 60
            ); //Some experimenting done here to get rotation kinda close...

            ShowDebugMessage("Instantiate Bogies", ErrorLoggingThreshold.important);
            leftFrontBogie = new BogieViewModel("LeftFrontBogie");
            _bogies.Add(leftFrontBogie);
            leftRearBogie = new BogieViewModel("LeftRearBogie");
            _bogies.Add(leftRearBogie);
            rightFrontBogie = new BogieViewModel("RightFrontBogie");
            _bogies.Add(rightFrontBogie);
            rightRearBogie = new BogieViewModel("RightRearBogie");
            _bogies.Add(rightRearBogie);

            ShowDebugMessage("Init Bogies", ErrorLoggingThreshold.important);
            leftFrontBogie.Init(ref pca, 1, ref SG51Conf, 5);
            leftRearBogie.Init(ref pca, 0, ref SG51Conf, -15);
            rightFrontBogie.Init(ref pca, 14, ref SG51Conf);
            rightRearBogie.Init(ref pca, 15, ref SG51Conf);

            ShowDebugMessage("Init Drive Motor Power", ErrorLoggingThreshold.important);
            leftMotorPower = new DrivePowerViewModel("LeftDriveMotors");
            leftMotorPower.Init(_device.Pins.D13, _device.Pins.D12, _device.Pins.D11);

            rightMotorPower = new DrivePowerViewModel("RightDriveMotors");
            rightMotorPower.Init(_device.Pins.D09, _device.Pins.D10, _device.Pins.D15);

            Test(requestedTesting);
        }

        public bool SetMotorPower(ref MovementMessageModel request) 
        {
            var result = false;

            try
            {
                leftMotorPower.SetMotorPower(request.LeftPower);
                rightMotorPower.SetMotorPower(request.RightPower);
                request.RequestStatus = Enumerations.Enumerations.MessageStatus.completedPendingConfirmation;
                result = true;

                request.RequestPerformedStamp = DateTimeOffset.Now;

                if (request.RequestedPowerDuration == null || 
                    !request.RequestedPowerDuration.Equals(TimeSpan.MinValue))
                {
                    if (request.RequestedPowerDuration == null)
                    {
                        request.RequestedPowerDuration = DateTimeOffset.MinValue.AddMilliseconds(500);
                    }

                    var duration = request.RequestedPowerDuration.TimeOfDay;

                    //Run the function as a thread.  This way it can be blocking, or not blocking.  Startup tests are better blocking.
                    //var t = Task.Run(() =>
                    //{
                        Thread.Sleep(duration);
                        Stop();
                    //});
                }

            }
            catch (Exception startEx)
            {
                result = false;
                ShowDebugMessage("Set Motor Power error: " + startEx.Message, ErrorLoggingThreshold.exception);
            }

            return result;
        }

        public void Stop()
        {
            try
            { 
                leftMotorPower.Stop();
                rightMotorPower.Stop();
            }
            catch (Exception stopEx)
            {
                ShowDebugMessage("Stop error: " + stopEx.Message, ErrorLoggingThreshold.exception);
            }
        }

        //with angle of 0 indicating straight ahead.
        //Reccommend staying between -70 and +70
        public void TurnBogiesTo(double angle)
        {
            TurnFrontBogiesTo(angle);
            TurnRearBogiesTo(angle * -1);
        }

        public void TurnBogiesBy(double amountToChangeAngleBy)
        {
            TurnFrontBogiesTo(leftFrontBogie.Position + amountToChangeAngleBy);
            TurnRearBogiesTo(leftFrontBogie.Position + amountToChangeAngleBy );
        }

        private void TurnFrontBogiesTo(double angle)
        {
            leftFrontBogie.Position = angle;
            rightFrontBogie.Position = angle;
        }

        private void TurnRearBogiesTo(double angle)
        {
            leftRearBogie.Position = angle;
            rightRearBogie.Position = angle;
        }

        private void TurnAllToAngle(double desiredAngleInDegrees)
        {
            TurnAllToAngle(desiredAngleInDegrees, TimeSpan.FromMilliseconds(500));
        }

        private void TurnAllToAngle(double desiredAngleInDegrees, TimeSpan PauseAfterMovement)
        {
            MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Blue);
            ShowDebugMessage("Go to " + desiredAngleInDegrees + " degrees");

            try
            {
                leftFrontBogie.Position = desiredAngleInDegrees;
                leftRearBogie.Position = desiredAngleInDegrees;
                rightFrontBogie.Position = desiredAngleInDegrees;
                rightRearBogie.Position = desiredAngleInDegrees;
            }
            catch (Exception ex)
            {
                ShowDebugMessage("Turn All Error: " + ex.Message);
                MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Red);
            }
            ShowDebugMessage("Sleep " + PauseAfterMovement.Milliseconds + " milliseconds");
            Thread.Sleep(PauseAfterMovement);
            MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Green);
        }

        //==============================
        //Testing
        //==============================

        public bool Test(TestMethodology requestedTesting)
        {
            var result = false;

            try
            {
                switch (requestedTesting)
                {
                    case TestMethodology.none: result = true; break;
                    case TestMethodology.iAmDeathIncarnate: result = ShuffleBaby(); break;
                    default:
                        {
                            result = TestMotorPower(requestedTesting);

                            if (result)
                            {
                                result = TestBogies(requestedTesting);
                            }
                        } break;
                }

            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public bool TestMotorPower(TestMethodology requestedTesting)
        {
            var result = false;

            switch (requestedTesting)
            {
                case TestMethodology.simple: result = MotorPowerSimpleTest(); break;
                case TestMethodology.thorough: result = MotorPowerThoroughTest(); break;
                default: result = true; break;
            }

            return result;
        }

        private bool MotorPowerSimpleTest()
        {
            var result = false;

            try
            {
                var testOne = new MovementMessageModel() { LeftPower = 0.5f, RightPower = 0.5f, RequestedPowerDuration = DateTimeOffset.MinValue.AddMilliseconds(500) };
                SetMotorPower(ref testOne);

                var testTwo = new MovementMessageModel() { LeftPower = -0.5f, RightPower = -0.5f, RequestedPowerDuration = DateTimeOffset.MinValue.AddMilliseconds(500) };
                SetMotorPower(ref testTwo);

                result = true;

            }
            catch (Exception ex)
            {

            }

            return result;
        }

        private bool MotorPowerThoroughTest()
        {
            var result = false;

            try
            {
                result = leftMotorPower.Test();

                if (result)
                {
                    result = rightMotorPower.Test();
                }

                if (result)
                {
                    result = MotorPowerSimpleTest();
                }

            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public bool TestBogies(TestMethodology requestedTesting)
        {
            bool success = true;
            MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Blue);

            switch (requestedTesting)
            {
                case TestMethodology.none: success = true; break;
                case TestMethodology.simple: success = ServoSimpleTest(); ; break;
                case TestMethodology.thorough: ServoThoroughTest(); break;
                default: success = true; break;
            }

            return success;
        }

        private bool ServoSimpleTest()
        {
            var result = false;

            try
            {
                ShowDebugMessage("Testing Bogies together");
                TurnAllToAngle(10);
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
                TurnAllToAngle(90);
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
                TurnAllToAngle(170);
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
                TurnAllToAngle(90);
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
                MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Green);
                ShowDebugMessage("Testing bogies complete");

                result = true;
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        private bool ServoThoroughTest()
        {
            var result = false;

            try
            {
                ShowDebugMessage("Testing Individual Bogies");

                foreach (var element in _bogies)
                {
                    ShowDebugMessage("Testing " + element.Name);
                    element.Test();
                    Thread.Sleep(TimeSpan.FromMilliseconds(250));
                }

                result = ServoSimpleTest();
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        private bool ShuffleBaby()
        {
            var result = true;

            try
            {
                //Nut bush?

                ShowDebugMessage("Shuffle, baby. "); //Play dubstep here
                while (result)
                {
                    try
                    {
                        int outer = 5;

                        while (outer > 0)
                        {
                            TurnAllToAngle(0);
                            TurnAllToAngle(90);
                            TurnAllToAngle(0);
                            TurnAllToAngle(90);

                            TurnAllToAngle(180);
                            TurnAllToAngle(90);
                            TurnAllToAngle(0);
                            TurnAllToAngle(180);

                            outer--;
                        }

                        int outerwiggle = 4;
                        while (outerwiggle >= 0)
                        {
                            int wiggle = 5;
                            while (wiggle > 0)
                            {
                                TurnAllToAngle(70);
                                TurnAllToAngle(110);
                                wiggle--;
                            }

                            Thread.Sleep(1000);
                            outerwiggle--;
                        }

                    }
                    catch (Exception ex)
                    {
                        result = false;
                        ShowDebugMessage("Initialize error: " + ex.Message, ErrorLoggingThreshold.exception);
                        MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Red);
                    }
                }

            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

    }
}
