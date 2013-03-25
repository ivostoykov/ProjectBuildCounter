ProjectBuildCounter is a small utility that auto increments the build number of Visual Studio projects. Increment is made on each build nevertheless it is Debug or Release.

This tool manipulates Properties\AssemblyInfo.cs file of a project. As the file name assume it contains somewhere in the contents the product version. It has following form Major, Minor, Build and Release as shown below:

  [assembly: AssemblyVersion("1.0.13.2165")]

ProjectBuildCounter reads AssemblyInfo.cs file and increments versions using following logic:

* Build number (3rd position) is incremented from 1 to 9999. When it goes beyond previous number (Minor) is incremented and build counter is set to one
* Minor is incremented from 1 to 99. When it goes beyond 99 previous number (Major) is incremented.
* Revision - last (4th) number - whenever it exists or not, for the moment is not manipulated.
