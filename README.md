# Taron
## Tom and Rob's Object Notation
Taron is a generic object notation format with a light-weight implementation for .Net. It's designed to compete against JSON in terms of usability and readability. It can be used to describe any data structure that JSON can, as well as include optional type information about values which can't be done cleanly in JSON. All parsed data is made available to the user code via the model which doesn't transform the data, but simply encapsulate it.

## Current Implementation Features
* Load and parse format into model with error handling
* Read data from a simple hierarchical model using Linq, recursion or basic loops
* Embedded LR(0) parser with grammar rules for easy extensibility

## Todo / Missing Features
* Benchmark and optimise parsing where needed
* Add system to convert model into a .Net object hierarchy
* Serialising model back to string
* Support for comments in the format
* Clean up API (shouldn't need to call into Parser etc)

## Parsing
The implementation includes a tight LR(0) parser and all lexer/grammar rules needed to parse the Taron format. Parse tables are built when the parser is instantiated and a single parser instance can parse multiple strings efficiently. Benchmarks pending.

## Format Example
The format follows a hierarchy of elements. An element can be a primitive value (string or number), an array or a map. The top level "root" element is always a map (as if the whole document is surrounded by a { }).

A map is surrounded by { } and contains key-value pairs, seperated by whitespace (or nothing). Each pair starts with an identifier, followed by an optional type name, followed either by a value. If the value is a primitive (string or number), an equals (=) must be used. If the value is a complex (map or array), an equals must not be used. Whitespace is ignored and can be used as desired to increase readability of the document.

An array is surrounded by [ ] and contains values, seperated by a comma (,). Each value may start with a type name, though this is not required. A value can be a primitive (string or number) or a complex (map or array). No trailing commas or empty entries are allowed. An array can contain elements of different types, though this is not recommended for consistency's sake.
```Behaviours
{
	thing <b_fuel>
	{
		FuelLevel = 0
		MaxFuelLevel = 1000
		Elements <Element>
		[
			{ Name = "Oxygen" },
			{ Name = "Nitrogen" },
			{ Name = "Robbygen" }
		]
	}
}


SomeVector
{
	x = 10
	y = 20
	<float>z = 24
}

e_welder <EntityDefinition>
{
	DisplayName = "Welder"
	SpriteSheet = "items/tools/general/Welder_off"
	Sprite = "Welder_off"
	Behaviours
	[
		<b_fuel>
		{
			FuelLevel = 0
			MaxFuelLevel = 1000
		},
		<b_fuel>
		{
			FuelLevel = 0
			MaxFuelLevel = 1500
		}
	]
	OrderInLayer = 180
}

EmptyMap {}
```
## Type Names
Any value may be preceded by a type name, which is indicated by an identifier wrapped in angular brackets (< >). Type names are always optional, but it is recommended usage is consistent. The type name by itself does not do anything and does not mutate the data. It is held as a string in the model and can be used by user code as desired. An example of good type name usage is to indicate which type to use when converting complex objects to .Net classes that utilise inheritance.

## Examples
One of Taron's strong points in user readability. This makes it ideal for configuration files where a GUI is not available or not flexible enough to fully customise all options. Annoying brackets and quotes that can go missing are kept to a minimum, and in it's basic form, a config file can look very simple.
```
WindowWidth = 1920
WindowHeight = 1080

FullScreen = 1

NickName = "Player"
```
Alternatively, settings can be grouped together in any arrangement.
```
Window
{
	Width = 1920
	Height = 1080
	FullScreen = 1
}

NickName = "Player"
```
For reference, the JSON equivalent:
```
{
	"Window":
	{
		"Width": 1920,
		"Height": 1080,
		"FullScreen": true
	},
	"NickName": "Player"
}
```
And the XML equivalent:
```
<Config>
	<Window>
		<Width>1920</Width>
		<Height>1080</Height>
		<FullScreen>1</FullScreen>
	</Window>
	<NickName>Player</NickName>
</Config>
```
