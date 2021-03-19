// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ViseR/UI/Gradient"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		[KeywordEnum(Linear,Radial)] _GradientType("Gradient Type", Float) = 0
		_ColorA("Color A", Color) = (1,1,1,1)
		_ColorB("Color B", Color) = (0,0,0,1)
		_Angle("Angle", Range( 0 , 1)) = 0.6817247
		_Contrast("Contrast", Float) = 1.31
		_RadialSize("Radial Size", Float) = 0.23
		_RadialOffset("Radial Offset", Range( -1 , 1)) = 0.6588235
		[Vector2Drawer]_UIPosition("UIPosition", Vector) = (0,0,0,0)
		[Vector2Drawer]_UISize("UISize", Vector) = (2,2,0,0)

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#define ASE_NEEDS_FRAG_COLOR
			#pragma multi_compile_local _GRADIENTTYPE_LINEAR _GRADIENTTYPE_RADIAL

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord2 : TEXCOORD2;
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float4 _ColorA;
			uniform float4 _ColorB;
			uniform float _Angle;
			uniform float2 _UIPosition;
			uniform float2 _UISize;
			uniform float _RadialOffset;
			uniform float _RadialSize;
			uniform float _Contrast;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				OUT.ase_texcoord2 = IN.vertex;
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				float GradientAngle81 = ( _Angle * 6.28318548202515 );
				float temp_output_69_0 = frac( ( GradientAngle81 * 4.0 ) );
				float angleMask107 = ( min( ( 1.0 - temp_output_69_0 ) , temp_output_69_0 ) * 2.0 );
				float4 appendResult118 = (float4(( ( IN.ase_texcoord2.xyz - float3( _UIPosition ,  0.0 ) ) / float3( _UISize ,  0.0 ) ) , 1.0));
				float2 UVs90 = ( (appendResult118).xy + 0.5 );
				float cos26 = cos( GradientAngle81 );
				float sin26 = sin( GradientAngle81 );
				float2 rotator26 = mul( ( ( ( 1.0 - angleMask107 ) * UVs90 ) + ( angleMask107 * ( ( 0.7071 * UVs90 ) + 0.1767771 ) ) ) - float2( 0.5,0.5 ) , float2x2( cos26 , -sin26 , sin26 , cos26 )) + float2( 0.5,0.5 );
				float2 appendResult140 = (float2(( ( _RadialOffset + 1.0 ) * 0.5 ) , 0.5));
				float cos137 = cos( GradientAngle81 );
				float sin137 = sin( GradientAngle81 );
				float2 rotator137 = mul( UVs90 - appendResult140 , float2x2( cos137 , -sin137 , sin137 , cos137 )) + appendResult140;
				float2 temp_cast_2 = (1.0).xx;
				#if defined(_GRADIENTTYPE_LINEAR)
				float staticSwitch37 = rotator26.y;
				#elif defined(_GRADIENTTYPE_RADIAL)
				float staticSwitch37 = ( distance( float2( 0,0 ) , ( ( rotator137 * 2.0 ) - temp_cast_2 ) ) - _RadialSize );
				#else
				float staticSwitch37 = rotator26.y;
				#endif
				float temp_output_96_0 = saturate( staticSwitch37 );
				float temp_output_98_0 = pow( temp_output_96_0 , _Contrast );
				float4 lerpResult31 = lerp( _ColorA , _ColorB , ( temp_output_98_0 / ( temp_output_98_0 + pow( ( 1.0 - temp_output_96_0 ) , _Contrast ) ) ));
				
				half4 color = ( tex2D( _MainTex, IN.texcoord.xy ) * IN.color * lerpResult31 * _Color );
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18707
-1920;390.8;1920;1133;1765.189;546.4847;1;True;True
Node;AmplifyShaderEditor.TauNode;134;-1606.967,-261.9006;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1772.681,-339.1403;Float;False;Property;_Angle;Angle;3;0;Create;True;0;0;False;0;False;0.6817247;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-1482.966,-284.9007;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;120;-2406.822,-605.6556;Float;False;Property;_UIPosition;UIPosition;7;0;Create;True;0;0;False;1;Vector2Drawer;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;81;-1350.991,-289.0252;Float;False;GradientAngle;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;117;-2405.821,-753.6554;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;82;-2250.494,-50.11325;Inherit;False;81;GradientAngle;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;123;-2167.822,-754.6554;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-2181.892,22.87202;Float;False;Constant;_Float3;Float 3;5;0;Create;True;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;121;-2162.655,-654.1902;Float;False;Property;_UISize;UISize;8;0;Create;True;0;0;False;1;Vector2Drawer;False;2,2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-2020.691,7.271911;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-1988.004,-639.6558;Float;False;Constant;_Float8;Float 8;7;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;124;-1976.004,-753.6555;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FractNode;69;-1887.092,11.17183;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;118;-1826.005,-752.6555;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;126;-1686.616,-757.4385;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;128;-1638.289,-678.6248;Float;False;Constant;_Float6;Float 6;7;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;71;-1757.091,-22.82825;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;127;-1469.289,-751.6248;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMinOpNode;74;-1602.091,-10.82816;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;141;-3293.412,533.9251;Float;False;Property;_RadialOffset;Radial Offset;6;0;Create;True;0;0;False;0;False;0.6588235;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;143;-3019.473,538.1234;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-1473.092,-11.82816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-1349.683,-757.0486;Float;False;UVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;107;-1337.689,-17.49088;Float;False;angleMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-2920.41,633.9251;Float;False;Constant;_Float9;Float 9;8;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-3163.156,241.9394;Inherit;False;90;UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-2899.473,538.1234;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-3157.608,167.7961;Float;False;Constant;_Float2;Float 2;5;0;Create;True;0;0;False;0;False;0.7071;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-3010.897,323.1813;Float;False;Constant;_Float4;Float 4;5;0;Create;True;0;0;False;0;False;0.1767771;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;140;-2710.41,614.9251;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-2749.044,542.1164;Inherit;False;90;UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-2977.444,224.7705;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-2792.264,713.319;Inherit;False;81;GradientAngle;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;-2933.506,119.159;Inherit;False;107;angleMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-2654.156,196.5326;Inherit;False;90;UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-2511.633,714.6025;Float;False;Constant;_Float0;Float 0;0;0;Create;True;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;-2808.897,305.1813;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;137;-2553.264,592.3192;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;76;-2639.336,123.8618;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-2652.956,282.3823;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-2340.323,592.0853;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-2471.257,179.6823;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-2345.323,685.0855;Float;False;Constant;_Float1;Float 1;0;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;23;-2191.323,592.0853;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;29;-2375.942,354.8171;Float;False;Constant;_Vector0;Vector 0;0;0;Create;True;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;78;-2319.656,256.9823;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-2414.311,481.5364;Inherit;False;81;GradientAngle;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-2051.635,663.2591;Float;False;Property;_RadialSize;Radial Size;5;0;Create;True;0;0;False;0;False;0.23;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;26;-2181.135,335.7992;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode;25;-2049.094,568.9876;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;30;-1988.816,335.7992;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleSubtractOpNode;146;-1882.417,569.6459;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;37;-1694.887,354.3599;Float;True;Property;_GradientType;Gradient Type;0;0;Create;True;0;0;False;0;False;1;0;0;True;;KeywordEnum;2;Linear;Radial;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;96;-1300.446,358.805;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-1332.446,500.8051;Float;False;Property;_Contrast;Contrast;4;0;Create;True;0;0;False;0;False;1.31;3.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;101;-1135.446,455.805;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;98;-1127.446,358.805;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;102;-979.4456,483.8051;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;-833.4456,421.805;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;99;-712.4457,360.805;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;33;-790.2389,18.8181;Float;False;Property;_ColorA;Color A;1;0;Create;True;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;34;-789.2389,190.8181;Float;False;Property;_ColorB;Color B;2;0;Create;True;0;0;False;0;False;0,0,0,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;154;-590.8345,-181.853;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;158;-607.1887,-103.4847;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;2;-283.2955,148.0738;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;31;-341.7851,317.1472;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;152;-251.9458,544.7813;Inherit;False;0;0;_Color;Shader;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-335.8515,-91.89822;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;97.50519,60.15918;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;153;263.554,-38.17689;Float;False;True;-1;2;ASEMaterialInspector;0;6;ViseR/UI/Gradient;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;True;2;False;-1;True;True;True;True;True;0;True;-9;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;135;0;27;0
WireConnection;135;1;134;0
WireConnection;81;0;135;0
WireConnection;123;0;117;0
WireConnection;123;1;120;0
WireConnection;67;0;82;0
WireConnection;67;1;68;0
WireConnection;124;0;123;0
WireConnection;124;1;121;0
WireConnection;69;0;67;0
WireConnection;118;0;124;0
WireConnection;118;3;119;0
WireConnection;126;0;118;0
WireConnection;71;0;69;0
WireConnection;127;0;126;0
WireConnection;127;1;128;0
WireConnection;74;0;71;0
WireConnection;74;1;69;0
WireConnection;143;0;141;0
WireConnection;73;0;74;0
WireConnection;90;0;127;0
WireConnection;107;0;73;0
WireConnection;145;0;143;0
WireConnection;140;0;145;0
WireConnection;140;1;142;0
WireConnection;60;0;61;0
WireConnection;60;1;91;0
WireConnection;94;0;60;0
WireConnection;94;1;95;0
WireConnection;137;0;92;0
WireConnection;137;1;140;0
WireConnection;137;2;136;0
WireConnection;76;0;108;0
WireConnection;75;0;108;0
WireConnection;75;1;94;0
WireConnection;21;0;137;0
WireConnection;21;1;22;0
WireConnection;77;0;76;0
WireConnection;77;1;93;0
WireConnection;23;0;21;0
WireConnection;23;1;24;0
WireConnection;78;0;77;0
WireConnection;78;1;75;0
WireConnection;26;0;78;0
WireConnection;26;1;29;0
WireConnection;26;2;83;0
WireConnection;25;1;23;0
WireConnection;30;0;26;0
WireConnection;146;0;25;0
WireConnection;146;1;133;0
WireConnection;37;1;30;1
WireConnection;37;0;146;0
WireConnection;96;0;37;0
WireConnection;101;0;96;0
WireConnection;98;0;96;0
WireConnection;98;1;97;0
WireConnection;102;0;101;0
WireConnection;102;1;97;0
WireConnection;100;0;98;0
WireConnection;100;1;102;0
WireConnection;99;0;98;0
WireConnection;99;1;100;0
WireConnection;31;0;33;0
WireConnection;31;1;34;0
WireConnection;31;2;99;0
WireConnection;5;0;154;0
WireConnection;5;1;158;0
WireConnection;4;0;5;0
WireConnection;4;1;2;0
WireConnection;4;2;31;0
WireConnection;4;3;152;0
WireConnection;153;0;4;0
ASEEND*/
//CHKSM=F8D437F43D721890CAC86E0C835864FC7F817584