using Grpc.Core;
using Grpc.Core.Interceptors;
using LkdinServerGrpc.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

public class ExceptionInterceptor : Interceptor
{

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (DomainException ex)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();

            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception exception)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Message);
            Console.ResetColor();

            throw new RpcException(new Status(StatusCode.Internal, exception.Message));
        }
    }
}