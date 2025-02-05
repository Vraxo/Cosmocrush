#version 330 core

// Input texture coordinates from the vertex shader.
in vec2 TexCoords;

// Output fragment color.
out vec4 FragColor;

// Uniforms
uniform sampler2D texture0;
uniform vec4 flash_color;
uniform float flash_value; // Expected range [0.0, 1.0]

void main() {
    // Sample the texture color using the provided texture coordinates.
    vec4 texture_color = texture(texture0, TexCoords);
    
    // Linearly interpolate between the texture color and flash_color.
    vec4 mixed_color = mix(texture_color, flash_color, flash_value);
    
    // Ensure the alpha remains the same as the texture's alpha.
    mixed_color.a = 1;
    
    // Output the final color.
    FragColor = mixed_color;
}