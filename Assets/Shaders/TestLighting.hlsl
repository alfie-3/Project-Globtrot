Shader "Shaders/StylisedShader"
{
    SubShader
    {
        Pass
        {                
            Name "ExamplePassName"
            Tags { "LightMode" = "ExampleLightModeTagValue" }

              // ShaderLab commands to set the render state go here

            HLSLPROGRAM
                // HLSL shader code goes here
            ENDHLSL
        }
    }
}