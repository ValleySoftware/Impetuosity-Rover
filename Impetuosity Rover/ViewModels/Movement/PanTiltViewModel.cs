using Impetuosity_Rover.Models;
using Impetuosity_Rover.ViewModels.Primary;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Movement
{
    public class PanTiltViewModel : ValleyBaseViewModel
    {
        private Pca9685 _pca;
        private BaseServoViewModel _pan;
        private BaseServoViewModel _tilt;

        public PanTiltViewModel(string name) : base(name)
        {

        }

        public void Init(ref Pca9685 pca)
        {
            _pca = pca;
            _pan = new BaseServoViewModel("Pan");
            _tilt = new BaseServoViewModel("Tilt");
            _pan.Init(ref pca, 4, ref Meadow.Foundation.Servos.NamedServoConfigs.SG90, 0, false, 10, 170, 90, 170); //Larger = left
            _tilt.Init(ref pca, 5, ref Meadow.Foundation.Servos.NamedServoConfigs.SG90, 0, false, 10, 170, 90, 45); // Smaller number = down

            //_pan.CentreBogie();
            //_tilt.CentreBogie();
            //Thread.Sleep(500);
            _pan.MoveToDefaultAngle();
            _tilt.MoveToDefaultAngle();

            IsReady = true;
        }

        private void PanTo(double newAngle)
        {
            if (!IsReady)
            {
                return;
            }

            if (_pan != null)
            {
                _pan.Position = newAngle;
            }
        }

        private void TiltTo(double newAngle)
        {
            if (!IsReady)
            {
                return;
            }
            if (_tilt != null)
            {
                _tilt.Position = newAngle;
            }
        }

        public bool ProcessPanTiltRequest(ref PanTiltMessageModel request)
        {

            try
            {
                Console.WriteLine(request.PanTiltSelect.ToString(), true);
                Console.WriteLine(Enumerations.Enumerations.PanOrTilt.Pan.ToString(), true);

                if (request.PanTiltSelect == Enumerations.Enumerations.PanOrTilt.Pan)
                {
                    Console.WriteLine("pan", true);
                    PanTo(request.RequestValue);
                    return true;
                }
                else
                {
                    Console.WriteLine("tilt", true);
                    TiltTo(request.RequestValue);
                    return true;
                }
            }
            catch (Exception parseException)
            {
                Console.WriteLine("pantilt request error " + parseException.Message, true);
                return false;
            }
        }

    }
}
