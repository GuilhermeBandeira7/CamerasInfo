using System.Net.NetworkInformation;
using CamerasInfo.Service;
using CamerasInfo.Context;
using CamerasInfo.Model;
using System.Data;

namespace CamerasInfo.Managers
{
    public static class CamManager
    {
        public static Ping_MongoDB PingMongoDB { get; set; } = new Ping_MongoDB();

        private static readonly CamInfoContext dbContext = new();

        private static readonly CameraService _service = new(new());

        private static readonly AvConfigService _avConfigService = new(new());

        private static List<Camera> cameras = new();

        private static List<Config> avConfigs = new();

        private static Dictionary<int, Task> PingTasks = new();

        public static List<Config> Configs = dbContext.Configs.ToList();

        private static List<CameraHelper> CamHelper { get; set; } = new();

        public static void InitializeCameraPing()
        {
            try
            {
                //DbChangeTracker.InitializeWatcher();
                //Return the disponibility of every config associated with a camera on the database.
                Task.Run(ReturnDisponibility);
                //Get all cameras from renovias database
                cameras = _service.GetCameras();
                if (cameras.Any() && cameras != null)
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
            while(true)
            {
                avConfigs = await _avConfigService.GetConfigs();
                if (!avConfigs.Any())
                    throw new PingException("No config found.");

                foreach (Config conf in avConfigs)
                {
                    float percentDisponibility = MongoDbManager.GetDisponibility(conf.Id);
                    conf.Value = percentDisponibility;
                    _ = _avConfigService.PutConfig(conf.Id, conf);
                    Console.WriteLine($"Config {conf.Id} has {percentDisponibility}% disponibility.");
                    Thread.Sleep(100);
                }
                Thread.Sleep(5000);
            }
        }

        private static void PingConfiguredCameras()
        {
            if (!Configs.Any())
                throw new PingException("No configuration found.");

            Task.Run(UpdateStatusAndVerificationOfCameras);

            foreach (Camera camera in cameras)
            {
                foreach (Config config in Configs)
                {
                    Config cameraConfig = camera.AvailabilityConfigs.First();
                    if (camera.AvailabilityConfigs.Select(x => x.Id).Contains(config.Id))
                    {
                        Task t = Task.Run(() => PingConfiguredCamera(config, camera));

                        if (!PingTasks.ContainsKey(config.Id))
                            PingTasks.Add(config.Id, t);
                    }
                }
            }
        }

        private static async Task UpdateStatusAndVerificationOfCameras()
        {
            while (true)
            {
                foreach(Camera cam in cameras)
                {
                    try
                    {
                        if (CamHelper.Any())
                        {
                            CameraHelper? camera = CamHelper.Where(c => c.CameraID == cam.Id)
                                .OrderByDescending(c => c.LastVerification).FirstOrDefault();
                            if (camera == null)
                                continue;

                            cam.LastVerification = camera.LastVerification;
                            cam.Status = camera.Status;

                            await _service.PutCamera(cam.Id, cam);
                        }                         
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                lock(CamHelper)
                {
                    if(CamHelper.Count() > 0)   
                        CamHelper.Clear(); 
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
       
            while (true)
            {
                if (intervalToPing.AddMilliseconds(config.PingTime) < DateTime.Now)
                {
                    intervalToPing = DateTime.Now;

                    //I nitalize async task to ping.
                    Task.Run(async () =>
                    {
                        try
                        {
                            Ping PingSender = new Ping();
                            DateTime pingTime = DateTime.Now;
                            mongoDoc.DateTime = DateTime.Now;    

                            //Get the Ping response.                  
                            PingReply PingReply = PingSender.Send(camToPing.Ip); //PingCamera(camToPing.Ip);
                            mongoDoc.Counter++;
                            config.currentStatus = "offline";

                            if (PingReply.Status == IPStatus.Success)
                            {
                                config.currentStatus = "online";                              
                                Console.WriteLine($"Ping to {camToPing.Ip} with config {config.Id} was successful.");
                            }                                            
                        }
                        catch (PingException pEx)
                        {
                            Console.WriteLine($"Error Message: {pEx.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Unexpected error ocurred: {ex.Message}");
                        }
                        finally
                        {
                            mongoDoc.Status = config.currentStatus;
                            MongoDbManager.SaveToMongo(mongoDoc);

                            //intermediary object to help setting up status and last verification time on the database.
                            CameraHelper helper = new()
                            {
                                CameraID = camToPing.Id,
                                Status = config.currentStatus,
                                LastVerification = DateTime.UtcNow
                            };
                            lock (CamHelper)
                                CamHelper.Add(helper);
                        }
                    });
                }
                Thread.Sleep(200);
            }
        }
    }
}
