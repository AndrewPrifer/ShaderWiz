using UnityEditor;
using UnityEngine;

namespace ShaderWizard {
    internal class SurfaceShaderEditor : EditorWindow {
        private bool _showFunctionsSettings = true;
        private bool _showInputSettings = true;
        private bool _showLightingSettings = true;
        private bool _showLightmapSettings = true;
        private bool _showShadowSettings = true;
        private bool _showMiscSettings = true;
        private bool _showIncludeSettings = true;
        private Vector2 _scrollPosition;
        private SurfaceShader _shader;

        private void OnGUI() {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            _showFunctionsSettings = SwGuiLayout.BeginControlGroup(_showFunctionsSettings, "Custom Functions");
            if (_showFunctionsSettings) {
                Shader.UseVertexModifier = SwGuiLayout.BeginToggleGroup("Use vertex modifier", Shader.UseVertexModifier);
                Shader.CustomDataPerVertex = EditorGUILayout.ToggleLeft("Calculate custom data per-vertex",
                    Shader.CustomDataPerVertex);
                SwGuiLayout.EndToggleGroup();
                Shader.UseFinalColorModifier = EditorGUILayout.ToggleLeft("Use final color modifier",
                    Shader.UseFinalColorModifier);
                Shader.UseTessellation = EditorGUILayout.ToggleLeft("Use DX11 tessellation ", Shader.UseTessellation);
                Shader.PassHalfDirInLighting =
                    EditorGUILayout.ToggleLeft("Pass half-direction vector into ligthing function",
                        Shader.PassHalfDirInLighting);
                Shader.UseBuiltinLighting = SwGuiLayout.BeginToggleGroup("Use built-in lighting model",
                    Shader.UseBuiltinLighting);
                Shader.LightingModel =
                    (LightingModel) EditorGUILayout.EnumPopup("Lighting model:", Shader.LightingModel);
                SwGuiLayout.EndToggleGroup();
                Shader.UseBuiltinLighting = SwGuiLayout.BeginToggleGroup("Use custom lighting model",
                    !Shader.UseBuiltinLighting);
                Shader.IncludeViewDirInLighting = EditorGUILayout.ToggleLeft("Include view direction parameter",
                    Shader.IncludeViewDirInLighting);
                Shader.UsePrePass = EditorGUILayout.ToggleLeft("Use prepass function (deferred lighting)", Shader.UsePrePass);
                SwGuiLayout.EndToggleGroup();
                EditorGUILayout.LabelField("Use custom lightmap decoder function:");
                EditorGUI.indentLevel++;
                Shader.UseSingleLightMap = SwGuiLayout.BeginToggleGroup("Single", Shader.UseSingleLightMap);
                Shader.ViewDirInSingle = EditorGUILayout.ToggleLeft("Include view direction parameter",
                    Shader.ViewDirInSingle);
                SwGuiLayout.EndToggleGroup();
                Shader.UseDualLightMap = SwGuiLayout.BeginToggleGroup("Dual", Shader.UseDualLightMap);
                Shader.ViewDirInDual = EditorGUILayout.ToggleLeft("Include view direction parameter",
                    Shader.ViewDirInDual);
                SwGuiLayout.EndToggleGroup();
                Shader.UseDirectionalLightMap = SwGuiLayout.BeginToggleGroup("Directional", Shader.UseDirectionalLightMap);
                Shader.ViewDirInDirectional = EditorGUILayout.ToggleLeft("Include view direction parameter",
                    Shader.ViewDirInDirectional);
                SwGuiLayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
            SwGuiLayout.EndControlGroup();

            _showInputSettings = SwGuiLayout.BeginControlGroup(_showInputSettings, "Optional Shader Input Values");
            if (_showInputSettings) {
                Shader.ViewDirInInput = EditorGUILayout.ToggleLeft("View direction", Shader.ViewDirInInput);
                Shader.VertColorInInput = EditorGUILayout.ToggleLeft("Per-vertex color", Shader.VertColorInInput);
                Shader.SsPositionInInput = EditorGUILayout.ToggleLeft("Screen space position", Shader.SsPositionInInput);
                Shader.WsPositionInInput = EditorGUILayout.ToggleLeft("World space position", Shader.WsPositionInInput);
                Shader.WorldReflectionVectorInInput = EditorGUILayout.ToggleLeft("World reflection vector",
                    Shader.WorldReflectionVectorInInput);
                Shader.WorldNormalInInput = EditorGUILayout.ToggleLeft("World normal vector", Shader.WorldNormalInInput);
                SwGuiLayout.Separator();
                Shader.OutNormalSpecified =
                    EditorGUILayout.ToggleLeft("Output normal is specified (in surface function)",
                        Shader.OutNormalSpecified);
            }
            SwGuiLayout.EndControlGroup();

            _showLightingSettings = SwGuiLayout.BeginControlGroup(_showLightingSettings, "Lighting");
            if (_showLightingSettings) {
                Shader.ApplyAmbient = EditorGUILayout.ToggleLeft("Apply ambient lights", Shader.ApplyAmbient);
                Shader.ApplyVertexLights = EditorGUILayout.ToggleLeft("Apply per-vertex lights in forward rendering",
                    Shader.ApplyVertexLights);
                Shader.EnableAdditivePass = EditorGUILayout.ToggleLeft("Enable forward rendering additive pass",
                    Shader.EnableAdditivePass);
                Shader.ViewDirPerVert = EditorGUILayout.ToggleLeft("Compute view direction per-vertex",
                    Shader.ViewDirPerVert);
            }
            SwGuiLayout.EndControlGroup();

            _showLightmapSettings = SwGuiLayout.BeginControlGroup(_showLightmapSettings, "Lightmaps");
            if (_showLightmapSettings) {
                Shader.SupportLightmaps = SwGuiLayout.BeginToggleGroup("Support lightmaps", Shader.SupportLightmaps);
                Shader.SupportDirectionalLightmaps = EditorGUILayout.ToggleLeft("Support directional lightmaps",
                    Shader.SupportDirectionalLightmaps);
                Shader.DualForward = EditorGUILayout.ToggleLeft("Dual lightmaps in forward rendering",
                    Shader.DualForward);
                SwGuiLayout.EndToggleGroup();
            }
            SwGuiLayout.EndControlGroup();

            _showShadowSettings = SwGuiLayout.BeginControlGroup(_showShadowSettings, "Shadows");
            if (_showShadowSettings) {
                Shader.AddShadowPasses = EditorGUILayout.ToggleLeft("Add shadow passes", Shader.AddShadowPasses);
                Shader.SupportAllShadowTypes =
                    EditorGUILayout.ToggleLeft("Support all shadow types in forward rendering",
                        Shader.SupportAllShadowTypes);
            }
            SwGuiLayout.EndControlGroup();

            _showMiscSettings = SwGuiLayout.BeginControlGroup(_showMiscSettings, "Misc Settings");
            if (_showMiscSettings) {
                Shader.AlphaBlended = EditorGUILayout.ToggleLeft("Alpha blended", Shader.AlphaBlended);
                Shader.DisableWhenSoftVegIsOff =
                    EditorGUILayout.ToggleLeft("Disable subshader when soft vegetation is off",
                        Shader.DisableWhenSoftVegIsOff);
                Shader.ForceNoShadowCasting = EditorGUILayout.ToggleLeft("Force no shadow casting",
                    Shader.ForceNoShadowCasting);
                Shader.IgnoreProjector = EditorGUILayout.ToggleLeft("Ignore projector", Shader.IgnoreProjector);
                Shader.RenderPosition = (RenderPosition) EditorGUILayout.EnumPopup("Rendering order position:", Shader.RenderPosition);
                Shader.CommentFinal =
                    EditorGUILayout.ToggleLeft("Put comments in compiled surface shader",
                        Shader.CommentFinal);
            }
            SwGuiLayout.EndControlGroup();

            _showIncludeSettings = SwGuiLayout.BeginControlGroup(_showIncludeSettings, "Include");
            if (_showIncludeSettings) {
                Shader.CgincTerrainEngine = EditorGUILayout.ToggleLeft("TerrainEngine.cginc", Shader.CgincTerrainEngine);
                Shader.CgincTessellation = EditorGUILayout.ToggleLeft("Tessellation.cginc", Shader.CgincTessellation);
            }
            SwGuiLayout.EndControlGroup();
            EditorGUILayout.EndScrollView();
        }

        public SurfaceShader Shader {
            private get { return _shader; }
            set {
                _shader = value;
                title = _shader.name;
            }
        }
    }
}