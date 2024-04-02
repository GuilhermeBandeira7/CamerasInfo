using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerasInfo.Camera
{
    public class CameraInfo
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string Highway { get; set; } = string.Empty;
        public string Parents { get; set; } = string.Empty;
        public string LATLNG { get; set; } = string.Empty;

        public List<AvailabilityConfig> AvailabilityConfigs { get; set; } = new();
    }
}
