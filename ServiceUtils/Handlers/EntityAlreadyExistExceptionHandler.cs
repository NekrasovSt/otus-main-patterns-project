using System.Net;
using ServiceUtils.Dto;
using ServiceUtils.Exceptions;
using ServiceUtils.Interfaces;

namespace ServiceUtils.Handlers;

public class EntityAlreadyExistExceptionHandler : IExceptionHandler
{
    public HandleExceptionResponse? HandleException(Exception exception)
    {
        if (exception is EntityAlreadyExistException ex)
        {
            return new HandleExceptionResponse(
                (int)HttpStatusCode.BadRequest,
                new ErrorDto { Message = ex.Message }
            );
        }
        return null;
    }
}