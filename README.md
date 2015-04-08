FreezingArcher
=========
A 3D game engine (framework) which is:
 * lightweight
 * fast
 * extensible
 * easy-to-use

How to clone
------------
```sh
$ git clone git@github.com:fin-ger/FreezingArcher.git
$ cd FreezingArcher
$ git submodule init
$ git submodule update
```

How to build
------------
### Windows
Use `WindowsDebug` or `WindowsRelease` build targets when developing with `VisualStudio 2013`. All other IDE's are not officially supported.

### Linux
Use `Debug` or `Release` build target when developing with `MonoDevelop >=5.0`. All other IDE's are not officially supported.

### MacOS
There is currently no support for building and running `FreezingArcher` under `MacOS`.

About the name
--------------
It was a name suggestion from github.

Documentation
-------------
Have a look at the [wiki](https://github.com/fin-ger/FreezingArcher/wiki) for some [examples](https://github.com/fin-ger/FreezingArcher/wiki/Components) and a [quickstart](https://github.com/fin-ger/FreezingArcher/wiki/Home#quickstart) guide.

CI
--
Have a look at our [CI System](http://jenkins.areon-dev.de) for the latest builds and build trends of our project.

Currently we are working hard to let failed builds create new issues.

External libraries
------------------
 * [STA.INIFile](http://www.codeproject.com/Articles/35401/A-Cross-platform-C-Class-for-Using-INI-Files-to-St) from Moreno Airoldi, licensed under [The Code Project Open License (CPOL)](http://www.codeproject.com/info/cpol10.aspx)
 * [Pencil.Gaming](https://github.com/andykorth/Pencil.Gaming) from Andy Korth and Antonie Blom, licensed under [MIT](https://github.com/andykorth/Pencil.Gaming/blob/master/Pencil.Gaming/LICENSE.TXT)  
 we created out own fork [here](https://github.com/martin31821/Pencil.Gaming)
 * [CommandLine](http://commandline.codeplex.com/) from Giacomo Stelluti Scala, licensed under [MIT](http://commandline.codeplex.com/license)
 * [Jitter Physics](http://jitter-physics.com) from Thorben Linneweber, licensed under a proprietary license which can be found in the original source code  
 we created our own fork [here](https://github.com/martin31821/jitterphysics)
 * [Assimp-Net](https://code.google.com/p/assimp-net/) from Nicholas Woodfield, licensed under [MIT](http://opensource.org/licenses/mit-license.php)  
we created our own fork [here](https://github.com/martin31821/assimp-net)
 * [GWEN dotnet](https://code.google.com/p/gwen-dotnet/), licensed under [MIT](http://opensource.org/licenses/mit-license.php)  
we created our own fork [here](https://github.com/martin31821/gwen-dotnet)

Additionally we use the awesome Jenkins CI System
