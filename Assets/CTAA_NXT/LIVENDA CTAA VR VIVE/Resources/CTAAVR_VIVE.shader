
// CTAA NXT VR Cinematic Temporal Anti-Aliasing Copyright 2017-2020 Livenda Labs Pty Ltd CTAAVR_VIVE

 //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDO NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI
 //IUDIUFUFIFA JIOI JMOAIEFJ MAIJAFPIO  A JFPEIOJF AMFAOEIFJAF ANFB AFUIWEFNA FHNAIEU NAFEIFLAL FN AFIHAIEIFBN
 //EYUIAY IHIFAU EN  NFAIUENFN NAFIEUHIU NOAIUFHWEKJNA N FANLEFUHALNF ADKFNFAIUH NFAIKHA NFANIAFNKJN NIAEKKEUR		

 //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDO NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI
 //IUDIUFUFIFA JIOI JMOAIEFJ MAIJAFPIO  A JFPEIOJF AMFAOEIFJAF ANFB AFUIWEFNA FHNAIEU NAFEIFLAL FN AFIHAIEIFBN
 //EYUIAY IHIFAU EN  NFAIUENFN NAFIEUHIU NOAIUFHWEKJNA N FANLEFUHALNF ADKFNFAIUH NFAIKHA NFANIAFNKJN NIAEKKEUR	

Shader "Hidden/CTAAVR_VIVE" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	
}

SubShader {
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }
	Pass {

CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma glsl
#pragma exclude_renderers d3d11_9x
#include "UnityCG.cginc"
                
float4 _MainTex_TexelSize;
uniform sampler2D _MainTex;
uniform sampler2D _Accum;
uniform sampler2D _Motion0;
uniform float4 _Jitter;//// xy = current frame, zw = previous

uniform sampler2D _CameraDepthTexture;
uniform float _motionDelta;
uniform float _AdaptiveResolve;

float4 _ControlParams;
float _RenderPath;

sampler2D _CameraDepthNormalsTexture;

uniform float4 _Sensitivity; 
uniform float _SampleDistance;

 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

  //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

  //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

float LmsT(float3 Color)
{
	return (Color.g * 2.0) + (Color.r + Color.b);
}

 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

inline float tsOYpf4(float3 Color, float Exposure) 
{
	return rcp(LmsT(Color) * Exposure + 4.0);
}

 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL
inline float tsOYpfInv4(float3 Color, float Exposure) 
{
	return 4.0 * rcp(LmsT(Color) * (-Exposure) + 1.0);
}
 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

float LmmaL2EE(float3 Color) 
{
	
		
		return dot(Color, float3(0.299, 0.587, 0.114));
	
		
	//	return dot(Color, float3(0.2126, 0.7152, 0.0722));
	
}

 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

inline float ICSSD(float3 rgdgrjy, float3 Ofgrreg, float3 Boxdregr)
{
	
	float3 bghttthdSSSSd = rcp(rgdgrjy);
	float3 bdgTrvythdffdh = (  Boxdregr  - Ofgrreg) * bghttthdSSSSd;
	float3 bfdjhytFFdfg = ((-Boxdregr) - Ofgrreg) * bghttthdSSSSd;
	return max(max(min(bdgTrvythdffdh.x, bfdjhytFFdfg.x), min(bdgTrvythdffdh.y, bfdjhytFFdfg.y)), min(bdgTrvythdffdh.z, bfdjhytFFdfg.z));
}

 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL
inline float HsCmm(float3 yuGGFrgrFGR, float3 fileiuaeirro, float3 nNminnD4F, float3 nNmxx4Zx)
{
	float3 Mnn = min(fileiuaeirro, min(nNminnD4F, nNmxx4Zx));
	float3 Max = max(fileiuaeirro, max(nNminnD4F, nNmxx4Zx));	
	float3 Avg2 = Max + Mnn;
	float3 ergerrggd = fileiuaeirro - yuGGFrgrFGR;
	float3 ergeghthtegrrggd = yuGGFrgrFGR - Avg2 * 0.5;
	float3 SvfDg = Max - Avg2 * 0.5;
	return saturate(ICSSD(ergerrggd, ergeghthtegrrggd, SvfDg));	
}

float tsOYpf(float3 Color, float Exposure) 
{
	return rcp(max(LmmaL2EE(Color) * Exposure, 1.0));
}

 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = v.texcoord.xy;
	return o;
}

 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL
