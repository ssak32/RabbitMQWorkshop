using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterGather.Client
{
    public class RabbitSender : IDisposable
    {
        const string _queueName = "WSQueue.ScatterGather1";
        const string _exchange = "WSExchange.ScatterGather";
        const string _hostName = "localhost";
        const string _userName = "guest";
        const string _password = "guest";
        const bool _isDurable = true;
        const string _virtualHost = "";
        const int _port = 0;

        string _responseQueue;
        ConnectionFactory _connectionFactory;
        IConnection _connection;
        IModel _model;
        QueueingBasicConsumer _consumer;


        public RabbitSender()
        {
            this.DisplaySettings();
            this.SetupRabbitMq();
        }

        private void DisplaySettings()
        {
            Console.WriteLine("Host: {0}", _hostName);
            Console.WriteLine("Username: {0}", _userName);
            Console.WriteLine("Password: {0}", _password);
            Console.WriteLine("QueueName: {0}", _queueName);
            Console.WriteLine("ExchangeName: {0}", _exchange);
            Console.WriteLine("VirtualHost: {0}", _virtualHost);
            Console.WriteLine("Port: {0}", _port);
            Console.WriteLine("Is Durable: {0}", _isDurable);
        }

        private void SetupRabbitMq()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            if (string.IsNullOrEmpty(_virtualHost) == false)
                _connectionFactory.VirtualHost = _virtualHost;
            if (_port > 0)
                _connectionFactory.Port = _port;

            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();

            //Create dynamic response queue
            _responseQueue = _model.QueueDeclare().QueueName;
            _consumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(_responseQueue, true, _consumer);
        }

        public List<string> Send(string message, string routingKey, TimeSpan timeout, int minResponses)
        {
            var responses = new List<string>();
            var correlationToken = Guid.NewGuid().ToString();

            //Setup properties
            var properties = _model.CreateBasicProperties();
            properties.ReplyTo = _responseQueue;
            properties.CorrelationId = correlationToken;

            //Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(message);

            //Send message
            var timeoutAt = DateTime.Now + timeout;
            _model.BasicPublish(_exchange, routingKey, properties, messageBuffer);

            //Wait for response
            while (DateTime.Now <= timeoutAt)
            {
                BasicDeliverEventArgs result = null;
                _consumer.Queue.Dequeue(10, out result);

                if (result == null)
                {
                    //No more messages on queue at preset so if we have already got
                    //lets just return those
                    if (responses.Count >= minResponses)
                        return responses;

                    Console.WriteLine("Waiting for responses");
                    Thread.Sleep(new TimeSpan(0, 0, 0, 0, 200));
                    continue;
                }

                var deliveryArgs = result;
                if (deliveryArgs.BasicProperties != null
                    || deliveryArgs.BasicProperties.CorrelationId == correlationToken)
                {
                    var response = Encoding.Default.GetString(deliveryArgs.Body);
                    Console.WriteLine("Sender got response: {0}", response);
                    responses.Add(response);
                }                
            }
            return responses;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
