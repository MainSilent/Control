using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;

namespace Control
{
    class Movies_TV
    {
        public static int processId;

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        public async Task<object> send(dynamic input)
        {
            GetWindowThreadProcessId((IntPtr)input.handle, out processId);
            Process Proc = Process.GetProcessById(processId);
            var app = new FlaUI.Core.Application(Proc);
            var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            switch (input.type)
            {
                case "play":
                    window.FindFirstDescendant(cf => cf.ByAutomationId("MTV_PlayPauseButton")).Click();
                    break;
                case "+30":
                    window.FindFirstDescendant(cf => cf.ByAutomationId("MTV_SkipForwardButton")).Click();
                    break;
                case "-10":
                    window.FindFirstDescendant(cf => cf.ByAutomationId("MTV_SkipBackButton")).Click();
                    break;
                case "expand":
                    window.FindFirstDescendant(cf => cf.ByAutomationId("FullWindowButton")).Click();
                    break;
                case "volume":
                    window.FindFirstDescendant(cf => cf.ByAutomationId("VolumeMuteButton")).Click();
                    break;
                case "setProgress":
                    var elem = window.FindFirstDescendant(cf => cf.ByAutomationId("ProgressSlider"));
                    elem.Patterns.RangeValue.Pattern.SetValue(input.percent);
                    break;
                case "setVolume":
                    try
                    {
                        var element = window.FindFirstDescendant(cf => cf.ByAutomationId("VolumeSlider"));
                        element.Patterns.RangeValue.Pattern.SetValue(input.percent);
                    }
                    catch(Exception msg)
                    {
                        window.FindFirstDescendant(cf => cf.ByAutomationId("VolumeMuteButton")).Click();
                        var element = window.FindFirstDescendant(cf => cf.ByAutomationId("VolumeSlider"));
                        element.Patterns.RangeValue.Pattern.SetValue(input.percent);
                    }
                    break;
            }

            return null;
        }

        public async Task<object> getCurrent(int input)
        {
            GetWindowThreadProcessId((IntPtr)input, out processId);
            Process Proc = Process.GetProcessById(processId);
            ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
            var app = new FlaUI.Core.Application(Proc);
            var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);
            var name = window.FindFirstDescendant(cf.ByAutomationId("MetadataPrimaryTextBlock"));
            var current = window.FindFirstDescendant(cf.ByAutomationId("TimeElapsedElement"));
            var duration = window.FindFirstDescendant(cf.ByAutomationId("TimeRemainingElement"));

            return $"{{\"title\": \"{name.Name}\", \"duration\": \"{duration.Name}\", \"current\": \"{current.Name}\"}}";
        }

        public async Task<object> getProgress(int input)
        {
            GetWindowThreadProcessId((IntPtr)input, out processId);
            Process Proc = Process.GetProcessById(processId);
            ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
            var app = new FlaUI.Core.Application(Proc);
            var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);
            var element = window.FindFirstDescendant(cf.ByAutomationId("ProgressSlider"));

            return (int)element.Patterns.RangeValue.Pattern.Value;
        }

        public async Task<object> getVolume(int input)
        {
            GetWindowThreadProcessId((IntPtr)input, out processId);
            Process Proc = Process.GetProcessById(processId);
            ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
            var app = new FlaUI.Core.Application(Proc);
            var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);
            var element = window.FindFirstDescendant(cf.ByAutomationId("VolumeSlider"));
            
            return (int)element.Patterns.RangeValue.Pattern.Value;
        }
    }
}
