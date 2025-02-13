#version 330 core

in vec2 fragTexCoord;  // Fragment texture coordinates
out vec4 fragColor;    // Output fragment color

uniform sampler2D texture0; // Texture uniform
uniform vec4 flash_color;   // Flash color (rgba)
uniform float flash_value;  // Flash strength (0.0 to 1.0)

void main()
{
    // Sample the texture
    vec4 texColor = texture(texture0, fragTexCoord);
    
    // Mix the flash color with the texture color based on the flash value
    vec4 finalColor = mix(texColor, flash_color, flash_value);
    
    // Preserve the alpha channel of the texture
    fragColor = vec4(finalColor.rgb, texColor.a);
}