namespace SimpleMan.Installer
{
    /// <summary>
    /// Contains plugin's info and dependencies
    /// </summary>
    public struct FPluginData
    {
        public string name;
        public string downloadURL;
        public string documentationURL;
        public string path;
        public string mainPackagePath;
        public string demoPackagePath;
        public FDependency[] dependencies;

        public FPluginData(string name, string downloadURL, string documentationURL, string path, string mainPackagePath, string demoPackagePath, FDependency[] dependencies)
        {
            this.name = name;
            this.downloadURL = downloadURL;
            this.path = path;
            this.documentationURL = documentationURL;
            this.mainPackagePath = mainPackagePath;
            this.demoPackagePath = demoPackagePath;
            this.dependencies = dependencies ?? System.Array.Empty<FDependency>();
        }
    }
}