ProjectBuildCounter is a small utility that auto increments the build
number of C# and VB.NET Visual Studio projects.
Increment is made on each build nevertheless it is Debug or Release.
View help.txt for HOWTO.

This tool manipulates Properties\AssemblyInfo.cs file of a project. 
As the file name assume it contains somewhere in the contents the 
product version. It has following form Major, Minor, Build and 
Release as shown below:

  [assembly: AssemblyVersion("1.0.13.2165")]

ProjectBuildCounter reads AssemblyInfo.cs file and increments versions using following logic:

  * Revision number (4th position) is incremented from 1 to 9999.
    When exceed the upper bound Build number is incremented and 
    Release is reset to one.
  * Build number (3rd position) is incremented from 1 to 999. 
    When it goes beyond previous number (Minor) is incremented
    and build counter is reset to one.
  * Minor is incremented from 1 to 99. When current number goes
    above 99 Major is incremented and Minor is reset to one.


Changes in ProjectBuildCounter 1.0.196
======================================
  Added
  -----
  * New help file added.
  * Documentation updated

  Bugs
  ----
  * bug with EventLog access priviliges fixed

Changes in ProjectBuildCounter 1.0.170.000
==========================================
  Added
  -----
  * AssemblyInformationalVersion increment if exists

Changes in ProjectBuildCounter 1.0.154.231
==========================================
  Added
  -----
  * Error registered in system event log (available through Event Log)
  * New parameter allowing to skip file version incrementation

  Bugs
  ----
  * bug fixed for version with less than four numbers

Changes in ProjectBuildCounter 1.0.101.1858
===========================================
  New features
  ------------

  * Added second parameter allowing to increment particular part of 
    the version - Major, Minor, Build or Release.



Changes in ProjectBuildCounter 1.0.68.718
=========================================

  New features
  ------------

  * Incrementing method made recursive avoiding multiple choices.



Changes in ProjectBuildCounter 1.0.42.1934
==========================================

  New features
  ------------

  * Record errors with the Windows Event Log
