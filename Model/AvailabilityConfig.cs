using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerasInfo.Camera
{
    public class AvailabilityConfig
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Value { get; set; }
        public double PingTime { get; set; }
        public int PingsToOffline { get; set; }
        public double VerificationTime { get; set; }
        public string currentStatus { get; set; } = string.Empty;

        [ForeignKey("CameraInfoId")]
        public CameraInfo Camera { get; set; } = new();


    }
}
