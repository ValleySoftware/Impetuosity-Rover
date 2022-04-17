using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using Impetuosity_Rover;
using static Impetuosity_Rover.Enumerations.Enumerations;
using Meadow.Gateway.WiFi;
using System.Threading.Tasks;
using Impetuosity_Rover.Models;

namespace Impetuosity_Rover.ViewModels
{
    public class CommunicationsViewModel : ValleyBaseViewModel
    {
        MapleServer mapleServer;
        WebMotorRequestHandler motorControlHandler;

        public CommunicationsViewModel(string name) : base(name)
        {
        }

        public async Task<bool> Init()
        {
            try
            {
                var connectionResult =
                    await MeadowApp.Device.WiFiAdapter.Connect(
                        Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);

                if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
                {
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }

                mapleServer = new MapleServer(
                    MeadowApp.Device.WiFiAdapter.IpAddress, 5417
                );

                mapleServer.Start();

                //motorControlHandler = new WebMotorRequestHandler();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Comms Init error "  + ex.Message);
                return false;
            }

            return true;
        }
    }

    public class WebMotorRequestHandler : RequestHandlerBase
    {
        public WebMotorRequestHandler()
        {
            Console.WriteLine("WebRequestHandler Started");

        }

        [HttpPost("/motorcontrol")]
        public IActionResult MotorControl()
        {
            Console.WriteLine("MapleWebMotorControlEndpointActivated", true);
            //Console.WriteLine(Body.ToString(), true);

            if (string.IsNullOrEmpty(Body))
            {
                return new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_UnknownError);
            }

            IActionResult result = null;

            try
            {
                MovementMessageModel request = System.Text.Json.JsonSerializer.Deserialize<MovementMessageModel>(Body);

                if (request != null)
                {
                    MeadowApp.Current.mainViewModel.messages.Add(request);
                    request.RequestReceivedStamp = DateTimeOffset.Now;
                    request.RequestStatus = MessageStatus.receivedPendingAction;

                    result = RequestSetMotorPower(ref request);
                }
            }
            catch (Exception deserializeEx)
            {
                Console.WriteLine("movement request deserialization error " + deserializeEx.Message, true);
                result = new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_ParseError); 
            }

            return result;
        }

        private IActionResult RequestSetMotorPower(ref MovementMessageModel request)
        {
            try
            {
                if (MeadowApp.Current.mainViewModel.Movement.SetMotorPower(ref request))
                {
                    return new OkResult();
                }
                else
                {
                    return new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_ActionError);
                }
                
            }
            catch (Exception parseException)
            {
                Console.WriteLine("movement request error " + parseException.Message, true);
                return new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_ParseError);
            }

        }

    }
}
