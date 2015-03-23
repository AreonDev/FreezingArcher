---
layout: wikipage
title: Object recycling and fast comparisons
wikiPageName: Object-recycling-and-fast-comparisons
menu: wiki
---

To reduce or prevent garbage collections we introduced an all new technique (which is not this new, but anyway, it's great!)

### Object recycling

The idea behind is, that you don't create new (potentially large) objects when you need them, but instead create a pool and keep used instances as reference and just clean them up and re-use them.

This is done in our framework with the ObjectManager and the FAObject classes which play together quite nicely.
As code is worth 1000 words, just get started:

```c#
using FreezingArcher.Core;

[TypeIdentifier(1)]
public class MySuperAwesomeGameObject : FAObject
{

}
```

We note the following things:

* You inherit from FAObject
* Every class which inherits from FAObject !must! have the TypeIdentifier Attribute.
 * In our library this is achieved by the post-processor at compile-time
* Your class should have a default constructor, as our ObjectManager uses this. 
 * Any other initializations should be done in Init() or so

So let's go on:

```c#
var objectManager = new ObjectManager();
var gameObject1 = objectManager.CreateOrRecycle<MySuperAwesomeGameObject>(1);
var gameObject2 = objectManager.CreateOrRecycle<MySuperAwesomeGameObject>(1);

gameObject1.Init(); //perform your construction logic here
gameObject2.Init(); 

//do stuff with your object

gameObject1.Destroy(); //destroy your object and recycle it

var gameObject3 = objectManager.CreateOrRecycle<MySuperAwesomeGameObject>(1);
//gameObject3 will in fact be the recycled gameObject1
gameObject3.Init();
gameObject2.Destroy();

//do stuff with gameObject3

gameObject3.Destroy();
```

Important:

* by calling Destroy you should clean all fields, properties, etc. as later callers will get the same physical object
* you replace all calls to new() by objectManager.CreateOrRecycle
* objectManager is thread-safe
