#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform float time;
uniform vec2 resolution;
uniform sampler2D texture0;
uniform float alpha;

void main()
{
    //vec2 rg = gl_FragCoord.xy / u_resolution;
    //float b = 0.5 * sin(u_time) + 0.5;
    //vec4 color = vec4(rg, b, 1.0);
    //vec4 texColor = texture(texture0, texCoord);
    
    //if (texColor.a == 1.0) FragColor = mix(color, texColor, 0.5);
    //else FragColor = texColor;

    vec4 color = texture(texture0, texCoord);

    if (color.a == 1.0) color.a = alpha;

    FragColor = color;
}