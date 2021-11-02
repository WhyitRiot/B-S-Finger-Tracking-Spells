using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace GestureCasting
{
    public class GestureCastController : MonoBehaviour
    {
        private string fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Settings/Settings.json");
        public GestureCastData data = new GestureCastData();
        public void Serialize(GestureCastData data)
        {
            string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(fileName, jsonString);
        }
        public void Deserialize()
        {
            string settingsFile = File.ReadAllText(fileName);
            GestureCastData settings = JsonConvert.DeserializeObject<GestureCastData>(settingsFile);
            this.data = settings;
        }
    }
}
