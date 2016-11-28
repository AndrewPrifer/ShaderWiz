# [ShaderWiz 1.0][Website]
ShaderWiz is a shader scaffolding tool for [Unity][] that helps you configure your shader to work with the engine. All the necessary property fields, input struct members, preprocessor directives, custom functions that you would normally have to write by hand when you create a shader are generated for you. ShaderWiz also includes helpful comments in the generated skeleton to help you get started quickly.

## Features
- Support for vertex shader options
- Support for fragment shader options
- Support for geometry shader options
- Support for surface shader options

## Example
```C
Shader "NewShader" {
    Subshader {
        Tags { "ForceNoShadowCasting" = "True" "IgnoreProjector" = "True" }
        
        Pass {
            CGPROGRAM
            
            #pragma vertex vertex
            #pragma fragment fragment
            #pragma target 2.0
            
            #include "UnityCG.cginc"
            
            // If you want to access a property, declare it here with the same name.
            
            struct v2f {
                // Define vertex output struct here.
            };
            
            v2f vertex (appdata_full v) {
                // Implement vertex shader here.
            }
            
            half4 fragment (v2f i) : COLOR {
                // Implement fragment shader here.
            }
            
            ENDCG
        }
        
    }
    
    Fallback "Diffuse"
}
```

## Screenshot
![Screenshot](http://i.imgur.com/fflmK8p.png)

## Links:
- [Asset Store][]
- [Forum Thread][]
- [Website][]

[Asset Store]: https://www.assetstore.unity3d.com/en/#!/content/29931
[Forum Thread]: http://forum.unity3d.com/threads/released-shaderwiz-shader-wizard-for-unity.299174
[Website]: http://andrewpeterprifer.me/ShaderWiz
[Unity]: http://unity3d.com
