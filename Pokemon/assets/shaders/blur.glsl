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

#type fragment

layout(location = 0) out vec4 color;

in vec2 texCoord0;

uniform sampler2D diffuse;

const vec2 blurScale = vec2(1.0 / 160.0, 1.0 / 144.0);

void main() {
	color = vec4(0.0);
	color += texture(diffuse, texCoord0 + (vec2(-3.0) * blurScale.xy)) * (1.0/64.0);
	color += texture(diffuse, texCoord0 + (vec2(-2.0) * blurScale.xy)) * (6.0/64.0);
	color += texture(diffuse, texCoord0 + (vec2(-1.0) * blurScale.xy)) * (15.0/64.0);
	color += texture(diffuse, texCoord0 + (vec2(0.0) * blurScale.xy))  * (20.0/64.0);
	color += texture(diffuse, texCoord0 + (vec2(1.0) * blurScale.xy))  * (15.0/64.0);
	color += texture(diffuse, texCoord0 + (vec2(2.0) * blurScale.xy))  * (6.0/64.0);
	color += texture(diffuse, texCoord0 + (vec2(3.0) * blurScale.xy))  * (1.0/64.0);
}