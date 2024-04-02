using CamerasInfo.Camera;
using CamerasInfo.Context;
using CamerasInfo.Service;
using CamerasInfo.Dialog;

namespace CamerasInfo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Dialog.Dialog.InitialDialogAsync();
        }
    }
}