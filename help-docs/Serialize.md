# Serialize to JsonObject
There are several options for serializing classes into Json. One important note is that your classes must have a parameterless constructors. 

In the example below, it is serializing a recursive `Tree` class. You may notice that some of the properties have some attributes added. The `JsonIgnoreProperty` attribute is is self-explanatory. The `JsonProperty` attribute is useful for either mapping the property to the Json property or for mapping a private property to a Json property.
```
    class Tree
    {

        public string Text { get; set; }

        public List<Tree> Children { get; } = new List<Tree>();

        [JsonIgnoreProperty]
        public string IgnoreMe { get; set; }

        [JsonProperty("visible")]
        public bool PropertyNameChange { get; set; }

		[JsonProperty]
        private bool PrivateProperty { get; set; }
    }

	static void Main(string[] args)
    {
        Tree root = new Tree()
        {
            Text = "Root", 
            Children = {new Tree(){Text = "Child1"}, new Tree(){Text = "Child2"}},
            PropertyNameChange = true
        };
        JsonObject json = new JsonObject(root);
        Console.WriteLine(json.ToString(Formatting.Indented));
    }

Output:
{
	"PrivateProperty": false,
	"Text": "Root",
	"Children": [
		{
			"PrivateProperty": false,
			"Text": "Child1",
			"Children": [],
			"visible": false
		},
		{
			"PrivateProperty": false,
			"Text": "Child2",
			"Children": [],
			"visible": false
		}
	],
	"visible": true
}
```
You could also mix multiple object into a single `JsonObject` and deserialize them back into their separate objects but you would want to make sure the property names are different so things don't get mixed up. In the next example, There is also a `Text` property but we are using the `JsonProperty` attribute so map it to `somethingelse_text`. 
```
class SomethingElse
{
    public bool IsSomething { get; set; }

	[JsonProperty("somethingelse_text")]
	public string Text { get; set; }
}

json.Add("OtherTree", new JsonObject(new Tree(){ Text = "Bla" }));
json.SerializeObject(new SomethingElse());
``` 
# Serializing to JsonArray
You can also serialize arrays or any type that inherits from `IEnumerable` and can also be done with passing the array to the `JsonArray` constructor or the `SerializeObject` method. Mixing object types is not possible, as mentioned in the previous section, unless the classes inherit from the same class or implementing the IncludeTypeInformation. More on that in the [Serialization Options](#Serialization-Options) section.

# Deserialize Object
To deserialize the `JsonObject` back to a class is accomplished by calling one of the `Deserialize...` methods. In the example below, we will use he output from the previous section, along with the mixed object added in the second example, and deserialize everything back out.
```
root = json.DeserializeObject<Tree>();
SomethingElse something = json.DeserializeObject<SomethingElse>();
Tree otherTree = (json["OtherTree"] as JsonObject)?.DeserializeObject<Tree>();
```
As you can see, both the `Tree` and `SomethingElse` classes used the same `JsonObject` instance and methods. The `OtherTree` took a little more effort to pull out since it was an object property.

# Deserialize JsonArray
Deserializing from a `JsonArray` is currently only possible with arrays and any class that implements `IList`. If inheriting from `IList`, you must have a `Add` method available that takes one parameter.

# Serialization Options
There is a class available int the `TG.JSON.Serialization` namespace called `JsonSerializationOptions`. This class can passed to each serialize/deserialize method and provides the following options.

- IncludeAttributes: bool
	- This will add a special property to the `JsonObjects`, and any child objects, that stores any attributes and their properties. It will try to re-apply them when deserializing.
- IncludeTypeInformation: bool
	- This will add a special property to the `JsonObjects`, and any child objects, with the `Type.FullName`. If this property is present during deserialization, it will use that type information to initialize the new object.
- MaxDepth: int
	- Ths specified the maximum depth to serialize a nested class. Default is int.MaxValue.
- IgnoreProperties: string[]
	- Any property names, in the string array, will not be serialized.
- SelectedProperties: string[]
	- Any property names, in the string array, will be the only properties to be serialized.
- ApplySelectedPropertiesOnChildren: bool
	- If true, all nested object and objects within arrays will be the only properties serialized.