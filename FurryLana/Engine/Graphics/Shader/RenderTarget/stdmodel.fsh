#version 330

uniform sampler2D DiffuseTexture;

in vec4 Position;
in vec3 Texcoord;

out vec4 DiffuseColor;

void main ()
{
  DiffuseColor = vec4 (1,1,1,1);
}
