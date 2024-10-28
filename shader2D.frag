#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform float u_time;
uniform vec2 u_resolution;
uniform sampler2D texture0;

void main()
{
    FragColor = texture(texture0, texCoord);
}