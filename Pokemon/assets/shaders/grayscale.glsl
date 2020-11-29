#type header
#version 330 core

#type vertex

layout(location = 0) in vec3 pos;
layout(location = 1) in vec2 texCoord;

out vec2 texCoord0;

uniform mat4 MVP;

void main() {
	gl_Position = MVP * vec4(pos, 1.0);
	texCoord0 = texCoord;
}

#type pixel

in vec2 texCoord0;

layout(location = 0) out vec4 color;

uniform sampler2D diffuse;

void main() {
	vec4 sampledColor = texture(diffuse, texCoord0);
	color = vec4(max(sampledColor.r, max(sampledColor.g, sampledColor.b)));
}
