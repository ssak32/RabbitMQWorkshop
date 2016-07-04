using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ScatterGather.Server3
{
    public class RabbitConsumer : IDisposable
    {
        const string _queueName = "WSQueue.ScatterGather3";
        const string _exchange = "WSExchange.ScatterGather";
        const string _hostName = "localhost";
        const string _userName = "guest";
        const string _password = "guest";
        const bool _isDurable = true;
        const string _virtualHost = "";
        const int _port = 0;

        public delegate void OnReceivedMessage(string message);
        public bool Enabled { get; internal set; }

        ConnectionFactory _connectionFactory;
        IConnection _connection;
        IModel _model;

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
            var consumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(_queueName, true, consumer);

            //consumer.Received += (model, ea) =>
            //{
            //    var body = ea.Body;
            //    var message = Encoding.UTF8.GetString(body);
            //    Console.WriteLine("Message Recieved - {0}", message);
            //    var response = string.Format("Processed message - {0} : Response is good", message);

            //    //Send Reponse
            //    var replyProperties = _model.CreateBasicProperties();
            //    replyProperties.CorrelationId = deliv
            //};

            while (Enabled)
            {
                //Get next message
                var deliveryArgs = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                //Deserialize message
                var message = Encoding.Default.GetString(deliveryArgs.Body);
                Console.WriteLine("Message Recieved - {0}", message);
                var response = string.Format("Processed message - {0} : Response is good", message);

                //Send Reponse
                var replyProperties = _model.CreateBasicProperties();
                replyProperties.CorrelationId = deliveryArgs.BasicProperties.CorrelationId;
                byte[] messageBuffer = Encoding.Default.GetBytes(response);
                _model.BasicPublish("", deliveryArgs.BasicProperties.ReplyTo, replyProperties, messageBuffer);

                //Handle Message
                Console.WriteLine("Finished Processing Message - {0}", message);

                ////Acknowledge message is processed
                //_model.BasicAck(deliveryArgs.DeliveryTag, false);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
