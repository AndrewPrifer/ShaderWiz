using UnityEditor;
using UnityEngine;

namespace ShaderWizard {
    internal class AboutWindow : EditorWindow {

        [MenuItem("Tools/ShaderWiz/About", false, 2)]
        private static void ShowWindow() {
            GetWindow(typeof(AboutWindow), false, "About");
        }

        private const string version = "0.8";

        private void OnGUI() {
            minSize = new Vector2(200, 100);
            GUIStyle style = new GUIStyle {richText = true};
            string textColor = "black";
            if (EditorGUIUtility.isProSkin) {
                textColor = "white";
            }

            EditorGUILayout.LabelField(string.Format("<color={0}><size=30>ShaderWiz</size>\n" +
                                                     "v{1}\n\n" +
                                                     "Created by A. Peter Prifer\n" +
                                                     "© 2015 Spherical Cube Games</color>", textColor, version), style);
        }
    }
}