using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topic.Server1
{
    public class RabbitConsumer : IDisposable
    {
        const string _queueName = "WSQueue.Topic1";
        const string _exchange = "WSExchange.Topic";
        const string _hostName = "localhost";
        const string _userName = "guest";
        const string _password = "guest";
        const bool _isDurable = true;
        const string _virtualHost = "";
        const int _port = 0;

        public delegate void ConsumeDelegate();
        public bool Enabled { get; internal set; }

        ConnectionFactory _connectionFactory;
        IConnection _connection;
        IModel _model;
        Subscription _subscription;

        public RabbitConsumer()
        {
            this.DisplaySettings();
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
            _model.BasicQos(0, 1, false);
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

        public void Start()
        {
            _subscription = new Subscription(_model, _queueName, false);

            var consumer = new ConsumeDelegate(Poll);
            consumer.Invoke();
        }

        private void Poll()
        {
            while (Enabled)
            {
                //Get next message
                var delivaryArgs = _subscription.Next();

                //Deserialize message
                var message = Encoding.Default.GetString(delivaryArgs.Body);

                //Handle message
                Console.WriteLine("Message Recieved - {0}", message);

                //Acknowledge messge is processed
                _subscription.Ack(delivaryArgs);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
