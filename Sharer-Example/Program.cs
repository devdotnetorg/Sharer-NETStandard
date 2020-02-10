using System;
using System.Collections.Generic;
using System.IO.Ports;
using Sharer;

namespace Sharer_Example
{
    class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("Sharer_Example");
            Console.WriteLine("Connect Arduino to PC");
            Console.WriteLine("[Press any key to continue...]");
            Console.ReadKey();
            Console.WriteLine();
            List<string> _listPorts = new List<string>();
            foreach (var portName in SerialPort.GetPortNames())
            {
                _listPorts.Add(portName);
            }
            Console.WriteLine("Serial ports available:");
            if (_listPorts.Count == 0)
            {
                Console.WriteLine("No Serial Port Available");
                Environment.Exit(-1);
            }
            //Check-A
            for (int i = 1; i <= _listPorts.Count; i++)
            {
                Console.WriteLine(String.Format("{0}. {1}", i, _listPorts[0]));
            } 
            Console.Write("Select the COM-port number and press Enter: ");
            var _strSelectNumberPort=Console.ReadLine();
            Console.WriteLine();
            int _intSelectNumberPort;
            string _portName = String.Empty;
            if (int.TryParse(_strSelectNumberPort, out _intSelectNumberPort))
            {
                _portName = _listPorts[_intSelectNumberPort-1];
            }
            // Connect to Arduino board
            var connection = new SharerConnection(_portName, 115200);
            connection.Connect();
            // Scan all functions shared
            connection.RefreshFunctions();
            int _intSelectTask=0;
            do
            {
                //Select Task
                Console.WriteLine("Tasks:");
                Console.WriteLine("1. The sum of two numbers A = 10 B = 12");
                Console.WriteLine("2. Turn on LED");
                Console.WriteLine("3. Turn off LED");
                Console.WriteLine("4. Get temperature value from DS18B20 sensor");
                Console.WriteLine("5. Exit");
                Console.Write("Select a task number and press Enter: ");
                var _strSelectTask = Console.ReadLine();
                Console.WriteLine();
                if (int.TryParse(_strSelectTask, out _intSelectTask))
                {
                    switch (_intSelectTask)
                    {
                        case 1:
                            // remote call function on Arduino and wait for the result
                            var result = connection.Call("Sum", 10, 12);
                            // Display the result
                            Console.WriteLine("Status : " + result.Status);
                            Console.WriteLine("Type : " + result.Type);
                            Console.WriteLine("Value : " + result.Value);
                            // Status : OK
                            // Type : int
                            // Value : 22
                            Console.WriteLine();
                            break;
                        case 2:
                            connection.Call("setLed", true);
                            Console.WriteLine("LED ON");
                            Console.WriteLine();
                            break;
                        case 3:
                            connection.Call("setLed", false);
                            Console.WriteLine("LED OFF");
                            Console.WriteLine();
                            break;
                        case 4:
                            // remote call function on Arduino and wait for the result
                            var _objTemperature = connection.Call("getTemperature").Value.ToString();
                            float _floatTemperature = (float)Convert.ToDouble(_objTemperature);
                            // Display the result
                            Console.WriteLine("Value : " + _floatTemperature);
                            // Type : float
                            Console.WriteLine();
                            break;
                        case 5:
                            Environment.Exit(0);
                            break;
                        default:
                            {
                                Console.WriteLine("Other number");
                                break;
                            }
                    }
                }
            } while (_intSelectTask!=5);
            //
            return 0;
        }
    }
}
