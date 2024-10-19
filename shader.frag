#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform float u_time;
uniform vec2 u_resolution;
uniform sampler2D texture0;
uniform sampler2D texture1;

void main()
{
    //vec2 uv = gl_FragCoord.xy / u_resolution;
    //float blue = sin(u_time) * 0.5 + 0.5;
    //vec4 color = vec4(uv, blue, 1.0); 

    //FragColor = mix(mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.5), color, 0.5);

    FragColor = texture(texture0, texCoord);
}