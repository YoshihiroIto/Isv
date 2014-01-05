#version 300 es

in vec4 inPosition;
in vec2 inTexcoord;

out vec2 texcoordA;
out vec2 texcoordB;
out vec2 texcoordC;

uniform mat4 texTransformA;
uniform mat4 texTransformB;
uniform mat4 texTransformC;

void main()
{         
	gl_Position = inPosition;

	texcoordA = (texTransformA * vec4(inTexcoord.x, inTexcoord.y, 0.0f, 1.0f)).xy;
	texcoordB = (texTransformB * vec4(inTexcoord.x, inTexcoord.y, 0.0f, 1.0f)).xy;
	texcoordC = (texTransformC * vec4(inTexcoord.x, inTexcoord.y, 0.0f, 1.0f)).xy;
}
