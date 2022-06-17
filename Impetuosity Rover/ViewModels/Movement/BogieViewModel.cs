using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Servos;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static Impetuosity_Rover.Enumerations.Enumerations;

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
            float minDegrees = 10,
            float maxDegrees = 170,
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

        public void Test()
        {
            Position = 10;
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            CentreBogie();
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            Position = 170;
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            CentreBogie();
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
        }


    }
}
