namespace ShaderWiz {
    internal static class HelpText {
        public const string ShaderName = "Your shader will appear in the material inspector listed under Name.";

        public const string ShaderProperties =
            "Shaders can have a list of properties. Any properties declared in a shader are shown in the material inspector inside Unity. Typical properties are the object color, textures, or just arbitrary values to be used by the shader.";

        public const string Subshaders =
            "Each shader is comprised of a list of sub-shaders. You must have at least one. When loading a shader, Unity will go through the list of subshaders, and pick the first one that is supported by the end user’s machine. If no subshaders are supported, Unity will try to use fallback shader. The name field is only for your convenience and will not be reflected in the generated code.";

        public const string Fallback =
            "After all Subshaders a Fallback can be defined. It basically says “if none of subshaders can run on this hardware, try using the ones from another shader”.";

        public const string VertexModifier =
            "It is possible to use a vertex modifier function that will modify incoming vertex data in the vertex shader. This can be used for procedural animation, extrusion along normals and so on.";

        public const string CustomDataPerVertex =
            "Using a vertex modifier function it is also possible to compute custom data in a vertex shader, which then will be passed to the surface shader function per-pixel.";

        public const string FinalColorModifier =
            "It is possible to use a final color modifier function that will modify final color computed by the shader.";

        public const string Tessellation =
            "Surface Shaders have some support for DirectX 11 GPU Tessellation. Vertex modifier is invoked after tessellation. Limited to triangle domain and shader model 5.";

        public const string HalfDirInLighting =
            "Pass half-direction vector into the lighting function instead of view-direction. Half-direction will be computed and normalized per vertex. This is faster, but not entirely correct.";

        public const string BuiltinLighting =
            "Built-in lighting models are Lambert (diffuse) and BlinnPhong (specular).";

        public const string CustomLighting = "Allows you to implement your own lighting model.";

        public const string SingleLightmap =
            "All static illumination (i.e. from baked only and auto lights, sky lights and emissive materials) gets baked into one set of lightmaps. These lightmaps are used on all lightmapped objects regardless of shadow distance.";

        public const string DualLightmap =
            "Dual lightmaps is Unity’s approach to make lightmapping work with specular, normal mapping and proper blending of baked and realtime shadows. It’s also a way to make your lightmaps look good even if the lightmap resolution is low.";

        public const string DirectionalLightmap =
            "Directional lightmaps store information on light directionality in a second set of lightmaps. The first set of lightmaps (color) stores just the diffuse lighting, same as Single Lightmaps do. The second set of lightmaps (scale) stores the ratio of the desaturated incoming light per basis vector.";

        public const string OutputNormalSpecified =
            "If you write to o.Normal and want to use reflection vector or normal vector based on per-pixel normal map (WorldReflectionVector (IN, o.Normal), WorldNormalVector (IN, o.Normal)), then you MUST check this checkbox.";

        public const string ForwardAdditive =
            "Disabling it makes the shader support one full directional light, with all other lights computed per-vertex/SH.";

        public const string TerrainInclude = "Helper functions for Terrain & Vegetation shaders.";

        public const string TessellationInclude = "Helper functions for tessellation.";

        public const string UnityCGInclude = "Commonly used global variables and helper functions.";

        public const string AutoLightInclude =
            "Lighting & shadowing functionality, e.g. surface shaders use this file internally.";

        public const string LightingInclude =
            "Standard surface shader lighting models; automatically included when you’re writing surface shaders.";

        public const string Passes = "The geometry of an onject is rendered once per pass.";

        public const string WriteToDepthBuffer =
            "Controls whether pixels from this object are written to the depth buffer. If you’re drawng solid objects, leave this on. If you’re drawing semitransparent effects, switch it off.";

        public const string DepthTestFunc = "How should depth testing be performed.";

        public const string UseDepthOffset =
            " This allows you to force one polygon to be drawn on top of another although they are actually in the same position. For example [Factor: 0, Unit: –1] pulls the polygon closer to the camera ignoring the polygon’s slope, whereas [Factor –1, Unit: –1] will pull the polygon even closer when looking at a grazing angle.";

        public const string DepthFactor = "Factor scales the maximum Z slope, with respect to X or Y of the polygon.";

        public const string DepthUnit = "Unit scales the minimum resolvable depth buffer value.";

        public const string Blending =
            "When graphics are rendered, after all shaders have executed and all textures have been applied, the pixels are written to the screen. How they are combined with what is already there is controlled by Blending.";

        public const string ApplyBlending = "If disabled, existing pixels are overwritten by the new ones";

        public const string SrcFactor = "The generated color is multiplied by the source factor.";

        public const string DstFactor = "The color already on screen is multiplied by destination factor.";

        public const string VertexAttributes = "The data that should be provided to the vertex program.";

        public const string UsePresetInput = "Use a built-in input structure.";

        public const string UseCustomInput = "Define your own input structure.";

        public const string InputPreset = "Appdata Base: vertex consists of position, normal and one texture coordinate.\n\n"+
                                            "Appdata Tan: vertex consists of position, tangent, normal and one texture coordinate.\n\n"+
                                            "Appdata Full: vertex consists of position, tangent, normal, two texture coordinates and color.";

        public const string CompileToGlsl =
            "(Deprecated in Unity 5) When compiling shaders for desktop OpenGL platforms, convert Cg/HLSL into GLSL (instead of default setting which is ARB vertex/fragment programs). Use this to enable derivative instructions, texture sampling with explicit LOD levels, etc.";

        public const string AutoNormalizeVectors =
            "(Deprecated in Unity 5) When compiling shaders for mobile GLSL (iOS/Android), enable automatic normalization of normal & tangent vectors.";

        public const string GenerateDebugInfo =
            "Generate debug information for shaders compiled for DirectX 11, this will allow you to debug shaders via Visual Studio 2012 (or higher) Graphics debugger.";

        public const string LightMode = "Light Mode defines a Pass’ role in the lighting pipeline.";

        public const string FaceCullMode = "Controls which sides of polygons should be culled (not drawn).";

        public const string RequireSoftVegetation =
            "Render this pass only if Soft Vegetation is on in Quality Settings.";

        public const string ApplyFog =
            "Fogging blends the color of the generated pixels down towards a constant color based on distance from camera. Fogging does not modify a blended pixel’s alpha value, only its RGB components.";

        public const string AllowExternalReference =
            "The name of the pass will be explicitly included, so it can be used externally from another shader.";


    }
}