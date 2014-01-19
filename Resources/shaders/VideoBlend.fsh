#version 140
#extension GL_OES_EGL_image_external : require

varying vec2 texcoordA;
varying vec2 texcoordB;
varying vec2 texcoordC;

uniform samplerExternalOES texA;
uniform samplerExternalOES texB;
uniform samplerExternalOES texC;

uniform vec3 blendRatio;

uniform vec2 cameraInvSize;

const vec3 monochromeScale = vec3(0.298912, 0.586611, 0.114478);
const float thresold = 0.85;

void main()            
{
    vec4 outputColor;

    outputColor.rgb =
        (texture2D(texA, texcoordA).rgb * blendRatio.x) +
        (texture2D(texB, texcoordB).rgb * blendRatio.y) +
        (texture2D(texC, texcoordC).rgb * blendRatio.z);

    outputColor.a = 1.0;

    vec3 c00 = texture2D(texC, texcoordC + (vec2(-1.0, -1.0) * cameraInvSize)).rgb;
    vec3 c01 = texture2D(texC, texcoordC + (vec2( 0.0, -1.0) * cameraInvSize)).rgb;
    vec3 c02 = texture2D(texC, texcoordC + (vec2( 1.0, -1.0) * cameraInvSize)).rgb;
    vec3 c10 = texture2D(texC, texcoordC + (vec2(-1.0,  0.0) * cameraInvSize)).rgb;
    vec3 c11 = texture2D(texC, texcoordC + (vec2( 0.0,  0.0) * cameraInvSize)).rgb;
    vec3 c12 = texture2D(texC, texcoordC + (vec2( 1.0,  0.0) * cameraInvSize)).rgb;
    vec3 c20 = texture2D(texC, texcoordC + (vec2(-1.0,  1.0) * cameraInvSize)).rgb;
    vec3 c21 = texture2D(texC, texcoordC + (vec2( 0.0,  1.0) * cameraInvSize)).rgb;
    vec3 c22 = texture2D(texC, texcoordC + (vec2( 1.0,  1.0) * cameraInvSize)).rgb;

    vec3 horizonColor  = c00 * +1.0;
    horizonColor      += c01 * +0.0;
    horizonColor      += c02 * -1.0;
    horizonColor      += c10 * +2.0;
    horizonColor      += c11 * +0.0;
    horizonColor      += c12 * -2.0;
    horizonColor      += c20 * +1.0;
    horizonColor      += c21 * +0.0;
    horizonColor      += c22 * -1.0;

    vec3 verticalColor = c00 * +1.0;
    verticalColor     += c01 * +2.0;
    verticalColor     += c02 * +1.0;
    verticalColor     += c10 * +0.0;
    verticalColor     += c11 * +0.0;
    verticalColor     += c12 * +0.0;
    verticalColor     += c20 * -1.0;
    verticalColor     += c21 * -2.0;
    verticalColor     += c22 * -1.0;

    vec3 destColor = sqrt(horizonColor * horizonColor + verticalColor * verticalColor);
    float gray = dot(destColor, monochromeScale);
    gray -= 1.0 - thresold;
    gray /= thresold;
    outputColor.rgb += vec3(gray, gray, gray);

    //
    gl_FragColor = outputColor;
}
