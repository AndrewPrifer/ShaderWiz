using System;
using System.CodeDom.Compiler;
using System.IO;
using UnityEngine;

namespace ShaderWizard {
    internal static class ShaderGenerator {
        public static string Generate(ShaderSettings shader, string tabString) {
            var writer = new IndentedTextWriter(new StringWriter(), tabString);

            // Shader "Name" {
            writer.WriteLine("Shader \"{0}\" {{", shader.Name);
            writer.Indent++;

            if (shader.GetProperties().Count != 0) {
                // Properties {
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
                                tex.TextureType.ToString().Replace("Two", "2"), tex.DefaultValue,
                                tex.TexGenMode == TexGenMode.None ? "" : "TexGen " + tex.TexGenMode.ToString());
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
                // }
                writer.WriteLine("}");
                writer.WriteLine();
            }

            foreach (Subshader subshader in shader.GetSubshaders()) {
                // Subshader {
                writer.WriteLine("Subshader {");
                writer.Indent++;
                if (subshader.SubshaderType == SubshaderType.Surface) {
                    var surface = (SurfaceShader) subshader;
                    if (surface.RenderPosition != RenderPosition.Geometry || surface.ForceNoShadowCasting ||
                        surface.IgnoreProjector) {
                        // Tags { 
                        writer.Write("Tags { ");
                        if (surface.RenderPosition != RenderPosition.Geometry)
                            writer.Write("\"Queue\" = \"{0}\" ", surface.RenderPosition);
                        if (surface.ForceNoShadowCasting) writer.Write("\"ForceNoShadowCasting\" = \"True\" ");
                        if (surface.IgnoreProjector) writer.Write("\"IgnoreProjector\" = \"True\" ");
                        // }
                        writer.WriteLine("}");
                        writer.WriteLine();
                    }
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
                    if (surface.CommentFinal) writer.Write("debug ");
                    writer.WriteLine();
                    // Includes
                    if (surface.CgincTerrainEngine) writer.WriteLine("#include {0}", "TerrainEngine.cginc");
                    if (surface.CgincTessellation) writer.WriteLine("#include {0}", "Tessellation.cginc");
                    writer.WriteLine();

                    if (shader.CommentShader)
                        writer.WriteLine(
                            "// If you want to access a property, declare it here with the same name and a matching type\n");

                    // Input
                    if (shader.CommentShader)
                        writer.WriteLine("// To use second uv set, declare uv2<TextureName> inside Input");
                    writer.WriteLine("struct Input {");
                    writer.Indent++;

                    if (surface.UvInInput) {
                        foreach (ShaderProperty property in shader.GetProperties()) {
                            if (property.PropertyType == PropertyType.Texture) {
                                writer.WriteLine("float2 uv{0};", property.Name);
                            }
                        }
                    }
                    if (surface.ViewDirInInput) writer.WriteLine("float3 viewDir;");
                    if (surface.VertColorInInput) writer.WriteLine("float4 vertColor : COLOR;");
                    if (surface.SsPositionInInput) writer.WriteLine("float4 screenPos;");
                    if (surface.WsPositionInInput) writer.WriteLine("float3 worldPos;");
                    if (surface.WorldReflectionVectorInInput) {
                        writer.Write("float3 worldRefl");
                        writer.WriteLine(surface.OutNormalSpecified ? "; INTERNAL_DATA" : ";");
                    }
                    if (surface.WorldNormalInInput) {
                        writer.Write("float3 worldNormal");
                        writer.WriteLine(surface.OutNormalSpecified ? "; INTERNAL_DATA" : ";");
                    }
                    writer.Indent--;
                    writer.WriteLine("};");
                    writer.WriteLine();
                    // Vertex function
                    if (surface.UseVertexModifier) {
                        writer.Write("void {0} (inout {1} v", "vert", "appdata_full");
                        writer.WriteLine(surface.CustomDataPerVertex ? ", out Input o) {" : ") {");
                        writer.Indent++;
                        writer.WriteLine(shader.CommentShader ? "// Modify vertices here" : "");
                        if (surface.CustomDataPerVertex) {
                            writer.WriteLine(shader.CommentShader
                                ? "// To pass custom data to surface function, write to o"
                                : "");
                        }
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
                        writer.WriteLine(shader.CommentShader ? "// Modify final color here" : "");
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
                        if (shader.CommentShader) writer.WriteLine("// Implement tessellation here");
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
                        if (shader.CommentShader) writer.WriteLine("// Implement custom lighting here");
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
                    if (surface.UseSingleLightMap) {
                        writer.Write("half4 Lighting{0}_SingleLightmap (SurfaceOutput s, fixed4 color", "Custom");
                        writer.WriteLine(surface.ViewDirInSingle ? ", half3 viewDir) {" : ") {");
                        writer.Indent++;
                        if (shader.CommentShader) writer.WriteLine("// Implement single lightmap decoder here");
                        writer.WriteLine(shader.CommentShader
                            ? "// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderLighting.html"
                            : "");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine();
                    }
                    if (surface.UseDualLightMap) {
                        writer.Write(
                            "half4 Lighting{0}_DualLightmap (SurfaceOutput s, fixed4 totalColor, fixed4 indirectOnlyColor, half indirectFade",
                            "Custom");
                        writer.WriteLine(surface.ViewDirInDual ? ", half3 viewDir) {" : ") {");
                        writer.Indent++;
                        if (shader.CommentShader) writer.WriteLine("// Implement dual lightmap decoder here");
                        writer.WriteLine(shader.CommentShader
                            ? "// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderLighting.html"
                            : "");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine();
                    }
                    if (surface.UseDirectionalLightMap) {
                        writer.Write("half4 Lighting{0}_DirLightmap (SurfaceOutput s, fixed4 color, fixed4 scale",
                            "Custom");
                        writer.WriteLine(surface.ViewDirInDirectional
                            ? ", half3 viewDir, bool surfFuncWritesNormal, out half3 specColor) {"
                            : ", bool surfFuncWritesNormal) {");
                        writer.Indent++;
                        if (shader.CommentShader) writer.WriteLine("// Implement directional lightmap decoder here");
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
                    writer.WriteLine(shader.CommentShader ? "// To create the shader, fill in the fields in o" : "");
                    if (shader.CommentShader)
                        writer.WriteLine("// Help: http://docs.unity3d.com/Manual/SL-SurfaceShaderExamples.html");
                    writer.Indent--;
                    writer.WriteLine("}");

                    writer.WriteLine("ENDCG");
                } else {
                    // TODO Custom shader
                }
                writer.Indent--;
                //     } 
                writer.WriteLine("}");
                writer.WriteLine();
            }

            if (shader.UseFallback) {
                // Fallback "fallback"
                writer.WriteLine("Fallback \"{0}\"", shader.Fallback);
            }

            writer.Indent--;
            // }
            writer.Write("}");
            return writer.InnerWriter.ToString();
        }
    }
}