using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderWiz {
    internal class VertFragEditor : EditorWindow {
        private VertFragPass _pass;
        private bool _showDepthTestSettings = true;
//        private bool _showAlphaTestSettings = true;
        private bool _showBlendingSettings = true;
        private bool _showVertexAttributes = true;
        private bool _showShaderSettings = true;
        private bool _showMiscSettings = true;
        private bool _showIncludeSettings = true;
        private Vector2 _scrollPosition;

        private void OnEnable() {
            minSize = new Vector2(350, 100);
        }

        private void OnGUI() {
            if (Pass == null) {
                EditorGUILayout.LabelField("No pass open.");
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Depth test
            _showDepthTestSettings = SwGuiLayout.BeginControlGroup(_showDepthTestSettings, "Depth Test");
            if (_showDepthTestSettings) {
                Pass.WriteToDepthBuffer = EditorGUILayout.ToggleLeft(new GUIContent("Write to depth buffer", HelpText.WriteToDepthBuffer), Pass.WriteToDepthBuffer);
                Pass.ZTestFunc = (CompareFunction) EditorGUILayout.EnumPopup(new GUIContent("Depth test function:", HelpText.DepthTestFunc), Pass.ZTestFunc);

                Pass.UseDepthOffset = SwGuiLayout.BeginToggleGroup(new GUIContent("Use offset", HelpText.UseDepthOffset), Pass.UseDepthOffset);
                Pass.DepthFactor = EditorGUILayout.FloatField(new GUIContent("Factor:", HelpText.DepthFactor), Pass.DepthFactor);
                Pass.DepthUnit = EditorGUILayout.FloatField(new GUIContent("Unit:", HelpText.DepthUnit), Pass.DepthUnit);
                SwGuiLayout.EndToggleGroup();
            }
            SwGuiLayout.EndControlGroup();

            // Alpha test (not used in programmable pipeline)
//            _showAlphaTestSettings = SwGuiLayout.BeginControlGroup(_showAlphaTestSettings, "Alpha Test");
//            if (_showAlphaTestSettings) {
//                Pass.UseAlphaTest = SwGuiLayout.BeginToggleGroup("Use alpha test", Pass.UseAlphaTest);
//                Pass.AlphaTestFunc = (CompareFunction) EditorGUILayout.EnumPopup("Test function:", Pass.AlphaTestFunc);
//                Pass.AlphaTestValue = Mathf.Clamp01(EditorGUILayout.FloatField("Test against:", Pass.AlphaTestValue));
//                SwGuiLayout.EndToggleGroup();
//            }
//            SwGuiLayout.EndControlGroup();

            // Blending options
            _showBlendingSettings = SwGuiLayout.BeginControlGroup(_showBlendingSettings, new GUIContent("Blending", HelpText.Blending));
            if (_showBlendingSettings) {
                Pass.ApplyBlending = SwGuiLayout.BeginToggleGroup(new GUIContent("Apply blending", HelpText.ApplyBlending), Pass.ApplyBlending);
                Pass.SourceBlendFactor = (BlendMode) EditorGUILayout.EnumPopup(new GUIContent("Source factor:", HelpText.SrcFactor), Pass.SourceBlendFactor);
                Pass.DestBlendFactor =
                    (BlendMode) EditorGUILayout.EnumPopup(new GUIContent("Destionation factor:", HelpText.DstFactor), Pass.DestBlendFactor);
                
                Pass.BlendAlphaSeparately = SwGuiLayout.BeginToggleGroup("Blend alpha separately",
                    Pass.BlendAlphaSeparately);
                Pass.AlphaSourceBlendFactor = (BlendMode)EditorGUILayout.EnumPopup("Source factor:", Pass.AlphaSourceBlendFactor);
                Pass.AlphaDestBlendFactor =
                    (BlendMode)EditorGUILayout.EnumPopup("Destionation factor:", Pass.AlphaDestBlendFactor);
                SwGuiLayout.EndToggleGroup();

                Pass.BlendOp = (BlendOp) EditorGUILayout.EnumPopup("Blend operation", Pass.BlendOp);
                SwGuiLayout.EndToggleGroup();
            }
            SwGuiLayout.EndControlGroup();

            //Vertex Attributes
            _showVertexAttributes = SwGuiLayout.BeginControlGroup(_showVertexAttributes, new GUIContent("Vertex Attributes", HelpText.VertexAttributes));
            if (_showVertexAttributes) {
                Pass.UsePresetInput = SwGuiLayout.BeginToggleGroup(new GUIContent("Use preset", HelpText.UsePresetInput), Pass.UsePresetInput);
                Pass.InputPreset = (VertexInputPreset) EditorGUILayout.EnumPopup(new GUIContent("Preset:", HelpText.InputPreset), Pass.InputPreset);
                SwGuiLayout.EndToggleGroup();

                Pass.UsePresetInput = !SwGuiLayout.BeginToggleGroup(new GUIContent("Use custom", HelpText.UseCustomInput), !Pass.UsePresetInput);
                Pass.UsePosition = EditorGUILayout.ToggleLeft("Position", Pass.UsePosition);
                Pass.UseNormal = EditorGUILayout.ToggleLeft("Normal", Pass.UseNormal);
                Pass.UseTexcoord = EditorGUILayout.ToggleLeft("First UV set", Pass.UseTexcoord);
                Pass.UseTexcoord1 = EditorGUILayout.ToggleLeft("Second UV set", Pass.UseTexcoord1);
                Pass.UseTangent = EditorGUILayout.ToggleLeft("Tangent", Pass.UseTangent);
                Pass.UseColor = EditorGUILayout.ToggleLeft("Color", Pass.UseColor);
                SwGuiLayout.EndToggleGroup();
            }
            SwGuiLayout.EndControlGroup();

            // Shader Options
            _showShaderSettings = SwGuiLayout.BeginControlGroup(_showShaderSettings, "Shader Options");
            if (_showShaderSettings) {
                Pass.UseGeometryShader = SwGuiLayout.BeginToggleGroup("Use geometry shader", Pass.UseGeometryShader);
                Pass.InputTopology = (InputTopology) EditorGUILayout.EnumPopup("Input topology:", Pass.InputTopology);
                Pass.OutputTopology = (OutputTopology) EditorGUILayout.EnumPopup("Output topology:", Pass.OutputTopology);
                Pass.MaxVertCount = Mathf.Max(EditorGUILayout.IntField("Max vertex count:", Pass.MaxVertCount), 0);
                SwGuiLayout.EndToggleGroup();

                Pass.CompileToGlsl = EditorGUILayout.ToggleLeft(new GUIContent("Compile to GLSL for desktop", HelpText.CompileToGlsl), Pass.CompileToGlsl);
                Pass.AutoNormalizeVectors = EditorGUILayout.ToggleLeft(new GUIContent("Auto-normalize normal/tangent vectors", HelpText.AutoNormalizeVectors),
                    Pass.AutoNormalizeVectors);
                Pass.GenerateDebugInfo = EditorGUILayout.ToggleLeft(new GUIContent("Generate debug info for DX11", HelpText.GenerateDebugInfo),
                    Pass.GenerateDebugInfo);
                Pass.ShaderTarget = (ShaderTarget) EditorGUILayout.EnumPopup("Shader target:", Pass.ShaderTarget);
            }
            SwGuiLayout.EndControlGroup();

            // Misc Settings
            _showMiscSettings = SwGuiLayout.BeginControlGroup(_showMiscSettings, "Misc Settings");
            if (_showMiscSettings) {
                Pass.LightMode = (LightMode) EditorGUILayout.EnumPopup(new GUIContent("Light mode:", HelpText.LightMode), Pass.LightMode);
                Pass.FaceCullMode = (CullMode) EditorGUILayout.EnumPopup(new GUIContent("Face culling:", HelpText.FaceCullMode), Pass.FaceCullMode);
                Pass.RequireSoftVegatation = EditorGUILayout.ToggleLeft(new GUIContent("Require soft vegetation", HelpText.RequireSoftVegetation),
                    Pass.RequireSoftVegatation);
                Pass.ApplyFog = EditorGUILayout.ToggleLeft(new GUIContent("Apply fog", HelpText.ApplyFog), Pass.ApplyFog);
                Pass.AllowExternalReference = EditorGUILayout.ToggleLeft(new GUIContent("Allow pass to be referenced externally", HelpText.AllowExternalReference), Pass.AllowExternalReference);
            }
            SwGuiLayout.EndControlGroup();

            // Include group
            _showIncludeSettings = SwGuiLayout.BeginControlGroup(_showIncludeSettings, "Include");
            if (_showIncludeSettings) {
                Pass.CgincUnityCg =
                    EditorGUILayout.ToggleLeft(new GUIContent("UnityCg.cginc", HelpText.UnityCGInclude),
                        Pass.CgincUnityCg);
                Pass.CgincTerrainEngine =
                    EditorGUILayout.ToggleLeft(new GUIContent("TerrainEngine.cginc", HelpText.TerrainInclude),
                        Pass.CgincTerrainEngine);
                Pass.CgincTessellation =
                    EditorGUILayout.ToggleLeft(new GUIContent("Tessellation.cginc", HelpText.TessellationInclude),
                        Pass.CgincTessellation);
                Pass.CgincAutoLight =
                    EditorGUILayout.ToggleLeft(new GUIContent("AutoLight.cginc", HelpText.AutoLightInclude),
                        Pass.CgincAutoLight);
                Pass.CgincLighting =
                    EditorGUILayout.ToggleLeft(new GUIContent("Lighting.cginc", HelpText.LightingInclude),
                        Pass.CgincLighting);
            }
            SwGuiLayout.EndControlGroup();

            EditorGUILayout.EndScrollView();
        }

        public VertFragPass Pass {
            private get { return _pass; }
            set {
                _pass = value;
                title = _pass.name + " - Pass";
            }
        }
    }
}