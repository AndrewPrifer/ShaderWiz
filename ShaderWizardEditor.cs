using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ShaderWizard {
    internal class ShaderWizardEditor : EditorWindow {
        #region  Fields

        private ReorderableList _propertyList;
        private Vector2 _scrollPosition;
        private ShaderSettings _shaderSettings;
        private bool _showCommentSettings;
        private bool _showFallbackSettings;
        private bool _showProperties;
        private bool _showSubshaders;
        private ReorderableList _subshaderList;

        #endregion

        private void Init() {
            _shaderSettings = CreateInstance<ShaderSettings>();
            _showProperties = true;
            _showSubshaders = true;
            _showFallbackSettings = true;
            _showCommentSettings = true;
        }

        [MenuItem("Tools/Shader Wizard")]
        private static void ShowWindow() {
            GetWindow(typeof (ShaderWizardEditor), false, "Shader Wizard");
        }

        private void OnEnable() {
            if (_shaderSettings == null) Init();

            const float typeWidth = 60f;
            const float defaultWidth = 160f;
            const float minMaxWidth = 60f;
            const float buttonWidth = 160f;

            InitPropertyList(typeWidth, defaultWidth, minMaxWidth);
            InitSubshaderList(typeWidth, buttonWidth);
        }

        private void OnGUI() {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _shaderSettings.Name = EditorGUILayout.TextField("Name", _shaderSettings.Name);

            _showProperties = SwGuiLayout.BeginControlGroup(_showProperties, "Properties");
            if (_showProperties) {
                _propertyList.DoLayoutList();
            }
            SwGuiLayout.EndControlGroup();

            _showSubshaders = SwGuiLayout.BeginControlGroup(_showSubshaders, "Subshaders");
            if (_showSubshaders) {
                _subshaderList.DoLayoutList();
            }
            SwGuiLayout.EndControlGroup();

            _showFallbackSettings = SwGuiLayout.BeginControlGroup(_showFallbackSettings, "Fallback");
            if (_showFallbackSettings) {
                _shaderSettings.UseFallback = SwGuiLayout.BeginToggleGroup("Use fallback shader",
                    _shaderSettings.UseFallback);
                _shaderSettings.Fallback = SwGuiLayout.ShaderPopup("Fallback", _shaderSettings.Fallback);
                SwGuiLayout.EndToggleGroup();
            }
            SwGuiLayout.EndControlGroup();

            _showCommentSettings = SwGuiLayout.BeginControlGroup(_showCommentSettings, "Comments");
            if (_showCommentSettings) {
                _shaderSettings.CommentShader =
                    EditorGUILayout.ToggleLeft("Put comments in generated shader", _shaderSettings.CommentShader);
            }
            SwGuiLayout.EndControlGroup();

            if (GUILayout.Button("Generate", "LargeButton")) {
                Debug.Log(ShaderGenerator.Generate(_shaderSettings, "    "));
            }

            EditorGUILayout.EndScrollView();
        }

        #region Subshader list

        private void InitSubshaderList(float typeWidth, float buttonWidth) {
            _subshaderList = new ReorderableList(_shaderSettings.GetSubshaders(), typeof (Subshader));
            _subshaderList.drawHeaderCallback = rect => {
                rect.xMin = 28;
                EditorGUI.LabelField(Utils.AnchorLeft(rect, 0, typeWidth), "Type");
                EditorGUI.LabelField(Utils.Anchor(rect, typeWidth, buttonWidth), "Name");
            };
            _subshaderList.drawElementCallback = (rect, index, active, focused) => {
                var element = (Subshader) _subshaderList.list[index];

                rect.y += 3;
                rect.yMax -= 3;

                EditorGUI.LabelField(Utils.AnchorLeft(rect, 0, typeWidth),
                    element.SubshaderType == SubshaderType.Surface ? "Surface" : "Custom");

                element.name = EditorGUI.TextField(Utils.Anchor(rect, typeWidth, buttonWidth), element.name);
                if (GUI.Button(Utils.AnchorRight(rect, 0, buttonWidth), "Edit subshader settings")) {
                    var window = (SurfaceShaderEditor) GetWindow(typeof (SurfaceShaderEditor));
                    window.Shader = (SurfaceShader) element;
                }
            };
            _subshaderList.onAddDropdownCallback = (rect, list) => {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Surface Shader"), false, AddShaderHandler, SubshaderType.Surface);
                menu.AddDisabledItem(new GUIContent("Custom Shader (coming soon)"));
                menu.ShowAsContext();
            };
        }

        private void AddShaderHandler(object userdata) {
            var type = (SubshaderType) userdata;
            switch (type) {
                case SubshaderType.Surface:
                    _subshaderList.list.Add(CreateInstance<SurfaceShader>());
                    break;
                case SubshaderType.Custom:
                    // todo custom subshader
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Property list

        private void InitPropertyList(float labelWidth, float defaultWidth, float minMaxWidth) {
            _propertyList = new ReorderableList(_shaderSettings.GetProperties(), typeof (ShaderProperty));
            _propertyList.drawHeaderCallback = rect => {
                rect.xMin = 28;
                EditorGUI.LabelField(Utils.AnchorLeft(rect, 0, labelWidth), "Type");
                var nameRect = Utils.Anchor(rect, labelWidth, defaultWidth + minMaxWidth);
                EditorGUI.LabelField(Utils.ShareRect(nameRect, 2, 0), "Name");
                EditorGUI.LabelField(Utils.ShareRect(nameRect, 2, 1), "Display Name");
                EditorGUI.LabelField(Utils.AnchorRight(rect, minMaxWidth, defaultWidth), "    Default");
                EditorGUI.LabelField(Utils.AnchorRight(rect, 0, minMaxWidth), "Miscellaneous");
            };
            _propertyList.drawElementCallback = (rect, index, active, focused) => {
                var element = (ShaderProperty) _propertyList.list[index];

                rect.y += 3;
                rect.yMax -= 3;

                DrawPropertyName(Utils.Anchor(rect, labelWidth, defaultWidth + minMaxWidth), element);
                DrawPropertySpecific(Utils.AnchorLeft(rect, 0, labelWidth),
                    Utils.AnchorRight(rect, 0, minMaxWidth), Utils.AnchorRight(rect, minMaxWidth, defaultWidth),
                    element);
            };
            _propertyList.onAddDropdownCallback = (rect, list) => {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Range"), false, AddPropertyHandler, PropertyType.Range);
                menu.AddItem(new GUIContent("Color"), false, AddPropertyHandler, PropertyType.Color);
                menu.AddItem(new GUIContent("Texture"), false, AddPropertyHandler, PropertyType.Texture);
                menu.AddItem(new GUIContent("Float"), false, AddPropertyHandler, PropertyType.Float);
                menu.AddItem(new GUIContent("Vector"), false, AddPropertyHandler, PropertyType.Vector);
                menu.ShowAsContext();
            };
        }

        private void DrawPropertyName(Rect nameRect, ShaderProperty property) {
            property.Name = EditorGUI.TextField(Utils.ShareRect(nameRect, 2, 0), property.Name);
            property.DisplayName = EditorGUI.TextField(Utils.ShareRect(nameRect, 2, 1),
                property.DisplayName);
        }

        private void DrawPropertySpecific(Rect labelRect, Rect minMaxRect, Rect defaultRect, ShaderProperty element) {
            const float equalsPercentage = 0.1f;
            var equalsRect = Utils.DivideRect(defaultRect, 0, equalsPercentage);
            var valueRect = Utils.DivideRect(defaultRect, 1, equalsPercentage);
            switch (element.PropertyType) {
                case PropertyType.Range:
                    var rangeProperty = (RangeProperty) element;
                    EditorGUI.LabelField(labelRect, "Range");
                    rangeProperty.MinValue = EditorGUI.FloatField(Utils.ShareRect(minMaxRect, 2, 0),
                        rangeProperty.MinValue);
                    rangeProperty.MaxValue = EditorGUI.FloatField(Utils.ShareRect(minMaxRect, 2, 1),
                        rangeProperty.MaxValue);
                    EditorGUI.LabelField(equalsRect, "=");
                    rangeProperty.DefaultValue = EditorGUI.Slider(valueRect, rangeProperty.DefaultValue,
                        rangeProperty.MinValue,
                        rangeProperty.MaxValue);
                    break;
                case PropertyType.Color:
                    var colorProperty = (ColorProperty) element;
                    EditorGUI.LabelField(labelRect, "Color");
                    EditorGUI.LabelField(equalsRect, "=");
                    colorProperty.DefaultValue = EditorGUI.ColorField(valueRect, colorProperty.DefaultValue);
                    break;
                case PropertyType.Texture:
                    var texProperty = (TextureProperty) element;
                    EditorGUI.LabelField(labelRect, "Texture");
                    EditorGUI.LabelField(equalsRect, "=");
                    texProperty.DefaultValue = (DefaultTexture) EditorGUI.EnumPopup(valueRect, texProperty.DefaultValue);
                    break;
                case PropertyType.Float:
                    var floatProperty = (FloatProperty) element;
                    EditorGUI.LabelField(labelRect, "Float");
                    EditorGUI.LabelField(equalsRect, "=");
                    floatProperty.DefaultValue = EditorGUI.FloatField(valueRect, floatProperty.DefaultValue);
                    break;
                case PropertyType.Vector:
                    var vecProperty = (VectorProperty) element;
                    EditorGUI.LabelField(labelRect, "Vector");
                    EditorGUI.LabelField(equalsRect, "=");
                    vecProperty.DefaultValue = SwGui.Vector4Field(valueRect, vecProperty.DefaultValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddPropertyHandler(object userData) {
            var type = (PropertyType) userData;
            switch (type) {
                case PropertyType.Range:
                    _propertyList.list.Add(CreateInstance<RangeProperty>());
                    break;
                case PropertyType.Color:
                    _propertyList.list.Add(CreateInstance<ColorProperty>());
                    break;
                case PropertyType.Texture:
                    _propertyList.list.Add(CreateInstance<TextureProperty>());
                    break;
                case PropertyType.Float:
                    _propertyList.list.Add(CreateInstance<FloatProperty>());
                    break;
                case PropertyType.Vector:
                    _propertyList.list.Add(CreateInstance<VectorProperty>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}