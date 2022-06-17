using Impetuosity_Rover.ViewModels.Primary;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
            _pan.Init(ref pca, 4, ref Meadow.Foundation.Servos.NamedServoConfigs.SG90, 0, false, 10, 170, 90, 175); //Larger = left
            _tilt.Init(ref pca, 5, ref Meadow.Foundation.Servos.NamedServoConfigs.SG90, 0, false, 10, 170, 90, 45); // Smaller number = down

            _pan.CentreBogie();
            _tilt.CentreBogie();
            Thread.Sleep(500);
            _pan.MoveToDefaultAngle();
            _tilt.MoveToDefaultAngle();
        }

        public void PanTo(double newAngle)
        {
            if (_pan != null)
            {
                _pan.Position = newAngle;
            }
        }

        public void TiltTo(double newAngle)
        {
            if (_tilt != null)
            {
                _tilt.Position = newAngle;
            }
        }

    }
}
