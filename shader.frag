#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform float u_time;
uniform vec2 u_resolution;
uniform sampler2D texture0;
//uniform sampler2D texture1;

void main()
{
    //vec2 rg = gl_FragCoord.xy / u_resolution;
    //float b = 0.5 * sin(u_time) + 0.5;
    //vec4 color = vec4(rg, b, 1.0);
    //vec4 tex0Color = texture(texture0, texCoord);
    //vec4 tex1Color = texture(texture1, texCoord);
    //vec4 texColor = mix(tex0Color, tex1Color, 0.5);

    //FragColor = mix(color, texColor, 0.5);
    FragColor = texture(texture0, texCoord);
    //FragColor = color;
}