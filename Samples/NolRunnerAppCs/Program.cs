using System;
using BossaNolRunner;

namespace NolRunnerAppCs
{
    class Program
    {
        static void Main(string[] args)
        {
            // NolRunner.startNolWithArgs(args);                    

            var arguments = NolRunner.parseArgs(args);
            NolRunner.startNol(arguments.Item1, arguments.Item2, arguments.Item3);
            
            Console.WriteLine("Press any key to continue..."); 
            Console.ReadLine();    
            NolRunner.stopNol();
        }
    }
}
