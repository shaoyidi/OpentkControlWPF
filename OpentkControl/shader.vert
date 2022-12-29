﻿#version 450 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 fragPos;

void main()
{
	gl_Position = vec4(aPosition,1.0)*model*view*projection;
	fragPos = vec3(vec4(aPosition,1.0)*model);
	//fragPos = vec3(vec4(aPosition,1.0)*model*view);
}

