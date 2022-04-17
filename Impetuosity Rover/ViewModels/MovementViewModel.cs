using Impetuosity_Rover.Models;
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

        public void Init(Pca9685 pca)
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

            ShowDebugMessage("Instantiate Bogies", true);
            leftFrontBogie = new BogieViewModel("LeftFrontBogie");
            _bogies.Add(leftFrontBogie);
            leftRearBogie = new BogieViewModel("LeftRearBogie");
            _bogies.Add(leftRearBogie);
            rightFrontBogie = new BogieViewModel("RightFrontBogie");
            _bogies.Add(rightFrontBogie);
            rightRearBogie = new BogieViewModel("RightRearBogie");
            _bogies.Add(rightRearBogie);

            ShowDebugMessage("Init Bogies", true);
            leftFrontBogie.Init(pca, 1, ref SG51Conf, 5);
            leftRearBogie.Init(pca, 0, ref SG51Conf, -15);
            rightFrontBogie.Init(pca, 14, ref SG51Conf);
            rightRearBogie.Init(pca, 15, ref SG51Conf, -5);

            //Alignment modifiers, to make sure all are indivitually tuned.
            //leftRearBogie.AlignmentModifier = -10;

            ShowDebugMessage("Init Drive Motor Power", true);
            leftMotorPower = new DrivePowerViewModel("LeftDriveMotors");
            leftMotorPower.Init(_device.Pins.D13, _device.Pins.D12, _device.Pins.D11);

            rightMotorPower = new DrivePowerViewModel("RightDriveMotors");
            rightMotorPower.Init(_device.Pins.D09, _device.Pins.D10, _device.Pins.D15);

            if (!MainViewModel.QuietStartup)
            {
                TestPower();
                TestBogies();
            }
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
                        request.RequestedPowerDuration = TimeSpan.FromMilliseconds(500);
                    }

                    var duration = request.RequestedPowerDuration;

                    var t = Task.Run(() =>
                    {
                        Thread.Sleep(duration);
                        Stop();
                    });
                }

            }
            catch (Exception startEx)
            {
                result = false;
                ShowDebugMessage("Set Motor Power error: " + startEx.Message, true);
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
                ShowDebugMessage("Stop error: " + stopEx.Message, true);
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

        public void TestPower()
        {
            var testOne = new MovementMessageModel() { LeftPower = 0.5f, RightPower = 0.5f, RequestedPowerDuration = TimeSpan.FromMilliseconds(500) };
            SetMotorPower(ref testOne);
            var testTwo = new MovementMessageModel() { LeftPower = -0.5f, RightPower = -0.5f, RequestedPowerDuration = TimeSpan.FromMilliseconds(500) };
            SetMotorPower(ref testTwo);

        }

        public void TestBogies(bool doLongerDanceTest = false)
        {
            bool success = true;
            MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Blue);
            ShowDebugMessage("Test all bogies. "); 

            if (doLongerDanceTest)
            {
                ShowDebugMessage("Shuffle, baby. "); //Play dubstep here
                while (success)
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
                        success = false;
                        ShowDebugMessage("Initialize error: " + ex.Message, true);
                        MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Red);
                    }
                }
            }
            else
            {
                ShowDebugMessage("Testing Individual Bogies");

                foreach (var element in _bogies)
                {
                    ShowDebugMessage("Testing " + element.Name);
                    element.Test();
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }

                ShowDebugMessage("Testing Bogies together");
                TurnAllToAngle(10);
                TurnAllToAngle(90);
                TurnAllToAngle(170);
                TurnAllToAngle(90);
                MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Green);
                ShowDebugMessage("Testing bogies complete");
            }
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
    }
}
