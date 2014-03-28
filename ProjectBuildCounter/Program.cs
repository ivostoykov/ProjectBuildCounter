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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectBuildCounter
{
  class Program
  {
    static List<String> _args;
    static int startVerNum = 3;
    static bool _noFileVer = false; // if true - file version won't be incremented
    /// <summary>
    /// This function will incriment versions. Builds are counted from 1 to 9999, Minor: from 1 to 99
    /// </summary>
    /// <param name="args">
    /// Command Line Arguments:
    /// 0 - Path to AssemblyInfo.cs.
    /// 1 - version to start incrementation from
    /// 2 - nofile - indicating to skip file version incrementation
    ///</param>
    static int Main(string[] args)
    {
      #region [args[] check & test]
      if(args.Length == 0) {  return 0;  }
      InputInit(args);
      if (!_args[0].Contains("AssemblyInfo.cs")) { _args[0] = Path.Combine(_args[0], "AssemblyInfo.cs"); } // only folder name supplied
      if (!File.Exists(_args[0]))
      {
        EventLog evlog = new EventLog("Application", Environment.MachineName, AppDomain.CurrentDomain.FriendlyName);
        evlog.WriteEntry(String.Format("File [{0}] not found", _args[0]), EventLogEntryType.Error);
        Console.Write("File [{0}] not found", _args[0]);
        return -1;
      }
      #endregion
      #region open, read and save cs file
      try   
      {
        String[] contents = File.ReadAllLines(_args[0], Encoding.UTF8);
        for (int x = 0; x < contents.Length; x++)
        {
          if (!contents[x].Contains("AssemblyVersion") && !contents[x].Contains("AssemblyFileVersion") && !contents[x].Contains("AssemblyInformationalVersion")) { continue; }
          contents[x] = ChangeVersion(contents[x]);
        }
        File.WriteAllLines(_args[0], contents, Encoding.UTF8); // save file
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

    /// <summary>
    /// check input and fill missing arguments
    /// </summary>
    /// <param name="args">command line input arguments</param>
    private static void InputInit(string[] args)
    {
      _args = args.ToList<String>();
      if (_args.Count < 1) { _args.Add("AssemblyInfo.cs"); }
      if (_args.Count == 1) { _args.Add("R"); } // default version number to start incrementation from
      if(_args.Count == 3) { _noFileVer = _args[2].ToLower().Equals("nofile");  } // if 3rd parameter supplied must be 'nofile'
      switch(_args[1])
      {
        case "M": startVerNum = 0; break;
        case "m": startVerNum = 1; break;
        case "b":
        case "B": startVerNum = 2; break;
        default:  startVerNum = 3; break; // default number to incrementat is Release
      }
    }

    /// <summary>
    /// Transforms version string into <seealso cref="System.Int32"/> increment numbers and return back the new numbers as a string
    /// </summary>
    /// <param name="str">Current version number as a <see cref="System.String"/></param>
    /// <returns><see cref="System.String"/> with the new version number</returns>
    private static String ChangeVersion(String str)
    {
      if(_noFileVer && str.Contains("AssemblyFileVersion")) {  return str;  }  // flag prohibiting file version incrementation is set
      str = str.Replace('*', '0');  // some files have * instead of build figure
      String sNums = Regex.Replace(str, @"[^\d\.]", ""); // get version numbers
      int[] v = Array.ConvertAll<String, Int32>(sNums.Split(new [] { '.' }, StringSplitOptions.RemoveEmptyEntries), e => Convert.ToInt32(e));
      Increment(ref v, startVerNum);
      return str.Replace(sNums, String.Join(".", Array.ConvertAll<Int32, String>(v, e => e.ToString()))); // replace old with new version
    }

    /// <summary>
    /// Recursively increment version mubers starting from right to left.
    /// </summary>
    /// <param name="ver">array with versions. Zero indexed is Major. Max indexed (3) is Release</param>
    /// <param name="idx">Starting number - from bigger to smaller</param>
    private static void Increment(ref int[] ver, int idx)
    {
      int maxVal = Convert.ToInt32(new String('9', idx + 1));
      try 
      {
        if (ver[idx] + 1 > maxVal)
        {
          Increment(ref ver, idx - 1);
          ver[idx] = 1;
          return;
        }
        ver[idx]++;
      } catch(Exception e)
      {
        try {
//          EventLog evlog = new EventLog("Application", Environment.MachineName, AppDomain.CurrentDomain.FriendlyName);
//          evlog.WriteEntry("Exception thrown: " + e.Message, EventLogEntryType.Error);
          (new EventLog("Application", Environment.MachineName, AppDomain.CurrentDomain.FriendlyName)).WriteEntry("Exception thrown: " + e.Message, EventLogEntryType.Error);
        } catch (Exception ex) {      }
        return;
      }
    }
  }
}