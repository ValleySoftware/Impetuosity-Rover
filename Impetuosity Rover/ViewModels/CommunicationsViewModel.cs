using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using static Impetuosity_Rover.Enumerations.Enumerations;

namespace Impetuosity_Rover.ViewModels
{
    public class CommunicationsViewModel : ValleyBaseViewModel
    {
        public CommunicationsViewModel(string name) : base(name)
        {
        }
    }

    public class WebRequestHandler : RequestHandlerBase
    {
        public WebRequestHandler()
        {
        }

        [HttpPost("/motorcontrol")]
        public IActionResult MotorControl()
        {
            if (Body == null)
            {
                return new StatusCodeResult((int)ErrorCodes.NoMessageBodyError);
            }

            //DurationTimeSpan/*/BothMotorPower
            //DurationTimeSpan/*/LeftMotorPower/*/RightMotorPower
            var message = Body.Split("/*/");

            IActionResult result = null;

            switch (message.Length)
            {
                case 0: result = new StatusCodeResult((int) ErrorCodes.NoMessageBodyError); break;
                case 2: result = RequestSetMotorPower(message[0], message[1]); break;
                case 3: result = RequestSetMotorPower(message[0], message[1], message[2]);  break;
                default: result = new StatusCodeResult((int)ErrorCodes.IncorrectParametersError); break;
            }

            return result;
        }

        private IActionResult RequestSetMotorPower(string duration, string both)
        {
            return RequestSetMotorPower(duration, both, both);
        }

        private IActionResult RequestSetMotorPower(string duration, string left, string right)
        {
            try
            {
                float leftPower;
                float rightPower;

                leftPower = float.Parse(left);
                rightPower = float.Parse(right);
                MeadowApp.Current.mainViewModel.Movement.SetMotorPower(leftPower, rightPower);
                return new OkResult();
            }
            catch (Exception parseException)
            {
                return new StatusCodeResult((int)ErrorCodes.ParseError);
            }

        }

    }
}
