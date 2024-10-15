#version 330 core
out vec4 FragColor;

in vec3 VertexColor;

uniform float u_time;
uniform vec2 u_resolution;

void main()
{
    //vec2 uv = gl_FragCoord.xy / u_resolution;
    //float blue = sin(u_time) * 0.5 + 0.5;
    //FragColor = vec4(uv, blue, 1.0); 

    FragColor = vec4(VertexColor, 1.0);
}