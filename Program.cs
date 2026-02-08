using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CRC_32_CLI {
    internal class Program {
        static void Main(string[] args) {

            if (args.Length == 0 || (args.Length == 1 && args[0] == "--help")) {

                showHelp();

            } else {
                parseInput(args);
            }

        }

        static void parseInput(string[] args) {

            // input: text or file name
            // direction: "forward" or "reverse"
            Dictionary<string, string> inputs = new Dictionary<string, string>();

            int i = 0;

            while (i < args.Length - 1) {
                string flag = args[i].ToLower();
                string value = args[i + 1];

                if (flag == "--text" || flag == "-t") {
                    if (inputs.ContainsKey("input")) {
                        // error: input already set
                        showHelp();
                        return;
                    }
                    inputs.Add("input", value);
                    i += 2;

                } else if (flag == "--file" || flag == "-t") {
                    if (inputs.ContainsKey("input")) {
                        // error: input already set
                        showHelp();
                        return;
                    }
                    inputs.Add("input", "./" + value);
                    i += 2;

                } else if (flag == "--direction" || flag == "-d") {
                    if (inputs.ContainsKey("direction")) {
                        // error: input already set
                        showHelp();
                        return;
                    }

                    if (!(value == "forward" || value == "reverse")) {
                        // error: direction not a valid value
                        showHelp();
                        return;
                    }
                    inputs.Add("direction", value);
                    i += 2;

                } else if (flag == "--hex" || flag == "-h") {
                    if (inputs.ContainsKey("view")) {
                        // error: view already set
                        showHelp();
                        return;
                    }

                    inputs.Add("view", value);
                    i += 1;

                } else if (flag == "--dec" || flag == "-d") {
                    if (inputs.ContainsKey("view")) {
                        // error: view already set
                        showHelp();
                        return;
                    }

                    inputs.Add("view", value);
                    i += 1;

                } else {
                    showHelp();
                    return;
                }
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

                if (inputs["direction"] == "reverse") {
                    // reverse polynomial, LSB -> MSB
                    // (used in modern x86, serial comms, Gzip, Python, Go)
                    crc = CRC32String(data, 0xEDB88320, true);
                } else {
                    // forward "normal" polynomial, MSB -> LSB (used in MPEG2)
                    crc = CRC32String(data, 0x04C11DB7, false); // BZIP2, Ethernet
                }

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


        static uint nextCRC(uint b, uint crc, uint divisor, bool isReverse, uint[] lookup) {
            if (isReverse) {
                // fast crc based on shifting and lookup table
                return lookup[(crc ^ b) & 0xFF] ^ (crc >> 8);
            } else {
                uint index = ((crc >> 24) ^ b) & 0xFF;
                return (crc << 8) ^ lookup[index];
            }
        }


        static uint CRC32File(string fileName, uint divisor, bool isReverse) {
            uint crc = 0xFFFFFFFF;

            try {
                // Use 'using' statement to ensure the FileStream is correctly disposed
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                    int byteData;
                    uint[] crcTable = new uint[256];

                    // precompute crc table
                    for (uint i = 0; i < 256; i++) {
                        crcTable[i] = CRCByte(i, divisor, isReverse);
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

        static uint CRC32String(byte[] data, uint divisor, bool isReverse) {
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


        static uint CRCByte(uint input, uint divisor, bool isReverse) {

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


        static void showHelp() {
            Console.WriteLine("Usage: CRC-32 --input \"ABC\"");
        }
    }
}
