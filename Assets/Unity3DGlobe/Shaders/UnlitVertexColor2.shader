Shader "Custom/UnlitVertexColor2"
{
 Properties
 {
  _Transparency("Transparency", Range(0.0,0.5)) = 0.25
 }
 SubShader
 {
  Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
  LOD 100
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha

  Pass
  {
   Cull Off
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag alpha
  // make fog work
  #pragma multi_compile_fog

  #include "UnityCG.cginc"

  struct appdata
  {
   float4 vertex : POSITION;
   float4 color:COLOR;
  };

  struct v2f
  {
   UNITY_FOG_COORDS(1)
   float4 vertex : SV_POSITION;
   float4 color:COLOR;
  };

  float _Transparency;

  v2f vert(appdata v)
  {
   v2f o;
   o.vertex = UnityObjectToClipPos(v.vertex);
   o.color = v.color;
   //o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
   UNITY_TRANSFER_FOG(o,o.vertex);
   return o;
  }

  fixed4 frag(v2f i) : SV_Target
  {
   // sample the texture
   // apply fog
   UNITY_APPLY_FOG(i.fogCoord, i.color);
      i.color.a = _Transparency;
   return i.color;
     }
     ENDCG
    }
 }
}
