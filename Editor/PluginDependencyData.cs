using System;

namespace SimpleMan.Installer
{
    [Serializable]
    /// <summary>
    /// Contains info about the dependency asset/plugin
    /// </summary>
    public struct PluginDependencyData
    {
        public string name;
        public string path;
        public string url;


        public PluginDependencyData(string name, string path, string url)
        {
            this.name = name;
            this.path = path;
            this.url = url;
        }
    }
}