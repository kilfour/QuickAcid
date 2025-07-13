using QuickExplainIt;

namespace QuickAcid.TestsDeposition._Tools;

public class CreateDoc
{
    [Fact]
    public void Go()
    {
        new Document().ToFile("README.md", typeof(CreateDoc).Assembly);
    }



    private const string Introduction =
@"# QuickMGenerate

## Introduction
An evolution from the QuickGenerate library.

Aiming for : 
- A terser (Linq) syntax.
- A better way of dealing with state.
- Better composability of generators.
- Better documentation.
- Fun.


 ---
";
    private const string AfterThoughts =
@"## After Thoughts

Well ... 
Goals achieved I reckon.
 * **A terser (Linq) syntax** :
For some who are not used it, it might get tricky to get into. 
I must say, I myself, only started using it when I started using [Sprache](https://github.com/sprache/Sprache). 
A beautifull Parsec inspired parsing library.
Stole some ideas from there, I must admit.

 * **A better way of dealing with state, better composability of generators** :
Here's an example of something simple that was quite hard to do in the old QuickGenerate :

```
var generator =
	from firstname in MGen.ChooseFromThese(DataLists.FirstNames)
	from lastname in MGen.ChooseFromThese(DataLists.LastNames)
	from provider in MGen.ChooseFromThese(""yahoo"", ""gmail"", ""mycompany"")
	from domain in MGen.ChooseFromThese(""com"", ""net"", ""biz"")
	let email = string.Format(""{0}.{1}@{2}.{3}"", firstname, lastname, provider, domain)
	select
		new Person
			{
				FirstName = firstname,
				LastName = lastname,
				Email = email
			};
var people = generator.Many(2).Generate();
foreach (var person in people)
{
	Console.Write(person);
}
```
Which outputs something like :
```
  Name : Claudia Coffey, Email : Claudia.Coffey@gmail.net.
  Name : Dale Weber, Email : Dale.Weber@mycompany.biz.
```
 * **Better documentation** : You're looking at it.
 * **Fun** : Well, yes it was.

Even though QuickMGenerate uses a lot of patterns (there's static all over the place) that I usually frown upon,
It's a lot less code, it's a lot more composable, it's, ... well, ... what QuickGenerate should have been.

";
}