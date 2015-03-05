using UnityEditor;
using UnityEngine;

namespace ShaderWiz {
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

        private void OnEnable() {
            minSize = new Vector2(350, 100);
        }

        private void OnGUI() {
            if (Shader == null) {
                EditorGUILayout.LabelField("No subshader open.");
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Custom functions group
            _showFunctionsSettings = SwGuiLayout.BeginControlGroup(_showFunctionsSettings, "Custom Functions");
            if (_showFunctionsSettings) {
                Shader.UseVertexModifier =
                    SwGuiLayout.BeginToggleGroup(new GUIContent("Use vertex modifier", HelpText.VertexModifier),
                        Shader.UseVertexModifier);

                Shader.CustomDataPerVertex =
                    EditorGUILayout.ToggleLeft(
                        new GUIContent("Calculate custom data per-vertex", HelpText.CustomDataPerVertex),
                        Shader.CustomDataPerVertex);

                SwGuiLayout.EndToggleGroup();
                Shader.UseFinalColorModifier =
                    EditorGUILayout.ToggleLeft(new GUIContent("Use final color modifier", HelpText.FinalColorModifier),
                        Shader.UseFinalColorModifier);

                Shader.UseTessellation =
                    EditorGUILayout.ToggleLeft(new GUIContent("Use DX11 tessellation", HelpText.Tessellation),
                        Shader.UseTessellation);

                Shader.PassHalfDirInLighting =
                    EditorGUILayout.ToggleLeft(
                        new GUIContent("Pass half-direction vector into ligthing function", HelpText.HalfDirInLighting),
                        Shader.PassHalfDirInLighting);

                Shader.UseBuiltinLighting =
                    SwGuiLayout.BeginToggleGroup(
                        new GUIContent("Use built-in lighting model", HelpText.BuiltinLighting),
                        Shader.UseBuiltinLighting);

                Shader.LightingModel =
                    (LightingModel) EditorGUILayout.EnumPopup("Lighting model:", Shader.LightingModel);
                SwGuiLayout.EndToggleGroup();

                Shader.UseBuiltinLighting =
                    !SwGuiLayout.BeginToggleGroup(new GUIContent("Use custom lighting model", HelpText.CustomLighting),
                        !Shader.UseBuiltinLighting);

                Shader.ViewDirInLighting = EditorGUILayout.ToggleLeft("Include view direction parameter",
                    Shader.ViewDirInLighting);

                Shader.UsePrePass = EditorGUILayout.ToggleLeft("Use prepass function (deferred lighting)",
                    Shader.UsePrePass);

                SwGuiLayout.EndToggleGroup();

                // Custom lightmap decoders
                EditorGUILayout.LabelField("Use custom lightmap decoder function:");
                EditorGUI.indentLevel++;

                Shader.UseSingleLightMap =
                    SwGuiLayout.BeginToggleGroup(new GUIContent("Single", HelpText.SingleLightmap),
                        Shader.UseSingleLightMap);
                Shader.ViewDirInSingle = EditorGUILayout.ToggleLeft("Include view direction parameter",
                    Shader.ViewDirInSingle);
                SwGuiLayout.EndToggleGroup();

                Shader.UseDualLightMap = SwGuiLayout.BeginToggleGroup(new GUIContent("Dual", HelpText.DualLightmap),
                    Shader.UseDualLightMap);
                Shader.ViewDirInDual = EditorGUILayout.ToggleLeft("Include view direction parameter",
                    Shader.ViewDirInDual);
                SwGuiLayout.EndToggleGroup();

                Shader.UseDirectionalLightMap =
                    SwGuiLayout.BeginToggleGroup(new GUIContent("Directional", HelpText.DirectionalLightmap),
                        Shader.UseDirectionalLightMap);
                Shader.ViewDirInDirectional = EditorGUILayout.ToggleLeft("Include view direction parameter",
                    Shader.ViewDirInDirectional);
                SwGuiLayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
            SwGuiLayout.EndControlGroup();

            // Shader input
            _showInputSettings = SwGuiLayout.BeginControlGroup(_showInputSettings, "Shader Input Values");
            if (_showInputSettings) {
                Shader.UvInInput = EditorGUILayout.ToggleLeft("Texture coordinates", Shader.UvInInput);
                Shader.ViewDirInInput = EditorGUILayout.ToggleLeft("View direction", Shader.ViewDirInInput);
                Shader.VertColorInInput = EditorGUILayout.ToggleLeft("Per-vertex color", Shader.VertColorInInput);
                Shader.SsPositionInInput = EditorGUILayout.ToggleLeft("Screen space position", Shader.SsPositionInInput);
                Shader.WsPositionInInput = EditorGUILayout.ToggleLeft("World space position", Shader.WsPositionInInput);
                Shader.WorldReflectionVectorInInput = EditorGUILayout.ToggleLeft("World reflection vector",
                    Shader.WorldReflectionVectorInInput);
                Shader.WorldNormalInInput = EditorGUILayout.ToggleLeft("World normal vector", Shader.WorldNormalInInput);
                SwGuiLayout.Separator();
                Shader.OutNormalSpecified =
                    EditorGUILayout.ToggleLeft(
                        new GUIContent("Surface function writes to output normal", HelpText.OutputNormalSpecified),
                        Shader.OutNormalSpecified);
            }
            SwGuiLayout.EndControlGroup();

            // Lighting group
            _showLightingSettings = SwGuiLayout.BeginControlGroup(_showLightingSettings, "Lighting");
            if (_showLightingSettings) {
                Shader.ApplyAmbient = EditorGUILayout.ToggleLeft("Apply ambient lights", Shader.ApplyAmbient);
                Shader.ApplyVertexLights = EditorGUILayout.ToggleLeft("Apply per-vertex lights in forward rendering",
                    Shader.ApplyVertexLights);
                Shader.EnableAdditivePass =
                    EditorGUILayout.ToggleLeft(
                        new GUIContent("Enable forward rendering additive pass", HelpText.ForwardAdditive),
                        Shader.EnableAdditivePass);
                Shader.ViewDirPerVert = EditorGUILayout.ToggleLeft("Compute view direction per-vertex",
                    Shader.ViewDirPerVert);
            }
            SwGuiLayout.EndControlGroup();

            //Lightmaps group
            _showLightmapSettings = SwGuiLayout.BeginControlGroup(_showLightmapSettings, "Lightmaps");
            if (_showLightmapSettings) {
                Shader.SupportLightmaps = SwGuiLayout.BeginToggleGroup("Support lightmaps", Shader.SupportLightmaps);
                Shader.SupportDirectionalLightmaps =
                    EditorGUILayout.ToggleLeft(
                        new GUIContent("Support directional lightmaps", HelpText.DirectionalLightmap),
                        Shader.SupportDirectionalLightmaps);
                Shader.DualForward =
                    EditorGUILayout.ToggleLeft(
                        new GUIContent("Dual lightmaps in forward rendering", HelpText.DualLightmap),
                        Shader.DualForward);
                SwGuiLayout.EndToggleGroup();
            }
            SwGuiLayout.EndControlGroup();

            // Shadows group
            _showShadowSettings = SwGuiLayout.BeginControlGroup(_showShadowSettings, "Shadows");
            if (_showShadowSettings) {
                Shader.AddShadowPasses = EditorGUILayout.ToggleLeft("Add shadow passes", Shader.AddShadowPasses);
                Shader.SupportAllShadowTypes =
                    EditorGUILayout.ToggleLeft("Support all shadow types in forward rendering",
                        Shader.SupportAllShadowTypes);
            }
            SwGuiLayout.EndControlGroup();

            // Miscellaneous group
            _showMiscSettings = SwGuiLayout.BeginControlGroup(_showMiscSettings, "Misc Settings");
            if (_showMiscSettings) {
                Shader.AlphaBlended = EditorGUILayout.ToggleLeft("Alpha blended", Shader.AlphaBlended);
                Shader.DisableWhenSoftVegIsOff =
                    EditorGUILayout.ToggleLeft("Disable subshader when soft vegetation is off",
                        Shader.DisableWhenSoftVegIsOff);
                Shader.ForceNoShadowCasting = EditorGUILayout.ToggleLeft(new GUIContent("Force no shadow casting", HelpText.ForceNoShadow),
                    Shader.ForceNoShadowCasting);
                Shader.IgnoreProjector = EditorGUILayout.ToggleLeft(new GUIContent("Ignore projector", HelpText.IgnoreProjector), Shader.IgnoreProjector);
                Shader.RenderPosition =
                    (RenderPosition)EditorGUILayout.EnumPopup(new GUIContent("Rendering order position:", HelpText.RenderingOrder), Shader.RenderPosition);
                Shader.CommentFinal =
                    EditorGUILayout.ToggleLeft("Put debug comments in compiled surface shader",
                        Shader.CommentFinal);
            }
            SwGuiLayout.EndControlGroup();

            // Include group
            _showIncludeSettings = SwGuiLayout.BeginControlGroup(_showIncludeSettings, "Include");
            if (_showIncludeSettings) {
                Shader.CgincTerrainEngine =
                    EditorGUILayout.ToggleLeft(new GUIContent("TerrainEngine.cginc", HelpText.TerrainInclude),
                        Shader.CgincTerrainEngine);
                Shader.CgincTessellation =
                    EditorGUILayout.ToggleLeft(new GUIContent("Tessellation.cginc", HelpText.TessellationInclude),
                        Shader.CgincTessellation);
            }
            SwGuiLayout.EndControlGroup();
            EditorGUILayout.EndScrollView();
        }

        public SurfaceShader Shader {
            private get { return _shader; }
            set {
                _shader = value;
                title = _shader.name + " - Surface";
            }
        }
    }
}