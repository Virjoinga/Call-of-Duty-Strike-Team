ÇMShader "Advanced/2_Layer_Blinking_2nd_UV_No_Lightmap" {
Properties {
 _TintColor ("Tint Color", Color) = (1,1,1,1)
 _texBase ("MainTex", 2D) = "" {}
 _tex2 ("Texture2", 2D) = "" {}
 _FadeOutDistNear ("Near fadeout dist", Float) = 0
 _FadeOutDistFar ("Far fadeout dist", Float) = 1000
 _Multiplier ("Color multiplier", Float) = 1
 _Bias ("Bias", Float) = 0.1
 _TimeOnDuration ("ON duration", Float) = 0.5
 _TimeOffDuration ("OFF duration", Float) = 0.5
 _BlinkingTimeOffsScale ("Blinking time offset scale (seconds)", Float) = 5
 _SizeGrowStartDist ("Size grow start dist", Float) = 5
 _SizeGrowEndDist ("Size grow end dist", Float) = 50
 _MaxGrowSize ("Max grow size", Float) = 2.5
 _NoiseAmount ("Noise amount (when zero, pulse wave is used)", Range(0,0.5)) = 0
 _Color ("Color", Color) = (1,1,1,1)
 _Row1_BlinkSpeed ("1st row blinking speed", Vector) = (1,1,1,1)
 _Row2_BlinkSpeed ("2nd row blinking speed", Vector) = (1,1,1,1)
}
SubShader { 
 Pass {
Program "vp" {
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _Row2_BlinkSpeed;
uniform highp vec4 _Row1_BlinkSpeed;
uniform highp vec4 _Color;
uniform highp float _NoiseAmount;
uniform highp float _BlinkingTimeOffsScale;
uniform highp float _TimeOffDuration;
uniform highp float _TimeOnDuration;
uniform highp float _Bias;
uniform highp float _Multiplier;
uniform highp float _FadeOutDistFar;
uniform highp float _FadeOutDistNear;
uniform highp vec4 _tex2_ST;
uniform highp vec4 _texBase_ST;


uniform highp vec4 _Time;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  lowp vec4 tmpvar_3;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_2 = ((_glesMultiTexCoord0.xy * _texBase_ST.xy) + _texBase_ST.zw);
  highp vec2 tmpvar_4;
  tmpvar_4 = ((_glesMultiTexCoord1.xy * _tex2_ST.xy) + _tex2_ST.zw);
  int index_5;
  index_5 = ((int(min (3.0, float(int((tmpvar_4.x / 0.5))))) * 4) + int(min (3.0, float(int((tmpvar_4.y / 0.25))))));
  highp float tmpvar_6;
  if ((index_5 == 0)) {
    tmpvar_6 = _Row1_BlinkSpeed.x;
  } else {
    if ((index_5 == 1)) {
      tmpvar_6 = _Row1_BlinkSpeed.y;
    } else {
      if ((index_5 == 2)) {
        tmpvar_6 = _Row1_BlinkSpeed.z;
      } else {
        if ((index_5 == 3)) {
          tmpvar_6 = _Row1_BlinkSpeed.w;
        } else {
          if ((index_5 == 4)) {
            tmpvar_6 = _Row2_BlinkSpeed.x;
          } else {
            if ((index_5 == 5)) {
              tmpvar_6 = _Row2_BlinkSpeed.y;
            } else {
              if ((index_5 == 6)) {
                tmpvar_6 = _Row2_BlinkSpeed.z;
              } else {
                tmpvar_6 = _Row2_BlinkSpeed.w;
              };
            };
          };
        };
      };
    };
  };
  highp float tmpvar_7;
  tmpvar_7 = ((_Time.y + (_BlinkingTimeOffsScale * _glesColor.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (gl_ModelViewMatrix * _glesVertex).xyz;
  highp float tmpvar_9;
  tmpvar_9 = sqrt(dot (tmpvar_8, tmpvar_8));
  highp float tmpvar_10;
  tmpvar_10 = clamp ((tmpvar_9 / _FadeOutDistNear), 0.0, 1.0);
  highp float tmpvar_11;
  tmpvar_11 = (1.0 - clamp ((max ((tmpvar_9 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
  highp float y_12;
  y_12 = (_TimeOnDuration + _TimeOffDuration);
  highp float tmpvar_13;
  tmpvar_13 = (tmpvar_7 / y_12);
  highp float tmpvar_14;
  tmpvar_14 = (fract(abs(tmpvar_13)) * y_12);
  highp float tmpvar_15;
  if ((tmpvar_13 >= 0.0)) {
    tmpvar_15 = tmpvar_14;
  } else {
    tmpvar_15 = -(tmpvar_14);
  };
  highp float t_16;
  t_16 = max (min ((tmpvar_15 / (_TimeOnDuration * 0.25)), 1.0), 0.0);
  highp float edge0_17;
  edge0_17 = (_TimeOnDuration * 0.75);
  highp float t_18;
  t_18 = max (min (((tmpvar_15 - edge0_17) / (_TimeOnDuration - edge0_17)), 1.0), 0.0);
  highp float tmpvar_19;
  tmpvar_19 = ((t_16 * (t_16 * (3.0 - (2.0 * t_16)))) * (1.0 - (t_18 * (t_18 * (3.0 - (2.0 * t_18))))));
  highp float tmpvar_20;
  tmpvar_20 = (tmpvar_7 * (6.28319 / _TimeOnDuration));
  highp float tmpvar_21;
  tmpvar_21 = ((_NoiseAmount * (sin(tmpvar_20) * ((0.5 * cos(((tmpvar_20 * 0.6366) + 56.7272))) + 0.5))) + (1.0 - _NoiseAmount));
  highp float tmpvar_22;
  if ((_NoiseAmount < 0.01)) {
    tmpvar_22 = tmpvar_19;
  } else {
    tmpvar_22 = tmpvar_21;
  };
  highp float tmpvar_23;
  tmpvar_23 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_24;
  tmpvar_24 = (((((tmpvar_23 * tmpvar_23) * (tmpvar_11 * tmpvar_11)) * _Color) * _Multiplier) * (tmpvar_22 + _Bias));
  tmpvar_3 = tmpvar_24;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _tex2;
uniform sampler2D _texBase;
void main ()
{
  gl_FragData[0] = (texture2D (_texBase, xlv_TEXCOORD0) + (texture2D (_tex2, xlv_TEXCOORD1) * xlv_TEXCOORD2));
}



#endif"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _Row2_BlinkSpeed;
uniform highp vec4 _Row1_BlinkSpeed;
uniform highp vec4 _Color;
uniform highp float _NoiseAmount;
uniform highp float _BlinkingTimeOffsScale;
uniform highp float _TimeOffDuration;
uniform highp float _TimeOnDuration;
uniform highp float _Bias;
uniform highp float _Multiplier;
uniform highp float _FadeOutDistFar;
uniform highp float _FadeOutDistNear;
uniform highp vec4 _tex2_ST;
uniform highp vec4 _texBase_ST;


uniform highp vec4 _Time;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  lowp vec4 tmpvar_3;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * _glesVertex);
  tmpvar_2 = ((_glesMultiTexCoord0.xy * _texBase_ST.xy) + _texBase_ST.zw);
  highp vec2 tmpvar_4;
  tmpvar_4 = ((_glesMultiTexCoord1.xy * _tex2_ST.xy) + _tex2_ST.zw);
  int index_5;
  index_5 = ((int(min (3.0, float(int((tmpvar_4.x / 0.5))))) * 4) + int(min (3.0, float(int((tmpvar_4.y / 0.25))))));
  highp float tmpvar_6;
  if ((index_5 == 0)) {
    tmpvar_6 = _Row1_BlinkSpeed.x;
  } else {
    if ((index_5 == 1)) {
      tmpvar_6 = _Row1_BlinkSpeed.y;
    } else {
      if ((index_5 == 2)) {
        tmpvar_6 = _Row1_BlinkSpeed.z;
      } else {
        if ((index_5 == 3)) {
          tmpvar_6 = _Row1_BlinkSpeed.w;
        } else {
          if ((index_5 == 4)) {
            tmpvar_6 = _Row2_BlinkSpeed.x;
          } else {
            if ((index_5 == 5)) {
              tmpvar_6 = _Row2_BlinkSpeed.y;
            } else {
              if ((index_5 == 6)) {
                tmpvar_6 = _Row2_BlinkSpeed.z;
              } else {
                tmpvar_6 = _Row2_BlinkSpeed.w;
              };
            };
          };
        };
      };
    };
  };
  highp float tmpvar_7;
  tmpvar_7 = ((_Time.y + (_BlinkingTimeOffsScale * _glesColor.z)) * tmpvar_6);
  highp vec3 tmpvar_8;
  tmpvar_8 = (gl_ModelViewMatrix * _glesVertex).xyz;
  highp float tmpvar_9;
  tmpvar_9 = sqrt(dot (tmpvar_8, tmpvar_8));
  highp float tmpvar_10;
  tmpvar_10 = clamp ((tmpvar_9 / _FadeOutDistNear), 0.0, 1.0);
  highp float tmpvar_11;
  tmpvar_11 = (1.0 - clamp ((max ((tmpvar_9 - _FadeOutDistFar), 0.0) * 0.2), 0.0, 1.0));
  highp float y_12;
  y_12 = (_TimeOnDuration + _TimeOffDuration);
  highp float tmpvar_13;
  tmpvar_13 = (tmpvar_7 / y_12);
  highp float tmpvar_14;
  tmpvar_14 = (fract(abs(tmpvar_13)) * y_12);
  highp float tmpvar_15;
  if ((tmpvar_13 >= 0.0)) {
    tmpvar_15 = tmpvar_14;
  } else {
    tmpvar_15 = -(tmpvar_14);
  };
  highp float t_16;
  t_16 = max (min ((tmpvar_15 / (_TimeOnDuration * 0.25)), 1.0), 0.0);
  highp float edge0_17;
  edge0_17 = (_TimeOnDuration * 0.75);
  highp float t_18;
  t_18 = max (min (((tmpvar_15 - edge0_17) / (_TimeOnDuration - edge0_17)), 1.0), 0.0);
  highp float tmpvar_19;
  tmpvar_19 = ((t_16 * (t_16 * (3.0 - (2.0 * t_16)))) * (1.0 - (t_18 * (t_18 * (3.0 - (2.0 * t_18))))));
  highp float tmpvar_20;
  tmpvar_20 = (tmpvar_7 * (6.28319 / _TimeOnDuration));
  highp float tmpvar_21;
  tmpvar_21 = ((_NoiseAmount * (sin(tmpvar_20) * ((0.5 * cos(((tmpvar_20 * 0.6366) + 56.7272))) + 0.5))) + (1.0 - _NoiseAmount));
  highp float tmpvar_22;
  if ((_NoiseAmount < 0.01)) {
    tmpvar_22 = tmpvar_19;
  } else {
    tmpvar_22 = tmpvar_21;
  };
  highp float tmpvar_23;
  tmpvar_23 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_24;
  tmpvar_24 = (((((tmpvar_23 * tmpvar_23) * (tmpvar_11 * tmpvar_11)) * _Color) * _Multiplier) * (tmpvar_22 + _Bias));
  tmpvar_3 = tmpvar_24;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _tex2;
uniform sampler2D _texBase;
void main ()
{
  gl_FragData[0] = (texture2D (_texBase, xlv_TEXCOORD0) + (texture2D (_tex2, xlv_TEXCOORD1) * xlv_TEXCOORD2));
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" }
"!!GLES"
}
}
 }
}
}