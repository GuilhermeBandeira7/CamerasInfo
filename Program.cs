using CamerasInfo.Context;
using CamerasInfo.Service;
using CamerasInfo.Dialog;
using CamerasInfo.Managers;

namespace CamerasInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            Dialog.Dialog.InitialDialogAsync();
            ThreadPool.SetMinThreads(400, 400);

            while (true)
            {
                Thread.Sleep(100);
            }

        }
    }
}