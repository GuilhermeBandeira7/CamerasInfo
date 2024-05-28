using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerasInfo.Model
{
    public class CameraHelper
    {
        public int CameraID { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
        public DateTime LastVerification { get; set; }
    }
}
