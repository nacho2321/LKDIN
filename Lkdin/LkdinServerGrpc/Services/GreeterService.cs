using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace LkdinServerGrpc
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<NumberResponse> AddNumbers(AddRequest request, ServerCallContext context)
        {
            return Task.FromResult(new NumberResponse
            {
                Result = request.Numberone + request.Numbertwo
            });
        }

        public override Task<UserResponseList> GiveMeUsers(UserRequest request, ServerCallContext context)
        {
            return Task.FromResult<UserResponseList>(CreateUserList());
        }

        private static UserResponseList CreateUserList()
        {
            var list = new UserResponseList();
            var user1 = new User { Name = "Pepe1", Address = "Mi calle 1111", Age = 22 };
            var user2 = new User { Name = "Pepe2", Address = "Mi calle 2222", Age = 23 };
            var user3 = new User { Name = "Pepe3", Address = "Mi calle 3333", Age = 24 };
            list.Users.Add(user1);
            list.Users.Add(user2);
            list.Users.Add(user3);
            return list;
        }
    }
}