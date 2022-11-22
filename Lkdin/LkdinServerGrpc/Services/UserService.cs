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
            Console.WriteLine($"Creando usuario {request.Name}");

            try
            {
                User usr = session.CreateUser(request.Name, request.Age, request.Professions.ToList<string>(), request.Country);
                return Task.FromResult(new MessageReply { Message = usr.ToString() }); // TODO - CAMBIAR. Retornar UserDTO o algo que referencie al nuevo usuario
            }
            catch (Exception ex) // TODO - ver como tirar exception del otro lado
            { 
                Console.WriteLine($"Error al crear usuario {request.Name}");
                return Task.FromResult(new MessageReply { Message = ex.Message });
            }
        }

        public override Task<MessageReply> DeleteUser(UserName request, ServerCallContext context)
        {
            UserLogic session = UserLogic.GetInstance();
            Console.WriteLine($"Eliminando usuario {request.Name}");

            try
            {
                session.DeleteUser(request.Name);
                return Task.FromResult(new MessageReply { Message = $"Usuario {request.Name}, eliminado" });
            }
            catch (Exception ex) // TODO - ver como tirar exception del otro lado
            { 
                Console.WriteLine($"Error al eliminar usuario {request.Name}");
                return Task.FromResult(new MessageReply { Message = ex.Message });
            }
        }

        public override Task<MessageReply> UpdateUser(UserDTO request, ServerCallContext context)
        {
            UserLogic session = UserLogic.GetInstance();
            Console.WriteLine($"Actualizando usuario {request.Name}");

            try
            {
                session.UpdateUser(request.Name, request.Age, request.Professions.ToList<string>(), request.Country);
                return Task.FromResult(new MessageReply { Message = $"Usuario {request.Name}, actualizado" });
            }
            catch (Exception ex) // TODO - ver como tirar exception del otro lado
            {
                Console.WriteLine($"Error al actualizar usuario {request.Name}");
                return Task.FromResult(new MessageReply { Message = ex.Message });
            }
        }
    }
}