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
      String sLine;
      StringBuilder sb = new StringBuilder();
      #region [args[] check & test]
      if (args.Length < 1)
      {
        args = new string[1];
        args[0] = "AssemblyInfo.cs";
      }  // asume file is in the same folder
      if (!File.Exists(args[0]))
      {	//exists the file?
        Console.Write("File [{0}] not found", args[0]);
        return -2;
      }
      #endregion
      #region open, read and save cs file
      try
      {			//get and read all lines
        using (StreamReader reader = new StreamReader(args[0], System.Text.Encoding.UTF8))
        {
          while ((sLine = reader.ReadLine()) != null)
          {
            if (!sLine.Contains("AssemblyVersion") && !sLine.Contains("AssemblyFileVersion"))
            {
              sb.AppendLine(sLine);
              continue;
            }
            //String sNums = (new Regex(@"[^\d\.\*]")).Replace(sLine.Replace('*', '0'), ""); // get version numbers
            sLine = sLine.Replace('*', '0');
            String sNums = Regex.Replace(sLine, @"[^\d\.\*]", ""); // get version numbers
            List<String> iNums = sNums.Split(new char[] { '.' }).ToList<String>(); // Regex.Split(sNums, ".");
            if (iNums.Count < 4) 
            {
              for (int x = iNums.Count; x < 4; x++) {  iNums.Add("0"); }
            }
            iNums[2] = (Convert.ToInt32(iNums[2]) + 1).ToString();
            if (Convert.ToInt32(iNums[2]) > 9999)
            {
              iNums[2] = "1";
              iNums[1] = (Convert.ToInt32(iNums[1]) + 1).ToString();
              if (Convert.ToInt32(iNums[0]) > 99)
              {
                iNums[1] = "1";
                iNums[0] = (Convert.ToInt32(iNums[0]) + 1).ToString();
              }
            }
            sb.AppendLine(sLine.Replace(sNums, String.Join(".", iNums)));
          }
        }
        using (StreamWriter sr = new StreamWriter(new FileStream(args[0], FileMode.OpenOrCreate), System.Text.Encoding.UTF8))
        {
          sr.WriteLine(sb.ToString());
        }
      #endregion
      }
      catch (Exception ex)
      {
        Console.Write("Error: {0}", ex.Message);
        return -4;
      }
      return 0;
    }
  }
}