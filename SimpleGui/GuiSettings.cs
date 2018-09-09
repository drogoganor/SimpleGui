using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleGui
{
    [Serializable]
    public class GuiSettings
    {
        public string Theme { get; set; } = "Default";

        public ControlSettings DefaultControlSettings { get; set; } = new ControlSettings();

        public Dictionary<string, ControlColorTheme> Themes { get; set; } = new Dictionary<string, ControlColorTheme>();

        public GuiSettings()
        {
            Themes.Add("Default", new ControlColorTheme());
        }
    }
}
