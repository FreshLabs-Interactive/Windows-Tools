using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsToolsLauncher
{
    public partial class MainWindow : Window
    {
        // Tools that are 64‑bit only (or not present in SysWOW64) and need the real System32
        private static readonly HashSet<string> NeedRealSystem32 = new HashSet<string>
        {
            "msconfig.exe",
            "cleanmgr.exe",   // Disk Cleanup – often 64‑bit only
            "osk.exe",        // On‑Screen Keyboard
            "narrator.exe",   // Narrator
            "magnify.exe",     // removed from list, but here for completeness
            "rstrui.exe"       // System Restore – often 64‑bit only
        };

        public MainWindow()
        {
            InitializeComponent();
            PopulateToolButtons();
        }

        /// <summary>
        /// Returns the real C:\Windows\System32 even when called from a 32-bit process.
        /// </summary>
        private static string GetRealSystem32Path()
        {
            // If we're 32‑bit and the OS is 64‑bit, use the Sysnative alias.
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                return @"C:\Windows\Sysnative";
            // Otherwise System32 is directly accessible.
            return Environment.GetFolderPath(Environment.SpecialFolder.System);
        }

        /// <summary>
        /// Resolves a command to the correct full path if it's a system tool.
        /// </summary>
        private static string ResolveCommand(string command)
        {
            // If it's already a full path, return as‑is
            if (Path.IsPathRooted(command))
                return command;

            // If the command is one that requires the real System32, fix its path.
            if (NeedRealSystem32.Contains(command.ToLowerInvariant()))
            {
                return Path.Combine(GetRealSystem32Path(), command);
            }

            // For everything else, just use the simple name – the shell will find it.
            return command;
        }

        private void PopulateToolButtons()
        {
            var tools = new (string Name, string Command)[]
            {
                ("Character Map",        "charmap"),
                ("Control Panel",        "control"),
                ("Device Manager",       "devmgmt.msc"),
                ("Disk Management",      "diskmgmt.msc"),
                ("Event Viewer",         "eventvwr.msc"),
                ("Task Manager",         "taskmgr"),
                ("Registry Editor",      "regedit"),
                ("System Information",   "msinfo32"),
                ("Resource Monitor",     "resmon"),
                ("Performance Monitor",  "perfmon"),
                ("Services",             "services.msc"),
                ("Computer Management",  "compmgmt.msc"),
                ("Command Prompt",       "cmd"),
                ("PowerShell",           "powershell"),
                ("Disk Cleanup",         "cleanmgr.exe"),
                ("Notepad",              "notepad"),
                ("Calculator",           "calc"),
                ("Paint",                "mspaint"),
                ("Snipping Tool",        "snippingtool"),
                ("Windows Features",     "optionalfeatures"),
                ("System Configuration", "msconfig.exe"),   // ✅ now works
                ("Task Scheduler",       "taskschd.msc"),
                ("Windows Firewall",     "wf.msc"),
                ("Local Users and Groups","lusrmgr.msc"),
                ("On-Screen Keyboard",   "osk.exe"),
                ("System Restore",       "rstrui.exe"),
                ("Narrator",             "narrator.exe"),
                ("Temp Folder",          Environment.GetEnvironmentVariable("TEMP")),
                
                ("Disk Defragmenter",      "dfrgui.exe"),
                
                
                ("User Accounts",           "netplwiz.exe"),
                ("Create Restore Point",        "SystemPropertiesProtection.exe"),
                ("Create EXE from File", "iexpress.exe")
                
                
            };

            foreach (var (name, command) in tools)
            {
                // Resolve the correct command before storing it
                string finalCommand = ResolveCommand(command);

                var btn = new Button
                {
                    Content = name,
                    Tag = finalCommand,
                    Width = 150,
                    Height = 40,
                    Margin = new Thickness(5),
                    FontSize = 13,
                    Background = new SolidColorBrush(Color.FromRgb(64, 64, 64)),
                    Foreground = Brushes.White,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
                    BorderThickness = new Thickness(1),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    ToolTip = $"{name}"
                   
                };
                
                btn.MouseEnter += (s, e) =>
                    btn.Background = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                btn.MouseLeave += (s, e) =>
                    btn.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));

                btn.Click += ToolButton_Click;

                ToolsWrapPanel.Children.Add(btn);
            }
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string command)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = command,
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to launch '{btn.Content}'\nError: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}