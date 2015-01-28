using System;
using System.CodeDom.Compiler;
using System.IO;

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
                            writer.WriteLine("{0} (\"{1}\", Range ({2}, {3})) = {4}", range.Name, range.DisplayName, range.MinValue, range.MaxValue, range.DefaultValue);
                            break;
                        case PropertyType.Color:
                            var color = (ColorProperty) property;
                            writer.WriteLine("{0} (\"{1}\", Color) = ({2},{3},{4},{5})", color.Name, color.DisplayName, color.DefaultValue.r, color.DefaultValue.g, color.DefaultValue.b, color.DefaultValue.a);
                            break;
                        case PropertyType.Texture:
                            // todo texgen, textype
                            var tex = (TextureProperty) property;
                            writer.WriteLine("{0} (\"{1}\", {2}) = \"{3}\" {{}}", tex.Name, tex.DisplayName, tex.TextureType.ToString().Replace("Two", "2"), tex.DefaultValue);
                            break;
                        case PropertyType.Float:
                            var num = (FloatProperty) property;
                            writer.WriteLine("{0} (\"{1}\", Float) = {2}", num.Name, num.DisplayName, num.DefaultValue);
                            break;
                        case PropertyType.Vector:
                            var vector = (VectorProperty) property;
                            writer.WriteLine("{0} (\"{1}\", Vector) = ({2},{3},{4},{5})", vector.Name, vector.DisplayName, vector.DefaultValue.x, vector.DefaultValue.y, vector.DefaultValue.z, vector.DefaultValue.w);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                writer.Indent--;
                // }
                writer.WriteLine("}");
            }

            foreach (Subshader subshader in shader.GetSubshaders()) {
                // Subshader {
                writer.WriteLine("Subshader {");
                writer.Indent++;
                if (subshader.SubshaderType == SubshaderType.Surface) {
                    var surface = (SurfaceShader) subshader;
                    if (surface.RenderPosition != RenderPosition.Geometry || surface.ForceNoShadowCasting || surface.IgnoreProjector) {
                        // Tags { 
                        writer.Write("Tags { ");
                        if (surface.RenderPosition != RenderPosition.Geometry) writer.Write("\"Queue\" = \"{0}\" ", surface.RenderPosition);
                        if (surface.ForceNoShadowCasting) writer.Write("\"ForceNoShadowCasting\" = \"True\" ");
                        if (surface.IgnoreProjector) writer.Write("\"IgnoreProjector\" = \"True\" ");
                        // }
                        writer.WriteLine("}");
                    }
                    writer.WriteLine("CGPROGRAM");
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
                    if (surface.CgincTerrainEngine) writer.WriteLine("#include {0}", "TerrainEngine.cginc");
                    if (surface.UseTessellation || surface.CgincTessellation) writer.WriteLine("#include {0}", "Tessellation.cginc");

                    writer.WriteLine("ENDCG");

                } else {
                    
                }
                writer.Indent--;
                //     } 
                writer.WriteLine("}");
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