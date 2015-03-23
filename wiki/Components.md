---
layout: wikipage
title: Components
wikiPageName: Components
menu: wiki
---

# Freezing Archer Core Components
This page should give you some examples on how to use the core components.

## Application Class

```c#
namespace FreezingArcher.Core
```

The application holds and creates a window, a game, a message manager and an input manager. Furthermore the application initializes all global components, manages initialization and loading of resources. The application has a global static `Instance` field. This is where a global application instance is stored. To create an application do something like this:

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

There are also some `Application.Create (...)` functions. You shall not use them as they are [`#evil`](http://www.emacswiki.org/emacs/Evil?interface=en).

## Renderer
Not available yet!

## Physics
Not available yet!

## Sound

```c#
namespace FreezingArcher.Audio
```

`FreezingArcher` ships with a simple interface for audio playback. `Sources` and `Sounds` may be loaded and created only with the `AudioManager` after the `Application` constructor.

**Loading a `Sound` and creating a `Source`:**

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

For a detailed description on how the properties and methods of the `Sound` (`ALBuffer`), `Source` (`ALSource`) and `Listener` (`ALListener`) work have a look at the [OpenAL Specifications](http://www.openal.org/documentation/openal-1.1-specification.pdf).

## Input Management
The input manager needs an update. Further information will be added when the update is complete.

## Localization

```c#
namespace FreezingArcher.Localization
```

The localizer is used to localize strings by locale. A `Localizer` holds multiple instances of a `LocalizationData`. A `LocalizationData` is associated with a locale. The currently supported locales are `en_US` and `de_DE`. The `LocalizationData` stores the actual strings used for localization. Each localization has a parent locale. If a string is not available in a locale the one from the parent will be selected. The locale specific strings are parsed from xml files stored in the `Localization/` folder of the output directory. The structure of the xml is like this:

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

## Messaging System

```c#
namespace FreezingArcher.Messaging
```

The messaging system is used to communicate between the components. The `MessageManager` manages the communication.

**Example usage:**

```c#
class ItemProducedMessage : IMessage
{
    public ItemProducedMessage (string item) { Item = item; }

    public string Item { get; protected set; }

    #region IMessage implementation
    public object Source { get; set; }
    public object Destination { get; set; }
    public int MessageId { get { return 42; }} // return unique identifier for this message type
    #endregion
}

class Producer : IMessageCreator
{
    public Producer (..., MessageManager msgMnr)
    {
        msgMnr += this;
        ...
    }
    ...
    public void Produce ()
    {
        if (MessageCreated != null)
            MessageCreated (new ItemProducedMessage ("item"));
    }
    ...
    public event MessageEvent MessageCreated;
}

class Consumer : IMessageConsumer
{
    public Consumer (..., MessageManager msgMnr)
    {
        msgMnr += this;
        ...
    }
    ...
    public void ConsumeMessage (IMessage msg)
    {
        ItemProducedMessage m = msg as ItemProducedMessage;
        if (m != null)
            Console.WriteLine (m.Item);
    }
    ...
    // array containing which messages this consumer accepts
    public int[] ValidMessages { get { return new int[] {42}; } }
}
```

## Logger

```c#
namespace FreezingArcher.Output
```

The logger is used for logging messages to the console and to a logfile located in the output directory. You may not use `Console.Write...` for command line output. Instead use the logger as the logger is aware of log levels and writes the output to a file. Additionally the logger runs in an extra thread. Therefore the game performance is not affected when producing a high amount of log lines. To add a log line do something like this:

```c#
Logger.Log.AddLogEntry (LogLevel.Debug, "AwesomeModule", "The awesome module has reached an awesomeness of {0}!", awesomeness);
```

### Available log levels:

1. **Debug**: Messages for debug usage
2. **Fine**: Totally unneccessary status information
3. **Info**: Standard log level
4. **Warning**: Just a warning, will not cause any crashes
5. **Error**: Error which may cause a crash
6. **Severe**: Severe error which will likely cause a crash
7. **Fatal**: Fatal error may be logged immediately before a crash
8. **Crash**: There was an error which the program couldn't handle and the game crashed

It is possible to set the log level of the logger via `Logger.Log.SetLogLevel (LogLevel level)`. However you may use the command line or the config file to set the log level. Otherwise those mechanism won't have any effect on the log level any more.

## Command Line Interface

```c#
namespace FreezingArcher.Configuration
```

The command line interface is used to pass non persistent information to the program. It can be used to override conigs from the config file.

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

* The `Action<List<string>>` is the function which is called when the arguments were successfully parsed and data is available. The generic type parameter has to be of the type `System.Collections.IList`.
* `FILES` is the text displayed for the meta value in the help message.
* `6` is maximum amount of `FILES` to appear. The parser would abort with an error if there were more than `6` files given. When set to `-1` (default) the amount of arguments is infinite.

`AddOptionList<T>` is similar to `AddOption<T>`. The generic type parameter has to be of the type `System.Collections.IList`.

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

## Config File

```c#
namespace FreezingArcher.Configuration
```

The `Application` is able to store and load settings in an `INI` config file. The config files are managed by the `ConfigManager`. New config files should be added to it. The `ConfigManager` sends `ConfigManagerItemAdded` and `ConfigManagerItemRemoved` messages through the `MessageManager`.

**Create a new config file:**

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

**Read from config file:**

```c#
ConfigManager.Instance["myconfig"].GetInteger ("section2", "awesomeness");
```

**Write to config file:**

```c#
ConfigManager.Instance["myconfig"].SetInteger ("section2", "awesomeness", 77);
```

**Save config:**

```c#
ConfigManager.Instance["myconfig"].Save ();
// OR
ConfigManager.Instance.SaveAll ();
```

If the config file is deleted it will be regenerated. If the config file is corrupted or options are missing the program will leave the file as is. To repair it delete the file and start the application. The default values will appear in the config file.

## Object Recycling

```c#
namespace FreezingArcher.Core
```

To reduce or prevent garbage collections we introduced an all new technique (which is not this new, but anyway, it's great!). Want to know [more](Object-recycling-and-fast-comparisons)?

## Dynamic Class Building

```c#
namespace FreezingArcher.Reflection
```

`DynamicClassBuilder` is a really great helper to make static libraries more dynamic.
What you can do is basically create new classes with properties, fields and methods on the fly, without the need to save them to disk. Want to know [more](Dynamic-Class-Builder)?

## Job Executer

```c#
namespace FreezingArcher.Core
```

The `JobExecuter` executes jobs of type `Action`. This can either be done in a sequential, single threaded way or in a parallel, multi threaded way. The job executer handles a `NeedsReexec` event if a job needs to be executed again for some reason.

**Example:**

```c#
List<Action> jobs = new List<Action> ();
// add jobs
...

exec = new JobExecuter ();
exec.InsertJobs (jobs);
// handle if the job executer recognizes that some jobs need to be executed again
exec.DoReexec += () => { ExecAgain = true; };
exec.ExecJobsParallel (Environment.ProcessorCount);
...

while (run)
{
    ...
    if (ExecAgain)
        exec.ExecJobsParallel (Environment.ProcessorCount);
    ...
}
```

