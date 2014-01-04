#version 300 es

in vec4 inPosition;
in vec2 inTexcoord;

out vec2 texcoord;

void main()
{         
	gl_Position = inPosition;

	texcoord = inTexcoord;
}

