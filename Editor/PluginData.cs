using System;

namespace SimpleMan.Installer
{
    [Serializable]
    /// <summary>
    /// Contains plugin's info and dependencies
    /// </summary>
    public struct PluginData
    {
        public string name;
        public string downloadURL;
        public string documentationURL;
        public string path;
        public PluginDependencyData[] dependencies;

        public PluginData(string name, string downloadURL, string documentationURL, string path, PluginDependencyData[] dependencies)
        {
            this.name = name;
            this.downloadURL = downloadURL;
            this.path = path;
            this.documentationURL = documentationURL;
            this.dependencies = dependencies ?? System.Array.Empty<PluginDependencyData>();
        }
    }

    [Serializable]
    public struct PluginsCollection
    {
        public PluginData[] datas;

        public PluginsCollection(PluginData[] datas)
        {
            this.datas = datas;
        }
    }
}