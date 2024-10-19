#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform float u_time;
uniform vec2 u_resolution;
uniform sampler2D texture0;

void main()
{
    vec2 rg = gl_FragCoord.xy / u_resolution;
    float b = 0.5 * sin(u_time) + 0.5;
    vec4 color = vec4(rg, b, 1.0);
    vec4 texColor = texture(texture0, texCoord);

    FragColor = mix(color, texColor, 0.5);
}