using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Text;

namespace BarcodeScannerRx
{
    static class Program
    {
        private static readonly ISubject<char> UserInput = new Subject<char>();

        static void Main(string[] args)
        {
            Console.WriteLine("Barcode reader test:");
            Console.WriteLine("Type characters to process, press enter when you are done.\n");

            UserInput
                .ToBarcodeReadings()
                .Subscribe(WriteOutput);

            PumpUserInput();
        }

        private static void PumpUserInput()
        {
            while(true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    UserInput.OnCompleted();
                    break;
                }

                UserInput.OnNext(key.KeyChar);
            }
        }

        static void WriteOutput(string output)
        {
            Console.WriteLine("\nReading: {0}\n", output == string.Empty ? "string.Empty" : output);
        }
    }
}
