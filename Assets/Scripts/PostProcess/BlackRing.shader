Shader "Unlit/BlackRing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VisibilityTexture ("Visibility Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            sampler2D _VisibilityTexture;
            float4 _VisibilityTexture_TexelSize;
            sampler2D _CameraDepthTexture;
            float4x4 _ViewProjectInverse;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldDirection : TEXCOORD1;
            };

            struct NearbyCells {
                int2 coords;
                float2 pos1;
                float2 pos2;
                float2 pos3;
                float fogAmount;
            };


            NearbyCells getNearbyCells(float3 position) {
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


                NearbyCells output;
                output.coords = int2(x1, z1);

                float2 terrainCoords1 = float2((float)x1 * _VisibilityTexture_TexelSize.x, (float)z1 * _VisibilityTexture_TexelSize.y);
                float2 terrainCoords2 = float2((float)x2 * _VisibilityTexture_TexelSize.x, (float)z2 * _VisibilityTexture_TexelSize.y);
                float2 terrainCoords3 = float2((float)x3 * _VisibilityTexture_TexelSize.x, (float)z3 * _VisibilityTexture_TexelSize.y);


                float d1 = 0.8f - ((xOrg - x1F) * (xOrg - x1F) + (z - (float)z1) * (z - (float)z1) * 0.866025404f * 0.866025404f);
                float d2 = (0.8f - ((xOrg - x2F) * (xOrg - x2F) + (z - (float)z2) * (z - (float)z2) * 0.866025404f * 0.866025404f));
                float d3 = (0.8f - ((xOrg - x3F) * (xOrg - x3F) + (z - (float)z3) * (z - (float)z3) * 0.866025404f * 0.866025404f));
                float distanceDifference = min(d1 - d2, d1 - d3);
                d1 = clamp(d1, 0.0f, 1.0f);
                d2 = clamp(d2 - sqrt(d1 - d2), 0.0f, 1.0f);
                d3 = clamp(d3 - sqrt(d1 - d3), 0.0f, 1.0f);
                float dDivider = 1.0f / (d1 + d2 + d3);
                d1 = d1 * dDivider;
                d2 = d2 * dDivider;
                d3 = d3 * dDivider;



                output.pos1 = float2(x1F * edgeDistance, z1 * 15);
                output.pos2 = float2(x2F * edgeDistance, z2 * 15);
                output.pos3 = float2(x3F * edgeDistance, z3 * 15);
                output.fogAmount = tex2D(_VisibilityTexture, terrainCoords1).r * d1 + tex2D(_VisibilityTexture, terrainCoords2).r * d2 + tex2D(_VisibilityTexture, terrainCoords3).r * d3;
                return output;
            }


            int2 fromPosition(float3 position, out float distanceFromCentre)
            {
                float x = position.x / (8.66025404f * 2.0);
                float y = -x;
                float z = position.z / (15.0f);

                float offset = position.z / (10 * 3.0);
                x -= offset;
                y -= offset;

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

                iX = iX + iZ / 2;

                x = position.x / (8.66025404f * 2.0);
                distanceFromCentre = step(0.6, sqrt(frac(iX - x) * frac(iX - x)/* + (z - iZ) * (z - iZ)*/));
                return int2(iX, iZ);
            }

            v2f vert(appdata i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                float4 D = mul(_ViewProjectInverse, float4((i.uv.x) * 2 - 1, (i.uv.y) * 2 - 1, 0.5, 1));
                D.xyz /= D.w;
                D.xyz -= _WorldSpaceCameraPos;
                float4 D0 = mul(_ViewProjectInverse, float4(0, 0, 0.5, 1));
                D0.xyz /= D0.w;
                D0.xyz -= _WorldSpaceCameraPos;
                o.worldDirection = D.xyz / length(D0.xyz);
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                depth = LinearEyeDepth(depth);
                /* World position of some random point along this ray */
                float3 WD = i.worldDirection;
                /* Multiply by 'depth' */
                WD *= depth;
                /* That's our world-coordinate position! */
                float3 W = WD + _WorldSpaceCameraPos;
                //float distanceFromCellCentre = 0.0f;
                NearbyCells nearbyCells = getNearbyCells(W);
                float2 coords = nearbyCells.coords;
                //float2 coords = fromPosition(W, distanceFromCellCentre);
                //distanceFromCellCentre = distanceFromCellCentre;
                //float highlight = coords.x % 2 * 0.1f + coords.y % 2 * 0.2f;

                /* Demo: multiply the pixel by frac(W.x), giving x-aligned bands of shadow */
                float4 c = tex2D(_MainTex, i.uv);
                float distanceFromCentre = sqrt((250 - W.x) * (250 - W.x) + (226 - W.z) * (226 - W.z));        
                c.rgb *= 1.0f - (0.5*smoothstep(218, 248, distanceFromCentre) + 0.5 * smoothstep(208, 218, distanceFromCentre));//clamp(0.0f, 1.0f, (1.0f - lerp(0.0f, 1.0f, (distanceFromCentre - 208) / 30)));// smoothstep(218, 278, distanceFromCentre));// *(1.0f - distanceFromCellCentre);;// * nearbyCells.fogAmount;// *(1.0f - distanceFromCellCentre);// * (0.7f + highlight);
                //c.rgb = c.rgb * nearbyCells.highlight.a + nearbyCells.highlight.rgb * (1.0f - nearbyCells.highlight.a);
                return c;
            }
            ENDCG
        }
    }
}
