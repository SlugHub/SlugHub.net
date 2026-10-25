# SlugHub #
## Highly configurable URL slug generator, with persistent storage ##

SlugHub takes a string as an input, and converts it into URL friendly `slug`

For example:

	var slugGenerator = new SlugGenerator();
	var slug = await slugGenerator.GenerateSlugAsync("Some text that needs slugging");
	Console.WriteLine(slug);

Output:

> some-text-that-needs-slugging

#### Grouping Key ####
The `Slug` object has an optional.0 `GroupingKey` property.  
This can be used to differentiate between applications, accounts, domains, or anything. You could use this to deploy a multi-tenant service that serves slugs to all of your services.

### Generating unique slugs using iteration ###

Calling `GenerateSlugAsync(input)` with text that's already been slugged once (*and therefore stored in the `SlugStore` - more on that later*) will trigger an **iteration**.  

Let's see an example:

	var slug1 = await slugGenerator.GenerateSlugAsync("My Title");
	var slug2 = await slugGenerator.GenerateSlugAsync("My Title");
	
	Console.WriteLine(slug1);
	Console.WriteLine(slug2);
	
The output here would be

>my-title  
>my-title-1

The iteration seed can be configured using `SlugGeneratorOptions` when creating an instance of `SlugGenerator`

	//start at 100
	var options = new SlugGeneratorOptions { IterationSeedValue = 100 };
	var slugGenerator = new SlugGenerator(options);

	var slug1 = await slugGenerator.GenerateSlugAsync("My Title");
	var slug2 = await slugGenerator.GenerateSlugAsync("My Title");
	
	Console.WriteLine(slug1);
	Console.WriteLine(slug2);

The output here would be

>my-title  
>my-title-100


### Generating unique slugs using "uniquifiers" ###

Uniquifiers are things that can create a unique version of your input.  
Imagine you want to create slugged titles for pizza shops.  
Now let's assume one is in London, one is in Leeds.
We can pass London and Leeds in as optional uniqueifier parameters, which will then be included, if the original slug is already taken:

	var slug1 = await slugGenerator.GenerateSlugAsync("Pizza Shop", "London");
	var slug2 = await slugGenerator.GenerateSlugAsync("Pizza Shop", "Leeds");
	
In this example, the 2 slugs generated would be:

> pizza-shop  
> pizza-shop-leeds

This is because pizza-shop initially did not exist, so it didn't need a uniqueifier.  
The second time it tried to generate pizza-shop, it existed, so used the uniqueifier.

## Storage ##

The default implementation of `ISlugStore` is a simple `InMemorySlugStore`  
Exactly as it sounds, this simply stores slugs in memory.  
They are not persisted outside of the application, and when the application exits, the stored slugs are lost.

If you want something more persistable, simply create one, implementing the lightweight `ISlugStore` interface:

    public interface ISlugStore
    {
        Task<bool> ExistsAsync(string slug);
        Task StoreAsync (Slug slug);
    }
