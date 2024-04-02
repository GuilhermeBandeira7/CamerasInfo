using CamerasInfo.Camera;
using CamerasInfo.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerasInfo.Service
{
    public class AvConfigService
    {
        CamInfoContext _context;

        public AvConfigService(CamInfoContext context)
        {
            _context = context;
        }

        //public async Task<AvailabilityConfig> GetConfig()
        //{
        //    List<AvailabilityConfig>? config = await _context.AvailabilityConfigs.ToListAsync();
        //}
    }
}
