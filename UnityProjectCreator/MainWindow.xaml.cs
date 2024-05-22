using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UnityProjectCreator.Util;

namespace UnityProjectCreator
{
    public partial class MainWindow : Window
    {
        private bool running;
        private bool useKeyboard;
        private bool useTmeout;

        private string? projectName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ValidProjectName()
        {
            if (projectName == null || projectName.Length < 4 || projectName.Length > 50)
                throw new Exception($"Invalid project name.\nNumber of characters must be greater than 4 and less than 50.");
        }

        private void CreateDirectory()
        {
            if (Directory.Exists(projectName))
                Directory.Delete(projectName, true);
            
            if (projectName != null)
                Directory.CreateDirectory(projectName);
        }

        private void CopyAssets(bool standalone)
        {
            FileUtil.CopyDirectory("_Template/Packages", $"{projectName}/Packages", true);
            FileUtil.CopyDirectory("_Template/ProjectSettings", $"{projectName}/ProjectSettings", true);
            FileUtil.CopyDirectory("_Template/Assets/_Addressables", $"{projectName}/Assets/_Addressables", true);
            FileUtil.CopyDirectory("_Template/Assets/_BuiltIn", $"{projectName}/Assets/_BuiltIn", true);
            FileUtil.CopyDirectory("_Template/Assets/_External", $"{projectName}/Assets/_External", true);
            FileUtil.CopyDirectory("_Template/Assets/AddressableAssetsData", $"{projectName}/Assets/AddressableAssetsData", true);
            FileUtil.CopyDirectory("_Template/Assets/TextMesh Pro", $"{projectName}/Assets/TextMesh Pro", true);
            FileUtil.CopyDirectory("_Template/Assets/Resources/Manager/Popup", $"{projectName}/Assets/Resources/Manager/Popup", true);
            FileUtil.CopyDirectory("_Template/Assets/Resources/Manager/Keyboard", $"{projectName}/Assets/Resources/Manager/Keyboard", true);
            FileUtil.CopyDirectory("_Template/Assets/Resources/Fonts & Materials", $"{projectName}/Assets/Resources/Fonts & Materials", true);
            FileUtil.CopyDirectory("_Template/Assets/_Scripts", $"{projectName}/Assets/_Scripts", true);

            if (!useKeyboard)
            {
                Directory.Delete($"{projectName}/Assets/_Scripts/Manager/Keyboard", true);

                if (File.Exists($"{projectName}/Assets/_Scripts/Manager/Keyboard.meta"))
                    File.Delete($"{projectName}/Assets/_Scripts/Manager/Keyboard.meta");

                Directory.Delete($"{projectName}/Assets/Resources/Manager/Keyboard", true);

                if (File.Exists($"{projectName}/Assets/Resources/Manager/Keyboard.meta"))
                    File.Delete($"{projectName}/Assets/Resources/Manager/Keyboard.meta");

                string initializeManager = File.ReadAllText($"{projectName}/Assets/_Scripts/Manager/InitializerManager.cs");
                initializeManager = initializeManager.Replace("\r\nusing Assets._Scripts.Manager.Keyboard;", "");
                initializeManager = initializeManager.Replace("\r\n            KeyboardManager.InstanceNW(this);", "");

                File.WriteAllText($"{projectName}/Assets/_Scripts/Manager/InitializerManager.cs", initializeManager);
            }

            if (!useTmeout)
            {
                Directory.Delete($"{projectName}/Assets/_Scripts/Manager/Timeout", true);

                if (File.Exists($"{projectName}/Assets/_Scripts/Manager/Timeout.meta"))
                    File.Delete($"{projectName}/Assets/_Scripts/Manager/Timeout.meta");

                string initializeManager = File.ReadAllText($"{projectName}/Assets/_Scripts/Manager/InitializerManager.cs");
                initializeManager = initializeManager.Replace("\r\nusing Assets._Scripts.Manager.Timeout;", "");
                initializeManager = initializeManager.Replace("\r\n            TimeoutManager.InstanceNW(this);", "");

                File.WriteAllText($"{projectName}/Assets/_Scripts/Manager/InitializerManager.cs", initializeManager);
            }

            if (standalone)
            {
                FileUtil.CopyDirectory("_Template/Assets/StreamingAssets/Manager/Language", $"{projectName}/Assets/StreamingAssets/Manager/Language", true);
                FileUtil.CopyDirectory("_Template/Assets/StreamingAssets/Manager/Setting", $"{projectName}/Assets/StreamingAssets/Manager/Setting", true);

                if (useKeyboard)
                    FileUtil.CopyDirectory("_Template/Assets/StreamingAssets/Manager/Keyboard", $"{projectName}/Assets/StreamingAssets/Manager/Keyboard", true);
            }
            else
            {
                FileUtil.CopyDirectory("_Template/Assets/StreamingAssets/Manager/Language", $"{projectName}/Assets/Resources/Manager/Language", true);
                FileUtil.CopyDirectory("_Template/Assets/StreamingAssets/Manager/Setting", $"{projectName}/Assets/Resources/Manager/Setting", true);

                if (useKeyboard)
                    FileUtil.CopyDirectory("_Template/Assets/StreamingAssets/Manager/Keyboard", $"{projectName}/Assets/Resources/Manager/Keyboard", true);
            }
        }

        private void DeleteMeta()
        {
            FileUtil.DeleteFiles("*.meta", "_Template", true);
        }

        private async Task RunAction(Action action)
        {
            if (running)
                return;

            running = true;

            useKeyboard = KeyboardManagerCkb.IsChecked != null && (bool)KeyboardManagerCkb.IsChecked;
            useTmeout = TimeoutManagerCkb.IsChecked != null && (bool)TimeoutManagerCkb.IsChecked;
            projectName = ProjectTbk.Text;

            bool complete = false;

            Status.Text = "RUNNING...";

            new Thread(() =>
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                complete = true;
            }).Start();

            while (!complete)
                await Task.Delay(10);

            running = false;

            Status.Text = "COMPLETE";
        }

        private async void DeleteMetaBtn_Click(object sender, RoutedEventArgs e)
        {
            await RunAction(DeleteMeta);
        }

        private async void StandAloneBtn_Click(object sender, RoutedEventArgs e)
        {
            await RunAction(() => 
            {
                ValidProjectName();
                CreateDirectory();
                CopyAssets(true);
            });
        }

        private async void MobileBtn_Click(object sender, RoutedEventArgs e)
        {
            await RunAction(() =>
            {
                ValidProjectName();
                CreateDirectory();
                CopyAssets(false);
            });
        }
    }
}
