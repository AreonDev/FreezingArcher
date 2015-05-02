---
layout: wikipage
title: Application
wikiPageName: Application
menu: wiki
---

```c#
namespace FreezingArcher.Core
```

The application holds and creates a window, a game, a message manager and an input manager. Furthermore the application
initializes all global components, manages initialization and loading of resources. The application has a global static
`Instance` field. This is where a global application instance is stored. To create an application do something like
this:

```c#
using FreezingArcher.Core;

Application.Instance = new Application ("my app name", commandlineArgs);
Application.Instance.Init ();    // initialize application and components
Application.Instance.Load ();    // load resources
MyEpicClassDoesAwesomeStuff ();  // do awesome stuff
Application.Instance.Run ();     // run the application - won't return until the application is closed.
Application.Instance.Destroy (); // cleanup resources, etc.
```

As an alternative you may create your own application instance and manage it by yourself:

```c#
using FreezingArcher.Core;

Application myapp = new Application ("my app name", commandlineArgs);
myapp.Init ();    // initialize application and components
myapp.Load ();    // load resources
AwesomeStuff ();  // do awesome stuff
myapp.Run ();     // run the application - won't return until the application is closed.
myapp.Destroy (); // cleanup resources, etc.
```

There are also some `Application.Create (...)` functions. You shall not use them as they are
[`#evil`](http://www.emacswiki.org/emacs/Evil?interface=en).

