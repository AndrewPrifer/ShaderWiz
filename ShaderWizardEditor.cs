using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ShaderWiz {
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
        private const float Padding = 2f;
        public const string ResPath = "Assets/Shader Wizard/InternalResources/";

        #endregion

        private void Init() {
            _shaderSettings = CreateInstance<ShaderSettings>();
            _showProperties = true;
            _showSubshaders = true;
            _showFallbackSettings = true;
            _showCommentSettings = true;
        }

        [MenuItem("Tools/ShaderWiz/Shader Wizard", false, 1)]
        private static void ShowWindow() {
            GetWindow(typeof (ShaderWizardEditor), false, "Shader Wizard");
        }

        private void OnEnable() {
            if (_shaderSettings == null) Init();

            // Property GUI sizes
            const float typeWidth = 60f;
            const float defaultWidth = 200f;
            const float miscWidth = 140f;
            const float buttonWidth = defaultWidth + miscWidth;

            minSize = new Vector2(typeWidth + buttonWidth + 230, 100);
            InitPropertyList(typeWidth, defaultWidth, miscWidth);
            InitSubshaderList(typeWidth, buttonWidth);
        }

        private void OnGUI() {
            //Debug.Log(position);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Shader name
            _shaderSettings.Name = EditorGUILayout.TextField(new GUIContent("Name", HelpText.ShaderName),
                _shaderSettings.Name);

            // Properies group
            _showProperties = SwGuiLayout.BeginControlGroup(_showProperties,
                new GUIContent("Properties", HelpText.ShaderProperties));
            if (_showProperties) {
                _propertyList.DoLayoutList();
            }
            SwGuiLayout.EndControlGroup();

            // Subshaders group
            _showSubshaders = SwGuiLayout.BeginControlGroup(_showSubshaders,
                new GUIContent("Subshaders", HelpText.Subshaders));
            if (_showSubshaders) {
                _subshaderList.DoLayoutList();
            }
            SwGuiLayout.EndControlGroup();

            // Fallback group
            _showFallbackSettings = SwGuiLayout.BeginControlGroup(_showFallbackSettings,
                new GUIContent("Fallback", HelpText.Fallback));
            if (_showFallbackSettings) {
                _shaderSettings.UseFallback = SwGuiLayout.BeginToggleGroup("Use fallback shader",
                    _shaderSettings.UseFallback);
                _shaderSettings.Fallback = SwGuiLayout.ShaderPopup("Fallback", _shaderSettings.Fallback);
                SwGuiLayout.EndToggleGroup();
            }
            SwGuiLayout.EndControlGroup();

            // Comments group
            _showCommentSettings = SwGuiLayout.BeginControlGroup(_showCommentSettings, "Comments");
            if (_showCommentSettings) {
                _shaderSettings.CommentShader =
                    EditorGUILayout.ToggleLeft("Put helper comments in generated shader", _shaderSettings.CommentShader);
            }
            SwGuiLayout.EndControlGroup();

            if (GUILayout.Button("Generate", "LargeButton")) {
                //Debug.Log(ShaderGenerator.Generate(_shaderSettings, "    "));
                if (!IsCorrect()) {
                    EditorUtility.DisplayDialog("Error", "Invalid property name.", "Ok");
                } else {
                    var path = EditorUtility.SaveFilePanel(
                        "Save shader",
                        "",
                        _shaderSettings.Name + ".shader",
                        "shader");

                    if (path.Length != 0) {
                        File.WriteAllText(path, ShaderGenerator.Generate(_shaderSettings, "    "));
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private bool IsCorrect() {
            // Validity check
            return
                _shaderSettings.GetProperties()
                    .Cast<ShaderProperty>()
                    .All(property => property.Name != null && Regex.IsMatch(property.Name, @"^[_a-zA-Z][_a-zA-Z0-9]*$"));
        }

        #region Subshader list

        private void InitSubshaderList(float typeWidth, float buttonWidth) {
            _subshaderList = new ReorderableList(_shaderSettings.GetSubshaders(), typeof (Subshader));

            // List header
            _subshaderList.drawHeaderCallback = rect => {
                rect.xMin = 28;
                EditorGUI.LabelField(Utils.AnchorLeft(rect, 0, typeWidth), "Type");
                EditorGUI.LabelField(Utils.Anchor(rect, typeWidth, buttonWidth), "Name");
            };

            // List elements
            _subshaderList.drawElementCallback = (rect, index, active, focused) => {
                var element = (Subshader) _subshaderList.list[index];

                rect.y += 3;
                rect.yMax -= 3;

                EditorGUI.LabelField(Utils.AnchorLeft(rect, 0, typeWidth),
                    element.SubshaderType == SubshaderType.Surface ? "Surface" : "Custom");
                element.name = EditorGUI.TextField(Utils.Pad(Utils.Anchor(rect, typeWidth, buttonWidth), 0, Padding),
                    element.name);

                if (GUI.Button(Utils.Pad(Utils.AnchorRight(rect, 0, buttonWidth), Padding, 0), "Edit subshader settings")) {
                    var window = (SurfaceShaderEditor) GetWindow(typeof (SurfaceShaderEditor));
                    window.Shader = (SurfaceShader) element;
                }
            };

            // Add to list
            _subshaderList.onAddDropdownCallback = (rect, list) => {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Surface Shader"), false, OnAddSubshader, SubshaderType.Surface);
                menu.AddDisabledItem(new GUIContent("Custom Shader (coming soon)"));
                menu.ShowAsContext();
            };
        }

        private void OnAddSubshader(object userdata) {
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

        private void InitPropertyList(float labelWidth, float defaultWidth, float miscWidth) {
            _propertyList = new ReorderableList(_shaderSettings.GetProperties(), typeof (ShaderProperty));

            // List header
            _propertyList.drawHeaderCallback = rect => {
                rect.xMin = 28;
                EditorGUI.LabelField(Utils.AnchorLeft(rect, 0, labelWidth), "Type");
                var nameRect = Utils.Anchor(rect, labelWidth, defaultWidth + miscWidth);
                EditorGUI.LabelField(Utils.ShareRect(nameRect, 2, 0), "Name");
                EditorGUI.LabelField(Utils.ShareRect(nameRect, 2, 1), "Display Name");
                EditorGUI.LabelField(Utils.AnchorRight(rect, miscWidth, defaultWidth), "Default Value");
                EditorGUI.LabelField(Utils.AnchorRight(rect, 0, miscWidth), "Miscellaneous");
            };

            // List elements
            _propertyList.drawElementCallback = (rect, index, active, focused) => {
                var element = (ShaderProperty) _propertyList.list[index];

                rect.y += 3;
                rect.yMax -= 3;

                DrawPropertyName(Utils.Anchor(rect, labelWidth, defaultWidth + miscWidth), element);
                DrawPropertySpecific(Utils.AnchorLeft(rect, 0, labelWidth),
                    Utils.AnchorRight(rect, 0, miscWidth),
                    Utils.Pad(Utils.AnchorRight(rect, miscWidth, defaultWidth), Padding, Padding),
                    element);
            };

            // Add to list
            _propertyList.onAddDropdownCallback = (rect, list) => {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Range"), false, OnAddProperty, PropertyType.Range);
                menu.AddItem(new GUIContent("Color"), false, OnAddProperty, PropertyType.Color);
                menu.AddItem(new GUIContent("Texture"), false, OnAddProperty, PropertyType.Texture);
                menu.AddItem(new GUIContent("Float"), false, OnAddProperty, PropertyType.Float);
                menu.AddItem(new GUIContent("Vector"), false, OnAddProperty, PropertyType.Vector);
                menu.ShowAsContext();
            };
        }

        private void DrawPropertyName(Rect nameRect, ShaderProperty property) {
            property.Name = EditorGUI.TextField(Utils.Pad(Utils.ShareRect(nameRect, 2, 0), 0, Padding), property.Name);
            property.DisplayName = EditorGUI.TextField(Utils.Pad(Utils.ShareRect(nameRect, 2, 1), Padding, 0),
                property.DisplayName);
        }

        private void DrawPropertySpecific(Rect labelRect, Rect miscRect, Rect defaultRect, ShaderProperty element) {
            switch (element.PropertyType) {
                // Range
                case PropertyType.Range:
                    var rangeProperty = (RangeProperty) element;
                    EditorGUI.LabelField(labelRect, "Range");
                    var labelWidth1 = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = Utils.TextWidth("Min: ");
                    rangeProperty.MinValue =
                        EditorGUI.FloatField(Utils.Pad(Utils.ShareRect(miscRect, 2, 0), Padding, Padding), "Min: ",
                            rangeProperty.MinValue);
                    EditorGUIUtility.labelWidth = Utils.TextWidth("Max: ");
                    rangeProperty.MaxValue = EditorGUI.FloatField(
                        Utils.Pad(Utils.ShareRect(miscRect, 2, 1), Padding, 0), "Max: ",
                        rangeProperty.MaxValue);
                    EditorGUIUtility.labelWidth = labelWidth1;
                    rangeProperty.DefaultValue = EditorGUI.Slider(defaultRect, rangeProperty.DefaultValue,
                        rangeProperty.MinValue,
                        rangeProperty.MaxValue);
                    break;

                // Color
                case PropertyType.Color:
                    var colorProperty = (ColorProperty) element;
                    EditorGUI.LabelField(labelRect, "Color");
                    colorProperty.DefaultValue = EditorGUI.ColorField(defaultRect, colorProperty.DefaultValue);
                    break;

                // Texture
                case PropertyType.Texture:
                    var texProperty = (TextureProperty) element;
                    EditorGUI.LabelField(labelRect, "Texture");
                    labelWidth1 = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = Utils.TextWidth("Auto UV: ");
                    texProperty.TexGenMode =
                        (TexGenMode) EditorGUI.EnumPopup(miscRect, "Auto UV: ", texProperty.TexGenMode);
                    EditorGUIUtility.labelWidth = labelWidth1;
                    texProperty.DefaultValue =
                        (DefaultTexture) EditorGUI.EnumPopup(defaultRect, texProperty.DefaultValue);
                    break;

                // Float
                case PropertyType.Float:
                    var floatProperty = (FloatProperty) element;
                    EditorGUI.LabelField(labelRect, "Float");
                    floatProperty.DefaultValue = EditorGUI.FloatField(defaultRect, floatProperty.DefaultValue);
                    break;

                // Vector
                case PropertyType.Vector:
                    var vecProperty = (VectorProperty) element;
                    EditorGUI.LabelField(labelRect, "Vector");
                    vecProperty.DefaultValue = SwGui.Vector4Field(defaultRect, vecProperty.DefaultValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnAddProperty(object userData) {
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