using System;
using System.Runtime.InteropServices;
using RJCP.IO.Ports;

namespace NetCoreSerialTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            String portAName;
            String portBName;
            if (args.Length == 2)
            {
                portAName = args[0];
                portBName = args[1];
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                portAName = "COM5";
                portBName = "COM7";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                portAName = "/dev/ttyUSB0";
                portBName = "/dev/ttyUSB1";
            }
            else
            {
                Console.WriteLine("Unsupported platform detected.");
                Console.ReadLine();
                return;
            }

            using (var portA = new SerialPortStream(portAName, 9600, 8, Parity.None, StopBits.One))
            {
                using (var portB = new SerialPortStream(portBName, 9600, 8, Parity.None, StopBits.One))
                {
                    portA.Open();
                    portB.Open();

                    Console.WriteLine("Ports opened!");

                    const Int32 numBites = 5;
                    var bites = new Byte[numBites];
                    
                    if (!portA.WriteAsync(new Byte[] {0x01, 0x02, 0x03, 0x04, 0x05}, 0, numBites).Wait(TimeSpan.FromSeconds(10)))
                    {
                        Console.WriteLine("Write failed!");
                        return;
                    }

                    Console.WriteLine("Write succeeded!");

                    Int32 readBytes = 0;
                    while (readBytes < numBites)
                    {
                        Int32 bytesRead = portB.Read(bites, readBytes, numBites - readBytes);
                        if (bytesRead <= 0) break;
                        readBytes += bytesRead;
                    }

                    Console.WriteLine(readBytes == numBites ? "The read succeeded." : "The read failed.");
                    Console.WriteLine($"{readBytes} bytes were read.");
                    Console.WriteLine(String.Join(", ", bites));
                }
            }
        }
    }
}
