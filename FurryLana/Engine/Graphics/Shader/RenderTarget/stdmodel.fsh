#version 330 core

uniform sampler2D DiffuseTexture;
uniform vec3 LightDirection;
uniform float Ambient;

in vec4 Position;
in vec3 Normal;
in vec2 TexCoord;

out vec4 DiffuseColor;

void main ()
{
    float lighting = clamp (dot (Normal, LightDirection), Ambient, 1.0);
    vec3 textureColor = texture (DiffuseTexture, TexCoord).rgb;
    DiffuseColor = vec4 (textureColor * lighting, 1.0);
}
