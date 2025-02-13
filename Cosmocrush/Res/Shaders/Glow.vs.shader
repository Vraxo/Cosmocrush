// Glow.shader - Fragment shader for sprite glow effect
#version 330

// Vertex Shader
in vec3 vertexPosition;
in vec2 vertexTexCoord;
in vec4 vertexColor;
in vec3 vertexNormal;

out vec2 fragTexCoord;
out vec4 fragColor;

uniform mat4 mvp;

void main()
{
    gl_Position = mvp * vec4(vertexPosition, 1.0);
    fragTexCoord = vertexTexCoord;
    fragColor = vertexColor;
}

// Fragment Shader
#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

out vec4 finalColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

void main()
{
    // Sample main texture
    vec4 texColor = texture(texture0, fragTexCoord);
    if (texColor.a == 0.0) discard;

    // Calculate texture dimensions
    vec2 texSize = textureSize(texture0, 0);
    vec2 texelSize = 1.0 / texSize;

    // Sample surrounding pixels for glow
    float edgeAlpha = 0.0;
    const int sampleDist = 2;
    for(int x = -sampleDist; x <= sampleDist; x++) {
        for(int y = -sampleDist; y <= sampleDist; y++) {
            vec2 offset = vec2(x, y) * texelSize;
            edgeAlpha += texture(texture0, fragTexCoord + offset).a;
        }
    }
    
    // Normalize and calculate glow strength
    float glowStrength = edgeAlpha / pow(sampleDist * 2.0 + 1.0, 2.0);
    glowStrength = smoothstep(0.1, 0.5, glowStrength) * 0.6;

    // Core visual properties
    vec3 coreColor = texColor.rgb * colDiffuse.rgb;
    vec3 glowColor = mix(vec3(1.0, 0.9, 0.5), coreColor, 0.3);

    // Combine effects
    vec3 finalRGB = coreColor + glowColor * glowStrength * (1.0 - texColor.a);
    finalColor = vec4(finalRGB, texColor.a * colDiffuse.a);
}