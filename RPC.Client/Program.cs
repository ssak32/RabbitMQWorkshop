﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPC.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Rabbitmq Message Sender");
            Console.WriteLine();
            Console.WriteLine();

            var messageCount = 0;
            var sender = new RabbitSender();

            Console.WriteLine("Press [enter] key to send a message");
            while (true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Q)
                    break;

                if (key.Key == ConsoleKey.Enter)
                {
                    var message = string.Format("Message: {0}", messageCount);
                    Console.WriteLine(string.Format("Sending - {0}", message));

                    var response = sender.Send(message, new TimeSpan(0, 0, 5, 0));
                    Console.WriteLine(string.Format("Response - {0}", response));
                    messageCount++;
                }
            }

            Console.ReadLine();
        }
    }
}
