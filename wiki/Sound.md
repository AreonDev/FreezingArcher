---
layout: wikipage
title: Sound
wikiPageName: Sound
menu: wiki
---

```c#
namespace FreezingArcher.Audio
```

`FreezingArcher` ships with a simple interface for audio playback. `Sources` and `Sounds` may be loaded and created only
with the `AudioManager` after the `Application` constructor.

## Loading a `Sound` and creating a `Source`:

```c#
using FreezingArcher.Audio;
using Pencil.Gaming.MathUtils;

// before init and load
AudioManager.LoadSound ("bigbuckbunny", "audio-files/bigbuckbunny.ogg");
AudioManager.LoadSound ("coconut", "audio-files/coconut2.wav");
Source s = AudioManager.CreateSource ("bunny", "bigbuckbunny", "coconut");

// after init and load
s.Loop = true;
s.Position = new Vector3 (-1, 4, 1);
...
AudioManager.PlaySource ("bunny");
```

For a detailed description on how the properties and methods of the `Sound` (`ALBuffer`), `Source` (`ALSource`) and
`Listener` (`ALListener`) work have a look at the
[OpenAL Specifications](http://www.openal.org/documentation/openal-1.1-specification.pdf).

