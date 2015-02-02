using UnityEditor;
using UnityEngine;

namespace ShaderWizard {
    internal static class SwGui {

        #region Shader popup

        private static string _selectedShader;

        public static string ShaderPopup(Rect position, GUIContent label, string selected, GUIStyle style) {
            var shaderPopupContent = new GUIContent(selected);
            if (label != null) {
                position = EditorGUI.PrefixLabel(position, label);
            }

            if (GUI.Button(position, shaderPopupContent, style)) {
                // Why u not work with CreateInstance?! Why u not work with Object?!
                var menuCommand = new MenuCommand(new ShaderEventHandler(), 0);

                // Create dummy material to make it not highlight any shaders inside:
                const string tmpStr = "Shader \"Hidden/tmp_shdr\"{SubShader{Pass{}}}";
                var temp = new Material(tmpStr);

                // Rebuild shader menu:
                UnityEditorInternal.InternalEditorUtility.SetupShaderMenu(temp);

                // Destroy temporary shader and material:
                Object.DestroyImmediate(temp.shader, true);
                Object.DestroyImmediate(temp, true);

                // Display shader popup:
                EditorUtility.DisplayPopupMenu(position, "CONTEXT/ShaderPopup", menuCommand);
            }

            if (_selectedShader != null) {
                selected = _selectedShader;
                _selectedShader = null;
            }

            return selected;
        }

        public static string ShaderPopup(Rect position, string label, string selected, GUIStyle style) {
            return ShaderPopup(position, new GUIContent(label), selected, style);
        }

        public static string ShaderPopup(Rect position, string selected, GUIStyle style) {
            return ShaderPopup(position, (GUIContent) null, selected, style);
        }

        public static string ShaderPopup(Rect position, GUIContent label, string selected) {
            return ShaderPopup(position, label, selected, EditorStyles.popup);
        }

        public static string ShaderPopup(Rect position, string label, string selected) {
            return ShaderPopup(position, label, selected, EditorStyles.popup);
        }

        public static string ShaderPopup(Rect position, string selected) {
            return ShaderPopup(position, selected, EditorStyles.popup);
        }

        private class ShaderEventHandler : ScriptableObject {
            private void OnSelectedShaderPopup(string command, Shader shader) {
                if (shader != null) {
                    _selectedShader = shader.name;
                }
            }
        }

        #endregion

        public static Vector4 Vector4Field(Rect position, Vector4 value) {
            var floats = new float[4];
            floats[0] = value.x;
            floats[1] = value.y;
            floats[2] = value.z;
            floats[3] = value.w;
            position.height = 16f;
            EditorGUI.BeginChangeCheck();
            ++EditorGUI.indentLevel;
            EditorGUI.MultiFloatField(position,
                new[] {new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z"), new GUIContent("W")}, floats);
            --EditorGUI.indentLevel;

            if (EditorGUI.EndChangeCheck()) {
                value.x = floats[0];
                value.y = floats[1];
                value.z = floats[2];
                value.w = floats[3];
            }

            return value;
        }
    }
}