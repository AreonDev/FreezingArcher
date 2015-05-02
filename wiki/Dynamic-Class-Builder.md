---
layout: wikipage
title: Dynamic Class Builder
wikiPageName: Dynamic-Class-Builder
menu: wiki
---

```c#
namespace FreezingArcher.Reflection
```

DynamicClassBuilder is a really great helper to make static libraries more dynamic.

What you can do is basically create new Classes with Properties, Fields and Methods on the fly,
without the need to save them to disk. Awesome, huh?


## Core Features

* Create new classes on the fly with a simple, convenient API which is documented below
* Classes can have methods with dynamic implementations
* Classes can have properties of any type, even other dynamic generated classes
* Properties and Methods can have CustomAttributes

## Let's get started!

### I want to build a new class on the fly, with some properties. 

What you do:

* Create some Property-objects
* Create a dynamic class builder object
* Attach the properties to the class builder
* Build your own type

```c#
using FreezingArcher.Core;

var intProperty = new Property("intProperty", typeof(int));
var stringProperty = new Property("stringProperty", typeof(string));
var classBuilder = new DynamicClassBuilder("className");
classBuilder.AddProperty(intProperty);
classBuilder.AddProperty(stringProperty);
```

Now we are ready to build our custom type:

```c#
var type = classBuilder.CreateType();
var instance = Activator.CreateInstance(type);
```

So there is the instance of our newly created type. 
Pretty straightforward, hm?

### What about methods?

What you do:

* Create a new Method-object
* Attach it to your previously created class builder
* Create your type

```c#
Func<object, string, int> implementation = (instance,str) => 
{
    Console.WriteLine(instance);
    return str.Length;
};
var method = new Method("DoFoo", implementation);
classBuilder.AddMethod(method);
```

The dynamic class builder will analyze the signature of your given implementation, create a new method, save the
delegate into a static field, and add a call to the given implementation. 

Looks easy? It is!


### What if I need to add Annotations / Attributes to some Properties?

Now it's a bit complicated. As you might know, Attributes in C# can be initialized using 3 different ways:

* via Constructor
* via so called "Named Properties"
* via "Named Fields"

Our Attribute abstraction supports all 3 ways in a convenient way. 
What you do:

* Create an Attribute object
* Call the "CallConstructor" method, as you would call the constructor of the attribute
* Add NamedParameters via the "AddNamedParameters" method
* Add the Attributes in the parameters of Properties or Methods

```c#
var attrib = new Attribute(typeof(YourAttribute));
attrib.CallConstructor("string", 1); //call constructor with string and int
attrib.AddNamedParameters(new Pair<string,object>("Required", true));
var property = new Property("PropertyName", typeof(int), attrib); 
classBuilder.AddProperty(property);
var type = classBuilder.CreateType();
```

> Warning: null-values in constructor parameters are currently not supported.

