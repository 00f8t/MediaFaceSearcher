using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFaceSearcher.Model;
using Newtonsoft.Json;

namespace MediaFaceSearcher.DAL
{
    public class SettingsDao : ISettingsDao
    {
        private const string SettingsFilePath = "config.cfg";
        public Settings Read()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var text = File.ReadAllText(SettingsFilePath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var settings = JsonConvert.DeserializeObject<Settings>(text);
                        if (settings != null)
                        {
                            return settings;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                // Log the exception
                Debug.WriteLine($"Error reading settings: {ex.Message}");
            }

            return new();
        }

        public void Save(Settings settings)
        {
            var serialized = JsonConvert.SerializeObject(settings);
            File.WriteAllText(SettingsFilePath, serialized);
        }
    }
}
