using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LkdinServerGrpc
{
    public class LogPublisher
    {
        public IModel Setting()
        {
            //1 - definimos un FACTORY para inicializar la conexion
            //2 - definir la connection
            //3 - definir el channel
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            {
                //4 - Declaramos la cola de mensajes
                channel.QueueDeclare(queue: "logs",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                return channel;
            }
        }

        public void Message(IModel channel, string log)
        {
            channel.BasicPublish(exchange: "",
                routingKey: "logs",
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(log)
                );
        }
    }
}
