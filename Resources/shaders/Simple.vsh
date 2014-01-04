#version 300 es

in vec4 inPosition;
in vec2 inTexcoord;

out vec2 texcoord;

uniform mat4 texTransform;

void main()
{         
	gl_Position = inPosition;

	texcoord = (texTransform * vec4(inTexcoord.x, inTexcoord.y, 0.0f, 1.0f)).xy;
}

