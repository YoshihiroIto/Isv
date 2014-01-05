#version 300 es
#extension GL_OES_EGL_image_external : require

in highp vec2 texcoordA;
in highp vec2 texcoordB;
in highp vec2 texcoordC;

out highp vec4 outFragColor;

uniform highp samplerExternalOES texA;
uniform highp samplerExternalOES texB;
uniform highp samplerExternalOES texC;

uniform highp vec3 blendRatio;

void main()            
{
	outFragColor.rgb =
		(texture(texA, texcoordA).rgb * blendRatio.x) +
		(texture(texB, texcoordB).rgb * blendRatio.y) +
		(texture(texC, texcoordC).rgb * blendRatio.z);

	outFragColor.a = 1.0f;
}
