#version 300 es
#extension GL_OES_EGL_image_external : require

in lowp vec2 texcoord;

out lowp vec4 outFragColor;

uniform lowp samplerExternalOES tex;

void main()            
{
   //outFragColor = vec4 (0.0, 0.0, 1.0, 1.0);

   outFragColor = texture(tex, texcoord);
}     
