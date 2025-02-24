#version 330

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;

// Input uniform values
uniform sampler2D texture0;

// Output fragment color
out vec4 finalColor;

// Hardcoded parameters
const vec2 size = vec2(1280.0, 720.0); // Framebuffer size
const float samples = 5.0;            // Pixels per axis; higher = bigger glow
const float quality = 2.5;            // Defines size factor: Lower = smaller glow, better quality
const float intensity = 1.2;          // Glow intensity multiplier
const float threshold = 0.6;          // Brightness threshold for glow extraction

void main() {
    vec4 color = texture(texture0, fragTexCoord);
    vec3 glowSum = vec3(0.0);
    vec2 texelSize = 1.0 / size;
    float sampleCount = 0.0;
    
    // Extract bright areas
    float brightness = dot(color.rgb, vec3(0.299, 0.587, 0.114));
    vec3 glowSource = brightness > threshold ? color.rgb : vec3(0.0);
    
    // Blur bright areas
    for (float x = -samples; x <= samples; x += quality) {
        for (float y = -samples; y <= samples; y += quality) {
            vec2 offset = vec2(x, y) * texelSize;
            vec4 sampleColor = texture(texture0, fragTexCoord + offset);
            float sampleBrightness = dot(sampleColor.rgb, vec3(0.299, 0.587, 0.114));
            
            if (sampleBrightness > threshold) {
                glowSum += sampleColor.rgb;
                sampleCount += 1.0;
            }
        }
    }
    
    vec3 blurredGlow = glowSum / max(sampleCount, 1.0) * intensity;
    finalColor = vec4(color.rgb + blurredGlow, color.a);
}
