using System.IO;
using Newtonsoft.Json;

namespace csutil
{
    public class Settings : SettingsData
    {
        public Settings(string fileName = "settings.json", bool useEncrypted = true)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException("Settings file not found");
            var settings = useEncrypted
                ? Encryption.OpenFile(fileName, "password", "iv")
                : File.ReadAllText(fileName);
            var data = JsonConvert.DeserializeObject<SettingsData>(settings);
            Update(data);
        }

        private void Update(SettingsData data)
        {
            ExampleSetting = data.ExampleSetting;
        }
    }

    public class SettingsData
    {
        public ExampleSetting ExampleSetting { get; set; }
    }

    public class ExampleSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}