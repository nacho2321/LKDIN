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
    public class LkdinService : Admin.AdminBase
    {
      
        public override Task<MessageReply> PostUser(UserDTO request, ServerCallContext context)
        {
            UserLogic session = UserLogic.GetInstance();
            Console.WriteLine($"Creando usuario {request.Name}");

            User usr = session.CreateUser(request.Name, request.Age, request.Professions.ToList<string>(), request.Country);
            return Task.FromResult(new MessageReply { Message = $"Usuario {usr.Name}, creado" });
        }

        public override Task<MessageReply> DeleteUser(UserName request, ServerCallContext context)
        {
            UserLogic session = UserLogic.GetInstance();

            Console.WriteLine($"Eliminando usuario {request.Name}");
                        
            session.DeleteUser(request.Name);
            return Task.FromResult(new MessageReply { Message = $"Usuario {request.Name}, eliminado" });
        }

        public override Task<MessageReply> UpdateUser(UserDTO request, ServerCallContext context)
        {
            UserLogic session = UserLogic.GetInstance();
            Console.WriteLine($"Actualizando usuario {request.Name}");

            session.UpdateUser(request.Name, request.Age, request.Professions.ToList<string>(), request.Country);
            return Task.FromResult(new MessageReply { Message = $"Usuario {request.Name}, actualizado" });
        }

        public override Task<MessageReply> PostJobProfile(JobProfileDTO request, ServerCallContext context)
        {
            JobProfileLogic session = JobProfileLogic.GetInstance();

            Console.WriteLine($"Creando perfil de trabajo {request.Name}");

            JobProfile profile = session.CreateJobProfile(request.Name, request.Description, request.ImagePath, request.Abilities.ToList<string>());
            return Task.FromResult(new MessageReply { Message = $"Perfil {profile.Name}, creado" });
        }
        public override Task<MessageReply> UpdateJobProfile(JobProfileDTO request, ServerCallContext context)
        {
            JobProfileLogic session = JobProfileLogic.GetInstance();

            Console.WriteLine($"Actualizando perfil de trabajo {request.Name}");

            session.UpdateJobProfile(request.Name, request.Description, request.ImagePath, request.Abilities.ToList<string>());
            return Task.FromResult(new MessageReply { Message = $"Perfil de trabajo {request.Name}, actualizado" });
        }

        public override Task<MessageReply> DeleteJobProfile(ProfileName request, ServerCallContext context)
        {
            JobProfileLogic session = JobProfileLogic.GetInstance();

            Console.WriteLine($"Eliminando perfil de trabajo {request.Name}");

            session.DeleteJobProfile(request.Name);
            return Task.FromResult(new MessageReply { Message = $"Perfil de trabajo {request.Name}, eliminado" });
        }

        public override Task<MessageReply> DeleteJobProfileImage(UserName request, ServerCallContext context)
        {
            JobProfileLogic session = JobProfileLogic.GetInstance();

            Console.WriteLine($"Eliminando imagen de perfil de trabajo {request.Name}");

            session.DeleteJobProfileImage(request.Name);
            return Task.FromResult(new MessageReply { Message = $"Imagen del perfil de trabajo {request.Name}, eliminada" });
        }

    }
}