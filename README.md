# TG.JSON
This is a .net library for creating, parsing, serializing and deserializing JSON.
It's small, fast and very easy to use.

## Basic Usage
```
JsonObject obj = new JsonObject("name", "John Doe", "age", 33, "sex", "Male", "married", true);
if (obj["age"] == 33)
{
  obj["isAwesome"] = true;
}
string json = obj.ToString(Formatting.Indented);
```
### Documentation
You can find documentation at http://troy.geigernet.org/Help/TG/JSON
