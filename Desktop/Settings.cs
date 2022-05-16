using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desktop;

public class Settings
{
    public string Engine1 { get; set; }
    public string Engine2 { get; set; }

    private const string fileName = "Settins.json";

    public void Save()
    {
        var s = JsonSerializer.Serialize(this);
        File.WriteAllText(fileName, s);
    }

    public static Settings Load()
    {
        if (File.Exists(fileName))
        {
            var s = File.ReadAllText(fileName);
            return (Settings)JsonSerializer.Deserialize(s, typeof(Settings));
        }

        return new Settings();

    }

}
