using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;
using static Impetuosity_Rover.Enumerations.Enumerations;
using Meadow.Gateway.WiFi;
using System.Threading.Tasks;
using Impetuosity_Rover.Models;
using Meadow.Foundation;

namespace Impetuosity_Rover.ViewModels
{
    public class CommunicationsViewModel : ValleyBaseViewModel
    {
        MapleServer mapleServer;
        int commsPort = 5417;
        

        public CommunicationsViewModel(string name) : base(name)
        {
        }

        public async Task<bool> Init()
        {

            Console.WriteLine("Starting WiFi", true);
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
                    MeadowApp.Device.WiFiAdapter.IpAddress,
                    port: commsPort,
                    advertise: true
                );

                Console.WriteLine("Starting Maple", true);
                mapleServer.Start();
                MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Green);
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
            Console.WriteLine("WebMotorRequestHandler constructor called.");
        }

        [HttpPost("/motorcontrol")]
        public IActionResult MotorControl()
        {
            Console.WriteLine("WebMotorRequestHandler MotorControl method");

            Console.WriteLine("MapleWebMotorControlEndpointActivated", true);

            if (string.IsNullOrEmpty(Body))
            {
                return new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_UnknownError);
            }

            IActionResult result = null;

            var m = new MovementMessageModel();
            m.MessageID = "aaaaaaa";
            m.RequestedPowerDuration = TimeSpan.FromMilliseconds(500);
            m.LeftPower = 0.5f;
            m.RightPower = 0.5f;
            m.RequestStop = false;

            var outgoing = SimpleJsonSerializer.JsonSerializer.SerializeObject(m);

            try
            {
                MovementMessageModel request = 
                    (MovementMessageModel)SimpleJsonSerializer.JsonSerializer.DeserializeString(
                        Body );

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
