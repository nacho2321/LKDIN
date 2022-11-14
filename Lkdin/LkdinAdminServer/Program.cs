using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace LkdinAdminServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            using var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            var numberReply = await client.AddNumbersAsync(
                new AddRequest { Numberone = 1, Numbertwo = 2 });
            Console.WriteLine("Add Numbers result: " + numberReply.Result);

            var userList = client.GiveMeUsers(new UserRequest());


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}