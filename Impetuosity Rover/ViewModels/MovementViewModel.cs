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

        BogieViewModel leftFrontBogie;
        BogieViewModel leftRearBogie;
        BogieViewModel rightFrontBogie;
        BogieViewModel rightRearBogie;

        DrivePowerViewModel leftMotorPower;
        DrivePowerViewModel rightMotorPower;

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
            leftMotorPower.Init();
            rightMotorPower = new DrivePowerViewModel();
            rightMotorPower.Init();
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
