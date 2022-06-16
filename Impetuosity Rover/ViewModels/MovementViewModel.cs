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
        private bool _bogiesTransitioning = false;
        
        private DrivePowerViewModel leftMotorPower;
        private DrivePowerViewModel rightMotorPower;

        public MovementViewModel(string name) :base(name)
        {

        }

        public static MovementMessageModel ModelFromJSONString(string json)
        {
            var result = new MovementMessageModel();

            try
            {
                json = json.Substring(2, json.Length - 3);
                string conditionOne = "\":";
                string conditionTwo = ",\"";

                var conditions = new string[2] { conditionOne, conditionTwo };
                var parts = json.Split(conditions, StringSplitOptions.RemoveEmptyEntries);

                int i = 0;

                while (i < parts.Length)
                {
                    if (i % 2 != 0 &&
                        parts[i][0].Equals("\""))
                    {
                        parts[i] = parts[i].Substring(1, parts[i].Length - 2);
                    }
                    
                    var ind = parts[1].IndexOf("\"");
                    if (ind > 0)
                    {
                        parts[i] = parts[i].Substring(0, ind);
                    }
                    
                    parts[i] = parts[i].Trim();
                }
            }
            catch (Exception ex)
            {

            }
            return result;

        }

        public void Init(ref Pca9685 pca, TestMethodology requestedTesting = TestMethodology.none)
        {
            mainViewModel.ShowDebugMessage(this, "Prepare Servo Conf");

            ServoConfig SG51Conf = new ServoConfig
            (
                minimumAngle : new Meadow.Units.Angle(0, Meadow.Units.Angle.UnitType.Degrees),
                maximumAngle : new Meadow.Units.Angle(180, Meadow.Units.Angle.UnitType.Degrees),
                minimumPulseDuration : 1000,
                //maximumPulseDuration : 2750,
                maximumPulseDuration: 2000,
                frequency : 60
            ); //Some experimenting done here to get rotation kinda close...

            mainViewModel.ShowDebugMessage(this, "Instantiate Bogies", ErrorLoggingThreshold.important);
            leftFrontBogie = new BogieViewModel("LeftFrontBogie");
            _bogies.Add(leftFrontBogie);
            leftRearBogie = new BogieViewModel("LeftRearBogie");
            _bogies.Add(leftRearBogie);
            rightFrontBogie = new BogieViewModel("RightFrontBogie");
            _bogies.Add(rightFrontBogie);
            rightRearBogie = new BogieViewModel("RightRearBogie");
            _bogies.Add(rightRearBogie);

            mainViewModel.ShowDebugMessage(this, "Init Bogies", ErrorLoggingThreshold.important);
            leftFrontBogie.Init(ref pca, 1, ref SG51Conf, -15);
            leftRearBogie.Init(ref pca, 0, ref SG51Conf, -15, true);
            rightFrontBogie.Init(ref pca, 15, ref SG51Conf, -20);
            rightRearBogie.Init(ref pca, 14, ref SG51Conf, 0, true);

            mainViewModel.ShowDebugMessage(this, "Init Drive Motor Power", ErrorLoggingThreshold.important);
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
                        request.RequestedPowerDuration = TimeSpan.FromMilliseconds(500);
                    }

                    var duration = request.RequestedPowerDuration;

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
                mainViewModel.ShowDebugMessage(this, "Set Motor Power error: " + startEx.Message, ErrorLoggingThreshold.exception);
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
                mainViewModel.ShowDebugMessage(this, "Stop error: " + stopEx.Message, ErrorLoggingThreshold.exception);
            }
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
            mainViewModel.ShowDebugMessage(this, "Go to " + desiredAngleInDegrees + " degrees");

            try
            {
                leftFrontBogie.Position = desiredAngleInDegrees;
                leftRearBogie.Position = desiredAngleInDegrees;
                rightFrontBogie.Position = desiredAngleInDegrees;
                rightRearBogie.Position = desiredAngleInDegrees;
            }
            catch (Exception ex)
            {
                mainViewModel.ShowDebugMessage(this, "Turn All Error: " + ex.Message, ErrorLoggingThreshold.exception );
                MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Red);
            }

            mainViewModel.ShowDebugMessage(this, "Sleep " + PauseAfterMovement.Milliseconds + " milliseconds");
            Thread.Sleep(PauseAfterMovement);
            MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Green);
        }

        private void CentreAllBogies()
        {
            leftFrontBogie.CentreBogie();
            leftRearBogie.CentreBogie();
            rightFrontBogie.CentreBogie();
            rightRearBogie.CentreBogie();
        }

        //with angle of 0 indicating straight ahead.
        //Reccommend staying between -70 and +70
        private void TurnBogiesTo(double angle)
        {
            if (angle >= -70 &&
                angle <= 70)
            {
                TurnFrontBogiesTo(angle);
                TurnRearBogiesTo(angle);
            }
        }

        private void TurnBogiesBy(double amountToChangeAngleBy)
        {
            TurnFrontBogiesTo(leftFrontBogie.Position + amountToChangeAngleBy);
            TurnRearBogiesTo(leftRearBogie.Position - amountToChangeAngleBy);
        }

        public bool TurnBogies(ref SteeringMessageModel request)
        {
            var result = false;

            if (!_bogiesTransitioning)
            {
                _bogiesTransitioning = true;

                request.RequestPerformedStamp = DateTimeOffset.Now;
                request.RequestStatus = Enumerations.Enumerations.MessageStatus.completedPendingConfirmation;

                try
                {
                    switch (request.RequestType)
                    {
                        case SteeringRequestType.AdjustBy: TurnBogiesBy(request.SteeringValue); break;
                        case SteeringRequestType.SetTo: TurnBogiesTo(request.SteeringValue); break;
                        case SteeringRequestType.Centre: CentreAllBogies(); break;
                        default:; break;
                    }

                    result = true;
                }
                catch (Exception broadSteeringRequestException)
                {
                    mainViewModel.ShowDebugMessage(this, "BroadSetSteering Error: " + broadSteeringRequestException.Message, ErrorLoggingThreshold.exception);
                }
                finally
                {
                    _bogiesTransitioning = false;
                }
            }


            return result;
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
                var testOne = new MovementMessageModel() { LeftPower = 0.5f, RightPower = 0.5f, RequestedPowerDuration = TimeSpan.FromMilliseconds(500) };
                SetMotorPower(ref testOne);

                var testTwo = new MovementMessageModel() { LeftPower = -0.5f, RightPower = -0.5f, RequestedPowerDuration = TimeSpan.FromMilliseconds(500) };
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
                mainViewModel.ShowDebugMessage(this, "Testing Bogies together");
                TurnAllToAngle(10);
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
                TurnAllToAngle(90);
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
                TurnAllToAngle(170);
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
                TurnAllToAngle(90);
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
                MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Green);
                mainViewModel.ShowDebugMessage(this, "Testing bogies complete");

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
                mainViewModel.ShowDebugMessage(this, "Testing Individual Bogies");

                foreach (var element in _bogies)
                {
                    mainViewModel.ShowDebugMessage(this, "Testing " + element.Name);
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

                mainViewModel.ShowDebugMessage(this, "Shuffle, baby. "); //Play dubstep here
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
                        mainViewModel.ShowDebugMessage(this, "Initialize error: " + ex.Message, ErrorLoggingThreshold.exception);
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
