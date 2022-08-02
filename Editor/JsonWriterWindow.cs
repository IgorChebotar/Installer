using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;

namespace SimpleMan.Installer
{
    /// <summary>
    /// GUI Json utility. For internal using only
    /// </summary>
    internal sealed class PluginInfoWriterWindow : EditorWindow
    {
        //------FIELDS
        private const string DEPENDENCIES_JSON_PATH = "/Plugins/SimpleMan/Installer/Dependencies.json";
        private static FPluginData[] _datas = System.Array.Empty<FPluginData>();



        //------METHODS
        [MenuItem("Tools/Simple Man/Plugin Info Writer", priority = 11)]
        public static void Init()
        {
            if (EditorWindow.HasOpenInstances<PluginInfoWriterWindow>())
            {
                EditorWindow.FocusWindowIfItsOpen<PluginInfoWriterWindow>();
            }
            else
            {
                PluginInfoWriterWindow instance = (PluginInfoWriterWindow)EditorWindow.GetWindow(typeof(PluginInfoWriterWindow));
                instance.Show();
            }

            string jsonText = File.ReadAllText(Application.dataPath + DEPENDENCIES_JSON_PATH);
            _datas = JsonConvert.DeserializeObject<FPluginData[]>(jsonText);
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "This is internal config window for Simple Man team only!", MessageType.Warning);

            for (int i = 0; i < _datas.Length; i++)
            {
                GUILayout.Space(40);

                _datas[i].name = EditorGUILayout.TextField("Name", _datas[i].name);
                _datas[i].documentationURL = EditorGUILayout.TextField("Documentation URL", _datas[i].documentationURL);
                _datas[i].downloadURL = EditorGUILayout.TextField("Download URL", _datas[i].downloadURL);
                _datas[i].path = EditorGUILayout.TextField("Path", _datas[i].path);
                _datas[i].mainPackagePath = EditorGUILayout.TextField("Main Package Path", _datas[i].mainPackagePath);
                _datas[i].demoPackagePath = EditorGUILayout.TextField("Demo Package Path", _datas[i].demoPackagePath);

                if (_datas[i].dependencies == null)
                {
                    _datas[i].dependencies = System.Array.Empty<FDependency>();
                }

                for (int j = 0; j < _datas[i].dependencies.Length; j++)
                {
                    GUILayout.Space(5);

                    _datas[i].dependencies[j].name = EditorGUILayout.TextField("  Dependency Name", _datas[i].dependencies[j].name);
                    _datas[i].dependencies[j].path = EditorGUILayout.TextField("  Dependency Path", _datas[i].dependencies[j].path);
                    _datas[i].dependencies[j].url = EditorGUILayout.TextField("  Dependency URL", _datas[i].dependencies[j].url);
                }


                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    _datas[i].dependencies = _datas[i].dependencies.Append(new FDependency()).ToArray();
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _datas[i].dependencies = _datas[i].dependencies.SkipLast(1).ToArray();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(40);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                _datas = _datas.Append(new FPluginData()).ToArray();
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                _datas = _datas.SkipLast(1).ToArray();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Apply changes", GUILayout.Height(30)))
            {
                string jsonText = JsonConvert.SerializeObject(_datas);
                File.WriteAllText(Application.dataPath + DEPENDENCIES_JSON_PATH, jsonText);
            }
        }
    }
}