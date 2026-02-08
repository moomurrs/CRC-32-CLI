
using System;

namespace Main_Driver {
    internal class Driver {
        static void Main(string[] args) {
            string ans = CRC_32_CLI.CRC32.Run(args);
            Console.ReadKey();
        }
    }
}
