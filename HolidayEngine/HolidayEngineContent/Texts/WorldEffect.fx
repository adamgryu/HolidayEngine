// -- Projection information.
float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

// -- Shadow Map Projection Information
float4x4 xShadowMapWorldViewProj;
float xlightNearClip = 5.0f;
float xlightFarClip = 800.0f;
Texture xShadowMap;
sampler ShadowMapSampler = sampler_state { texture = <xShadowMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};

// -- Texture information.
Texture xTexture;
float xAlpha;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = POINT; minfilter = POINT; mipfilter=POINT; AddressU = clamp; AddressV = clamp;};

// -- Light information.
float xAmbience;
float xLightIntensity;
float3 xLightDirection; // NOTE: this vector should be normalized when imported.
float3 xLightPosition;

// ==============================
// ====== SIMPLE TECHNIQUE ======
// ==============================

struct VSInput
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal   : NORMAL;
};

struct VSOutput
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
};

struct PSOutput
{
	float4 Color : COLOR0;
};

VSOutput VSSimple(VSInput input)
{
    VSOutput output = (VSOutput)0;

    float4 worldPosition = mul(input.Position, xWorld);
    float4 viewPosition = mul(worldPosition, xView);
    output.Position = mul(viewPosition, xProjection);
	output.TexCoord = input.TexCoord;
	output.Normal = normalize(mul(input.Normal, (float3x3)xWorld));

    return output;
}

PSOutput PSSimple(VSOutput input)
{
	PSOutput output = (PSOutput)0;
	output.Color = tex2D(TextureSampler, input.TexCoord);

	float diffuseBrightness = saturate(dot(xLightDirection, input.Normal)) * xLightIntensity;
	output.Color.rgb *= saturate(diffuseBrightness + xAmbience);

	clip(output.Color.a < 0.1f ? -1:1 );

    return output;
}

technique Simple
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VSSimple();
        PixelShader = compile ps_2_0 PSSimple();
    }
}

// ==============================
// ====== POINT TECHNIQUE ======
// ==============================

struct VSOutputPoint
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float4 Position3D : TEXCOORD2;
	float4 PositionByLight : TEXCOORD3;
};

VSOutputPoint VSPoint(VSInput input)
{
    VSOutputPoint output = (VSOutputPoint)0;

    float4 worldPosition = mul(input.Position, xWorld);
    float4 viewPosition = mul(worldPosition, xView);
    output.Position = mul(viewPosition, xProjection);
	output.Position3D = worldPosition; // Needed for point lighting.
	output.TexCoord = input.TexCoord; // Needed for texture.
	output.Normal = normalize(mul(input.Normal, (float3x3)xWorld)); // Needed for point lightng.
	output.PositionByLight = mul(input.Position, xShadowMapWorldViewProj); // Needed for shadow.

    return output;
}

PSOutput PSPoint(VSOutputPoint input)
{
	float4 baseColor = tex2D(TextureSampler, input.TexCoord);
	clip(baseColor.a < 0.1f ? -1:1 );
	PSOutput output = (PSOutput)0;
	output.Color = baseColor;

	float2 ProjectedTexCoords;
    ProjectedTexCoords[0] = (input.PositionByLight.x/input.PositionByLight.w/2.0f) + 0.5f;
    ProjectedTexCoords[1] = -(input.PositionByLight.y/input.PositionByLight.w/2.0f) + 0.5f;

	float diffuseBrightness = 0;
	if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
	{
		float depthStoredByShadowMap = tex2D(ShadowMapSampler, ProjectedTexCoords).r;
		float realDistance = (input.PositionByLight.w - xlightNearClip) / (xlightFarClip - xlightNearClip);
		if ((realDistance - 1.0f/100.0f) <= depthStoredByShadowMap)
		{
			float3 lightDir = normalize(input.Position3D - xLightPosition);
			diffuseBrightness = saturate(dot(-lightDir, input.Normal)) * xLightIntensity; 
		}
	}
	else
	{
		float3 lightDir = normalize(input.Position3D - xLightPosition);
		diffuseBrightness = saturate(dot(-lightDir, input.Normal)) * xLightIntensity; 
	}

	output.Color.rgb *= (diffuseBrightness + xAmbience);
	output.Color.a *= xAlpha;

    return output;
}

technique Point
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VSPoint();
        PixelShader = compile ps_2_0 PSPoint();
    }
}

// ===============================
// ======= COLOR TECHNIQUE =======
// ===============================

struct VSInputColor
{
    float4 Position : POSITION;
	float4 Color : COLOR0;
};

struct VSOutputColor
{
    float4 Position : POSITION;
	float4 Color : COLOR0;
};

VSOutputColor VSColor(VSInputColor input)
{
    VSOutputColor output = (VSOutputColor)0;

    float4 worldPosition = mul(input.Position, xWorld);
    float4 viewPosition = mul(worldPosition, xView);
    output.Position = mul(viewPosition, xProjection);
	output.Color = input.Color;

    return output;
}

PSOutput PSColor(VSOutputColor input)
{
	PSOutput output = (PSOutput)0;
	output.Color = input.Color;

    return output;
}

technique Color
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VSColor();
        PixelShader = compile ps_2_0 PSColor();
    }
}

// ===================================
// ======= SHADOWMAP TECHNIQUE =======
// ===================================

struct VSInputShadowMap
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VSOutputShadowMap
{
    float4 Position : POSITION;
	float Depth : TEXCOORD0;
	float2 TexCoord : TEXCOORD1;
};

VSOutputShadowMap VSShadowMap(VSInputShadowMap input)
{
    VSOutputShadowMap output = (VSOutputShadowMap)0;

    output.Position = mul(input.Position, xShadowMapWorldViewProj);
	output.Depth = (output.Position.w - xlightNearClip) / (xlightFarClip - xlightNearClip);
	output.TexCoord = input.TexCoord;

    return output;
}

PSOutput PSShadowMap(VSOutputShadowMap input)
{
	float4 baseColor = tex2D(TextureSampler, input.TexCoord);
	clip(baseColor.a < 0.1f ? -1:1 );

	PSOutput output = (PSOutput)0;
	output.Color.a = 1;
	output.Color.rgb = input.Depth;

    return output;
}

technique ShadowMap
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VSShadowMap();
        PixelShader = compile ps_2_0 PSShadowMap();
    }
}