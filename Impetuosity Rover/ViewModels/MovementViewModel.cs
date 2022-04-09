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

namespace Impetuosity_Rover.ViewModels
{
    public class MovementViewModel : ValleyBaseViewModel
    {

        private BogieViewModel leftFrontBogie;
        private BogieViewModel leftRearBogie;
        private BogieViewModel rightFrontBogie;
        private BogieViewModel rightRearBogie;
        
        private DrivePowerViewModel leftMotorPower;
        private DrivePowerViewModel rightMotorPower;

        public MovementViewModel(string name) :base(name)
        {

        }

        public void Init(Pca9685 bus)
        {
            ShowDebugMessage("Prepare Servo Conf");

            ServoConfig SG51Conf = new ServoConfig
            (
                new Meadow.Units.Angle(0, Meadow.Units.Angle.UnitType.Degrees),
                new Meadow.Units.Angle(180, Meadow.Units.Angle.UnitType.Degrees),
                1000,
                2750,
                60
            ); //Some experimenting done here to get rotation kinda close...

            ShowDebugMessage("Instantiate Bogies", true);
            leftFrontBogie = new BogieViewModel("LeftFrontBogie");
            leftRearBogie = new BogieViewModel("LeftRearBogie");
            rightFrontBogie = new BogieViewModel("RightFrontBogie");
            rightRearBogie = new BogieViewModel("RightRearBogie");

            ShowDebugMessage("Init Bogies", true);
            leftFrontBogie.Init(bus, 15, SG51Conf);
            leftRearBogie.Init(bus, 14, SG51Conf);
            rightFrontBogie.Init(bus, 0, SG51Conf);
            rightRearBogie.Init(bus, 1, SG51Conf);

            ShowDebugMessage("Init Drive Motor Power", true);
            leftMotorPower = new DrivePowerViewModel("LeftDriveMotors");
            leftMotorPower.Init(_device.Pins.D09, _device.Pins.D10, _device.Pins.D04);

            rightMotorPower = new DrivePowerViewModel("RightDriveMotors");
            rightMotorPower.Init(_device.Pins.D05, _device.Pins.D06, _device.Pins.D03);
        }

        public void SetMotorPower(float leftPower, float rightPower) 
        {
            try
            { 
                leftMotorPower.SetMotorPower(leftPower);
                rightMotorPower.SetMotorPower(rightPower);
            }
            catch (Exception startEx)
            {
                ShowDebugMessage("Set Motor Power error: " + startEx.Message, true);
            }
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
            ShowDebugMessage("Motor Test Forward");
            SetMotorPower(0.5f, 0.5f);
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            ShowDebugMessage("Motor Test Stop");
            Stop();
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            ShowDebugMessage("Motor Test Backwards");
            SetMotorPower(-0.5f, -0.5f);
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            ShowDebugMessage("Motor Test Stop");
            Stop();
        }

        public void TestBogies(bool doLongerDanceTest)
        {
            bool success = true;
            ShowDebugMessage("Shuffle, baby. "); //Play dubstep here

            if (doLongerDanceTest)
            {
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
                                TurnAllToAngle(70, TimeSpan.FromTicks(250));
                                TurnAllToAngle(110, TimeSpan.FromTicks(250));
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
                TurnAllToAngle(70, TimeSpan.FromMilliseconds(250));
                TurnAllToAngle(110, TimeSpan.FromMilliseconds(250));
                TurnAllToAngle(90, TimeSpan.FromMilliseconds(250));
            }
        }

        private void TurnAllToAngle(double desiredAngleInDegrees)
        {
            TurnAllToAngle(desiredAngleInDegrees, TimeSpan.FromMilliseconds(500));
        }

        private void TurnAllToAngle(double desiredAngleInDegrees, TimeSpan PauseAfterMovement)
        {

            MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Yellow);
            ShowDebugMessage("Go to " + desiredAngleInDegrees + " degrees");

            leftFrontBogie.Position = desiredAngleInDegrees;
            leftRearBogie.Position = desiredAngleInDegrees;
            rightFrontBogie.Position = desiredAngleInDegrees;
            rightRearBogie.Position = desiredAngleInDegrees;

            MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Blue);
            ShowDebugMessage("Sleep " + PauseAfterMovement.Milliseconds + " milliseconds");
            Thread.Sleep(PauseAfterMovement);
        }
    }
}
