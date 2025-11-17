using System.Net;
using ServiceUtils.Dto;
using ServiceUtils.Exceptions;
using ServiceUtils.Interfaces;

namespace ServiceUtils.Handlers;

public class EntityNotFoundExceptionHandler : IExceptionHandler
{
    public HandleExceptionResponse? HandleException(Exception exception)
    {
        if (exception is EntityNotFoundException ex)
        {
            return new HandleExceptionResponse(
                (int)HttpStatusCode.NotFound,
                new ErrorDto { Id = ex.Id, Message = ex.Message }
            );
        }
        return null;
    }
}