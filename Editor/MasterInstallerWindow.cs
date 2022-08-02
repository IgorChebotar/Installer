using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SimpleMan.Installer
{
    public class InstallWindow : EditorWindow
    {
        //------FIELDS
        private const string DEPENDENCIES_JSON_PATH = "/Plugins/SimpleMan/Installer/Dependencies.json";
        private static FPluginData[] _tabDatas = System.Array.Empty<FPluginData>();
        private static GUIStyle _labelStyle = new GUIStyle();
        private static Color _unpressedTabButtonColor = new Color(0.8f, 0.8f, 0.8f);




        //------PROPERTIES
        public static int CurrentTabIndex { get; private set; } = 0;




        //------METHODS
        [MenuItem("Tools/Simple Man/Master Installer", priority = 11)]
        public static void Init()
        {
            InstallWindow window = (InstallWindow)EditorWindow.GetWindow(typeof(InstallWindow));
            window.titleContent = new GUIContent("Simple Man solutions installer");
            window.minSize = new Vector2(600, 400);
            window.Show();

            string jsonText = File.ReadAllText(Application.dataPath + DEPENDENCIES_JSON_PATH);
            _tabDatas = JsonConvert.DeserializeObject<FPluginData[]>(jsonText);

            _labelStyle.fontSize = 15;
            _labelStyle.fontStyle = FontStyle.Bold;
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            _labelStyle.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
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

                FPluginData currentTabData = _tabDatas[CurrentTabIndex];
                bool[] dependenciesExistState = GetDependenciesInstalledState(currentTabData.dependencies);
                bool allright = dependenciesExistState.All(x => x == true);

                void DrawHead()
                {
                    GUILayout.Space(20);
                    GUILayout.Label(currentTabData.name, _labelStyle);
                    GUILayout.Space(10);
                    GUILayout.Label("Dependencies:");
                }
                DrawHead();

                void DrawDependencies()
                {

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

                    if (IsImported(currentTabData.mainPackagePath))
                    {
                        string buttonName = IsIstalled(currentTabData.path) ? "Reinstall" : "Install";
                        if (GUILayout.Button(buttonName, GUILayout.Height(30)))
                        {
                            AssetDatabase.ImportPackage(currentTabData.mainPackagePath, true);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Download", GUILayout.Height(30)))
                        {
                            Application.OpenURL(currentTabData.downloadURL);
                        }
                    }

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
                    GUI.enabled = true;
                }
                DrawInstallButton();

                GUILayout.EndVertical();
            }
            DrawBody();

            GUILayout.EndHorizontal();
        }

        protected bool[] GetDependenciesInstalledState(FDependency[] datas)
        {
            bool[] result = new bool[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                result[i] = IsIstalled(datas[i].path);
            }

            return result.ToArray();
        }

        private bool IsImported(string packagePath)
        {
            var mainPackage = AssetDatabase.LoadAssetAtPath<Object>(packagePath);
            return mainPackage != null;
        }

        protected bool IsIstalled(string path)
        {
            string[] subfolders = AssetDatabase.GetSubFolders(path);
            return subfolders.Length > 1;
        }
    }
}