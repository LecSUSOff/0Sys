using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cosmos.System.FileSystem.VFS;
using Sys = Cosmos.System;
using System.Threading;

namespace _0Sys
{
    public class Kernel : Sys.Kernel
    {
        Sys.FileSystem.CosmosVFS FileSystem;
        string currentdir = @"0:\";
        int i = 0;
        string OS = "0Sys";
        string ver = "0.3";
        string kernel = "COSMOS";
        string app = "Terminal";

        protected override void BeforeRun()
        {
            Console.Beep();
            FileSystem = new Sys.FileSystem.CosmosVFS();
            Cosmos.System.FileSystem.VFS.VFSManager.RegisterVFS(FileSystem);
            Stat();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            ClearConsole();
            Console.WriteLine("0Sys booted successfully!");
            Console.Beep();
        }

        protected override void Run()
        {
            Console.Write(currentdir + ">> ");
            var input = Console.ReadLine();
            if (i >= 15)
            {
                ClearConsole();
            }
            else
            {
                i++;
            }
            Do(input);
        }
        public void Do(string cmd)
        {
            string dr = currentdir;
            string filename = "";
            string dirname = "";
            string context = "";
            switch (cmd)
            {
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unknown command; Use help to get command list");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "help":
                    if (i >= 4)
                    {
                        ClearConsole();
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Commands:" +
                        "\nshutdown         |   Turns off you PC" +
                        "\nreboot           |   Reboot your PC" +
                        "\nmkfile           |   Creates a new file with specified name" +
                        "\nmkdir            |   Creates a new directory with specified name" +
                        "\nread             |   Reads content from file" +
                        "\nwrite            |   Writes content into a file" +
                        "\ndelfile          |   Deletes file" +
                        "\ndeldir           |   Deletes directory" +
                        "\nmvfile           |   Moves a file" +
                        "\ncfile            |   Copies a file" +
                        "\ngo               |   Changes current directory to specified" +
                        "\ndir              |   Displays a list of folders and files in the directory" +
                        "\nroot             |   Changes current directory to 0:\\" +
                        "\nstat             |   Shows statistics of file system, PC and OS" +
                        "\ndatetime         |   Displays date and time" +
                        "\nclear            |   Clears display" +
                        "\nproperties(props)|   Displays properties of a file");
                    Console.ForegroundColor= ConsoleColor.White;
                    i += 16;
                    break;
                case "datetime":
                    Console.WriteLine(DateTime.Now);
                    i++;
                    break;
                case "properties" or "props":
                    try
                    {
                        Console.Write("Enter file name: ");
                        i++;
                        filename = Console.ReadLine();
                        string content = File.ReadAllText(currentdir + filename);
                        string[] fn = filename.Split(@"\");

                        Console.WriteLine("File name: " + fn[fn.Length-1]);
                        Console.WriteLine("File path: " + currentdir + filename);
                        Console.WriteLine("File size: " + content.Length + " bytes");
                        Console.WriteLine("Content: " + content);
                        i += 4;
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    break;
                case "clear":
                    ClearConsole();
                    break;
                case "stat":
                    Stat();
                    break;
                case "go":
                    Console.Write("Enter path: ");
                    currentdir = Console.ReadLine();
                    i++;
                    if (!Directory.Exists(currentdir))
                    {
                        currentdir = dr;
                        Exeption("Directory doesn't exists");
                        i++;
                    }
                    break;
                case "run":
                    Console.Write("Enter file name: ");
                    filename = Console.ReadLine();
                    try
                    {
                        RunApp(File.ReadAllText(currentdir + filename));
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    break;
                case "write":
                    Console.Write("Enter file name: ");
                    filename = Console.ReadLine();
                    app = filename;
                    ClearConsole();
                    context = EditorApp();
                    app = "Terminal";
                    ClearConsole();
                    try
                    {
                        File.WriteAllText(currentdir + filename, "");
                        string[] words = context.Split(" ");
                        foreach (string word in words)
                        {
                            switch (word) 
                            {
                                default:
                                    File.WriteAllText(currentdir + filename, File.ReadAllText(currentdir + filename) + word + " ");
                                    break;
                                case @"\n":
                                    File.WriteAllText(currentdir + filename, File.ReadAllText(currentdir + filename) + "\n");
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    break;
                case "read":
                    Console.Write("Enter file name: ");
                    filename = Console.ReadLine();
                    try
                    {
                        Console.WriteLine(File.ReadAllText(currentdir + filename));
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    i++;
                    break;
                case "mvfile":
                    Console.Write("Enter file name: ");
                    filename = Console.ReadLine();
                    Console.Write("Enter directory to move: ");
                    dirname = Console.ReadLine();
                    MoveFile(filename, dirname);
                    break;
                case "cfile":
                    try
                    {
                        Console.Write("Enter file name: ");
                        filename = Console.ReadLine();
                        Console.Write("Enter directory to move: ");
                        dirname = Console.ReadLine();
                        File.Copy(filename, dirname);
                        i += 2;
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    break;
                case "dir":
                    try
                    {
                        var directory_list = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing(currentdir);
                        foreach (var directoryEntry in directory_list)
                        {
                            try
                            {
                                var entry_type = directoryEntry.mEntryType;
                                if (entry_type == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("| [File]     |       " + directoryEntry.mName);
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                if (entry_type == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.Directory)
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine("| [Directory]|       " + directoryEntry.mName);
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                i++;
                            }
                            catch (Exception e)
                            {
                                Exeption(e.ToString());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    break;
                case "mkfile":
                    Console.Write("Enter your file name: ");
                    filename = Console.ReadLine();
                    try
                    {
                        mkFile(currentdir + @"\" + filename);
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    i++;
                    break;
                case "mkdir":
                    Console.Write("Enter your directory name: ");
                    dirname = Console.ReadLine();
                    try
                    {
                        FileSystem.CreateDirectory(currentdir + @"\" + dirname);
                        currentdir += @"\" + dirname;
                        i++;
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    break;
                case "root":
                    currentdir = @"0:\";
                    break;
                case "delfile":
                    Console.Write("Enter your file name: ");
                    filename = Console.ReadLine();
                    i++;
                    try
                    {
                        Sys.FileSystem.VFS.VFSManager.DeleteFile(currentdir + filename);
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    break;
                case "deldir":
                    Console.Write("Enter your directory name: ");
                    dirname = Console.ReadLine();
                    i++;
                    try
                    {
                        Sys.FileSystem.VFS.VFSManager.DeleteDirectory(currentdir + dirname, true);
                    }
                    catch (Exception e)
                    {
                        Exeption(e.ToString());
                    }
                    break;
                case "shutdown":
                    Cosmos.System.Power.Shutdown();
                    break;
                case "reboot":
                    Console.Clear();
                    Thread.Sleep(2000);
                    ClearConsole();
                    Console.WriteLine("0Sys booted successfully!");
                    break;
            }
        }
        public void mkFile(string path) 
        {
            FileSystem.CreateFile(path);
        }
        public void Exeption(string message)
        { 
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + message);
            i++;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void ClearConsole() 
        {
            var datetime = DateTime.Now.ToString();
            i = 0;
            Console.Clear();
            TopBar(app, OS + " " + ver, datetime);
        }
        public void Stat()
        {
            if (i >= 9)
            {
                ClearConsole();
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("OS: " + OS);
            Console.WriteLine("OS Version: " + ver);
            Console.WriteLine("OS Kernel: " + kernel);
            var available_space = FileSystem.GetAvailableFreeSpace(@"0:\");
            Console.WriteLine("Available Disks: " + VFSManager.GetDisks());
            Console.WriteLine("Available Free Space: " + available_space + " bytes");
            var fs_type = FileSystem.GetFileSystemType(@"0:\");
            Console.WriteLine("File System Type: " + fs_type);
            uint RAM = Cosmos.Core.CPU.GetAmountOfRAM();
            uint av_RAM = (uint)Cosmos.Core.GCImplementation.GetAvailableRAM();
            uint u_RAM = RAM - av_RAM;
            string CPUBrand = Cosmos.Core.CPU.GetCPUBrandString();
            string CPUVendor = Cosmos.Core.CPU.GetCPUVendorName();
            Console.WriteLine("Amount of RAM: " + RAM + "MB");
            Console.WriteLine("Available RAM: " + av_RAM + "MB");
            Console.WriteLine("Used RAM: " + u_RAM + "MB (" + u_RAM / RAM * 100 + "%)");
            Console.WriteLine("CPU: " + CPUBrand);
            Console.WriteLine("CPU Vendor: " + CPUVendor);
            Console.ForegroundColor= ConsoleColor.White;
            i += 11;
        }
        public static void MoveFile(string file, string newpath)
        {
            try
            {
                File.Copy(file, newpath);
                File.Delete(file);
            }
            catch (Exception e)
            {
                Console.ForegroundColor =  ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        static void TopBar(string left, string middle, string right)
        {
            int totalLength = 80;
            string result = FormatTopBar(left, middle, right, totalLength);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(result);
            Console.ResetColor();
        }

        static string FormatTopBar(string left, string middle, string right, int totalLength)
        {
            int leftPadding = 0;
            int rightPadding = 0;

            int leftLength = left.Length;
            int middleLength = middle.Length;
            int rightLength = right.Length;

            int middleStartPos = (totalLength - middleLength) / 2;

            leftPadding = middleStartPos - leftLength;
            rightPadding = totalLength - middleStartPos - middleLength - rightLength;

            if (leftPadding < 0)
            {
                left = left.Substring(0, middleStartPos);
                leftPadding = 0;
            }

            if (rightPadding < 0)
            {
                right = right.Substring(0, totalLength - middleStartPos - middleLength);
                rightPadding = 0;
            }

            return left.PadRight(left.Length + leftPadding) +
                   middle +
                   right.PadLeft(right.Length + rightPadding);
        }
        public string EditorApp()
        {
            Console.Write(">> ");
            string context = Console.ReadLine();
            if (context.EndsWith(@"\n"))
            {
                context += " " + EditorApp();
            }
            return context;
        }
        public void RunApp(string content)
        {
            string[] lines = content.Split("\n");
            foreach (string line in lines)
            {
                string[] prts = line.Split(" ");
                switch (prts[0].ToLower())
                {
                    default:
                        Do(prts[0].Trim()); break;
                    case "appname":
                        app = prts[1];
                        break;
                    case "run":
                        ClearConsole();
                        break;
                    case "clear":
                        switch (prts[1])
                        {
                            case "0":
                                Console.Clear();
                                break;
                            case "1":
                                ClearConsole();
                                break;
                        }
                        break;
                    case "write":
                        int n = 1;
                        while (n < prts.Length - 1)
                        {
                            Console.Write(prts[n] + " ");
                            n++;
                        }
                        break;
                    case "newline":
                        Console.WriteLine();
                        break;
                    case "read":
                        Console.ReadLine();
                        break;
                    case "file":
                        mkFile(prts[1]);
                        break;
                    case "err":
                        Exeption(prts[1]);
                        break;
                    case "note":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(prts[1]);
                        Console.ForegroundColor= ConsoleColor.White;
                        break;
                    case "exit":
                        app = "Terminal";
                        ClearConsole();
                        break;
                }
            }
        }
    }
}
