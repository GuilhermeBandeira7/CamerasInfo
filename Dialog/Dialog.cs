using CamerasInfo.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CamerasInfo.Dialog
{
    public static class Dialog
    {
        public static void InitialDialogAsync()
        {
            Console.WriteLine("=======================");
            Console.WriteLine("Welcome to CamerasInfo");
            Console.WriteLine("=======================\n");

            CamManager.InitializeCameraPing();
        }
    }
}
