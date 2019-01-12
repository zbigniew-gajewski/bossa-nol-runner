using System;
using BossaNolRunner;

namespace NolRunnerAppCs
{
    class Program
    {
        static void Main(string[] args)
        {
            NolRunner.startNol(args);
    
            Console.WriteLine("Press any key to continue..."); 
            Console.ReadLine();    

            NolRunner.stopNol();
        }
    }
}
