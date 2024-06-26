﻿using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;
using static Impetuous.Enumerations.Enumerations;
using Meadow.Gateway.WiFi;
using System.Threading.Tasks;
using Impetuous.Models;
using Meadow.Foundation;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Impetuosity_Rover.ViewModels.Primary;

namespace Impetuosity_Rover.ViewModels.Comms
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

            mainViewModel.MasterStatus.ShowDebugMessage(this, "Starting WiFi", ErrorLoggingThreshold.important);

            mainViewModel.MasterStatus.CommsStatus = ComponentStatus.Initialising;

            try
            {
                var connectionResult =
                    await MeadowApp.Device.WiFiAdapter.Connect(
                        Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);

                if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
                {
                    mainViewModel.MasterStatus.CommsStatus = ComponentStatus.Error;
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }

                mapleServer = new MapleServer(
                    MeadowApp.Device.WiFiAdapter.IpAddress,
                    port: commsPort,
                    advertise: true
                );
                mapleServer.DeviceName = "Impetuosity Rover";
                mainViewModel.MasterStatus.ShowDebugMessage(this, "Starting Maple", ErrorLoggingThreshold.important);
                mapleServer.Start();
                success = true;

                mainViewModel.MasterStatus.CommsStatus = ComponentStatus.Ready;
            }
            catch (Exception ex)
            {
                success = false;
                mainViewModel.MasterStatus.CommsStatus = ComponentStatus.Error;
                mainViewModel.MasterStatus.ShowDebugMessage(this, "Comms Init error " + ex.Message, ErrorLoggingThreshold.exception);
            }

            if (success)
            {
                mainViewModel.MasterStatus.CommsStatus = ComponentStatus.Ready;
                mainViewModel.MasterStatus.ShowDebugMessage(this, "WiFI and Maple startup completed.", ErrorLoggingThreshold.important);
            }
            else
            {
                mainViewModel.MasterStatus.CommsStatus = ComponentStatus.Error;
            }

            return success;
        }

        public class WebLightsRequestHandler : RequestHandlerBase
        {
            public WebLightsRequestHandler()
        {
            //Console.WriteLine("WebMotorRequestHandler constructor called.");
        }

            [HttpPost("/lightcontrol")]
            public IActionResult LightControl()
        {
            Console.WriteLine("MapleLightControlEndpointActivated.");
            IActionResult result = null;

            string bodyText;

            if (Context.Request.HasEntityBody)
            {
                bodyText = ReadBodyFromStream(Context.Request);
                //Console.WriteLine($"Body is {bodyText} ");

                try
                {
                    var model = new LightMessageModel();
                    SSJSONStringToObject(bodyText, model);

                    if (model != null)
                    {
                        model.OriginalMessageString = bodyText;
                        MeadowApp.Current.mainViewModel.messages.Add(model);
                        model.RequestReceivedStamp = DateTimeOffset.Now;
                        model.RequestStatus = MessageStatus.receivedPendingAction;

                        result = RequestSetLights(ref model);
                    }
                }
                catch (Exception )
                {
                    //Console.WriteLine("movement request deserialization error " + deserializeEx.Message, true);
                    result = new StatusCodeResult(valleyMapleError_ParseError);
                }
            }
            else
            {
                return new StatusCodeResult(valleyMapleError_UnknownError);
            }

            return result;
        }

            private IActionResult RequestSetLights(ref LightMessageModel request)
        {
            try
            {
                if (MeadowApp.Current.mainViewModel.Lights.SetLight(ref request))
                {
                    return new OkResult();
                }
                else
                {
                    return new StatusCodeResult(valleyMapleError_ActionError);
                }

            }
            catch (Exception parseException)
            {
                Console.WriteLine("light request error " + parseException.Message, true);
                return new StatusCodeResult(valleyMapleError_ParseError);
            }

        }
        }

        public class WebMotorRequestHandler : RequestHandlerBase
        {
            public WebMotorRequestHandler()
            {
                //Console.WriteLine("WebMotorRequestHandler constructor called.");
            }

            [HttpPost("/motorcontrol")]
            public IActionResult MotorControl()
        {
            //Console.WriteLine("MapleWebMotorControlEndpointActivated.");
            IActionResult result = null;

            string bodyText;

            if (Context.Request.HasEntityBody)
            {
                bodyText = ReadBodyFromStream(Context.Request);
                //Console.WriteLine($"Body is {bodyText} ");

                try
                {
                    var model = new MovementMessageModel();
                    SSJSONStringToObject(bodyText, model);

                    if (model != null)
                    {
                        model.OriginalMessageString = bodyText;
                        MeadowApp.Current.mainViewModel.messages.Add(model);
                        model.RequestReceivedStamp = DateTimeOffset.Now;
                        model.RequestStatus = MessageStatus.receivedPendingAction;

                        result = RequestSetMotorPower(ref model);
                    }
                }
                catch (Exception )
                {
                    //Console.WriteLine("movement request deserialization error " + deserializeEx.Message, true);
                    result = new StatusCodeResult(valleyMapleError_ParseError);
                }
            }
            else
            {
                return new StatusCodeResult(valleyMapleError_UnknownError);
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
                    return new StatusCodeResult(valleyMapleError_ActionError);
                }

            }
            catch (Exception parseException)
            {
                Console.WriteLine("movement request error " + parseException.Message, true);
                return new StatusCodeResult(valleyMapleError_ParseError);
            }

        }
        }

        public class WebSteeringRequestHandler : RequestHandlerBase
        {
            public WebSteeringRequestHandler()
            {
                //Console.WriteLine("WebSteeringRequestHandler constructor called.");
            }

            [HttpPost("/steeringcontrol")]
            public IActionResult SteeringControl()
            {
                //Console.WriteLine("MapleWebSteeringControlEndpointActivated.");
                IActionResult result = null;

                string bodyText;

                if (Context.Request.HasEntityBody)
                {
                    bodyText = ReadBodyFromStream(Context.Request);
                    //Console.WriteLine($"Body is {bodyText} ");

                    try
                    {
                        var model = new SteeringMessageModel();
                        SSJSONStringToObject(bodyText, model);

                        if (model != null)
                        {
                            model.OriginalMessageString = bodyText;
                            MeadowApp.Current.mainViewModel.messages.Add(model);
                            model.RequestReceivedStamp = DateTimeOffset.Now;
                            model.RequestStatus = MessageStatus.receivedPendingAction;

                            result = RequestChangeSteering(ref model);
                        }
                    }
                    catch (Exception )
                    {
                        //Console.WriteLine("steering request deserialization error " + deserializeEx.Message, true);
                        result = new StatusCodeResult(valleyMapleError_ParseError);
                    }
                }
                else
                {
                    return new StatusCodeResult(valleyMapleError_UnknownError);
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
                        return new StatusCodeResult(valleyMapleError_ActionError);
                    }

                }
                catch (Exception )
                {
                    //Console.WriteLine("steering request error " + parseException.Message, true);
                    return new StatusCodeResult(valleyMapleError_ParseError);
                }

            }
        }


        public class WebPanTiltRequestHandler : RequestHandlerBase
        {
            public WebPanTiltRequestHandler()
            {
                //Console.WriteLine("WebPanTiltRequestHandler constructor called.");
            }

            [HttpPost("/pantiltcontrol")]
            public IActionResult MotorControl()
            {
                //Console.WriteLine("MapleWebPanTiltRequestHandlerEndpointActivated.");
                IActionResult result = null;

                string bodyText;

                if (Context.Request.HasEntityBody)
                {
                    bodyText = ReadBodyFromStream(Context.Request);
                    //Console.WriteLine($"Body is {bodyText} ");

                    try
                    {
                        var model = new PanTiltMessageModel();
                        SSJSONStringToObject(bodyText, model);

                        if (model != null)
                        {
                            model.OriginalMessageString = bodyText;
                            MeadowApp.Current.mainViewModel.messages.Add(model);
                            model.RequestReceivedStamp = DateTimeOffset.Now;
                            model.RequestStatus = MessageStatus.receivedPendingAction;

                            result = RequestPanTilt(ref model);
                        }
                    }
                    catch (Exception )
                    {
                        //Console.WriteLine("steering request deserialization error " + deserializeEx.Message, true);
                        result = new StatusCodeResult(valleyMapleError_ParseError);
                    }
                }
                else
                {
                    return new StatusCodeResult(valleyMapleError_UnknownError);
                }


                return result;
            }

            private IActionResult RequestPanTilt(ref PanTiltMessageModel request)
            {
                try
                {
                    if (MeadowApp.Current.mainViewModel.Movement.PanTilt.ProcessPanTiltRequest(ref request))
                    {
                        return new OkResult();
                    }
                    else
                    {
                        return new StatusCodeResult(valleyMapleError_ActionError);
                    }

                }
                catch (Exception )
                {
                    //Console.WriteLine("pantilt request error " + parseException.Message, true);
                    return new StatusCodeResult(valleyMapleError_ParseError);
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

            public static MessageBaseModel SSJSONStringToObject(string json, object destinationObject)
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
                    string splitConditionOne = "\":";
                    string splitConditionTwo = ",\"";

                    char quotationMark = '"';

                    var splitConditions = new string[2] { splitConditionOne, splitConditionTwo };
                    var parts = json.Split(splitConditions, StringSplitOptions.RemoveEmptyEntries);

                    int i = 0;

                    while (i < parts.Length)
                    {
                        //int mod = i % 2;

                        //if (mod != 0 &&

                        if (!string.IsNullOrEmpty(parts[i]) &&
                            parts[i][0].Equals(quotationMark))
                        {
                            parts[i] = parts[i].Substring(1, parts[i].Length - 2);
                        }

                        var ind = parts[i].IndexOf(quotationMark);
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
                        catch (Exception)
                        {

                        }

                        buildListLoop = buildListLoop + 2;
                    }

                    return result;
                }
                catch (Exception )
                {
                    return null;
                }
            }

        private static MessageBaseModel KeyValuePairListToObject(
            List<KeyValuePair<string, string>> kvpList,
            object destinationObject)
        {
            Type objType = destinationObject.GetType();

            //Console.WriteLine($"JSONStringToKeyValuePairs: object is identified as {objType}");
            //var destinationObject = anonDestinationObject as anonDestinationObject.GetType();

            if (destinationObject == null)
            {
                //Console.WriteLine($"JSONStringToKeyValuePairs error, destinationObject is null.  Exiting.");
                return null;
            }


            foreach (var keyValuePair in kvpList)
            {
                try
                {
                   // Console.WriteLine(keyValuePair.Key);

                    //Find the matching property in the destination object by Json object key (name)
                    var destinationObjectProperty = objType.GetProperty(keyValuePair.Key);

                    if (destinationObjectProperty == null)
                    {
                        //Console.WriteLine($"object property {keyValuePair.Key} is a UNMATCHED with value {keyValuePair.Value}");
                    }
                    else
                    {
                        //Console.WriteLine($"object property {keyValuePair.Key} is a {destinationObjectProperty.PropertyType} with value {keyValuePair.Value}");
                    }

                    if (destinationObjectProperty != null &&
                        !string.IsNullOrEmpty(keyValuePair.Value))
                    {
                        bool identified = false;

                        if (destinationObjectProperty.PropertyType == typeof(int))
                        {
                            destinationObjectProperty.SetValue(destinationObject, Convert.ToInt32(keyValuePair.Value.ToString()), null);
                            identified = true;
                        }

                        if (!identified &&
                            destinationObjectProperty.PropertyType == typeof(string))
                        {
                            destinationObjectProperty.SetValue(destinationObject, keyValuePair.Value, null);
                            identified = true;
                        }

                        if (!identified &&
                            destinationObjectProperty.PropertyType == typeof(DateTime))
                        {
                            destinationObjectProperty.SetValue(destinationObject, Convert.ToDateTime(keyValuePair.Value), null);
                            identified = true;
                        }

                        if (!identified &&
                            destinationObjectProperty.PropertyType == typeof(TimeSpan))
                        {
                            var t = TimeSpan.Parse(keyValuePair.Value);
                            destinationObjectProperty.SetValue(
                                destinationObject,
                                t, null);
                            identified = true;
                        }

                        if (!identified &&
                            destinationObjectProperty.PropertyType == typeof(DateTimeOffset))
                        {
                            var d = new DateTimeOffset(
                                    Convert.ToInt32(keyValuePair.Value.Substring(0, 4)),
                                    Convert.ToInt32(keyValuePair.Value.Substring(4, 2)),
                                    Convert.ToInt32(keyValuePair.Value.Substring(6, 2)),
                                    Convert.ToInt32(keyValuePair.Value.Substring(8, 2)),
                                    Convert.ToInt32(keyValuePair.Value.Substring(8, 2)),
                                    Convert.ToInt32(keyValuePair.Value.Substring(8, 2)), TimeSpan.Zero);
                            destinationObjectProperty.SetValue(destinationObject, d, null);
                            identified = true;
                        }

                        if (destinationObjectProperty.PropertyType == typeof(bool))
                        {
                            destinationObjectProperty.SetValue(destinationObject, Convert.ToBoolean(keyValuePair.Value.ToString()), null);
                            identified = true;
                        }

                        if (!identified &&
                            (destinationObjectProperty.PropertyType == typeof(float) ||
                                destinationObjectProperty.PropertyType == typeof(double)))
                        {
                            string s = keyValuePair.Value.ToString();
                            double d = Convert.ToDouble(s);
                            float f = (float)d;
                            destinationObjectProperty.SetValue(destinationObject, f, null);
                            identified = true;

                        }

                        if (!identified &&
                            destinationObjectProperty.PropertyType == typeof(SteeringRequestType))
                        {
                            destinationObjectProperty.SetValue(destinationObject, Convert.ToInt32(keyValuePair.Value.ToString()), null);
                            identified = true;
                        }

                        if (!identified &&
                            destinationObjectProperty.PropertyType == typeof(PanTiltSelect))
                        {
                            destinationObjectProperty.SetValue(destinationObject, Convert.ToInt32(keyValuePair.Value.ToString()), null);
                            identified = true;
                        }

                        if (!identified &&
                            destinationObjectProperty.PropertyType == typeof(MessageStatus))
                        {
                            destinationObjectProperty.SetValue(destinationObject, Convert.ToInt32(keyValuePair.Value.ToString()), null);
                            identified = true;
                        }

                        if (!identified &&
                            destinationObjectProperty.PropertyType == typeof(LightSelect))
                        {
                            destinationObjectProperty.SetValue(destinationObject, Convert.ToInt32(keyValuePair.Value.ToString()), null);
                            identified = true;
                        }

                        if (!identified)
                        {
                            //Console.WriteLine($"JSON Object Convert {keyValuePair.Key} is UNIDENTIFIED with value {keyValuePair.Value}");
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        "property value error on " +
                        keyValuePair.Key +
                        " - " +
                        keyValuePair.Value +
                        " : " +
                        e.Message,
                        true);
                }
            }

            return (MessageBaseModel)destinationObject;

        }
    }
}
