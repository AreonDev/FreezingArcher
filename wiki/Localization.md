---
layout: wikipage
title: Localization
wikiPageName: Localization
menu: wiki
---

```c#
namespace FreezingArcher.Localization
```

The localizer is used to localize strings by locale. A `Localizer` holds multiple instances of a `LocalizationData`. A
`LocalizationData` is associated with a locale. The currently supported locales are `en_US` and `de_DE`. The
`LocalizationData` stores the actual strings used for localization. Each localization has a parent locale. If a string
is not available in a locale the one from the parent will be selected. The locale specific strings are parsed from xml
files stored in the `Localization/` folder of the output directory. The structure of the xml is like this:

```xml
<!-- The parent attribute is optional -->
<locale parent="en_US">
    <string name="awesome_text">Awesome text in my language!!!1111</string>
</locale>
```

Get a string from the localizer with the current locale:

```c#
Console.WriteLine (Localizer.Instance.GetValueForName ("awesome_text"));
```

Set the current locale:

```c#
Localizer.Instance.CurrentLocale = LocaleEnum.de_DE;
```

If the locale changes an `UpdateLocale` message will be send via the message manager.

