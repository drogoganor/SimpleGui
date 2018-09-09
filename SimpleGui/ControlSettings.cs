using Newtonsoft.Json;
using System;
using System.Numerics;

namespace SimpleGui
{
    [Serializable]
    public class ControlSettings
    {
        [JsonIgnore]
        public ControlColorTheme Colors { get; set; } = new ControlColorTheme();

        public ControlSettings()
        {

        }
    }
}
