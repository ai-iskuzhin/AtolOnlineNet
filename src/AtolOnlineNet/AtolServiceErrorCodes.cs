namespace AtolOnlineNet;

/// <summary>
/// Symbolic names for the service-level error codes returned in the <c>error.code</c> field
/// (раздел 8.1 спецификации). These are distinct from the ККТ driver error codes (раздел 8.2).
/// </summary>
public static class AtolServiceErrorCodes
{
    /// <summary>Неизвестная ошибка (0).</summary>
    public const int Undefined = 0;

    /// <summary>Сервер не смог обработать входной чек (1).</summary>
    public const int IncomingChequeProcessingFailed = 1;

    /// <summary>Передан некорректный токен (10).</summary>
    public const int MissingToken = 10;

    /// <summary>Срок действия токена истёк (11).</summary>
    public const int ExpiredToken = 11;

    /// <summary>Неверный логин или пароль (12).</summary>
    public const int WrongLoginOrPassword = 12;

    /// <summary>Ошибка валидации входящего запроса (13).</summary>
    public const int ValidationException = 13;

    /// <summary>Пользователь заблокирован (14).</summary>
    public const int UserBlocked = 14;

    /// <summary>Код группы не соответствует токену (20).</summary>
    public const int GroupCodeAndTokenDontMatch = 20;

    /// <summary>Код группы не поддерживает данную версию протокола (21).</summary>
    public const int NotSupportedGroupCodeForProtocol = 21;

    /// <summary>Передан некорректный UUID или UUID не найден (30).</summary>
    public const int MissingUuid = 30;

    /// <summary>Операция не поддерживается (31).</summary>
    public const int IncomingOperationNotSupported = 31;

    /// <summary>Ошибка валидации входного чека (32).</summary>
    public const int IncomingValidationException = 32;

    /// <summary>Чек с таким external_id уже существует и ещё не обработан (33).</summary>
    public const int IncomingExternalIdAlreadyExists = 33;

    /// <summary>Состояние чека не найдено, попробуйте позднее (34).</summary>
    public const int StateCheckNotFound = 34;

    /// <summary>Некорректный запрос (40).</summary>
    public const int BadRequest = 40;

    /// <summary>Некорректный Content-Type (41).</summary>
    public const int UnsupportedMediaType = 41;

    /// <summary>Ошибка конфигурации сервера (50).</summary>
    public const int ErrorServerConfiguration = 50;
}
