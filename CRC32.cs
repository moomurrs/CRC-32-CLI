using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CRC_32_CLI {
    internal class CRC32 {
        private static uint nextCRC(uint b, uint crc, uint divisor, bool isReverse, uint[] lookup) {
            if (isReverse) {
                // fast crc based on shifting and lookup table
                return lookup[(crc ^ b) & 0xFF] ^ (crc >> 8);
            } else {
                uint index = ((crc >> 24) ^ b) & 0xFF;
                return (crc << 8) ^ lookup[index];
            }
        }

        static private uint CRCByte(uint input, uint divisor, bool isReverse) {

            if (isReverse) {
                for (int k = 8; k > 0; k--) {
                    if ((input & 1) == 1) {
                        input = (input >> 1) ^ divisor;
                    } else {
                        input >>= 1;
                    }
                }
            } else {
                input <<= 24;

                for (int k = 8; k > 0; k--) {
                    // check the most significant bit
                    if ((input & 0x80000000) != 0) {
                        input = (input << 1) ^ divisor;
                    } else {
                        input <<= 1;
                    }
                }
            }

            return input;
        }

        private static uint CRC32File(string fileName, uint divisor, bool isReverse) {
            uint crc = 0xFFFFFFFF;

            try {
                // Use 'using' statement to ensure the FileStream is correctly disposed
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                    int byteData;
                    uint[] crcTable = new uint[256];

                    // precompute crc table
                    for (uint i = 0; i < 256; i++) {
                        crcTable[i] = CRC32.CRCByte(i, divisor, isReverse);
                    }

                    // read a byte
                    // the returned value is not -1 (end of file)
                    while ((byteData = fileStream.ReadByte()) != -1) {

                        byte b = (byte)byteData;

                        crc = nextCRC(b, crc, divisor, isReverse, crcTable);
                    }
                }
            } catch (FileNotFoundException) {
                Console.WriteLine($"Error: The file '{fileName}' was not found.");
            } catch (Exception ex) {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // final mask
            crc ^= 0xFFFFFFFF;

            return crc;
        }

        private static uint CRC32String(byte[] data, uint divisor, bool isReverse) {
            int length = data.Length;

            uint[] crcTable = new uint[256];

            // precompute crc table
            for (uint i = 0; i < 256; i++) {
                crcTable[i] = CRCByte(i, divisor, isReverse);
            }

            uint crc = 0xFFFFFFFF;

            // fast crc based on shifting and lookup table
            for (uint i = 0; i < data.Length; i++) {
                crc = nextCRC(data[i], crc, divisor, isReverse, crcTable);
            }

            // final mask
            crc ^= 0xFFFFFFFF;

            return crc;
        }

        public static void showHelp() {
            const string usage = @"NAME
CRC-32 CLI - Compute CRC-32 checksum

Synopsis
CRC-32 <FILE_INPUT> [-][SWITCH] <STRING_INPUT>

Description
A commandline interface for generating IEEE CRC-32 on a given file or string.

Switches
--text or -t
    Compute checksum on a string input. 
    Default is file-mode if this switch is not specified.

--forward or -f
    Compute the check using a forward divisor. 
    Default is reverse divisor if this switch is not specified.

--decimal or -d
    Output the checksum in decimal format. 
    Default is hexidecimal if this switch is not specified.


Example 1
CRC-32 -t ""ABC""
    Compute the checksum on string ""ABC"" using the default reverse divisor. 
    Show the checksum in default hexidecimal.

Example 2
CRC-32 file.mp3 -f
    Compute the checksum on file ""file.mp3"" using the forward divisor.
    Show the checksum in default hexidecimal.

Example 3
CRC-32 -t ""ABC"" -f -d
    Compute the checksum on string ""ABC"" using the forward divisor. 
    Show the checksum in decimal.";
            Console.WriteLine(usage);
        }

        public static void run(string[] args) {
            if (args.Length == 0 || (args.Length == 1 && (args[0] == "--help" || args[0] == "-h"))) {

                CRC32.showHelp();

            } else {
                CRC32.parseInput(args);
            }
        }


        private static void parseInput(string[] args) {

            // input: text or file name
            // direction: "forward" or "reverse"
            // view: "hex" or "dec"
            Dictionary<string, string> inputs = new Dictionary<string, string>();

            int i = 0;

            while (i < args.Length) {
                string sw = args[i].ToLower();

                if (i == 0 && !(sw == "--text" || sw == "-t" || sw == "--forward" || sw == "-f" || sw == "--decimal" || sw == "-d")) {
                    // assuming file mode
                    string fileName = sw;

                    inputs.Add("input", "./" + fileName);
                    i += 1;
                } else if (sw == "--text" || sw == "-t") {
                    // string mode specified
                    if (inputs.ContainsKey("input")) {
                        // error: input already set
                        showHelp();
                        return;
                    }
                    string value = args[i + 1];
                    inputs.Add("input", value);
                    i += 2;

                } else if (sw == "--forward" || sw == "-f") {
                    // forward divisor specified
                    if (inputs.ContainsKey("direction")) {
                        // error: input already set
                        showHelp();
                        return;
                    }

                    inputs.Add("direction", "forward");
                    i += 1;

                } else if (sw == "--decimal" || sw == "-d") {
                    // view decimal specified
                    if (inputs.ContainsKey("view")) {
                        // error: view already set
                        showHelp();
                        return;
                    }

                    inputs.Add("view", "decimal");
                    i += 1;

                } else {
                    // error: no flag match
                    showHelp();
                    return;
                }
            }

            if (!(inputs.ContainsKey("input"))) {
                // error: input direction not specified
                showHelp();
                return;
            }


            if (!(inputs.ContainsKey("direction"))) {
                // if direction not specified, use default "reverse"
                inputs.Add("direction", "reverse");
            }

            if (!(inputs.ContainsKey("view"))) {
                // if view not specified, use default hex
                inputs.Add("view", "hex");
            }

            if (inputs["input"][0] != '.') {
                // text mode

                byte[] data = Encoding.UTF8.GetBytes(inputs["input"]);
                uint crc;

                // select divisor orientation
                if (inputs["direction"] == "reverse") {
                    // reverse polynomial, LSB -> MSB
                    // (used in modern x86, serial comms, Gzip, Python, Go)
                    crc = CRC32String(data, 0xEDB88320, true);
                } else {
                    // forward "normal" polynomial, MSB -> LSB (used in MPEG2)
                    crc = CRC32String(data, 0x04C11DB7, false); // BZIP2, Ethernet
                }

                // select output format (hex or dec)
                if (inputs["view"] == "hex") {
                    Console.WriteLine("0x{0:X}", crc);
                } else {
                    Console.WriteLine(crc);
                }

            } else {
                // file mode

                string data = inputs["input"];
                uint crc;

                if (inputs["direction"] == "reverse") {
                    // reverse polynomial, LSB -> MSB
                    // (used in modern x86, serial comms, Gzip, Python, Go)
                    crc = CRC32File(data, 0xEDB88320, true);
                } else {
                    // forward "normal" polynomial, MSB -> LSB (used in MPEG2)
                    crc = CRC32File(data, 0x04C11DB7, false); // BZIP2, Ethernet
                }

                if (inputs["view"] == "hex") {
                    Console.WriteLine("0x{0:X}", crc);
                } else {
                    Console.WriteLine(crc);
                }
            }
        }
    }
}
