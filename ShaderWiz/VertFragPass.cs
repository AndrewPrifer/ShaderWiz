using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderWiz {
    internal class VertFragPass : Pass {
        public bool WriteToDepthBuffer { get; set; }
        public CompareFunction ZTestFunc { get; set; }
        public bool UseDepthOffset { get; set; }
        public float DepthFactor { get; set; }
        public float DepthUnit { get; set; }
        public bool UseAlphaTest { get; set; }
        public CompareFunction AlphaTestFunc { get; set; }
        public float AlphaTestValue { get; set; }
        public bool ApplyBlending { get; set; }
        public BlendMode SourceBlendFactor { get; set; }
        public BlendMode DestBlendFactor { get; set; }
        public bool BlendAlphaSeparately { get; set; }
        public BlendMode AlphaSourceBlendFactor { get; set; }
        public BlendMode AlphaDestBlendFactor { get; set; }
        public BlendOp BlendOp { get; set; }
        public bool UseGeometryShader { get; set; }
        public bool UseHullShader { get; set; }
        public bool UseDomainShader { get; set; }
        public bool CompileToGlsl { get; set; }
        public bool AutoNormalizeVectors { get; set; }
        public bool GenerateDebugInfo { get; set; }
        public ShaderTarget ShaderTarget { get; set; }
        public Renderer ExcludeRenderer { get; set; }
        public LightMode LightMode { get; set; }
        public List<RenderOption> RequireOptions { get; set; }
        public CullMode FaceCullMode { get; set; }
        public bool ApplyFog { get; set; }
        public bool AllowExternalReference { get; set; }
    }
}