﻿using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;
using static Impetuosity_Rover.Enumerations.Enumerations;
using Meadow.Gateway.WiFi;
using System.Threading.Tasks;
using Impetuosity_Rover.Models;
using Meadow.Foundation;
using System.Net;
using System.IO;
using System.Threading;
using LitJson;
using System.Collections.Generic;

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
            var success = false;

            mainViewModel.ShowDebugMessage(this, "Starting WiFi", ErrorLoggingThreshold.important);

            MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Yellow);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Task t = Task.Run(async() =>
            {
                mainViewModel.ShowDebugMessage(this, "WiFi status flash starting", ErrorLoggingThreshold.debug);

                while (!token.IsCancellationRequested)
                {
                    MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Green);
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Blue);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                mainViewModel.ShowDebugMessage(
                    this, 
                    "WiFi status flash stopping", 
                    ErrorLoggingThreshold.debug);
            }, token);

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
                mapleServer.DeviceName = "Impetuosity Rover";
                mainViewModel.ShowDebugMessage(this, "Starting Maple", ErrorLoggingThreshold.important);
                mapleServer.Start();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                mainViewModel.ShowDebugMessage(this, "Comms Init error " + ex.Message, ErrorLoggingThreshold.exception);
            }

            //Request cancellation.
            tokenSource.Cancel();

            Thread.Sleep(2500);
            // Cancellation should have happened, so call Dispose.
            tokenSource.Dispose();

            if (success)
            {
                MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Green);
                mainViewModel.ShowDebugMessage(this, "WiFI and Maple startup completed.", ErrorLoggingThreshold.important);
                mainViewModel.Display.ShowMessage(new List<string>() { "Startup Complete" });
            }
            else
            {
                MeadowApp.Current.mainViewModel.onboardLed.SetColor(Color.Red);
            }

            return success;
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
            Console.WriteLine("MapleWebMotorControlEndpointActivated.");
            IActionResult result = null;

            string bodyText;

            if (Context.Request.HasEntityBody)
            {
                bodyText = ReadBodyFromStream(this.Context.Request);
                Console.WriteLine($"Body is {bodyText} ");                

                try
                {
                    var model = new MovementMessageModel();
                    //var model = JsonMapper.ToObject<MovementMessageModel>(bodyText);
                    SSJSONStringToObject(bodyText, model);

                    if (model != null)
                    {
                        MeadowApp.Current.mainViewModel.messages.Add(model);
                        model.RequestReceivedStamp = DateTimeOffset.Now;
                        model.RequestStatus = MessageStatus.receivedPendingAction;

                        result = RequestSetMotorPower(ref model);
                    }
                }
                catch (Exception deserializeEx)
                {
                    Console.WriteLine("movement request deserialization error " + deserializeEx.Message, true);
                    result = new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_ParseError);
                }
            }
            else
            {
                return new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_UnknownError);
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



        public class WebSteeringRequestHandler : RequestHandlerBase
        {
            public WebSteeringRequestHandler()
            {
                Console.WriteLine("WebSteeringRequestHandler constructor called.");
            }

            [HttpPost("/steeringcontrol")]
            public IActionResult SteeringControl()
            {
                Console.WriteLine("MapleWebSteeringControlEndpointActivated.");
                IActionResult result = null;

                string bodyText;

                if (Context.Request.HasEntityBody)
                {
                    bodyText = ReadBodyFromStream(this.Context.Request);
                    Console.WriteLine($"Body is {bodyText} ");

                    try
                    {
                        var model = new SteeringMessageModel();

                        //var model = JsonMapper.ToObject<SteeringMessageModel>(bodyText);
                        SSJSONStringToObject(bodyText, model);
                        if (model != null)
                        {
                            MeadowApp.Current.mainViewModel.messages.Add(model);
                            model.RequestReceivedStamp = DateTimeOffset.Now;
                            model.RequestStatus = MessageStatus.receivedPendingAction;

                            result = RequestChangeSteering(ref model);
                        }
                    }
                    catch (Exception deserializeEx)
                    {
                        Console.WriteLine("steering request deserialization error " + deserializeEx.Message, true);
                        result = new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_ParseError);
                    }
                }
                else
                {
                    return new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_UnknownError);
                }



                return result;
            }



            private IActionResult RequestChangeSteering(ref SteeringMessageModel request)
            {
                try
                {
                        if (MeadowApp.Current.mainViewModel.Movement.TurnBogies(ref request))
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
                    Console.WriteLine("steering request error " + parseException.Message, true);
                    return new StatusCodeResult(Enumerations.Enumerations.valleyMapleError_ParseError);
                }

            }
        }

        //=======================
        //Shared code
        //=======================

        public static string ReadBodyFromStream(HttpListenerRequest request)
        {
            MemoryStream memstream = new MemoryStream();
            request.InputStream.CopyTo(memstream);
            memstream.Position = 0;
            string text;
            using (StreamReader reader = new StreamReader(memstream))
            {
                text = reader.ReadToEnd();
            }
            return text;
        }

        public static MessageBaseModel SSJSONStringToObject(string json, MessageBaseModel destinationObject)
        {
            var l = JSONStringToKeyValuePairs(json);

            if (l != null)
            {
                return KeyValuePairListToObject(l, destinationObject);
            }
            else
            {
                return null;
            }
        }

        private static List<KeyValuePair<string, string>> JSONStringToKeyValuePairs(string json)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

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
                    //int mod = i % 2;

                    //if (mod != 0 &&

                    if (!string.IsNullOrEmpty(parts[i]) &&
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

                    i++;
                }


                int buildListLoop = 0;

                while (buildListLoop < parts.Length)
                {
                    try
                    {
                        var element = 
                            new KeyValuePair<string, string>(
                                parts[buildListLoop], 
                                parts[buildListLoop + 1]);
                        result.Add(element);
                    }
                    catch (Exception e)
                    {

                    }

                    buildListLoop = buildListLoop + 2;
                }

                    return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static MessageBaseModel KeyValuePairListToObject(
            List<KeyValuePair<string, string>> kvp, 
            object destinationObject)
        {
            if (destinationObject == null)
            {
                return null;
            }

            Type a = destinationObject.GetType();

            foreach (var element in kvp)
            {
                try
                {
                    var prop = a.GetProperty(element.Key);
                    if (prop != null)
                    {
                        prop.SetValue(destinationObject, element.Value);
                    }
                }
                catch (Exception e)
                {

                }
            }

            return (MessageBaseModel)destinationObject;
        }

    }
}
