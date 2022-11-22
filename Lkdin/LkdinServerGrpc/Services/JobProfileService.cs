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
    public class JobProfileService : Admin.AdminBase
    {
        
        public override Task<MessageReply> PostJobProfile(JobProfileDTO request, ServerCallContext context)
        {
            JobProfileLogic session = JobProfileLogic.GetInstance();
            Console.WriteLine($"Creando perfil de trabajo {request.Name}");

            try
            {
                JobProfile profile = session.CreateJobProfile(request.Name, request.Description, request.ImagePath, request.Abilities.ToList<string>());
                return Task.FromResult(new MessageReply { Message = profile.ToString() }); // TODO - CAMBIAR. Retornar JobProfileDTO o algo que referencie al nuevo perfil
            }
            catch (Exception ex) // TODO - ver como tirar exception del otro lado
            {
                Console.WriteLine($"Error al crear perfil de trabajo {request.Name}");
                return Task.FromResult(new MessageReply { Message = ex.Message });
            }
        }

        public override Task<MessageReply> DeleteJobProfile(ProfileName request, ServerCallContext context)
        {
            JobProfileLogic session = JobProfileLogic.GetInstance();
            Console.WriteLine($"Eliminando perfil de trabajo {request.Name}");

            try
            {
                session.DeleteJobProfile(request.Name);
                return Task.FromResult(new MessageReply { Message = $"Perfil de trabajo {request.Name}, eliminado" });
            }
            catch (Exception ex) // TODO - ver como tirar exception del otro lado
            {
                Console.WriteLine($"Error al eliminar perfil de trabajo {request.Name}");
                return Task.FromResult(new MessageReply { Message = ex.Message });
            }
        }

        public override Task<MessageReply> UpdateJobProfile(JobProfileDTO request, ServerCallContext context)
        {
            JobProfileLogic session = JobProfileLogic.GetInstance();
            Console.WriteLine($"Actualizando perfil de trabajo {request.Name}");

            try
            {
                session.UpdateJobProfile(request.Name, request.Description, request.ImagePath, request.Abilities.ToList<string>());
                return Task.FromResult(new MessageReply { Message = $"Perfil de trabajo {request.Name}, actualizado" });
            }
            catch (Exception ex) // TODO - ver como tirar exception del otro lado
            {
                Console.WriteLine($"Error al actualizar prerfil de trabajo {request.Name}");
                return Task.FromResult(new MessageReply { Message = ex.Message });
            }
        }
    }
}