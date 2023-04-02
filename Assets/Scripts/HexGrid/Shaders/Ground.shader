Shader "Custom/Ground"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
        _WaterNormal("Water Normal", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _Terrain ("Terrain", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _WaterNormal;
        sampler2D _Noise;
        sampler2D _Terrain;
        uniform float4 _Terrain_TexelSize;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        struct NearbyTerrain {
            int2 coords;
            float4 terrain1;
            float4 terrain2;
            float4 terrain3;
            float factor1;
            float factor2;
            float factor3;
            float2 pos1;
            float2 pos2;
            float2 pos3;
            float4 highlight;
        };


        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        NearbyTerrain getNearbyTerrain(float3 position) {
            float edgeDistance = 8.66025404f * 2.0;
            float xOrg = position.x / edgeDistance;
            float x = xOrg;
            float y = -x;

            float offset = position.z / (10 * 3.0);
            x -= offset;
            y -= offset;
            float z = -x - y;

            int iX = (int)round(x);
            int iY = (int)round(y);
            int iZ = (int)round(z);
            if (iX + iY + iZ != 0)
            {
                float dX = abs(x - iX);
                float dY = abs(y - iY);
                float dZ = abs(-x - y - iZ);

                if (dX > dY && dX > dZ)
                {
                    iX = -iY - iZ;
                }
                else if (dZ > dY)
                {
                    iZ = -iX - iY;
                }
            }
            int x1 = iX;
            int z1 = iZ;
            float2 direction = normalize(float2(xOrg - (iX + iZ * 0.5), (z - iZ) * 0.866025404f));
            float2 xAxis = float2(1.0f, 0.0f);
            float dotProduct = dot(direction, xAxis);

            int2 terrain2Offset;
            int2 terrain3Offset;

            if (direction.y > 0) {
                if (dotProduct < -0.5) {
                    terrain2Offset = int2(-1, 0);
                    terrain3Offset = int2(-1, 1);
                }
                else if (dotProduct < 0.5) {
                    terrain2Offset = int2(-1, 1);
                    terrain3Offset = int2(0, 1);
                }
                else {
                    terrain2Offset = int2(0, 1);
                    terrain3Offset = int2(1, 0);
                }
            }
            else {
                if (dotProduct < -0.5) {
                    terrain2Offset = int2(-1, 0);
                    terrain3Offset = int2(0, -1);
                }
                else if (dotProduct < 0.5) {
                    terrain2Offset = int2(0, -1);
                    terrain3Offset = int2(1, -1);
                }
                else {
                    terrain2Offset = int2(1, -1);
                    terrain3Offset = int2(1, 0);
                }
            }

            int x2 = x1 + terrain2Offset.x;
            int x3 = x1 + terrain3Offset.x;
            int z2 = z1 + terrain2Offset.y;
            int z3 = z1 + terrain3Offset.y;

            float x1F, x2F, x3F;
            x1F = (float)x1 + (float)z1 * 0.5f;
            x2F = (float)x2 + (float)z2 * 0.5f;
            x3F = (float)x3 + (float)z3 * 0.5f;

            x1 = x1 + z1 / 2;
            x2 = x2 + z2 / 2;
            x3 = x3 + z3 / 2;


            NearbyTerrain output;
            output.coords = int2(x1, z1);

            
            float2 terrainCoords1 = float2((float)x1 * _Terrain_TexelSize.x, (float)z1 * _Terrain_TexelSize.y);
            float2 terrainCoords2 = float2((float)x2 * _Terrain_TexelSize.x, (float)z2 * _Terrain_TexelSize.y);
            float2 terrainCoords3 = float2((float)x3 * _Terrain_TexelSize.x, (float)z3 * _Terrain_TexelSize.y);

            output.terrain1 = tex2D(_Terrain, terrainCoords1);
            output.terrain2 = tex2D(_Terrain, terrainCoords2);
            output.terrain3 = tex2D(_Terrain, terrainCoords3);


            float heightDifference2 = abs(output.terrain1.r - output.terrain2.r) * 10;
            float heightDifference3 = abs(output.terrain1.r - output.terrain3.r) * 10;
            float d1 = 0.8f - ((xOrg - x1F) * (xOrg - x1F) + (z - (float)z1) * (z - (float)z1) * 0.866025404f * 0.866025404f);
            float d2 = (0.8f - ((xOrg - x2F) * (xOrg - x2F) + (z - (float)z2) * (z - (float)z2) * 0.866025404f * 0.866025404f));
            float d3 = (0.8f - ((xOrg - x3F) * (xOrg - x3F) + (z - (float)z3) * (z - (float)z3) * 0.866025404f * 0.866025404f));
            float highlightStrength = (1 - smoothstep(0.0f, 0.1f, min(d1 - d2 + (1.0f - abs(output.terrain1.a - output.terrain2.a)),
                d1 - d3 + (1.0f - abs(output.terrain1.a - output.terrain3.a)))));// *step(0.0f, position.y);
            float cellBorderStrength = (1 - smoothstep(0.0f, 0.03f, min(d1 - d2, d1 - d3)));
            d1 = clamp(d1, 0.0f, 1.0f);
            d2 = clamp(d2 - sqrt(d1 - d2) * (0.25 * heightDifference2 * heightDifference2), 0.0f, 1.0f);
            d3 = clamp(d3 - sqrt(d1 - d3) * (0.25 * heightDifference3 * heightDifference3), 0.0f, 1.0f);
            float dDivider = 1.0f / (d1 + d2 + d3);
            d1 = d1 * dDivider;
            d2 = d2 * dDivider;
            d3 = d3 * dDivider;



            output.pos1 = float2(x1F * edgeDistance, z1 * 15);
            output.pos2 = float2(x2F * edgeDistance, z2 * 15);
            output.pos3 = float2(x3F * edgeDistance, z3 * 15);
            output.factor1 = d1;
            output.factor2 = d2;
            output.factor3 = d3;
            output.highlight = float4(0.6f, 0.6f, 1.0f, highlightStrength);
            //output.highlight = float4(0.0f, 0.0f, 0.0f, cellBorderStrength);
            return output;
        }

        int2 fromPosition(float3 position)
        {
            float x = position.x / (8.66025404f * 2.0);
            float y = -x;

            float offset = position.z / (10 * 3.0);
            x -= offset;
            y -= offset;

            int iX = (int)round(x);
            int iY = (int)round(y);
            int iZ = (int)round(-x - y);

            if (iX + iY + iZ != 0)
            {
                float dX = abs(x - iX);
                float dY = abs(y - iY);
                float dZ = abs(-x - y - iZ);

                if (dX > dY && dX > dZ)
                {
                    iX = -iY - iZ;
                }
                else if (dZ > dY)
                {
                    iZ = -iX - iY;
                }
            }

            iX = iX + iZ / 2;
            return int2(iX, iZ);
        }

        float4 getRiver(float2 pos, NearbyTerrain nearbyTerrain, float noise) {
            float minDistance = 20;

            float distance1 = (distance(pos, nearbyTerrain.pos1) + 100 * step(1.0f - nearbyTerrain.terrain1.b, 0.95f)) * 0.86;
            if (distance1 < minDistance) minDistance = distance1;
            /*
            float distance2 = distance(pos, nearbyTerrain.pos2) + 100 * step(1.0f - nearbyTerrain.terrain2.b, 0.95f);
            if (distance2 < minDistance) minDistance = distance2;
            float distance3 = distance(pos, nearbyTerrain.pos3) + 100 * step(1.0f - nearbyTerrain.terrain3.b, 0.95f);
            if (distance3 < minDistance) minDistance = distance3;
            */

            float2 dir12 = normalize(nearbyTerrain.pos2 - nearbyTerrain.pos1);
            float2 pos12 = nearbyTerrain.pos1 + dot(dir12, pos - nearbyTerrain.pos1) * dir12;
            float distance4 = distance(pos, pos12) + 100 * step(1.0f - nearbyTerrain.terrain1.b, 0.95f) + 100 * step(1.0f - nearbyTerrain.terrain2.b, 0.95f);
            if (distance4 < minDistance) minDistance = distance4;

            float2 dir13 = normalize(nearbyTerrain.pos3 - nearbyTerrain.pos1);
            float2 pos13 = nearbyTerrain.pos1 + dot(dir13, pos - nearbyTerrain.pos1) * dir13;
            float distance5 = distance(pos, pos13) + 100 * step(1.0f - nearbyTerrain.terrain1.b, 0.95f) + 100 * step(1.0f - nearbyTerrain.terrain3.b, 0.95f);
            if (distance5 < minDistance) minDistance = distance5;

            float depth = 0.8 * (1.0f - smoothstep(2.0f, 4.0f, minDistance + noise));
            return float4(0.05619437, 0.4568464, 0.3867925, depth);
        }

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            NearbyTerrain terrain = getNearbyTerrain(IN.worldPos);
            float2 uv = float2(IN.worldPos.x / 10, IN.worldPos.z / 10);
            float2 uvNoise = float2(IN.worldPos.x / 20, IN.worldPos.z / 20);
            float2 uvNoiseBig = float2(IN.worldPos.x / 100, IN.worldPos.z / 100);
            float4 noiseSample = tex2D(_Noise, uvNoise);
            float4 noiseSampleBig = tex2D(_Noise, uvNoiseBig);
            float noise = (noiseSample.r + noiseSampleBig.r) / 2;



            float2 uvWater1 = (float2(1.0f, 1.0f) + ((0.25 * uv + 0.5 * float2(_Time.x, _Time.x))));
            float2 uvWater2 = (float2(1.0f, 1.0f) + ((0.5 * uv + 0.5 * float2(0, -_Time.x))));
            float4 river = float4(1.0f, 1.0f, 1.0f, 0.0f);
            if (terrain.terrain1.b < 0.06f || terrain.terrain2.b < 0.06f || terrain.terrain3.b < 0.06f) {
                river = getRiver(float2(IN.worldPos.x + noiseSampleBig.r, IN.worldPos.z + noiseSampleBig.g), terrain, 2 - 4 * noiseSampleBig.r);
            }
            river.a *= smoothstep(-0.5, -0.2, IN.worldPos.y);
            float3 waterNormal1 = UnpackScaleNormal(tex2D(_WaterNormal, uvWater1), 0.8 * river.a);
            float3 waterNormal2 = UnpackScaleNormal(tex2D(_WaterNormal, uvWater2), 0.8 * river.a);
            float2 uvOffset = (waterNormal1 + waterNormal2).xy * smoothstep(0.0, 0.2, river.a);
            uvOffset *= 0.2;
            uv += float2(0.1f * noiseSampleBig.g, 0.1f * noiseSampleBig.b);
            uv += uvOffset;
            uv = uv % 1;
            uv += float2(1.0, 1.0);
            uv = uv % 1; 
            //int2 coords = fromPosition(IN.worldPos);
            //float2 terrainCoords = float2((float)coords.x * _Terrain_TexelSize.x, (float)coords.y * _Terrain_TexelSize.y);

            float offset = 0.003;
            // Albedo comes from a texture tinted by color
            float4 albedo1 = tex2D (_MainTex, float2(uv.x * (0.25 - 2 * offset) + 0.0 + offset, uv.y * (0.25 - 2 * offset) + 0.75 + offset)) * _Color;
            float4 albedo2 = tex2D(_MainTex, float2(uv.x * (0.25 - 2 * offset) + 0.25 + offset, uv.y * (0.25 - 2 * offset) + 0.75 + offset)) * _Color;
            float4 albedo3 = tex2D(_MainTex, float2(uv.x * (0.25 - 2 * offset) + 0.5 + offset, uv.y * (0.25 - 2 * offset) + 0.75 + offset)) * _Color;
            float4 albedo4 = tex2D(_MainTex, float2(uv.x * (0.25 - 2 * offset) + 0.75 + offset, uv.y * (0.25 - 2 * offset) + 0.75 + offset)) * _Color;

             //float4 normal2 = tex2D(_MainTex, float2(uv.x * (0.25 - 2 * offset) + 0.25 + offset, uv.y * (0.25 - 2 * offset) + 0.5 + offset));
            //float4 normal3 = tex2D(_MainTex, float2(uv.x * (0.25 - 2 * offset) + 0.5 + offset, uv.y * (0.25 - 2 * offset) + 0.5 + offset));
            //float4 normal4 = tex2D(_MainTex, float2(uv.x * (0.25 - 2 * offset) + 0.75 + offset, uv.y * (0.25 - 2 * offset) + 0.5 + offset));

            /*
            float4 AO1 = tex2D(_MainTex, float2(uv.x * 0.246 + 0.002, uv.y * 0.246 + 0.252));
            float4 AO2 = tex2D(_MainTex, float2(uv.x * 0.246 + 0.252, uv.y * 0.246 + 0.252));
            float4 AO3 = tex2D(_MainTex, float2(uv.x * 0.246 + 0.502, uv.y * 0.246 + 0.252));
            float4 AO4 = tex2D(_MainTex, float2(uv.x * 0.246 + 0.752, uv.y * 0.246 + 0.252));

            float4 smoothness1 = tex2D(_MainTex, float2(uv.x * 0.246 + 0.002, uv.y * 0.246 + 0.002));
            float4 smoothness2 = tex2D(_MainTex, float2(uv.x * 0.246 + 0.252, uv.y * 0.246 + 0.002));
            float4 smoothness3 = tex2D(_MainTex, float2(uv.x * 0.246 + 0.502, uv.y * 0.246 + 0.002));
            float4 smoothness4 = tex2D(_MainTex, float2(uv.x * 0.246 + 0.752, uv.y * 0.246 + 0.002));
            */

            
            float waterAvailability = noise * 0.5f - 0.25f + 1.0f * (terrain.factor1 * terrain.terrain1.g + terrain.factor2 * terrain.terrain2.g + terrain.factor3 * terrain.terrain3.g);
            waterAvailability = waterAvailability * smoothstep(0.0f, 0.2f, IN.worldPos.y) * (1.0 - smoothstep(0.0, 0.2, river.a));
            float factor1 = 1.0f;
            float factor2 = smoothstep(0.25, 0.35, waterAvailability);
            float factor3 = smoothstep(0.45, 0.65, waterAvailability);
            float factor4 = smoothstep(0.8, 1.1, waterAvailability);
            factor1 -= factor2;
            factor2 -= factor3;
            factor3 -= factor4;

            float4 c = albedo1 * factor1 + albedo2 * factor2 + albedo3 * factor3 + albedo4 * factor4;
            c *= 0.4 * noiseSampleBig.r + 0.8f;
            //float4 c = terrain.factor1 * terrain.terrain1 + terrain.factor2 * terrain.terrain2 + terrain.factor3 * terrain.terrain3;
            //c = float4(terrain.distanceSqr1, 1.0f, 1.0f, 1.0f);
            //float highlight = terrain.coords.x % 2 * 0.1f + terrain.coords.y % 2 * 0.2f;
            
            
            o.Albedo = (1.0f - 0.6 * river.a * river.a) * c.rgb + 0.6 * river.a * river.a * river.rgb; //*(0.9 + highlight);
            o.Albedo = o.Albedo * (1 - terrain.highlight.a) + terrain.highlight.rgb * terrain.highlight.a;
            o.Emission = terrain.highlight.rgb * terrain.highlight.a * (1.0f * 0.1f * sin(_Time.w * 2));
            float3 normal1 = UnpackScaleNormal(tex2D(_MainTex, float2(uv.x * (0.25 - 2 * offset) + 0.0 + offset, uv.y * (0.25 - 2 * offset) + 0.5 + offset)), 1.0f - river.a);
            o.Normal = normal1 + waterNormal1 + waterNormal2;
            //o.Albedo = normal1 + waterNormal1 + waterNormal2;

            //o.Occlusion = AO1 * factor1 + AO2 * factor2 + AO3 * factor3 + AO4 * factor4;
            //o.Smoothness = smoothness1 * factor1 + smoothness2 * factor2 + smoothness3 * factor3 + smoothness4 * factor4;
            // Metallic and smoothness come from slider variables
            o.Specular = 0.05 * river.a;
            o.Smoothness = /*(1 - river.a) * */_Glossiness + river.a;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
