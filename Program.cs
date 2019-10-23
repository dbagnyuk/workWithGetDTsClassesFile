#define FIRST
//#define SECOND
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ServiceModel;
using workWithGetDTsClassesFile.ISca;

namespace workWithGetDTsClassesFile
{
    class Program
    {
        static void Main(string[] args)
        {
#if FIRST
            if (args.Length == 0 || args.Length > 5)
                exitError();

            if (args.Length == 1 && args[0] == "/?")
                writeAppHelp();

            long inputedPA = 0;
            string lookedSrv = null;
            string sortTDsType = "asc";
            string sortSrvsType = "asc";
            int sortIdx = 1;

            bool writeFile = false;
            string pathToFile = null;
            FileStream fileStream = null;
            StreamWriter streamWriter = null;


            for (int i = 0; i < args.Length; i++)
            {
                if (i == 0 && args[i].Length != 12)
                    exitError();
                else if (i == 0 && !long.TryParse(args[0], out inputedPA))
                    exitError();

                if (i > 0 && !string.Equals(args[i].ToLower(), "asc") && !string.Equals(args[i].ToLower(), "desc") && !string.Equals(args[i].ToLower(), "/f"))
                {
                    foreach (char c in args[i])
                        if (!char.IsLetterOrDigit(c))
                            exitError();
                    lookedSrv = Convert.ToString(args[i]).ToUpper();
                }

                if (i > 0 && (string.Equals(args[i].ToLower(), "asc") || string.Equals(args[i].ToLower(), "desc")) && sortIdx++ == 1)
                    sortTDsType = Convert.ToString(args[i].ToLower());

                if (i > 0 && (string.Equals(args[i].ToLower(), "asc") || string.Equals(args[i].ToLower(), "desc")) && sortIdx == 2)
                    sortSrvsType = Convert.ToString(args[i].ToLower());

                if (i > 0 && args[i] == "/f")
                    writeFile = true;
            }

            Console.Clear();

            if (writeFile)
            {
                string userName = Environment.UserName;
                pathToFile = @"c:\Users\" + userName + "\\Temp\\" + inputedPA + ".txt";
                if (File.Exists(pathToFile))
                    fileStream = new FileStream(pathToFile, FileMode.Truncate);
                else
                    fileStream = new FileStream(pathToFile, FileMode.CreateNew);

                streamWriter = new StreamWriter(fileStream);
            }

#endif
#if SECOND
            long inputedPA = 277300065848;
            string lookedSrv = "TEST666";
            string sortTDsType = "asc";
            string sortSrvsType = "asc";
#endif
            ScaClient scaClient = new ScaClient("ConfigurationService_ISca", new EndpointAddress("http://msk-dev-foris:8106/SCA"));
            var scaOutput = scaClient.GetTDs(new GetTDsInput() { PANumber = inputedPA });

            PAid searchedPA = new PAid();

            searchedPA.processInputData(inputedPA, scaOutput);
            searchedPA.lookupSrv(lookedSrv);
            searchedPA.sortTDsList(sortTDsType, sortSrvsType);

            searchedPA.writePAtoConsole();
            searchedPA.writeTDwithSrvtoConsole(lookedSrv);
#if FIRST
            if (writeFile)
            {
                searchedPA.writePAtoFile(ref streamWriter);
                searchedPA.writeTDwithSrvtoFile(lookedSrv, ref streamWriter);
                streamWriter.Close();
                fileStream.Close();
                fileWritePath(pathToFile);
            }
#endif
            Console.Write("\nPress Enter for exit...");
            Console.Read();
        }

#if FIRST
        static void exitError()
        {
            Console.WriteLine("\nWrong input, please use 'tworkWithGetDTsClassesFile.exe /?' for help.");
            System.Environment.Exit(1);
        }
        static void fileWritePath(string fp)
        {
            Console.WriteLine($"\nOutput saved as \"{fp}\"");
        }
        static void writeAppHelp()
        {
            Console.WriteLine("\nUsage:\n\tworkWithGetDTsClassesFile.exe PA [/f] [Srv] [TDs sort] [Srvs sort]");
            Console.WriteLine("Input:\n\tPA - Personal Account {must be 12 digits long}.");
            Console.WriteLine("Optional:\n\tSrv       - Service code which you looking for" +
                                       "\n\t            (must not contain special symbols).");
            Console.WriteLine("\tTDs sort  - {asc|desc} sorting for Terminal Devices (default {asc}).");
            Console.WriteLine("\tSrvs sort - {asc|desc} sorting for Service codes (default {asc}).");
            Console.WriteLine("\n\t/f        - for write output into file.");
            Console.WriteLine("\n\t/? - for this help.");
            Console.WriteLine("\nExample:\n\tworkWithGetDTsClassesFile.exe 277300065848 /f TEST666 desc desc");
            System.Environment.Exit(0);
        }
#endif
    }
}