using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

/// <summary>
/// This DTO is used to transfer information about a user within the application and to client application.
/// Note that it doesn't contain a password property and that is why you should use DTO rather than entities to use only the data that you need or protect sensible information.
/// </summary>
public class EventDTO
{
    public Guid Id { get; set; }
    
    public Guid UserCreatorId { get; set; } = default!;

    public string Content { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Location { get; set; } = default!;

    public string Date { get; set; } = default!;

}
