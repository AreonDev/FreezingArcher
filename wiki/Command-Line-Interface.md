---
layout: wikipage
title: Command Line Interface
wikiPageName: Command-Line-Interface
menu: wiki
---

```c#
namespace FreezingArcher.Configuration
```

The command line interface is used to pass non persistent information to the program. It can be used to override conigs
from the config file.

**Command line argument like `-e|--example VALUE` can be created like this:**

```c#
CommandLineInterface.Instance.AddOption<int> (my_int =>
{
    switch (my_int)
    {
    case 0:
        Console.WriteLine ("Nothing to say.");
        break;
    case 1:
        Console.WriteLine ("You're the one!");
        break;
    case 3:
        Console.WriteLine ("Foo makes bar without bla...");
        break;
    case 42:
        Console.WriteLine ("AWESOME!!!!1111111");
        break;
    default:
        Console.WriteLine ("You shall not pass!");
        break;
    }
}, 'e', "example", "Check it out, an awesome example!", "INT", true, 3);
```

* The `Action<int>` is the function which is called when this option was successfully parsed and data is available.
* `e` is the short name of the option accessible with the `-`.
* `example` is the long name of the option accessible with the `--`.
* `Check it out, an awesome example!` is the text displayed in the help message.
* `INT` is the text displayed for the meta value in the help message.
* `true` describes whether this option is required or not.
* `3` describes the default value if an invalid value or nothing is given.

**To receive arguments not passed with an option like `appname val1 val2 val3 ...` do:**

```c#
CommandLineInterface.Instance.SetValueList<List<string>> (arglist =>
{
    foreach (var arg in arglist)
        Console.WriteLine (arg);
}, "FILES", 6);
```

* The `Action<List<string>>` is the function which is called when the arguments were successfully parsed and data is
available. The generic type parameter has to be of the type `System.Collections.IList`.
* `FILES` is the text displayed for the meta value in the help message.
* `6` is maximum amount of `FILES` to appear. The parser would abort with an error if there were more than `6` files
given. When set to `-1` (default) the amount of arguments is infinite.

`AddOptionList<T>` is similar to `AddOption<T>`. The generic type parameter has to be of the type
`System.Collections.IList`.

`AddOptionArray<T>` is similar to `AddOption<T>`. The action handler needs to be like `Action<T[]>`.

**To set a help text do something like this:**

```c#
CommandLineInterface.Instance.SetHelp (
    "awesome-foo", // program name
    "v2.1.4",      // version
    "Darth Vader", // author(s)
    2015,          // year
    'h',           // short name
    "help",        // long name
    true,          // additional new line after every option
    true,          // add dashes to options
    null,          // IEnumerable<string> lines to appear before options list
    null           // IEnumerable<string> lines to appear after options list
);
```

