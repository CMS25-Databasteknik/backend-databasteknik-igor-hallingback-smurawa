using Backend.Application.Common;
using Backend.Domain.Modules.ParticipantContactTypes.Models;

namespace Backend.Application.Modules.ParticipantContactTypes.Outputs;

public sealed record ParticipantContactTypeResult : ResultBase<ParticipantContactType>
{
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public ParticipantContactTypeResult() : base() { }
}

public sealed record ParticipantContactTypeListResult : ResultBase<IReadOnlyList<ParticipantContactType>>
{
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public ParticipantContactTypeListResult() : base() { }
}

public sealed record ParticipantContactTypeDeleteResult : ResultBase<bool>
{
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public ParticipantContactTypeDeleteResult() : base() { }
}
