#version 300 es
#extension GL_OES_EGL_image_external : require

in lowp vec2 texcoord;

out lowp vec4 outFragColor;

uniform lowp samplerExternalOES tex;

void main()            
{
   outFragColor = texture(tex, texcoord);
}     
