using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Headers.WinClient
{
    public class RabbitSender : IDisposable
    {
        const string _queueName = "";
        const string _exchange = "WSExchange.Headers";
        const string _hostName = "localhost";
        const string _userName = "guest";
        const string _password = "guest";
        const bool _isDurable = true;
        const string _virtualHost = "";
        const int _port = 0;

        ConnectionFactory _connectionFactory;
        IConnection _connection;
        IModel _model;


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
        }

        public void Send(string message, Dictionary<string, string> headers)
        {
            //Setup properties
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = new Dictionary<string, object>();
            foreach (var header in headers)
            {
                Console.WriteLine("Header - Key: {0}, Value: {1}", header.Key, header.Value);
                properties.Headers.Add(header.Key, header.Value);
            }

            //Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(message);

            //Send message
            _model.BasicPublish(_exchange, _queueName, properties, messageBuffer);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}