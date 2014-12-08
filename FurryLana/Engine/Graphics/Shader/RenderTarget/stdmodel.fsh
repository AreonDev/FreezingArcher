#version 330 core

uniform sampler2D DiffuseTexture;

in vec4 Position;
in vec3 Normal;
in vec2 Texcoord;

out vec4 DiffuseColor;

void main ()
{
    DiffuseColor = Position;
}