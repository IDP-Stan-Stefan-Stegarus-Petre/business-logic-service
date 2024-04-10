using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Core.Entities;

/// <summary>
/// This is an example for a comment entity, it will be mapped to a single table and each property will have it's own column except for entity object references also known as navigation properties.
/// </summary>
public class FeedBack : BaseEntity
{
    public string Content { get; set; } = default!; //text

    public int Rating { get; set; } // can be used for checkbox button

    public string Email { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;

    public string TypeOfAppreciation { get; set; } = default!; // good, bad, very good, very bad, etc.. for select

    public bool IsUserExperienceEnjoyable { get; set; } = false; // for radio button

    public Guid UserId { get; set; } // foreign key
    public User? User { get; set; } // one to one relationship with user

}
