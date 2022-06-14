using Meadow.Foundation.ICs.IOExpanders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Impetuosity_Rover.ViewModels
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
        }
    }
}
