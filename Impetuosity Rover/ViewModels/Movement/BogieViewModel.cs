using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Servos;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static Impetuous.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels.Movement
{
    public class BogieViewModel : BaseServoViewModel
    {

        public BogieViewModel(string name) : base(name)
        {

        }

        public override bool Init(
            ref Pca9685 pca,
            int servoPortIndex,
            ref ServoConfig servoConfig,
            double alignmentModifier = 0,
            bool isReversed = false,
            float minDegrees = 0,
            float maxDegrees = 180,
            float centreAngle = 90,
            float defaultAngle = 90)
        {
            return base.Init(
                ref pca,
                servoPortIndex,
                ref servoConfig,
                alignmentModifier,
                isReversed,
                minDegrees,
                maxDegrees,
                centreAngle,
                defaultAngle);
        }


    }
}
