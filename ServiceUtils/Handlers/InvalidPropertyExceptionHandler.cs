using System.Net;
using ServiceUtils.Dto;
using ServiceUtils.Exceptions;
using ServiceUtils.Interfaces;

namespace ServiceUtils.Handlers;

public class InvalidPropertyExceptionHandler : IExceptionHandler
{
    public HandleExceptionResponse? HandleException(Exception exception)
    {
        if (exception is InvalidPropertyException ex)
        {
            return new HandleExceptionResponse(
                (int)HttpStatusCode.BadRequest,
                new ErrorDto { Id = ex.Id, Message = ex.Message }
            );
        }
        return null;
    }
}