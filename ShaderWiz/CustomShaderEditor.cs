using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ShaderWiz {
    internal class CustomShaderEditor : EditorWindow {
        const float typeWidth = 110f;
        const float buttonWidth = 150f;

        private Vector2 _scrollPosition;
        private bool _showPassSettings = true;
        private bool _showSubshaderSettings = true;
        private CustomShader _shader;
        private ReorderableList _passList;
        private const float Padding = 2f;

        private void OnEnable() {
            minSize = new Vector2(typeWidth + buttonWidth + 230, 100);

            if (Shader != null) {
                InitPassList(typeWidth, buttonWidth);
            }
        }

        private void InitPassList(float typeWidth, float buttonWidth) {
            _passList = new ReorderableList(Shader.GetPasses(), typeof(Pass));

            // List header
            _passList.drawHeaderCallback = rect => {
                rect.xMin = 28;
                EditorGUI.LabelField(Utils.AnchorLeft(rect, 0, typeWidth), "Type");
                EditorGUI.LabelField(Utils.Anchor(rect, typeWidth, buttonWidth), "Name");
            };

            // List elements
            _passList.drawElementCallback = (rect, index, active, focused) => {
                var element = (Pass)_passList.list[index];

                rect.y += 3;
                rect.yMax -= 3;

                EditorGUI.LabelField(Utils.AnchorLeft(rect, 0, typeWidth),
                    element.PassType == PassType.VertFrag ? "Vertex/Fragment" : "Fixed Function");
                element.name = EditorGUI.TextField(Utils.Pad(Utils.Anchor(rect, typeWidth, buttonWidth), 0, Padding),
                    element.name);

                if (GUI.Button(Utils.Pad(Utils.AnchorRight(rect, 0, buttonWidth), Padding, 0), "Edit pass settings")) {
                    switch (element.PassType) {
                        case PassType.VertFrag:
                            var vertFragWindow = (VertFragEditor)GetWindow(typeof(VertFragEditor));
                            vertFragWindow.Pass = (VertFragPass)element;
                            break;
                        case PassType.FixedFunction:
                            throw new NotImplementedException();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            };

            // Add to list
            _passList.onAddDropdownCallback = (rect, list) => {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Programmable Pass"), false, OnAddPass, PassType.VertFrag);
                menu.AddDisabledItem(new GUIContent("Fixed Function Pass (coming soon)"));
                menu.ShowAsContext();
            };
        }

        private void OnAddPass(object userdata) {
            var type = (PassType)userdata;
            switch (type) {
                case PassType.VertFrag:
                    _passList.list.Add(CreateInstance<VertFragPass>());
                    break;
                case PassType.FixedFunction:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnGUI() {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _showPassSettings = SwGuiLayout.BeginControlGroup(_showPassSettings, "Passes");
            if (_showPassSettings) {
                _passList.DoLayoutList();
            }
            SwGuiLayout.EndControlGroup();

            _showSubshaderSettings = SwGuiLayout.BeginControlGroup(_showSubshaderSettings, "Subshader Settings");
            if (_showSubshaderSettings) {
                Shader.ForceNoShadowCasting = EditorGUILayout.ToggleLeft("Force no shadow casting",
                    Shader.ForceNoShadowCasting);
                Shader.IgnoreProjector = EditorGUILayout.ToggleLeft("Ignore projector", Shader.IgnoreProjector);
                Shader.RenderPosition =
                    (RenderPosition)EditorGUILayout.EnumPopup("Rendering order position:", Shader.RenderPosition);
            }
            SwGuiLayout.EndControlGroup();

            EditorGUILayout.EndScrollView();
        }

        public CustomShader Shader {
            private get { return _shader; }
            set {
                _shader = value;
                title = _shader.name + " - Custom";

                InitPassList(typeWidth, buttonWidth);
            }
        }
    }
}