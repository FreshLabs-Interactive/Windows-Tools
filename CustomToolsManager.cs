// In CustomToolsManager.cs
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace WindowsToolsLauncher
{
    public class CustomTool
    {
        public string Name { get; set; }
        public string Command { get; set; }
    }

    public static class CustomToolsManager
    {
        private static readonly string FilePath = Path.Combine(
            System.AppDomain.CurrentDomain.BaseDirectory, "customtools.json");

        public static List<CustomTool> Load()
        {
            if (!File.Exists(FilePath))
                return new List<CustomTool>();
            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<List<CustomTool>>(json)
                       ?? new List<CustomTool>();
            }
            catch { return new List<CustomTool>(); }
        }

        public static void Save(List<CustomTool> tools)
        {
            string json = JsonConvert.SerializeObject(tools, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }
    }
}