namespace FinIA.Application.Security;

public interface IUserAnonymizer
{
    Guid Anonymize(Guid userId);
}
