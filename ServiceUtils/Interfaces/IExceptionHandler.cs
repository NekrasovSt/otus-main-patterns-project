using ServiceUtils.Dto;

namespace ServiceUtils.Interfaces;

public interface IExceptionHandler
{
    /// <summary>
    /// Обработка исключения
    /// </summary>
    HandleExceptionResponse? HandleException(Exception exception);
}