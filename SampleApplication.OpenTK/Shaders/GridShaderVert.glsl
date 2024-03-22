#version 400 core
layout (location = 0) in vec3 aPosition;

uniform mat4 persp;
uniform mat4 view;

out float dist;

void main()
{
    gl_Position = vec4(aPosition, 1.0)*view*persp;

    vec4 eye_pos = vec4(aPosition,1.0)*view;
    dist = abs(eye_pos.z/eye_pos.w);
}