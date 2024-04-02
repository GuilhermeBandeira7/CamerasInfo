using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using CamerasInfo.Model;
using System.Xml.Serialization;
using CamerasInfo.Service;
using CamerasInfo.Context;
using CamerasInfo.Camera;
using CamerasInfo.Helpers;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Diagnostics;

namespace CamerasInfo.Managers
{
    public static class CamManager
    {
        private  static Ping PingSender { get; set; } = new Ping();

        public static PingReply? PingReply { get; set; }

        public static Ping_MongoDB PingMongoDB { get; set; } = new Ping_MongoDB();

        private static CamInfoContext dbContext = new CamInfoContext();

        private static CameraService _service = new CameraService(dbContext);

        private static List<CameraInfo> Cameras = new List<CameraInfo>();

        public static async Task InitializeCameraPing()
        {
            try
            {
                //Get all cameras from renovias database
                Cameras = await _service.GetCameras();

                if (Cameras.Any())
                {
                    await PingCamerasFromDb();
                }
            }
            catch (PingException pEx)
            {
                Console.WriteLine(pEx.Message);
            }
        }

        private static async Task<PingReply> PingCamera(string address)
        {
            try
            {
                return await PingSender.SendPingAsync(address);             
            }
            catch (PingException pEx)
            {
                throw new PingException($"Failed to PING the IP: {address} " +
                    $"\nError Message: " + pEx.Message );
            }
        }

        private static async Task PingCamerasFromDb()
        {
            //Ping all cameras at a range set by intervalToPing
            DateTime intervalToPing = DateTime.Now;
            intervalToPing.AddMilliseconds(1000);

            int cont = 0;
            while (true)
            {           
                if(intervalToPing < DateTime.Now)
                {
                    intervalToPing = DateTime.Now;
                    cont = 0;

                    //Keeps track of the time took to ping all cameras.
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    foreach (CameraInfo cam in Cameras)
                    { 
                        await Task.Run(async () =>
                        {
                            try
                            {
                                DateTime pingTime = DateTime.UtcNow;
                                string formattedPingTime = FormatDate.ToLocalTime(pingTime);

                                PingReply = await PingCamera(cam.Ip);

                                if (PingReply.Status == IPStatus.Success)
                                {
                                    Console.WriteLine($"Ping to {cam.Ip} was successful.");
                                    MongoDbManager.SaveToMongo(cam.Ip, formattedPingTime, true);
                                    cont++;
                                }
                            }
                            catch (PingException pEx) 
                            {
                                throw (pEx);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Unexpected error ocurred: {ex.Message}");
                            }
                        });
                    }
                    stopwatch.Stop();
                    // Get the elapsed time the operation took to complete.
                    TimeSpan elapsedTime = stopwatch.Elapsed;

                    Console.WriteLine("Cont: " + cont.ToString() + " Elapsed time: " + elapsedTime.ToString());
                }
                Thread.Sleep(200);
            }
        }
    }
}
