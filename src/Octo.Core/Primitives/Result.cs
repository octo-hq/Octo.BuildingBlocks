using Flunt.Notifications;

namespace Octo.Core.Primitives;

public class Result : Notifiable<Notification>
{
    public Result() { }

    protected Result(Notifiable<Notification> validations)
    {
        if (validations != null)
            AddNotifications(validations.Notifications);
    }

    public static Result Success() => new();
    public static Result Failure(Notifiable<Notification> validations) => new(validations);
}

public class Result<TValue> : Result
{
    // Transformado em propriedade pública para o System.Text.Json conseguir serializar corretamente na API
    public TValue? Value { get; }

    protected Result(TValue? value)
    {
        if (value is Notifiable<Notification> validations && !validations.IsValid)
            AddNotifications(validations.Notifications);
        else
            Value = value;
    }

    // Construtor protegido para Falha (reutiliza o construtor da classe base)
    protected Result(Notifiable<Notification> validations) : base(validations)
    {
        Value = default;
    }

    public static Result<TValue> Success(TValue value) => new(value);

    // Método estático de falha para o tipo genérico
    public new static Result<TValue> Failure(Notifiable<Notification> validations) => new(validations);
}