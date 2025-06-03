#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;

uniform mat4 view;

out vec4 frag_Color;

void main()
{
    gl_Position = view * vec4(aPosition, 1.0);
    frag_Color = aColor;
}
