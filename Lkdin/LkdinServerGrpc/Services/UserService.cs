using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using LkdinServerGrpc.Domain;
using LkdinServerGrpc.Logic;
using Microsoft.Extensions.Logging;

namespace LkdinServerGrpc
{
    public class UserService : Admin.AdminBase
    {
      
        public override Task<MessageReply> PostUser(UserDTO request, ServerCallContext context)
        {
            UserLogic session = UserLogic.GetInstance();
            Console.WriteLine("Antes de crear el usuario con nombre {0}", request.Name);

            
            User usr = session.CreateUser(request.Name, request.Age, request.Professions.ToList<string>(), request.Country);
            return Task.FromResult(new MessageReply { Message = usr.ToString() }); //CAMBIAR
        }
        /*
        public override Task<UserResponseList> GetUsers(UserRequest request, ServerCallContext context)
        {
            return Task.FromResult<UserResponseList>();
        }
        */


    }
}