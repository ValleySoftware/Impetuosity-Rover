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

        public MovementViewModel() :base()
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

            ShowDebugMessage("Instantiate Bogies");
            leftFrontBogie = new BogieViewModel();
            leftRearBogie = new BogieViewModel();
            rightFrontBogie = new BogieViewModel();
            rightRearBogie = new BogieViewModel();

            ShowDebugMessage("Init Bogies");
            leftFrontBogie.Init(bus, 15, SG51Conf);
            leftRearBogie.Init(bus, 14, SG51Conf);
            rightFrontBogie.Init(bus, 0, SG51Conf);
            rightRearBogie.Init(bus, 1, SG51Conf);

            ShowDebugMessage("Init Drive Motor Power");
            leftMotorPower = new DrivePowerViewModel();
            leftMotorPower.Init(_device.Pins.D13, _device.Pins.D12, _device.Pins.D11);
            rightMotorPower = new DrivePowerViewModel();
            rightMotorPower.Init(_device.Pins.D02, _device.Pins.D03, _device.Pins.D04);
        }

        public void SetMotorPower(float leftPower, float rightPower) 
        {
            leftMotorPower.SetMotorPower(leftPower);
            rightMotorPower.SetMotorPower(rightPower);
        }

        public void Stop()
        {
            leftMotorPower.Stop();
            rightMotorPower.Stop();
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
            SetMotorPower(50, 50);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            SetMotorPower(0, 0);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            SetMotorPower(-50, -50);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            SetMotorPower(0, 0);
        }

        public void TestBogies()
        {
            bool success = true;
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
            ShowDebugMessage("Sleep " + PauseAfterMovement.Ticks + " ticks");
            Thread.Sleep(PauseAfterMovement);
        }
    }
}
