using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleMan.Installer
{
    public class MainInstallWindow : EditorWindow
    {
        private const string DEPENDENCIES_JSON_PATH = "/Plugins/SimpleMan/Installer/Dependencies.json";
        private static PluginData[] _tabDatas = System.Array.Empty<PluginData>();
        private static GUIStyle _labelStyle = new GUIStyle();
        private static Color _unpressedTabButtonColor = new Color(0.8f, 0.8f, 0.8f);




        public static int CurrentTabIndex { get; private set; } = 0;




        [MenuItem("Tools/Simple Man/Main Installer", priority = 11)]
        public static void Init()
        {
            MainInstallWindow window = (MainInstallWindow)EditorWindow.GetWindow(typeof(MainInstallWindow));
            window.titleContent = new GUIContent("Simple Man solutions installer");
            window.minSize = new Vector2(600, 400);
            window.Show();

            string jsonText = File.ReadAllText(Application.dataPath + DEPENDENCIES_JSON_PATH);
            _tabDatas = JsonUtility.FromJson<PluginsCollection>(jsonText).datas;

            _labelStyle.fontSize = 15;
            _labelStyle.fontStyle = FontStyle.Bold;
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            _labelStyle.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            if (_tabDatas.Length == 0)
            {
                Init();
                return;
            }

            Color defaultColor = GUI.color;
            GUILayout.BeginHorizontal();
            
            void DrawTabSelector()
            {
                GUILayout.BeginVertical(GUILayout.Width(200));
                GUILayout.Space(20);
                for (int i = 0; i < _tabDatas.Length; i++)
                {
                    if (CurrentTabIndex != i)
                        GUI.color = _unpressedTabButtonColor;

                    if (GUILayout.Button(_tabDatas[i].name, GUILayout.Height(30)))
                    {
                        CurrentTabIndex = i;
                    }
                    GUI.color = defaultColor;
                }
                GUILayout.EndVertical();
            }
            DrawTabSelector();

            void DrawBody()
            {
                GUILayout.BeginVertical();

                PluginData currentTabData = _tabDatas[CurrentTabIndex];
                bool[] dependenciesExistState = GetDependenciesInstalledState(currentTabData.dependencies);
                bool allright = dependenciesExistState.All(x => x == true);

                void DrawHead()
                {
                    GUILayout.Space(20);
                    GUILayout.Label(currentTabData.name, _labelStyle);
                    GUILayout.Space(10);

                    if (currentTabData.dependencies != null)
                        GUILayout.Label("Dependencies:");
                }
                DrawHead();

                void DrawDependencies()
                {
                    if (currentTabData.dependencies == null)
                        return;

                    for (int i = 0; i < currentTabData.dependencies.Length; i++)
                    {
                        GUILayout.BeginHorizontal();

                        GUI.enabled = false;
                        EditorGUILayout.Toggle($" - {currentTabData.dependencies[i].name}", dependenciesExistState[i]);
                        GUI.enabled = true;

                        if (GUILayout.Button("Download", GUILayout.Width(100)))
                        {
                            Application.OpenURL(currentTabData.dependencies[i].url);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                DrawDependencies();

                void DrawInstallButton()
                {
                    GUILayout.Space(20);
                    if (!allright)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.HelpBox(
                            "One or more of dependencies was not found. " +
                            "Import and install the dependencies first", MessageType.Error);
                    }


                    if (IsImported(currentTabData.path))
                    {
                        string buttonName = IsIstalled(currentTabData.path) ? "Reinstall" : "Install";
                        if (GUILayout.Button(buttonName, GUILayout.Height(30)))
                        {
                            AssetDatabase.ImportPackage(currentTabData.path + "/Editor/MainPackage.unitypackage", true);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Download", GUILayout.Height(30)))
                        {
                            Application.OpenURL(currentTabData.downloadURL);
                        }
                    }
                    GUI.enabled = true;

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Documentation", GUILayout.Height(20)))
                    {
                        Application.OpenURL(currentTabData.documentationURL);
                    }

                    if (GUILayout.Button("Check updates", GUILayout.Height(20)))
                    {
                        Application.OpenURL(currentTabData.downloadURL);
                    }
                    GUILayout.EndHorizontal();
                }
                DrawInstallButton();

                GUILayout.EndVertical();
            }
            DrawBody();

            GUILayout.EndHorizontal();
        }

        protected bool[] GetDependenciesInstalledState(PluginDependencyData[] datas)
        {
            if (datas == null)
                return new bool[] { true };

            bool[] result = new bool[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                result[i] = IsIstalled(datas[i].path);
            }

            return result.ToArray();
        }

        protected bool IsIstalled(string path)
        {
            string[] subfolders = AssetDatabase.GetSubFolders(path);
            return subfolders.Length > 1;
        }

        private bool IsImported(string path)
        {
            string packagePath = path + "/Editor/MainPackage.unitypackage";
            var mainPackage = AssetDatabase.LoadAssetAtPath<Object>(packagePath);
            return mainPackage != null;
        }

        private void CreateEmptyDependenciesFile()
        {
            _tabDatas = new PluginData[]
            {
                new PluginData(
                    "Utilities",
                    "DownloadURL",
                    "DocURL",
                    "FolderPath",
                    new PluginDependencyData[] { new PluginDependencyData("Depencency name", "Dependency path", "DepencecyURL")})
            };

            PluginsCollection collection = new PluginsCollection(_tabDatas);
            string jsonText = JsonUtility.ToJson(collection, true);
            File.WriteAllText(Application.dataPath + DEPENDENCIES_JSON_PATH, jsonText);
        }
    }
}