using CamerasInfo.Context;
using CamerasInfo.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
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

        public async Task<Config> GetConfig(int configId)
        {
            try
            {
                Config? config = await _context.Configs.Where(c => c.Id == configId).FirstOrDefaultAsync();

                if (config != null)
                    return config;
                return new();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new();
            }
        }

        public async Task<List<Config>> GetConfigs()
        {
            try
            {
                List<Config>? config = await _context.Configs.ToListAsync();

                if (!config.Any())
                    throw new Exception("Any configuration was found.");
                return config;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new();

            }
        }

        public async Task<Response> PostConfig(Config config)
        {
            try
            {
                await _context.Configs.AddAsync(config);
                await _context.SaveChangesAsync();
                return new Response(true, "Config added successfully.");
            }
            catch (Exception ex)
            {
                return new Response(false, ex.Message);
            }
        }

        public async Task<Response> PutConfig(int configId, Config newConfig)
        {
            try
            {
                Config? conf = await _context.Configs.Where(cam => cam.Id == configId).FirstOrDefaultAsync();
                if (conf != null)
                {
                    _context.Entry(newConfig).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return new Response(true, "Config updated.");
                }
                else
                    throw new Exception("Failed to update.");
            }
            catch (Exception ex)
            {
                return new Response(false, ex.Message);
            }
        }

        public async Task<Response> DeleteConfig(int configId)
        {
            try
            {
                await _context.Configs.Where(conf => conf.Id == configId).ExecuteDeleteAsync();
                await _context.SaveChangesAsync();
                return new Response(true, "Config deleted successfully.");
            }
            catch (Exception ex) 
            {
                return new Response(false, ex.Message);
            }
        }
    }
}
