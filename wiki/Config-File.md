---
layout: wikipage
title: Config File
wikiPageName: Config-File
menu: wiki
---

```c#
namespace FreezingArcher.Configuration
```

The `Application` is able to store and load settings in an `INI` config file. The config files are managed by the
`ConfigManager`. New config files should be added to it. The `ConfigManager` sends `ConfigManagerItemAdded` and
`ConfigManagerItemRemoved` messages through the `MessageManager`.

## Create a new config file:

```c#
using System.Collections.Generic;
using FreezingArcher.Configuration;
using FreezingArcher.Core;
using Section = System.Collections.Generic.Dictionary<string, FreezingArcher.Configuration.Value>;

// create new default values for the config
Pair<string, Dictionary<string, Value>> myconfig =
    new Pair<string, Dictionary<string, Section>> ("myconfig", new Dictionary<string, Section> ());
Section myconfig_section1 = new Section (); // create new section
Section myconfig_section2 = new Section ();
myconfig.B.Add ("section1", myconfig_section1); // add new section to myconfig and name is "section1"
myconfig.B.Add ("section2", myconfig_section2);
myconfig_section1.Add ("option1", new Value (true)); // add option1 to section1 with default value true
myconfig_section1.Add ("option2", new Value (1.2));
myconfig_section2.Add ("awesomeness", new Value (42));

ConfigManager.Instance.Add (new ConfigFile (myconfig.A, myconfig.B, messageManager));
```

The `ConfigFile` sends `ConfigFileSaved` and `ConfigFileValueSet` messages through the `MessageManager`.

## Read from config file:

```c#
ConfigManager.Instance["myconfig"].GetInteger ("section2", "awesomeness");
```

## Write to config file:

```c#
ConfigManager.Instance["myconfig"].SetInteger ("section2", "awesomeness", 77);
```

## Save config:

```c#
ConfigManager.Instance["myconfig"].Save ();
// OR
ConfigManager.Instance.SaveAll ();
```

If the config file is deleted it will be regenerated. If the config file is corrupted or options are missing the program
will leave the file as is. To repair it delete the file and start the application. The default values will appear in the
config file.

