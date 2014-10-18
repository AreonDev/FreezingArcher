#!/bin/bash

MAKEFILE=$'CSC=gmcs
CFLAGS=-r:System.Drawing -r:System.Core -r:Pencil.Gaming/lib/NVorbis.dll -optimize+ -debug- -target:library -platform:anycpu -unsafe+
FILES='

while IFS= read -r -d $'\0' file; do
    MAKEFILE="$MAKEFILE$file "
done < <(find . -name "*.cs" -type f -print0)

CONFIGS1=$'
compat_glfw2:
\trm -r "Pencil.Gaming/bin/Compatibility - GLFW2"
\tmkdir "Pencil.Gaming/bin/Compatibility - GLFW2/lib" -p
\tcp "Pencil.Gaming/lib/NVorbis.dll" "Pencil.Gaming/bin/Compatibility - GLFW2/lib"
\tcp -r "Pencil.Gaming/natives32-glfw2" "Pencil.Gaming/bin/Compatibility - GLFW2/lib32"
\tcp -r "Pencil.Gaming/natives64-glfw2" "Pencil.Gaming/bin/Compatibility - GLFW2/lib64"
\tcp "Pencil.Gaming/lib/Pencil.Gaming.dll.config" "Pencil.Gaming/bin/Compatibility - GLFW2/lib/Pencil.Gaming.dll.config"
\t$(CSC) $(FILES) -out:"Pencil.Gaming/bin/Compatibility - GLFW2/lib/Pencil.Gaming.dll" -define:USE_GL_COMPAT\;USE_GLFW2 $(CFLAGS)'
CONFIGS2=$'
compat_glfw3:
\trm -r "Pencil.Gaming/bin/Compatibility - GLFW3"
\tmkdir "Pencil.Gaming/bin/Compatibility - GLFW3/lib" -p
\tcp "Pencil.Gaming/lib/NVorbis.dll" "Pencil.Gaming/bin/Compatibility - GLFW3/lib"
\tcp -r "Pencil.Gaming/natives32-glfw3" "Pencil.Gaming/bin/Compatibility - GLFW3/lib32"
\tcp -r "Pencil.Gaming/natives64-glfw3" "Pencil.Gaming/bin/Compatibility - GLFW3/lib64"
\tcp "Pencil.Gaming/lib/Pencil.Gaming.dll.config" "Pencil.Gaming/bin/Compatibility - GLFW3/lib/Pencil.Gaming.dll.config"
\t$(CSC) $(FILES) -out:"Pencil.Gaming/bin/Compatibility - GLFW3/lib/Pencil.Gaming.dll" -define:USE_GL_COMPAT\;USE_GLFW3 $(CFLAGS)'
CONFIGS3=$'
core_glfw2:
\trm -r "Pencil.Gaming/bin/Core - GLFW2"
\tmkdir "Pencil.Gaming/bin/Core - GLFW2/lib" -p
\tcp "Pencil.Gaming/lib/NVorbis.dll" "Pencil.Gaming/bin/Core - GLFW2/lib"
\tcp -r "Pencil.Gaming/natives32-glfw2" "Pencil.Gaming/bin/Core - GLFW2/lib32"
\tcp -r "Pencil.Gaming/natives64-glfw2" "Pencil.Gaming/bin/Core - GLFW2/lib64"
\tcp "Pencil.Gaming/lib/Pencil.Gaming.dll.config" "Pencil.Gaming/bin/Core - GLFW2/lib/Pencil.Gaming.dll.config"
\t$(CSC) $(FILES) -out:"Pencil.Gaming/bin/Core - GLFW2/lib/Pencil.Gaming.dll" -define:USE_GL_CORE\;USE_GLFW2 $(CFLAGS)'
CONFIGS4=$'
core_glfw3:
\trm -r "Pencil.Gaming/bin/Core - GLFW3"
\tmkdir "Pencil.Gaming/bin/Core - GLFW3/lib" -p
\tcp "Pencil.Gaming/lib/NVorbis.dll" "Pencil.Gaming/bin/Core - GLFW3/lib"
\tcp -r "Pencil.Gaming/natives32-glfw3" "Pencil.Gaming/bin/Core - GLFW3/lib32"
\tcp -r "Pencil.Gaming/natives64-glfw3" "Pencil.Gaming/bin/Core - GLFW3/lib64"
\tcp "Pencil.Gaming/lib/Pencil.Gaming.dll.config" "Pencil.Gaming/bin/Core - GLFW3/lib/Pencil.Gaming.dll.config"
\t$(CSC) $(FILES) -out:"Pencil.Gaming/bin/Core - GLFW3/lib/Pencil.Gaming.dll" -define:USE_GL_CORE\;USE_GLFW3 $(CFLAGS)'
MAKEFILE=$MAKEFILE$CONFIGS1$CONFIGS2$CONFIGS3$CONFIGS4

echo "$MAKEFILE" > Makefile
echo "Makefile generated successfully."
