# Dassie Preprocessor

A small command line application and Dassie compiler extension allowing you to embed [Dassie](https://github.com/loschsoftware/dc) expressions into any file.

## How to build and install (Windows)
````
git clone https://github.com/jgh07/Dassie.Preprocessor.git
cd Dassie.Preprocessor
build
````
On Linux, you'll need to manually build using the ``dotnet`` command. You will probably run into issues either way, since the Dassie compiler codebase is sadly still pretty Windows-centric.

After building, you can either use the application directly using the ``pp`` command, or import it as a compiler extension and run it as ``dc pp``:
````
dc package import build/pp.dll
````
## Syntax
Here is an example file with embedded Dassie expressions:
````
#head {
    import System
}

#members {
    Add (x: int32, y: int32) = x + y
    GetBalance (): int32 = 3
}

The ultimate answer to the great question of life, the universe and everything is ${21 * 2}.
Today is ${DateTime.Now}.
I have €${GetBalance} in my bank account.
Adding 5 and 10 together yields ${Add 5, 10}.
````
Assuming the above text is stored in a file called ``example.txt``, running the command ``dc pp example.txt`` will yield an output file called ``example.out.txt`` with the following content (at the time of writing):
````
The ultimate answer to the great question of life, the universe and everything is 42.
Today is 01.09.2024 23:41:25.
I have €3 in my bank account.
Adding 5 and 10 together yields 15.
````

A *template file* contains 3 elements:
- Plain text
- Dassie expressions: ``${ ... }``
- Named *sections*: ``#<Name> { ... }``

**Sections** contain Dassie source code, but are not included in the output file. They are used to define functions (in the ``members`` section) and declare imports (in the ``head`` section).
