using UnityEditor;
using UnityEngine;

namespace ShaderWiz {
    internal static class SwGuiLayout {

        #region Shader popup

        public static string ShaderPopup(GUIContent label, string selected, GUIStyle style) {
            return SwGui.ShaderPopup(EditorGUILayout.GetControlRect(), label, selected, style);
        }

        public static string ShaderPopup(string label, string selected, GUIStyle style) {
            return SwGui.ShaderPopup(EditorGUILayout.GetControlRect(), new GUIContent(label), selected, style);
        }

        public static string ShaderPopup(string selected, GUIStyle style) {
            return SwGui.ShaderPopup(EditorGUILayout.GetControlRect(false), (GUIContent)null, selected, style);
        }

        public static string ShaderPopup(GUIContent label, string selected) {
            return SwGui.ShaderPopup(EditorGUILayout.GetControlRect(), label, selected, EditorStyles.popup);
        }

        public static string ShaderPopup(string label, string selected) {
            return SwGui.ShaderPopup(EditorGUILayout.GetControlRect(), label, selected, EditorStyles.popup);
        }

        public static string ShaderPopup(string selected) {
            return SwGui.ShaderPopup(EditorGUILayout.GetControlRect(false), selected, EditorStyles.popup);
        }

        #endregion

        #region Control group

        public static bool BeginControlGroup(bool foldout, GUIContent content) {
            EditorGUILayout.BeginVertical("HelpBox");
            foldout = EditorGUILayout.Foldout(foldout, content);
            return foldout;
        }

        public static bool BeginControlGroup(bool foldout, string content) {
            return BeginControlGroup(foldout, new GUIContent(content));
        }

        public static void EndControlGroup() {
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Toggle group

        public static bool BeginToggleGroup(GUIContent content, bool toggle) {
            toggle = EditorGUILayout.ToggleLeft(content, toggle, EditorStyles.label);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!toggle);
            GUILayout.BeginVertical();
            return toggle;
        }

        public static bool BeginToggleGroup(string label, bool toggle) {
            return BeginToggleGroup(new GUIContent(label), toggle);
        }

        public static void EndToggleGroup() {
            GUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
        }

        #endregion

        public static void Separator() {
            GUILayout.Button("", "sv_iconselector_sep");
        }
    }
}