/*
 * Created by SharpDevelop.
 * User: ivo
 * Date: 25.3.2013 ã.
 * Time: 19:24 ÷.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectBuildCounter
{
  class Program
  {
    /// <summary>
    /// This function will incriment versions. Builds are counted from 1 to 9999, Minor: from 1 to 99
    /// </summary>
    /// <param name="args">
    /// Command Line Arguments:
    /// 0 - Path to application manifest (.exe.manifest).
    ///</param>
    static int Main(string[] args)
    {
      #region [args[] check & test]
      if (args.Length < 1)
      {
        args = new string[1];
        args[0] = "AssemblyInfo.cs";  // asume file is in the same folder
      }
      if (!File.Exists(args[0]))
      {
        Console.Write("File [{0}] not found", args[0]);
        return -1;
      }
      #endregion
      #region open, read and save cs file
      try
      {			//get and read all lines
        String[] contents = File.ReadAllLines(args[0], Encoding.UTF8);
        for (int x = 0; x < contents.Length; x++)
        {
          if (!contents[x].Contains("AssemblyVersion") && !contents[x].Contains("AssemblyFileVersion")) { continue; }  
          contents[x] = contents[x].Replace('*', '0');  // some files have * instead of build figure
          String sNums = Regex.Replace(contents[x], @"[^\d\.\*]", ""); // get version numbers
          contents[x] = contents[x].Replace(sNums, Increment(sNums)); // replace old with new version
        }
        File.WriteAllLines(args[0], contents, Encoding.UTF8); // save file
      }
      catch (Exception e)
      {
        TextWriter errWriter = Console.Error;
        EventLog evlog = new EventLog("Application", Environment.MachineName, AppDomain.CurrentDomain.FriendlyName);
        errWriter.WriteLine("(Reading File) Exception thrown: " + e);
        evlog.WriteEntry("(Reading File) Exception thrown: " + e.Message, EventLogEntryType.Error);
        return -2;
      }
      #endregion
      return 0;
    }

    private static string Increment( String sNums )
    {
      List<Int32> iNums = sNums.Split(new char[] { '.' }).ToList<String>().ConvertAll(el => Convert.ToInt32(el)); // split for easier manipulation
      for (int i = iNums.Count; i < 4; i++) { iNums.Add(0); } // add lacking versions - There must be 4 numbers: Major, Minor, Build, Release
      iNums[2] += 1;
      if (iNums[2] > 9999) // build version
      {
        iNums[2] = 1;
        iNums[1] += 1;
        if (iNums[0] > 99) // Minor version
        {
          iNums[1] = 1;
          iNums[0] += 1;
        }
      }
      return String.Join(".", iNums.ConvertAll(el => el.ToString()).ToArray<String>());
    }
  }
}