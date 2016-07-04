using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerQueue.ConfigureRabbitMQ
{
    class Program
    {
        #region Constant Strings
        const string _queueName = "WSQueue.WorkerQueue";
        const string _exchange = "WSExchange.WorkerQueue";
        const string _hostName = "localhost";
        const string _userName = "guest";
        const string _password = "guest";
        #endregion
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Workshop RabbitMQ management!");
            while (true)
            {
                Console.WriteLine("Please select:");
                Console.WriteLine(" [1] Create Exchange & Queue");
                Console.WriteLine(" [2] RemoveExchange & Queue");

                ConsoleKeyInfo key = Console.ReadKey();

                if (Char.IsNumber(key.KeyChar))
                {
                    Int32 number = Int32.Parse(key.KeyChar.ToString());
                    Console.WriteLine();

                    switch (number.ToString())
                    {
                        case "1": CreateWorkshopExchangeAndQueue(); break;
                        case "2": RemoveWorkshopExchangeAndQueue(); break;
                        default:
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        private static void CreateWorkshopExchangeAndQueue()
        {
            var factory = new ConnectionFactory() { HostName = _hostName, UserName = _userName, Password = _password };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                #region Configuring Exchange & Queue. Skips if both already exists.
                Console.ForegroundColor = ConsoleColor.Green;

                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                Console.WriteLine("Created Queue : " + _queueName);

                channel.ExchangeDeclare(_exchange, ExchangeType.Fanout);
                Console.WriteLine("Created Exchange : " + _exchange);

                channel.QueueBind(_queueName, _exchange, "");
                Console.WriteLine("Binding Exchange '" + _exchange + "' with Queue '" + _queueName + "'");

                Console.ResetColor();
                #endregion
            }
        }

        private static void RemoveWorkshopExchangeAndQueue()
        {
            var factory = new ConnectionFactory() { HostName = _hostName, UserName = _userName, Password = _password };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                #region Removing Exchange & Queue.
                Console.ForegroundColor = ConsoleColor.Red;

                channel.QueueDelete(_queueName);
                Console.WriteLine("Removed Queue : " + _queueName);

                channel.ExchangeDelete(_exchange);
                Console.WriteLine("Removed Exchanged : " + _exchange);

                Console.ResetColor();
                #endregion
            }
        }
    }
}
