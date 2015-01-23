Never Null Proxy
====

This is a method interceptor using castle dynamic proxy that gives you a call ch ain that never produces a null reference exception.

For example:

```csharp
var user = new User().NeverNull();

var street = user.School.District.Street.Final();

Assert.That(street, Is.Null);
```

For a more indepth explanation visit http://onoffswitch.net/minimizing-null-ref/