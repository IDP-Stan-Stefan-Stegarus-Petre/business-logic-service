using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace MobyLabWebProgramming.Core.Errors;

/// <summary>
/// Common error messages that may be reused in various places in the code.
/// </summary>
public static class CommonErrors
{
    public static ErrorMessage UserNotFound => new(HttpStatusCode.NotFound, "User doesn't exist!", ErrorCodes.EntityNotFound);
    public static ErrorMessage UserNotAuthorizedPost => new(HttpStatusCode.Unauthorized, "User not authorized to see post details!", ErrorCodes.CannotSee);
    public static ErrorMessage PostNotFound => new(HttpStatusCode.NotFound, "Post doesn't exist!", ErrorCodes.EntityNotFound);
    public static ErrorMessage LikeNotFound => new(HttpStatusCode.NotFound, "Like doesn't exist!", ErrorCodes.EntityNotFound);
    public static ErrorMessage LikeNotAdded => new(HttpStatusCode.BadRequest, "Like not added!", ErrorCodes.EntityNotFound);
    public static ErrorMessage LikeAlreadyExists => new(HttpStatusCode.BadRequest, "Like already exists!", ErrorCodes.CannotAdd);
    public static ErrorMessage FileNotFound => new(HttpStatusCode.NotFound, "File not found on disk!", ErrorCodes.PhysicalFileNotFound);
    public static ErrorMessage TechnicalSupport => new(HttpStatusCode.InternalServerError, "An unknown error occurred, contact the technical support!", ErrorCodes.TechnicalError);
    public static ErrorMessage CommentNotFound => new(HttpStatusCode.NotFound, "Comment doesn't exist!", ErrorCodes.EntityNotFound);
    public static ErrorMessage CommentNotAdded => new(HttpStatusCode.BadRequest, "Comment not added!", ErrorCodes.EntityNotFound);
    public static ErrorMessage CommentNotModified => new(HttpStatusCode.BadRequest, "Comment not allowed to be modified!", ErrorCodes.CannotUpdate);
    public static ErrorMessage EventNotFound => new(HttpStatusCode.NotFound, "Event doesn't exist!", ErrorCodes.EntityNotFound);
    public static ErrorMessage EventNotModified => new(HttpStatusCode.BadRequest, "Event not allowed to be modified!", ErrorCodes.CannotUpdate);
    public static ErrorMessage FeedbackNotFound => new(HttpStatusCode.NotFound, "Feedback doesn't exist!", ErrorCodes.EntityNotFound);
    public static ErrorMessage FeedBackOnlyForAdmin => new(HttpStatusCode.BadRequest, "Feedback can be seen only by admin!", ErrorCodes.CannotSee);
    public static ErrorMessage FeedbackAlreadyExists => new(HttpStatusCode.BadRequest, "Feedback already exists!", ErrorCodes.CannotAdd);
}
