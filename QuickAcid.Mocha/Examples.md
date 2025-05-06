```csharp
var engine = new Engine()
    .SetValue("log", new Action<object>(Console.WriteLine));
    
engine.Execute(@"
    function hello() { 
        log('Hello World');
    };
 
    hello();
");

var square = new Engine()
    .SetValue("x", 3) // define a new variable
    .Evaluate("x * x") // evaluate a statement
    .ToObject(); // converts the value to .NET

var p = new Person {
    Name = "Mickey Mouse"
};

var engine = new Engine()
    .SetValue("p", p)
    .Execute("p.Name = 'Minnie'");

Assert.AreEqual("Minnie", p.Name);

var result = new Engine()
    .Execute("function add(a, b) { return a + b; }")
    .Invoke("add",1, 2); // -> 3

var engine = new Engine()
   .Execute("function add(a, b) { return a + b; }");

engine.Invoke("add", 1, 2); // -> 3


var engine = new Engine(options =>
{
    options.EnableModules(@"C:\Scripts");
})

var ns = engine.Modules.Import("./my-module.js");

var value = ns.Get("value").AsString();

engine.Modules.Add("user", "export const name = 'John';");

var ns = engine.Modules.Import("user");

var name = ns.Get("name").AsString();


```