using CamerasInfo.Camera;
using CamerasInfo.Context;
using CamerasInfo.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerasInfo.Service
{
    public class CameraService : IDisposable
    {
        CamInfoContext _context;

        public CameraService(CamInfoContext context)
        {
            _context = context;
        }

        //Whoever calls this function needs to make sure that returned != null
        public async Task<List<CameraInfo>> GetCameras()
        {
            try
            {
                List<CameraInfo> cameras = await _context.Camera.ToListAsync();
                if(!cameras.Any())
                {
                    throw new Exception("No Cameras found.");
                }
                return cameras;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new();
            }
        }

        //Whoever calls this functions needs to make sure that returned != null
        public async Task<CameraInfo> GetCameraById(long id)
        {
            try
            {
                CameraInfo? camera = await _context.Camera.Where(cam => cam.Id == id).FirstOrDefaultAsync();
                if(camera != null)
                    return camera;
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new CameraInfo();
            }
        }

        public async Task<Response> PutCamera(long id, CameraInfo camInfo)
        {
            try
            {
                CameraInfo? cam = await _context.Camera.Where(cam => cam.Id == id).FirstOrDefaultAsync();
                if(cam != null)
                {
                    _context.Entry(camInfo).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return new Response(true, "Camera updated.");
                }
                else
                    throw new Exception("Failed to update.");
            }
            catch (Exception ex)
            {
                return new Response (false, ex.Message);
            }
        }

        public async Task<Response> PostCamera(CameraInfo camInfo)
        {
            try
            {
                await _context.Camera.AddAsync(camInfo);
                await _context.SaveChangesAsync();
                return new Response(true, "Camera added successfully.");
            }
            catch (Exception ex)
            {
                return new Response(false, ex.Message);
            }
        }

        public async Task<Response> DeleteCamera(long id)
        {
            try
            {
                await _context.Camera.Where(cam => cam.Id == id).ExecuteDeleteAsync();
                await _context.SaveChangesAsync();
                return new Response(true, "Camera deleted successfully.");
            }
            catch (Exception ex) 
            {
                return new Response(false, ex.Message);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
