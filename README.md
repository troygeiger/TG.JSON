# TG.JSON
TG.JSON is a .NET library for creating, parsing, serializing and deserializing JSON. It's small, fast, flexible and very easy to use.

![NuGet Build & Deploy](https://github.com/troygeiger/TG.JSON/workflows/NuGet%20Build%20&%20Deploy/badge.svg)

### Key Features
- Parse from strings, streams and file paths.
- Output JSON using .ToString().
- Output JSON to file using .Write().
- Format output with multiple styles.
- Directly add, remove and manipulate values.
- Serialize and Deserialize objects and arrays.
- Encrypt and Decrypt values for local config storage.
- Bind to WinForms and WPF controls. 

### Compiled Framework
- .NET 2.0
- .NET 3.5
- .NET 4.0
- .NET 4.5
- .NET Standard 1.0
- .NET Standard 1.3
- .NET Standard 2.0

## Basic Usage
```
JsonObject obj = new JsonObject(
  "name", "John Doe",
  "age", 33,
  "sex", "Male",
  "married", true);

if (obj["age"] == 33)
{
  obj["isAwesome"] = true;
}
Console.WriteLine(obj.ToString(Formatting.Indented));

Output:
{
  "name": "John Doe",
  "age": 33,
  "sex": "Male",
  "married": true,
  "isAwesome": true
}
```
In the example above, it is creating a JsonObject by passing pairs of string and a value. In this case, the JsonObject constructor is receiving an object array where the odd index of the array is a string in which defines the property name and the even index is the value.

With the `if` statement, you can see that it is implicitly comparing the value of `age` to see if it equals `33`. In this example, that evaluates `true` and then set property `isAwesome` to `true`. Note that `isAwesome` was not previously defined. This works by automatically adding it if the property doesn't exist.

Next, the `obj` JsonObject is output to string with the format set to Indented. By default, the indent character is tab. This can be changed by setting JsonObject's `IndentString` property. One thing to note is that all child values evaluate the root `IndentString` property when inserting indents.

## Additional Documentation
Below is in index of additional documentation;
- [Formatting Output](help-docs/Formatting.md)
- [Serialize Objects](help-docs/Serialize.md)
