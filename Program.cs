using System;
using System.IO;

namespace XlfMagician
{
    class Program
    {
        public string SourceXlfPath { get; set; }
        public string TargetXlfPath { get; set; }
        static void Main(string[] args)
        {
            (string sourceXlfPath, string targetXlfPath, bool isStrict) parsedArgs = ParseArgs(args);

            var magician = new Magician()
            {
                SourceXlfPath = parsedArgs.sourceXlfPath,
                TargetXlfPath = parsedArgs.targetXlfPath
            };
            var result = magician.XlfMerge(parsedArgs.isStrict);
            Console.WriteLine(
                "\n\n" 
                + "Strictly matching nodes count: " + result.strictChangeCount
                + "\n"
                + "Loosely matching nodes count: " + result.looseChangeCount
            );;
            Console.WriteLine("Output file can be found at: " + result.resultFilePath);

        }

        private static (string sourceXlfPath, string targetXlfPath, bool isStrict) ParseArgs(string[] args) {
            try { 
                if ((args[0] != "--strict" || args[0] != "--loose") && args[1] != "--source" && args[3] != "--target")
                {
                    Console.WriteLine("invalid arguments");
                    throw new ArgumentException("invalid arguments: provide valid arguments in the following format: magician.exe --source ./filename --target ./filename2 ");
                } else
                {
                    if (args[0] == "--loose")
                        return (args[2], args[4], false);
                    else
                        return (args[2], args[4], true);
                } 
            }
            catch
            {
                throw new ArgumentException("too few arguments: please provide valid arguments in the following format: magician.exe --strict/loose --source ./filename --target ./filename2 ");
            }

        }
    }
}
