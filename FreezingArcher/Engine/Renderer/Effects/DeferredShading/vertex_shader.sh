//Input format
//####################################################
layout(location = 0) in vec3 InPosition;
layout(location = 2) in vec3 InTexCoord1;
//####################################################

//Output per Pixel
//####################################################
out gl_PerVertex
{
        vec4 gl_Position;
        float gl_PointSize;
        float gl_ClipDistance[];
};

layout(location = 0) out vec2 texcoord; // texture coordinate

void main()
{
        gl_Position = InPosition;
        texcoord = InTexCoord;
}