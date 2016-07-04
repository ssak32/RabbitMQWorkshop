using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPC.Client
{
    public class RabbitSender : IDisposable
    {
        const string _queueName = "WSQueue.RPC";
        const string _exchange = "WSExchange.RPC";
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

        public string Send(string message, TimeSpan timeSpan)
        {
            var correlationToken = Guid.NewGuid().ToString();

            //Setup properties
            var properties = _model.CreateBasicProperties();
            properties.ReplyTo = _responseQueue;
            properties.CorrelationId = correlationToken;

            //Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(message);

            //Send message
            var timeoutAt = DateTime.Now + timeSpan;
            _model.BasicPublish(_exchange, _queueName, properties, messageBuffer);

            //Wait for response
            while (DateTime.Now <= timeoutAt)
            {
                var deliveryArgs = _consumer.Queue.Dequeue();
                if (deliveryArgs.BasicProperties != null
                    && deliveryArgs.BasicProperties.CorrelationId == correlationToken)
                {
                    var response = Encoding.Default.GetString(deliveryArgs.Body);
                    return response;
                }
            }
            throw new TimeoutException(@"The response was not returned before the timeout span.");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