float4 frag (v2f i) : COLOR
{

 //---------------------------------------------------------------------------------------
 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL
 //---------------------------------------------------------------------------------------
 float2 sdlfkgjkldfjg;

 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL
		
 float  InExxSs49SS = 0.1;
 
 float2  kkjfeiiuueue = _MainTex_TexelSize.xy*0.55;
 
 float CSZf = 0.5;
 float2 xOffsets[4];
 
 xOffsets[0] = float2( -kkjfeiiuueue.x, -kkjfeiiuueue.y )*CSZf;
 xOffsets[1] = float2(  kkjfeiiuueue.x, -kkjfeiiuueue.y )*CSZf;
 xOffsets[2] = float2( -kkjfeiiuueue.x,  kkjfeiiuueue.y )*CSZf;
 xOffsets[3] = float2(  kkjfeiiuueue.x,  kkjfeiiuueue.y )*CSZf;


 float4 ifeiejeiiua = tex2D(_Motion0, i.uv);

 sdlfkgjkldfjg = ( ifeiejeiiua.xy );
 
 float Hidkwiue8d = 2;
 float HydBlrrDfs = saturate(abs(sdlfkgjkldfjg.x) * Hidkwiue8d + abs(sdlfkgjkldfjg.y) * Hidkwiue8d);
 	
	float2  uv = i.uv;

						
	float4 nduyeuiaa = tex2D(_MainTex, uv.xy - kkjfeiiuueue );
	float4 ooeijdhfak = tex2D(_MainTex, uv.xy + float2(  0, -kkjfeiiuueue.y ) );
	float4 reeuaer = tex2D(_MainTex, uv.xy + float2(  kkjfeiiuueue.x, -kkjfeiiuueue.y ) );
	float4 utyeuyded = tex2D(_MainTex, uv.xy + float2(  -kkjfeiiuueue.x, 0 ) );
	float4 bbvmmdjye = tex2D(_MainTex, uv.xy);
	float4 neu7dkkks = tex2D(_MainTex, uv.xy + float2(   kkjfeiiuueue.x, 0 ) );
	float4 uiiiiaaae = tex2D(_MainTex, uv.xy + float2( -kkjfeiiuueue.x,  kkjfeiiuueue.y ) );
	float4 cnmcnjde = tex2D(_MainTex, uv.xy + float2(  0,  kkjfeiiuueue.y ) );
	float4 yusqqwer = tex2D(_MainTex, uv.xy + kkjfeiiuueue );

	 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL
 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

       // half oieoifei = (LmmaL2EE(tex2D(_MainTex, uv.xy +  float2(  0, -kkjfeiiuueue.y  ) )) );
       // half oieoidee = (LmmaL2EE( tex2D(_MainTex, uv.xy + float2(  0,  kkjfeiiuueue.y  ) )) );
       // half uyeuydjjd = (LmmaL2EE(tex2D(_MainTex, uv.xy  + float2(  kkjfeiiuueue.x , 0  ) )) );
       // half oooiiiue = (LmmaL2EE(tex2D(_MainTex, uv.xy  + float2( -kkjfeiiuueue.x , 0  ) )) );
         //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

       // half IUeuyyydY = (LmmaL2EE(tex2D(_MainTex, uv.xy))) - (oieoifei + oieoidee + uyeuydjjd + oooiiiue)*0.25;

        half grdiccx       = 0.1;// << -- HARDCODE IN FOR OPENVR   --saturate(abs(IUeuyyydY))--;      

        //grdiccx = 0.1;//saturate(pow(grdiccx, 2)*15) ;

        InExxSs49SS = 1;	
		
		nduyeuiaa.rgb *= tsOYpf4(nduyeuiaa.rgb, InExxSs49SS);
		ooeijdhfak.rgb *= tsOYpf4(ooeijdhfak.rgb, InExxSs49SS);
		reeuaer.rgb *= tsOYpf4(reeuaer.rgb, InExxSs49SS);
		utyeuyded.rgb *= tsOYpf4(utyeuyded.rgb, InExxSs49SS);
		bbvmmdjye.rgb *= tsOYpf4(bbvmmdjye.rgb, InExxSs49SS);
		neu7dkkks.rgb *= tsOYpf4(neu7dkkks.rgb, InExxSs49SS);
		uiiiiaaae.rgb *= tsOYpf4(uiiiiaaae.rgb, InExxSs49SS);
		cnmcnjde.rgb *= tsOYpf4(cnmcnjde.rgb, InExxSs49SS);
		yusqqwer.rgb *= tsOYpf4(yusqqwer.rgb, InExxSs49SS);		
			
			 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL

		float4 fileiuaeirro= 
			nduyeuiaa * 0.0625 + 
			ooeijdhfak * 0.125 +
			reeuaer * 0.0625 +
			utyeuyded * 0.125 +
			bbvmmdjye * 0.25 +
			neu7dkkks * 0.125 +
			uiiiiaaae * 0.0625 +
			cnmcnjde * 0.125 +
			yusqqwer * 0.0625;
	 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL		
			
		float4	 jduduywjkd = fileiuaeirro;		

		float4 ndfrgsDD = min(min(nduyeuiaa, reeuaer), min(uiiiiaaae, yusqqwer));		
		float4 nFFdgTRFF43eeT = max(max(nduyeuiaa, reeuaer), max(uiiiiaaae, yusqqwer));		
		float4 nNminnD4F = min(min(min(ooeijdhfak, utyeuyded), min(bbvmmdjye, neu7dkkks)), cnmcnjde);		
		float4 nNmxx4Zx = max(max(max(ooeijdhfak, utyeuyded), max(bbvmmdjye, neu7dkkks)), cnmcnjde);		
		ndfrgsDD = min(ndfrgsDD, nNminnD4F);
		nFFdgTRFF43eeT = max(nFFdgTRFF43eeT, nNmxx4Zx);
	    nNminnD4F = nNminnD4F * 0.5 + ndfrgsDD * 0.5;
		nNmxx4Zx = nNmxx4Zx * 0.5 + nFFdgTRFF43eeT * 0.5;
		//---------------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------------
	    //---------------------------------------------------------------------------------------
	    //---------------------------------------------------------------------------------------
	    float4 ouhtgbggr = tex2D(_Accum, i.uv-sdlfkgjkldfjg);
		
	
	    ouhtgbggr.rgb *= tsOYpf4(ouhtgbggr.rgb, InExxSs49SS);
		
		
		float LmissXg = LmsT(nNminnD4F.rgb);
		float LgmXXdfgsDDDf = LmsT(nNmxx4Zx.rgb);
		float UfDDFddfs = LmsT(ouhtgbggr.rgb);
		float UfjfKKLCoBB = LgmXXdfgsDDDf - LmissXg;
		float clmdjuuubde = HsCmm(ouhtgbggr.rgb, jduduywjkd.rgb, nNminnD4F.rgb, nNmxx4Zx.rgb);

		//float MV2 = ifeiejeiiua.z;

		ouhtgbggr.rgb = lerp(ouhtgbggr.rgb, jduduywjkd.rgb, clmdjuuubde );

		float AdjhdFFFd = saturate(HydBlrrDfs) * 0.5;
		float AOjdhFFFC = 0.0;
		
		AdjhdFFFd = saturate(AdjhdFFFd + 1/(1.0 + UfjfKKLCoBB * AOjdhFFFC));
		fileiuaeirro.rgb = lerp(fileiuaeirro.rgb, bbvmmdjye.rgb, AdjhdFFFd);
		//#endif

		float celsotrsm = saturate(length(sdlfkgjkldfjg)*25);

		float _TdjeKfhjuewennsjdE= 1/lerp (grdiccx*4+_ControlParams.y,0.75,celsotrsm);

		_TdjeKfhjuewennsjdE *= 1/(grdiccx+0.653);

		 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 		 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 		 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL
		float hisueyufey = (_TdjeKfhjuewennsjdE + HydBlrrDfs * _TdjeKfhjuewennsjdE);
	
		float fjudueiakd = UfDDFddfs * hisueyufey * (1.0 + HydBlrrDfs * hisueyufey * 4.0);
		float blenfiuntwwq = saturate(fjudueiakd * rcp(UfDDFddfs + UfjfKKLCoBB));		
			
		float yifeindndue = lerp(blenfiuntwwq, (sqrt(blenfiuntwwq)), saturate(length(sdlfkgjkldfjg)) );			
	
		ouhtgbggr = lerp(ouhtgbggr, fileiuaeirro, yifeindndue);

		ouhtgbggr.rgb *= tsOYpfInv4(ouhtgbggr.rgb, InExxSs49SS);
			
		ouhtgbggr.rgb = -min(-ouhtgbggr.rgb, 0.0);

		 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 		 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 		 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL
	 	return  ouhtgbggr;
	 	 //EFWUAHFILHA N AFOEIFUAOE AFEJOIJ AF AFJEOJA EOFADLSFK FJAEOFIJA NFAOEHNA DLKJFN FAHIEUHL FFNAFNEI 
 		 //OAFIE FHAFOAIEWFH ANAFIEFUHAON ADFAJFAWEHFUINADFADSJFN DFNJKSDFNILAUEWHF NDSFHAULIHENF JNFALI NAEFIN
 		 //APFOIAF JAADSFLKJ HLHAFUIEH NAILFUEN ALJKFNMDS FAHDF AJFOAEWFJ NAIEUFHHL KAJN NALFB ALJFK DNFAIL
	
}
ENDCG
	}
}

Fallback off

}