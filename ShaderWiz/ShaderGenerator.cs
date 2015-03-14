using System;
using System.CodeDom.Compiler;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderWiz {
    internal static class ShaderGenerator {
        public static string Generate(ShaderSettings shader, string tabString) {
            var writer = new IndentedTextWriter(new StringWriter(), tabString);

            // Shader name
            writer.WriteLine("Shader \"{0}\" {{", shader.Name);
            writer.Indent++;

            // Shader properties
            if (shader.GetProperties().Count != 0) {
                writer.WriteLine("Properties {");
                writer.Indent++;

                foreach (ShaderProperty property in shader.GetProperties()) {
                    switch (property.PropertyType) {
                        case PropertyType.Range:
                            var range = (RangeProperty) property;
                            writer.WriteLine("{0} (\"{1}\", Range ({2}, {3})) = {4}", range.Name, range.DisplayName,
                                range.MinValue, range.MaxValue, range.DefaultValue);
                            break;
                        case PropertyType.Color:
                            var color = (ColorProperty) property;
                            writer.WriteLine("{0} (\"{1}\", Color) = ({2},{3},{4},{5})", color.Name, color.DisplayName,
                                color.DefaultValue.r, color.DefaultValue.g, color.DefaultValue.b, color.DefaultValue.a);
                            break;
                        case PropertyType.Texture:
                            // todo texgen, textype
                            var tex = (TextureProperty) property;
                            writer.WriteLine("{0} (\"{1}\", {2}) = \"{3}\" {{{4}}}", tex.Name, tex.DisplayName,
                                tex.TextureType.ToString().Replace("Two", "2"), tex.DefaultValue.ToString().ToLower(),
                                tex.TexGenMode == TexGenMode.None ? "" : "TexGen " + tex.TexGenMode);
                            break;
                        case PropertyType.Float:
                            var num = (FloatProperty) property;
                            writer.WriteLine("{0} (\"{1}\", Float) = {2}", num.Name, num.DisplayName, num.DefaultValue);
                            break;
                        case PropertyType.Vector:
                            var vector = (VectorProperty) property;
                            writer.WriteLine("{0} (\"{1}\", Vector) = ({2},{3},{4},{5})", vector.Name,
                                vector.DisplayName, vector.DefaultValue.x, vector.DefaultValue.y, vector.DefaultValue.z,
                                vector.DefaultValue.w);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                writer.Indent--;
                writer.WriteLine("}");
                writer.WriteLine();
            }

            // Subshaders
            foreach (Subshader subshader in shader.GetSubshaders()) {
                writer.WriteLine("Subshader {");
                writer.Indent++;

                // Tags
                if (subshader.RenderPosition != RenderPosition.Geometry || subshader.ForceNoShadowCasting ||
                    subshader.IgnoreProjector) {
                    writer.Write("Tags { ");
                    if (subshader.RenderPosition != RenderPosition.Geometry)
                        writer.Write("\"Queue\" = \"{0}\" ", subshader.RenderPosition);
                    if (subshader.ForceNoShadowCasting) writer.Write("\"ForceNoShadowCasting\" = \"True\" ");
                    if (subshader.IgnoreProjector) writer.Write("\"IgnoreProjector\" = \"True\" ");
                    writer.WriteLine("}");
                    writer.WriteLine();
                }

                // Surface shader
                if (subshader.SubshaderType == SubshaderType.Surface) {
                    var surface = (SurfaceShader) subshader;

                    // HLSL begin
                    writer.WriteLine("CGPROGRAM");

                    // Pragmas
                    writer.Write("#pragma ");
                    writer.Write("surface {0} ", "surf");

                    if (surface.UseBuiltinLighting) {
                        writer.Write(surface.LightingModel + " ");
                    } else {
                        writer.Write("Lighting{0} ", "Custom");
                    }

                    if (surface.UseVertexModifier) writer.Write("vertex:{0} ", "vert");
                    if (surface.UseFinalColorModifier) writer.Write("finalcolor:{0} ", "color");
                    if (surface.UseTessellation) writer.Write("tessellate:{0} ", "tess");
                    if (surface.AlphaBlended) writer.Write("alpha ");
                    if (surface.AddShadowPasses) writer.Write("addshadow ");
                    if (surface.DualForward) writer.Write("dualforward ");
                    if (surface.SupportAllShadowTypes) writer.Write("fullforwardshadows ");
                    if (surface.DisableWhenSoftVegIsOff) writer.Write("softvegetation ");
                    if (!surface.ApplyAmbient) writer.Write("noambient ");
                    if (!surface.ApplyVertexLights) writer.Write("novertexlights ");
                    if (!surface.SupportLightmaps) writer.Write("nolightmap ");
                    if (!surface.SupportDirectionalLightmaps) writer.Write("nodirlightmap ");
                    if (!surface.EnableAdditivePass) writer.Write("noforwardadd ");
                    if (surface.ViewDirPerVert) writer.Write("approxview ");
                    if (surface.PassHalfDirInLighting) writer.Write("halfasview ");
                    writer.WriteLine();

                    // #pragma debug on separate line
                    if (surface.CommentFinal) writer.WriteLine("#pragma debug");

                    // Includes
                    if (surface.CgincTerrainEngine) writer.WriteLine("#include {0}", "\"TerrainEngine.cginc\"");
                    if (surface.CgincTessellation) writer.WriteLine("#include {0}", "\"Tessellation.cginc\"");
                    writer.WriteLine();

                    if (shader.CommentShader) {
                        writer.WriteLine(
                            "// If you want to access a property, declare it here with the same name and a matching type.");
                        writer.WriteLine();
                    }

                    // Input
                    if (shader.CommentShader) {
                        writer.WriteLine("// To use a uv set, declare uv<TextureName> inside Input.");
                        writer.WriteLine("// To use second uv set, declare uv2<TextureName> inside Input.");
                    }
                    writer.WriteLine("struct Input {");
                    writer.Indent++;

                    if (surface.ViewDirInInput) writer.WriteLine("float3 viewDir;");
                    if (surface.VertColorInInput) writer.WriteLine("float4 vertColor : COLOR;");
                    if (surface.SsPositionInInput) writer.WriteLine("float4 screenPos;");
                    if (surface.WsPositionInInput) writer.WriteLine("float3 worldPos;");
                    if (surface.WorldReflectionVectorInInput) writer.WriteLine("float3 worldRefl;");
                    if (surface.WorldNormalInInput) writer.WriteLine("float3 worldNormal;");
                    if (surface.OutNormalSpecified &&
                        (surface.WorldReflectionVectorInInput || surface.WorldNormalInInput))
                        writer.WriteLine("INTERNAL_DATA");
                    if (shader.CommentShader && surface.CustomDataPerVertex) {
                        writer.WriteLine();
                        writer.WriteLine("// Declare custom input members here. Name cannot begin with 'uv'.");
                    }
                    writer.Indent--;
                    writer.WriteLine("};");
                    writer.WriteLine();

                    // Vertex function
                    if (surface.UseVertexModifier) {
                        writer.Write("void {0} (inout {1} v", "vert", "appdata_full"); // Always takes appdata_full!
                        writer.WriteLine(surface.CustomDataPerVertex ? ", out Input o) {" : ") {");
                        writer.Indent++;
                        if (surface.CustomDataPerVertex) writer.WriteLine("UNITY_INITIALIZE_OUTPUT(Input,o);");
                        writer.WriteLine(shader.CommentShader ? "// Modify vertices here" : "");
                        if (surface.CustomDataPerVertex && shader.CommentShader)
                            writer.WriteLine("// To pass custom data to surface function, write to o.");
                        if (shader.CommentShader)
                            writer.WriteLine("// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderExamples.html");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine();
                    }

                    // Color function
                    if (surface.UseFinalColorModifier) {
                        writer.WriteLine("void {0} (Input IN, SurfaceOutput o, inout fixed4 color) {{", "color");
                        writer.Indent++;
                        writer.WriteLine(shader.CommentShader ? "// Modify final color here." : "");
                        if (shader.CommentShader)
                            writer.WriteLine("// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderExamples.html");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine();
                    }

                    // Tessellation function
                    if (surface.UseTessellation) {
                        writer.WriteLine("float4 {0} (appdata v0, appdata v1, appdata v2) {{", "tess");
                        writer.Indent++;
                        if (shader.CommentShader) writer.WriteLine("// Implement tessellation here.");
                        writer.WriteLine(shader.CommentShader
                            ? "// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderTessellation.html"
                            : "");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine();
                    }

                    // Custom lighting
                    if (!surface.UseBuiltinLighting) {
                        writer.Write("half4 Lighting{0} (SurfaceOutput s, half3 lightDir, ", "Custom");
                        writer.WriteLine(surface.ViewDirInLighting ? "half3 viewDir, half atten) {" : "half atten) {");
                        writer.Indent++;
                        if (shader.CommentShader) writer.WriteLine("// Implement custom lighting here.");
                        writer.WriteLine(shader.CommentShader
                            ? "// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderLighting.html"
                            : "");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine();

                        if (surface.UsePrePass) {
                            writer.WriteLine("half4 Lighting{0}_PrePass (SurfaceOutput s, half4 light) {{", "Custom");
                            writer.Indent++;
                            if (shader.CommentShader)
                                writer.WriteLine("// Implement prepass function for deferred lighting here");
                            writer.WriteLine(shader.CommentShader
                                ? "// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderLighting.html"
                                : "");
                            writer.Indent--;
                            writer.WriteLine("}");
                            writer.WriteLine();
                        }
                    }

                    // Lightmap decoder
                    // Single
                    if (surface.UseSingleLightMap) {
                        writer.Write("half4 Lighting{0}_SingleLightmap (SurfaceOutput s, fixed4 color", "Custom");
                        writer.WriteLine(surface.ViewDirInSingle ? ", half3 viewDir) {" : ") {");
                        writer.Indent++;
                        if (shader.CommentShader) writer.WriteLine("// Implement single lightmap decoder here.");
                        writer.WriteLine(shader.CommentShader
                            ? "// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderLighting.html"
                            : "");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine();
                    }

                    // Dual
                    if (surface.UseDualLightMap) {
                        writer.Write(
                            "half4 Lighting{0}_DualLightmap (SurfaceOutput s, fixed4 totalColor, fixed4 indirectOnlyColor, half indirectFade",
                            "Custom");
                        writer.WriteLine(surface.ViewDirInDual ? ", half3 viewDir) {" : ") {");
                        writer.Indent++;
                        if (shader.CommentShader) writer.WriteLine("// Implement dual lightmap decoder here.");
                        writer.WriteLine(shader.CommentShader
                            ? "// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderLighting.html"
                            : "");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine();
                    }

                    // Directional
                    if (surface.UseDirectionalLightMap) {
                        writer.Write("half4 Lighting{0}_DirLightmap (SurfaceOutput s, fixed4 color, fixed4 scale",
                            "Custom");
                        writer.WriteLine(surface.ViewDirInDirectional
                            ? ", half3 viewDir, bool surfFuncWritesNormal, out half3 specColor) {"
                            : ", bool surfFuncWritesNormal) {");
                        writer.Indent++;
                        if (shader.CommentShader) writer.WriteLine("// Implement directional lightmap decoder here.");
                        writer.WriteLine(shader.CommentShader
                            ? "// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderLighting.html"
                            : "");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine();
                    }

                    // Surface function
                    writer.WriteLine("void surf (Input IN, inout SurfaceOutput o) {");
                    writer.Indent++;
                    writer.WriteLine(shader.CommentShader ? "// To complete the shader, fill in the fields in o." : "");
                    if (shader.CommentShader)
                        writer.WriteLine("// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderExamples.html");
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("ENDCG");
                } else {
                    var custom = (CustomShader)subshader;

                    foreach (Pass pass in custom.GetPasses()) {
                        if (pass.PassType == PassType.VertFrag) {
                            var vfPass = (VertFragPass) pass;

                            writer.WriteLine("Pass {");
                            writer.Indent++;

                            // Name
                            if (vfPass.AllowExternalReference) {
                                writer.WriteLine("Name \"{0}\"", vfPass.name);
                            }

                            // Tags
                            if (vfPass.LightMode != LightMode.Always || vfPass.RequireSoftVegatation) {
                                writer.Write("Tags { ");
                                if (vfPass.LightMode != LightMode.Always) {
                                    writer.Write("\"LightMode\" = \"{0}\" ", vfPass.LightMode);
                                }
                                if (vfPass.RequireSoftVegatation) {
                                    writer.Write("\"RequireOptions\" = \"SoftVegetation\" ");
                                    writer.WriteLine("}");
                                }
                            }

                            // Cull
                            if (vfPass.FaceCullMode != CullMode.Back) {
                                writer.WriteLine("Cull {0}", vfPass.FaceCullMode);
                            }

                            // ZTest
                            if (vfPass.ZTestFunc != CompareFunction.LessEqual) {
                                writer.WriteLine("ZTest {0}", vfPass.ZTestFunc);
                            }

                            // ZWrite
                            if (!vfPass.WriteToDepthBuffer) {
                                writer.WriteLine("ZWrite Off");
                            }

                            // Fog
                            if (!vfPass.ApplyFog) {
                                writer.WriteLine("Fog { Mode Off }");
                            }

                            // AlphaTest (to be moved to fixed function)
//                            if (vfPass.UseAlphaTest) {
//                                writer.WriteLine("AlphaTest {0} {1}", vfPass.AlphaTestFunc, vfPass.AlphaTestValue);
//                            }

                            // Blend
                            if (vfPass.ApplyBlending) {
                                writer.Write("Blend {0} {1}", vfPass.SourceBlendFactor, vfPass.DestBlendFactor);
                                if (vfPass.BlendAlphaSeparately) {
                                    writer.WriteLine(", {0} {1}", vfPass.AlphaSourceBlendFactor, vfPass.AlphaDestBlendFactor);
                                } else {
                                    writer.WriteLine();
                                }
                            }

                            // BlendOp
                            if (vfPass.BlendOp != BlendOp.Add) {
                               writer.WriteLine("BlendOp {0}", vfPass.BlendOp);
                            }

                            // Offset
                            if (vfPass.UseDepthOffset) {
                                writer.WriteLine("Offset {0} {1}", vfPass.DepthFactor, vfPass.DepthUnit);
                            }

                            writer.WriteLine("CGPROGRAM");
                            writer.WriteLine();

                            writer.WriteLine("#pragma vertex {0}", "vertex");
                            writer.WriteLine("#pragma fragment {0}", "fragment");

                            if (vfPass.UseGeometryShader) writer.WriteLine("#pragma geometry {0}", "geometry");

                            switch (vfPass.ShaderTarget) {
                                case ShaderTarget.ShaderModel2:
                                    writer.WriteLine("#pragma target 2.0");
                                    break;
                                case ShaderTarget.ShaderModel3:
                                    writer.WriteLine("#pragma target 3.0");
                                    break;
                                case ShaderTarget.ShaderModel4:
                                    writer.WriteLine("#pragma target 4.0");
                                    break;
                                case ShaderTarget.ShaderModel5:
                                    writer.WriteLine("#pragma target 5.0");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            if (vfPass.CompileToGlsl) writer.WriteLine("#pragma glsl");
                            if (!vfPass.AutoNormalizeVectors) writer.WriteLine("#pragma glsl_no_auto_normalization");
                            if (vfPass.GenerateDebugInfo) writer.WriteLine("#pragma enable_d3d11_debug_symbols");
                            writer.WriteLine();

                            if (vfPass.CgincUnityCg) writer.WriteLine("#include \"UnityCG.cginc\"");
                            if (vfPass.CgincTerrainEngine) writer.WriteLine("#include \"TerrainEngine.cginc\"");
                            if (vfPass.CgincTessellation) writer.WriteLine("#include \"Tessellation.cginc\"");
                            if (vfPass.CgincAutoLight) writer.WriteLine("#include \"AutoLight.cginc\"");
                            if (vfPass.CgincLighting) writer.WriteLine("#include \"Lighting.cginc\"");
                            if (vfPass.CgincUnityCg || vfPass.CgincTerrainEngine || vfPass.CgincTessellation || vfPass.CgincAutoLight || vfPass.CgincLighting) {
                                writer.WriteLine();
                            }

                            //Input
                            if (!vfPass.UsePresetInput) {
                                writer.WriteLine("struct {0} {{", "appdata");
                                writer.Indent++;

                                if (vfPass.UsePosition) writer.WriteLine("float4 vertex : POSITION;");
                                if (vfPass.UseNormal) writer.WriteLine("float3 normal : NORMAL;");
                                if (vfPass.UseTexcoord) writer.WriteLine("float4 texcoord0 : TEXCOORD0;");
                                if (vfPass.UseTexcoord1) writer.WriteLine("float4 texcoord1 : TEXCOORD1;");
                                if (vfPass.UseTangent) writer.WriteLine("float4 tangent : TANGENT;");
                                if (vfPass.UseColor) writer.WriteLine("float4 color : COLOR;");

                                writer.Indent--;
                                writer.WriteLine("};");
                                writer.WriteLine();
                            }

                            if (shader.CommentShader) {
                                writer.WriteLine(
                                    "// If you want to access a property, declare it here with the same name and a matching type.");
                                writer.WriteLine();
                            }

                            // Vert out
                            writer.WriteLine("struct {0} {{", vfPass.UseGeometryShader ? "v2g" : "v2f");
                            writer.Indent++;
                            if (shader.CommentShader) writer.Write("// Define vertex output struct here.");
                            writer.WriteLine();
                            writer.Indent--;
                            writer.WriteLine("};");
                            writer.WriteLine();

                            // Geo out
                            if (vfPass.UseGeometryShader) {
                                writer.WriteLine("struct {0} {{", "g2f");
                                writer.Indent++;
                                if (shader.CommentShader) writer.Write("// Define geometry output struct here.");
                                writer.WriteLine();
                                writer.Indent--;
                                writer.WriteLine("};");
                                writer.WriteLine();
                            }

                            // Vertex shader
                            string vertInput;
                            if (vfPass.UsePresetInput) {
                                switch (vfPass.InputPreset) {
                                    case VertexInputPreset.AppdataBase:
                                        vertInput = "appdata_base";
                                        break;
                                    case VertexInputPreset.AppdataTan:
                                        vertInput = "appdata_tan";
                                        break;
                                    case VertexInputPreset.AppdataFull:
                                        vertInput = "appdata_full";
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            } else {
                                vertInput = "appdata";
                            }
                            writer.WriteLine("{0} {1} ({2} v) {{", vfPass.UseGeometryShader ? "v2g" : "v2f", "vertex", vertInput);
                            writer.Indent++;
                            if (shader.CommentShader) writer.Write("// Implement vertex shader here.");
                            writer.WriteLine();
                            writer.Indent--;
                            writer.WriteLine("}");
                            writer.WriteLine();
                            
                            // Geometry shader
                            if (vfPass.UseGeometryShader) {
                                writer.WriteLine("[maxvertexcount({0})]", vfPass.MaxVertCount);
                                writer.WriteLine("void {0} ({1} {2} input[{3}], inout {4}<{5}> output) {{", "geometry", vfPass.InputTopology.ToString().ToLower(), "v2g", (int) vfPass.InputTopology, vfPass.OutputTopology, "g2f");
                                writer.Indent++;
                                if (shader.CommentShader) writer.Write("// Implement geometry shader here.");
                                writer.WriteLine();
                                writer.Indent--;
                                writer.WriteLine("}");
                                writer.WriteLine();
                            }

                            // Fragment shader
                            writer.WriteLine("half4 {0} ({1} i) : COLOR {{", "fragment", vfPass.UseGeometryShader ? "g2f" : "v2f");
                            writer.Indent++;
                            if (shader.CommentShader) writer.Write("// Implement fragment shader here.");
                            writer.WriteLine();
                            writer.Indent--;
                            writer.WriteLine("}");
                            writer.WriteLine();
                            
                            writer.WriteLine("ENDCG");

                            writer.Indent--;
                            writer.WriteLine("}");
                            writer.WriteLine();
                        } else {
                            // TODO fixed function pass
                        }
                    }
                }
                writer.Indent--;
                writer.WriteLine("}");
                writer.WriteLine();
            }

            // Fallback shader
            if (shader.UseFallback) {
                writer.WriteLine("Fallback \"{0}\"", shader.Fallback);
            } else {
                writer.WriteLine("Fallback Off");
            }

            writer.Indent--;
            writer.Write("}");
            return writer.InnerWriter.ToString();
        }
    }
}