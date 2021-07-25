# ENCODE: An ECS Conversion Designer and Education Tool

ENCODE is a tool that can automatically generate an ECS design from Object-Oriented Source code. At the moment the tool works on C# projects. It uses Visual Studios Roslyn APIs, some custom code analysis and some dark magic to extract the whole data structure of the source code. 

Designing components for ECS is a balance between data that is processed together and data that 'belongs' together (think all physics data in a component). Where this balance lies, and what a 'good' design means is different for every person and project. Some people prefer large components, other make a component for every variable. 

ENCODE, and the techniques it uses are based around these two ideas: what classes have the same variables (class inheritance) and in what methods are these variables used? To fit the preferences of different people and projects, parameters can be used to alter the output design. 

The output design is shown in two ways. First you can compare the original classes and methods with the entities and systems to see if everything is converted properly. Secondly, you can view the whole design to find commonalities between systems and entities.

ENCODE was created as part of my Master Thesis. While I am proud of the result, it is far from finished. If you input a project with large classes and methods, you will get a bad design and large designs create problems for the visualization.

As part of my thesis, I interviewed some people and gather information on ECS and designing for ECS. The results of this interview and some more in-depth explanation on ENCODE can be found in the [Accompanying Thesis](https://github.com/AnneVanEde/ms_thesis).
