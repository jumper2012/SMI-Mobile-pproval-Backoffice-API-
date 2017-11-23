using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JrzAsp.Helper.Packager {
    internal class Program {
        private static void Main(string[] args) {
            try {
                var packagerExeLocation = Assembly.GetExecutingAssembly().Location;
                var defaultSlnDir =
                    Path.GetFullPath(Path.Combine(Path.GetDirectoryName(packagerExeLocation), "../../../"));

                Console.WriteLine();
                Console.WriteLine($"Hello! Use this program to package JrzAsp's libs for easy usage in other project.");
                Console.WriteLine($"Make sure that you've built the solution first using Visual Studio.");

                Console.WriteLine();
                Console.WriteLine($"Input JrzAsp solution dir:");
                Console.WriteLine($"Default '{defaultSlnDir}'");
                Console.Write($"> ");
                var slnDir = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(slnDir)) slnDir = defaultSlnDir;
                if (!Directory.Exists(slnDir)) {
                    Console.WriteLine("That directory doesn't exists.");
                    Exit();
                    return;
                }

                Console.WriteLine();
                Console.WriteLine($"Input target dir:");
                Console.Write("> ");
                var targetDir = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(targetDir)) {
                    Console.WriteLine("You must input target dir.");
                    Exit();
                    return;
                }

                var libDirs = Directory.EnumerateDirectories(slnDir)
                    .Where(d => d.Contains("JrzAsp.Lib.") || d.Contains("JrzAsp.Mvc.")).ToList();
                var libNames = libDirs.Select(LibName).ToList();

                if (libDirs.Count == 0) {
                    Console.WriteLine("No JrzAsp libs found in that solution dir.");
                    Exit();
                    return;
                }

                Console.WriteLine();
                Console.WriteLine($"Input lib selection mode:");
                Console.WriteLine("1). Exclude inputted");
                Console.WriteLine("2). Include inputted");
                Console.Write("> ");
                var libSelectModeStr = Console.ReadLine();
                var libSelectModeInt = int.Parse(libSelectModeStr);
                var isExcludeSelectMode = libSelectModeInt == 1;
                Console.WriteLine(isExcludeSelectMode ? "Will use EXCLUDE." : "Will use INCLUDE.");

                Console.WriteLine();
                Console.WriteLine($"Found the following libs: ");
                var num = 1;
                foreach (var ln in libNames) {
                    Console.WriteLine($"{num}). {ln}");
                    num++;
                }

                Console.WriteLine();
                Console.WriteLine(
                    $"Input lib number to {(isExcludeSelectMode ? "EXCLUDE" : "INCLUDE")} (separate by comma):");
                Console.Write("> ");
                var inputLibNums = Console.ReadLine();
                var libNums = new List<int>();
                if (!string.IsNullOrWhiteSpace(inputLibNums)) {
                    libNums.AddRange(
                        inputLibNums.Split(',', ';', ' ', '\t').Where(d => !string.IsNullOrWhiteSpace(d))
                            .Select(d => int.Parse(d.Trim()) - 1)
                    );
                }

                var packagedLibNames = new List<string>();
                var packagedLibDirs = new List<string>();
                for (var i = 0; i < libNames.Count; i++) {
                    if (isExcludeSelectMode) {
                        if (libNums.Contains(i)) continue;
                    } else {
                        if (!libNums.Contains(i)) continue;
                    }
                    packagedLibNames.Add(libNames[i]);
                    packagedLibDirs.Add(libDirs[i]);
                }
                if (packagedLibDirs.Count == 0) {
                    Console.WriteLine($"No libs to package.");
                    Exit();
                    return;
                }

                Console.WriteLine();
                Console.WriteLine($"Packager will perform packaging for the following libs:");
                foreach (var ln in packagedLibNames) Console.WriteLine($"* {ln}");
                Console.WriteLine();
                Console.WriteLine($"Proceed? (type YES or NO, defaults to NO):");
                Console.Write("> ");
                var proceed = Console.ReadLine()?.ToLower() == "yes";

                Console.WriteLine();
                if (proceed) {
                    foreach (var libDir in packagedLibDirs) {
                        var libName = LibName(libDir);
                        Console.WriteLine($"Packaging {libName} bin ...");
                        var packageLibDir = Path.Combine(targetDir, libName);
                        var libBinDir = Path.Combine(libDir, "bin");
                        if (Directory.Exists(packageLibDir)) {
                            Console.WriteLine($"| Cleanup target dir ...");
                            Directory.Delete(packageLibDir, true);
                        }
                        var libFiles = Directory.GetFiles(libBinDir, "*", SearchOption.AllDirectories);
                        var fileCount = 0;
                        foreach (var libFile in libFiles) {
                            Console.WriteLine(
                                $"| Copying to target [{fileCount + 1}/{libFiles.Length}] ... ({libFile.Replace(libBinDir, "")})");

                            var targetLibFile = libFile.Replace(libBinDir, packageLibDir);
                            var targetFileDir = Path.GetDirectoryName(targetLibFile);
                            if (!Directory.Exists(targetFileDir)) Directory.CreateDirectory(targetFileDir);
                            File.Copy(libFile, targetLibFile, true);

                            fileCount++;
                        }
                    }
                    Console.WriteLine($"Done!");
                } else {
                    Console.WriteLine("Cancelling...");
                }

                Console.WriteLine();
                Exit();
            } catch (Exception ex) {
                Console.WriteLine();
                Console.WriteLine($"------");
                Console.WriteLine($"ERROR!");
                Console.WriteLine($"------");
                WriteErrorInfo(ex);
                Exit();
            }
        }

        private static void Exit() {
            Console.WriteLine($"==========================");
            Console.WriteLine($"Press any key to exit...");
            Console.ReadKey();
        }

        private static string LibName(string libDir) {
            return libDir.Substring(libDir.LastIndexOf("\\", StringComparison.Ordinal) + 1);
        }

        private static void WriteErrorInfo(Exception ex) {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"------");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine($"------");
            if (ex.InnerException != null) WriteErrorInfo(ex.InnerException);
        }
    }
}