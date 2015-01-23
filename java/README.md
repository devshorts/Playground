Java Enumerable
----

This is an exersize to implement .NET style enumerable functions on deferred lazy collections using Java.

The goal, to provide

- Map
- Filter
- FlatMap
- OrderBy
- Take
- TakeWhile
- Skip
- SkipWhile
- Iter
- Iteri

The functionality already exists with java 8 streams, but this is fun to do :)

Example
---

Currently you can already do this using the following example:

```java
List<String> strings = asList("oooo", "ba", "baz", "booo");            
                                                                       
Enumerable<String> items = Enumerable.init(strings)            
                                     .orderBy(i -> i.length());
                                                                       
for(String x : items){                                                 
    System.out.println(x);                                             
}                                                                      
```

And stuff like

```java
List<Integer> items = Enumerable.init(strings)            
					            .orderBy(String::length)
					            .map(String::length)    
					            .filter(i -> i == 2)     
                                .toList();                
```

