using System;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET
{
    public static class ElectronApp
    {
        public static BrowserWindow MainWindow { get; private set; }

        public static void OnReadyToShow()
        {
            MainWindow.Show();
        }

        public static async Task OnSayHello(object args)
        {
            MessageBoxResult result = await Electron.Dialog.ShowMessageBoxAsync(MainWindow, new MessageBoxOptions("Hi! How are you doing today?") {
                Title = "Message from Window Process!",
                Buttons = new[]{"Fine!", "So so..."},
                Type = MessageBoxType.question
            });

            Electron.IpcMain.Send(MainWindow, 
                                  "say-hello-response", 
                                  result.Response == 0 ? "I'm Fine!" : "Could be better...");
        }

        public static async Task Run()
        {
            var options = new BrowserWindowOptions
            {
                Show = false
            };
            MainWindow = await Electron.WindowManager.CreateWindowAsync(options);
            MainWindow.OnReadyToShow += OnReadyToShow;
            MainWindow.OnClose += OnClose;

            Electron.IpcMain.On("say-hello", async (args) => await OnSayHello(args));
        }

        private static void OnClose()
        {
            Electron.App.Quit();
        }
    }
}