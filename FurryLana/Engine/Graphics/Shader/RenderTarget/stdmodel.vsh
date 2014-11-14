#version 130

uniform mat4 ProjMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;

in vec3 inPosition;
in vec3 inCoord;

out vec4 outPosition;
out vec3 outCoord;

void main ()
{
  mat4 ModelViewProjMatrix = ViewMatrix * ModelMatrix * ProjMatrix;

  vec4 outPosition = vec4 (inPosition, 1.0);

  outCoord = inCoord;

  gl_Position = ModelViewProjMatrix * outPosition;
}
