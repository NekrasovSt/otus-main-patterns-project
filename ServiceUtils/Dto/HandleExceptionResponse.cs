using System.Diagnostics.CodeAnalysis;

namespace ServiceUtils.Dto;

[ExcludeFromCodeCoverage]
public class HandleExceptionResponse
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public HandleExceptionResponse(int code, ErrorDto error)
    {
        Code = code;
        Error = error;
    }

    /// <summary>
    /// Статус код
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Описание ошибки
    /// </summary>
    public ErrorDto Error { get; set; }
}