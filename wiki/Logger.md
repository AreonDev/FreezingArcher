---
layout: wikipage
title: Logger
wikiPageName: Logger
menu: wiki
---

```c#
namespace FreezingArcher.Output
```

The logger is used for logging messages to the console and to a logfile located in the output directory. You may not use
`Console.Write...` for command line output. Instead use the logger as the logger is aware of log levels and writes the
output to a file. Additionally the logger runs in an extra thread. Therefore the game performance is not affected when
producing a high amount of log lines. To add a log line do something like this:

```c#
Logger.Log.AddLogEntry (LogLevel.Debug, "AwesomeModule",
                        "The awesome module has reached an awesomeness of {0}!",
                        awesomeness);
```

## Available log levels:

1. **Debug**: Messages for debug usage
2. **Fine**: Totally unneccessary status information
3. **Info**: Standard log level
4. **Warning**: Just a warning, will not cause any crashes
5. **Error**: Error which may cause a crash
6. **Severe**: Severe error which will likely cause a crash
7. **Fatal**: Fatal error may be logged immediately before a crash
8. **Crash**: There was an error which the program couldn't handle and the game crashed

It is possible to set the log level of the logger via `Logger.Log.SetLogLevel (LogLevel level)`. However you may use the
command line or the config file to set the log level. Otherwise those mechanism won't have any effect on the log level
any more.

