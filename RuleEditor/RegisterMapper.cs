using Mapster;
using RuleEditor.Dto;
using RuleEditor.Models;

namespace RuleEditor;

public class RegisterMapper: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Rule, RuleDto>()
            .RequireDestinationMemberSource(true);
    }
}