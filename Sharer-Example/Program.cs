using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using Sharer;

namespace Sharer_Example
{
    class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("Sharer_Example build 11");
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
                string _portName = String.Empty; //Serial port
                do
                {
                    for (int i = 1; i <= _listPorts.Count; i++)
                    {
                        Console.WriteLine(String.Format("{0}. {1}", i, _listPorts[i-1]));
                    }
                    Console.Write("Select the COM-port number and press Enter: ");
                    var _strSelectNumberPort = Console.ReadLine();
                    Console.WriteLine();
                    int _intSelectNumberPort;
                    if (int.TryParse(_strSelectNumberPort, out _intSelectNumberPort))
                    {
                        if(_intSelectNumberPort< _listPorts.Count+1) 
                            _portName = _listPorts[_intSelectNumberPort - 1];
                    }
                } while (_portName == String.Empty);
                // Connect to Arduino board
                Console.WriteLine("Select Port: "+ _portName);
                var connection = new SharerConnection(_portName, 115200);
                //       
                connection.Ready += _connection_Ready;
                connection.InternalError += _connection_InternalError;
                Console.WriteLine("Connect...");
                connection.Connect();
                // Scan all functions shared
                if (connection != null && connection.Connected)
                {
                    //Only required for Linux
                    Task.Delay(2000).Wait(); // Wait 2 seconds with blocking
                    connection.RefreshFunctions();
                    //Only required for Linux
                    Task.Delay(2000).Wait(); // Wait 2 seconds with blocking
                    connection.RefreshVariables();
                }
                else
                {
                    Console.WriteLine("No Connection");
                    Environment.Exit(-1);
                }
                int _intSelectTask=0;
                //
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
                                // Type : float
                                float _floatTemperature = (float)Convert.ToDouble(_objTemperature);
                                // Display the result
                                Console.WriteLine("Value : " + _floatTemperature);
                                Console.WriteLine();
                                break;
                            case 5:
                                Console.WriteLine("Exit App");
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
                if (connection != null)
                {
                    //Only required for Linux
                    Task.Delay(2000).Wait(); // Wait 1 seconds with blocking
                    connection.Disconnect();
                }
                return 0;
            }
            catch (Exception ex)
            {
                    handleException(ex);
                    return -1;
            }
        }
        private static void handleException(Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            Console.WriteLine("Exit App");
        }
        private static void _connection_InternalError(object o, ErrorEventArgs e)
        {
            Console.WriteLine("Internal error: " + e.Exception.ToString());
            Console.WriteLine("Exit App");
            Environment.Exit(-1);
        }
        private static void _connection_Ready(object sender, EventArgs e)
        {
            //Ready
            Console.WriteLine("Event Ready");
        }
    }
}
