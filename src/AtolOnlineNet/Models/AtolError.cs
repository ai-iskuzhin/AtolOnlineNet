namespace AtolOnlineNet.Models;

/// <summary>Объект ошибки сервиса АТОЛ Онлайн (поле <c>error</c>).</summary>
public sealed record AtolError
{
    /// <summary>Уникальный идентификатор ошибки (<c>error_id</c>).</summary>
    public string? ErrorId { get; init; }

    /// <summary>Код ошибки (<c>code</c>). См. разделы 8.1 (ошибки сервиса) и 8.2 (ошибки ККТ) спецификации.</summary>
    public int Code { get; init; }

    /// <summary>Текст ошибки (<c>text</c>).</summary>
    public string? Text { get; init; }

    /// <summary>Тип источника ошибки (<c>type</c>).</summary>
    public AtolErrorType Type { get; init; }
}
