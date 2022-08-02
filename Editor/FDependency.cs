namespace SimpleMan.Installer
{
    /// <summary>
    /// Contains info about the dependency asset/plugin
    /// </summary>
    public struct FDependency
    {
        public string name;
        public string path;
        public string url;


        public FDependency(string name, string path, string url)
        {
            this.name = name;
            this.path = path;
            this.url = url;
        }
    }
}