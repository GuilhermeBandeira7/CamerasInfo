using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Xml.Serialization;
using CamerasInfo.Service;
using CamerasInfo.Context;
using CamerasInfo.Helpers;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MongoDB.Bson;

namespace CamerasInfo.Managers
{
    public static class CamManager
    {
        public static Ping_MongoDB PingMongoDB { get; set; } = new Ping_MongoDB();

        private static CamInfoContext dbContext = new();

        private static CameraService _service = new(dbContext);

        private static AvConfigService _avConfigService = new(dbContext);

        private static List<Camera> cameras = new();

        private static List<Config> avConfigs = new();

        private static Dictionary<int, Task> pingTasks = new();

        public static List<Config> configs = dbContext.Configs.ToList();

        public static async void InitializeCameraPing()
        {
            try
            {
                //Return the disponibility of every config associated with a camera on the database.
                await ReturnDisponibility();
                //Get all cameras from renovias database
                cameras = _service.GetCameras();
                if (cameras.Any())
                    PingConfiguredCameras();

            }
            catch (PingException pEx)
            {
                Console.WriteLine(pEx.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        private static async Task ReturnDisponibility()
        {
            avConfigs = await _avConfigService.GetConfigs();
            if (!avConfigs.Any())
                throw new PingException("No config found.");
            foreach(Config conf in avConfigs)
            {
                float percentDisponibility = MongoDbManager.GetDisponibility(conf.Id);
                conf.Value = percentDisponibility;
                _ = _avConfigService.PutConfig(conf.Id, conf);
                Console.WriteLine($"Config {conf.Id} has {percentDisponibility}% disponibility.");
            }       
        }

        private static void PingConfiguredCameras()
        {
            if (!configs.Any())
                throw new PingException("No configuration found.");

            foreach (Camera camera in cameras)
            {
                foreach (Config config in configs)
                {
                    if (camera.AvailabilityConfigs.Contains(config))
                    {
                        Task t = Task.Run(() => PingConfiguredCamera(config, camera));

                        if (!pingTasks.ContainsKey(config.Id))
                            pingTasks.Add(config.Id, t);
                    }
                }
            }
        }

        /// <summary>
        /// Ping all configured cameras from oracle Db.
        /// </summary>
        /// <param name="config">Config object to specify ping parameters.</param>
        /// <param name="camToPing">Camera object to ping.</param>
        private static void PingConfiguredCamera(Config config, Camera camToPing)
        {
            //Creates new document template to save on Mongo DB.
            Ping_MongoDB mongoDoc = new Ping_MongoDB();

            DateTime intervalToPing = DateTime.Now;
            mongoDoc.AvailabilityConfig = config.Id;

            mongoDoc.Counter = MongoDbManager.DocumentLastCount(config.Id);
            if (mongoDoc.Counter < 0)
                mongoDoc.Counter = 0;
       
            //mongoDoc = (Config)bson;
            Ping PingSender = new Ping();
            while (true)
            {
                if (intervalToPing.AddMilliseconds(config.PingTime) < DateTime.Now)
                {
                    intervalToPing = DateTime.Now;

                    //Initalize async task to ping.
                    Task.Run(async () =>
                    {
                        try
                        {
                            DateTime pingTime = DateTime.UtcNow;
                            //string formattedPingTime = FormatDate.ToLocalTime(pingTime);
                            mongoDoc.DateTime = DateTime.UtcNow;

                            //Get the Ping response.                  
                            PingReply PingReply = PingSender.Send(camToPing.Ip); //PingCamera(camToPing.Ip);
                            mongoDoc.Counter++;

                            if (PingReply.Status == IPStatus.Success)
                            {
                                config.currentStatus = "online";
                                Console.WriteLine($"Ping to {camToPing.Ip} with config {config.Id} was successful.");
                            }
                            else
                            {
                                config.currentStatus = "offline";
                                Console.WriteLine($"Failed to PING the IP: {camToPing.Ip} with config {config.Id}");
                            }

                            mongoDoc.Status = config.currentStatus;
                            MongoDbManager.SaveToMongo(mongoDoc);
                        }
                        catch (PingException pEx)
                        {
                            Console.WriteLine($"Error Message: {pEx.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Unexpected error ocurred: {ex.Message}");
                        }
                    });
                }
                Thread.Sleep(200);
            }
        }
    }
}
