using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderWiz {
    internal class VertFragEditor : EditorWindow {
        private VertFragPass _pass;
        private bool _showDepthTestSettings = true;
        private bool _showAlphaTestSettings = true;
        private bool _showBlendingSettings = true;
        private bool _showShaderSettings = true;
        private bool _showMiscSettings = true;
        private Vector2 _scrollPosition;
        private bool _showIncludeSettings = true;

        private void OnEnable() {
            minSize = new Vector2(350, 100);
        }

        private void OnGUI() {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Depth test
            _showDepthTestSettings = SwGuiLayout.BeginControlGroup(_showDepthTestSettings, "Depth Test");
            if (_showDepthTestSettings) {
                Pass.WriteToDepthBuffer = EditorGUILayout.ToggleLeft("Write to depth buffer", Pass.WriteToDepthBuffer);
                Pass.ZTestFunc = (CompareFunction) EditorGUILayout.EnumPopup("Depth test function:", Pass.ZTestFunc);

                Pass.UseDepthOffset = SwGuiLayout.BeginToggleGroup("Use offset", Pass.UseDepthOffset);
                Pass.DepthFactor = EditorGUILayout.FloatField("Factor:", Pass.DepthFactor);
                Pass.DepthUnit = EditorGUILayout.FloatField("Unit:", Pass.DepthUnit);
                SwGuiLayout.EndToggleGroup();
            }
            SwGuiLayout.EndControlGroup();

            // Alpha test
            _showAlphaTestSettings = SwGuiLayout.BeginControlGroup(_showAlphaTestSettings, "Alpha Test");
            if (_showAlphaTestSettings) {
                Pass.UseAlphaTest = SwGuiLayout.BeginToggleGroup("Use alpha test", Pass.UseAlphaTest);
                Pass.AlphaTestFunc = (CompareFunction) EditorGUILayout.EnumPopup("Test function:", Pass.AlphaTestFunc);
                Pass.AlphaTestValue = EditorGUILayout.FloatField("Test against:", Pass.AlphaTestValue);
                SwGuiLayout.EndToggleGroup();
            }
            SwGuiLayout.EndControlGroup();

            // Blending options
            _showBlendingSettings = SwGuiLayout.BeginControlGroup(_showBlendingSettings, "Blending");
            if (_showBlendingSettings) {
                Pass.ApplyBlending = SwGuiLayout.BeginToggleGroup("Apply blending", Pass.ApplyBlending);
                Pass.SourceBlendFactor = (BlendMode) EditorGUILayout.EnumPopup("Source factor:", Pass.SourceBlendFactor);
                Pass.DestBlendFactor =
                    (BlendMode) EditorGUILayout.EnumPopup("Destionation factor:", Pass.DestBlendFactor);
                
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

            // Shader Options
            _showShaderSettings = SwGuiLayout.BeginControlGroup(_showShaderSettings, "Shader Options");
            if (_showShaderSettings) {
                Pass.UseGeometryShader = EditorGUILayout.ToggleLeft("Use geometry shader", Pass.UseGeometryShader);
                Pass.UseHullShader = EditorGUILayout.ToggleLeft("Use hull shader", Pass.UseHullShader);
                Pass.UseDomainShader = EditorGUILayout.ToggleLeft("Use domain shader", Pass.UseDomainShader);
                Pass.CompileToGlsl = EditorGUILayout.ToggleLeft("Compile to GLSL for desktop", Pass.CompileToGlsl);
                Pass.AutoNormalizeVectors = EditorGUILayout.ToggleLeft("Auto-normalize normal/tangent vectors",
                    Pass.AutoNormalizeVectors);
                Pass.GenerateDebugInfo = EditorGUILayout.ToggleLeft("Generate debug info for DX11",
                    Pass.GenerateDebugInfo);
                Pass.ShaderTarget = (ShaderTarget) EditorGUILayout.EnumPopup("Shader target:", Pass.ShaderTarget);
            }
            SwGuiLayout.EndControlGroup();

            // Misc Settings
            _showMiscSettings = SwGuiLayout.BeginControlGroup(_showMiscSettings, "Misc Settings");
            if (_showMiscSettings) {
                Pass.LightMode = (LightMode) EditorGUILayout.EnumPopup("Light mode:", Pass.LightMode);
                Pass.FaceCullMode = (CullMode) EditorGUILayout.EnumPopup("Face culling:", Pass.FaceCullMode);
                Pass.RequireSoftVegatation = EditorGUILayout.ToggleLeft("Require soft vegetation",
                    Pass.RequireSoftVegatation);
                Pass.ApplyFog = EditorGUILayout.ToggleLeft("Apply fog", Pass.ApplyFog);
                Pass.AllowExternalReference = EditorGUILayout.ToggleLeft("Allow pass to be referenced externally", Pass.AllowExternalReference);
            }
            SwGuiLayout.EndControlGroup();

            // Include group
            _showIncludeSettings = SwGuiLayout.BeginControlGroup(_showIncludeSettings, "Include");
            if (_showIncludeSettings) {
                Pass.CgincUnityCg =
                    EditorGUILayout.ToggleLeft(new GUIContent("UnityCg.cginc"),
                        Pass.CgincUnityCg);
                Pass.CgincTerrainEngine =
                    EditorGUILayout.ToggleLeft(new GUIContent("TerrainEngine.cginc", HelpText.TerrainInclude),
                        Pass.CgincTerrainEngine);
                Pass.CgincTessellation =
                    EditorGUILayout.ToggleLeft(new GUIContent("Tessellation.cginc", HelpText.TessellationInclude),
                        Pass.CgincTessellation);
                Pass.CgincAutoLight =
                    EditorGUILayout.ToggleLeft(new GUIContent("AutoLight.cginc"),
                        Pass.CgincAutoLight);
                Pass.CgincLighting =
                    EditorGUILayout.ToggleLeft(new GUIContent("Lighting.cginc"),
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