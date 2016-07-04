using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing.ConfigureRabbitMQ
{
    class Program
    {
        #region Constant Strings
        const string _queueName1 = "WSQueue.Routing1";
        const string _queueName2 = "WSQueue.Routing2";

        const string _routingKey1 = "1";
        const string _routingKey2 = "2";

        const string _exchange = "WSExchange.Routing";       
        
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

                channel.QueueDeclare(queue: _queueName1, durable: false, exclusive: false, autoDelete: false, arguments: null);
                Console.WriteLine("Created Queue : " + _queueName1);

                channel.QueueDeclare(queue: _queueName2, durable: false, exclusive: false, autoDelete: false, arguments: null);
                Console.WriteLine("Created Queue : " + _queueName2);

                channel.ExchangeDeclare(_exchange, ExchangeType.Direct);
                Console.WriteLine("Created Exchange : " + _exchange);

                channel.QueueBind(_queueName1, _exchange, _routingKey1);
                Console.WriteLine("Binding Exchange '" + _exchange + "' with Queue '" + _queueName1 + "'" + "' and RoutingKey '" + _routingKey1);

                channel.QueueBind(_queueName1, _exchange, _routingKey2);
                Console.WriteLine("Binding Exchange '" + _exchange + "' with Queue '" + _queueName1 + "'" + "' and RoutingKey '" + _routingKey2);

                channel.QueueBind(_queueName2, _exchange, _routingKey2);
                Console.WriteLine("Binding Exchange '" + _exchange + "' with Queue '" + _queueName2 + "'" + "' and RoutingKey '" + _routingKey2);

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

                channel.QueueDelete(_queueName1);
                Console.WriteLine("Removed Queue : " + _queueName1);

                channel.QueueDelete(_queueName2);
                Console.WriteLine("Removed Queue : " + _queueName2);

                channel.ExchangeDelete(_exchange);
                Console.WriteLine("Removed Exchanged : " + _exchange);

                Console.ResetColor();
                #endregion
            }
        }
    }
}
