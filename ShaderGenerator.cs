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

            foreach (var subshader in shader.GetSubshaders()) {
                //     Subshader {
                writer.WriteLine("Subshader {");
                writer.Indent++;

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