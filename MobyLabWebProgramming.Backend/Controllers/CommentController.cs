using System.Reflection.Metadata.Ecma335;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Infrastructure.Services.Implementations;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;
namespace MobyLabWebProgramming.Backend.Controllers;

/// <summary>
/// This is a controller example for CRUD operations on users.
/// </summary>
[ApiController] // This attribute specifies for the framework to add functionality to the controller such as binding multipart/form-data.
[Route("api/[controller]/[action]")] // The Route attribute prefixes the routes/url paths with template provides as a string, the keywords between [] are used to automatically take the controller and method name.
public class CommentController: ControllerBase // Here we use the AuthorizedController as the base class because it derives ControllerBase and also has useful methods to retrieve user information.
{
    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public ICommentService CommentService { get; set; }
    public CommentController(ICommentService _CommentService) // Also, you may pass constructor parameters to a base class constructor and call as specific constructor from the base class.
    {
        CommentService = _CommentService;
    }

    [Authorize] // You need to use this attribute to protect the route access, it will return a Forbidden status code if the JWT is not present or invalid, and also it will decode the JWT token.
    [HttpGet("{id:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetById/<some_guid>.
    public async Task<ActionResult<RequestResponse<CommentDTO>>> GetById([FromRoute] Guid id) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            var link = "http://localhost:5000/api/Comment/GetById/" + id.ToString();
            var response = await client.GetAsync(link);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // Access the response field
                var responseData = jsonObject?.response;
                var errorData = jsonObject?.errorMessage;

                if (responseData != null)
                {
                    // Deserialize the response field into a CommentDTO object
                    var comment = JsonConvert.DeserializeObject<CommentDTO>(responseData.ToString());
                    return Ok(comment);
                }
                else
                {
                    // Deserialize the errorMessage field into an ErrorMessage object
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                    return BadRequest(error);
                }
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                var errorData = jsonObject?.errorMessage;
                var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                return BadRequest(error);
            }
        }
    }

    [Authorize]
    [HttpGet("count-Comments/{idPost:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetPage.
    public async Task<ActionResult<RequestResponse<int>>> GetPostComments([FromRoute] Guid idPost) // The FromQuery attribute will bind the parameters matching the names of                                                                                                                                   // the PaginationSearchQueryParams properties to the object in the method parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            var link = "http://localhost:5000/api/Comment/GetPostComments/count-Comments/" + idPost.ToString();
            var response = await client.GetAsync(link);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // Access the response field
                var responseData = jsonObject?.response;
                var errorData = jsonObject?.errorMessage;

                if (responseData != null)
                {
                    // Deserialize the response field into a CommentDTO object
                    var comment = JsonConvert.DeserializeObject<int>(responseData.ToString());
                    return Ok(comment);
                }
                else
                {
                    // Deserialize the errorMessage field into an ErrorMessage object
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                    return BadRequest(error);
                }
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                var errorData = jsonObject?.errorMessage;
                var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                return BadRequest(error);
            }
        }
    }

    //GetCommentsForPost
    [HttpGet("Comments/{idPost}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetPage.
    public async Task<ActionResult<RequestResponse<List<CommentDTO>>>> GetCommentsForPost([FromRoute] Guid idPost) // The FromQuery attribute will bind the parameters matching the names of                                                                                                                                    // the PaginationSearchQueryParams properties to the object in the method parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            var link = "http://localhost:5000/api/Comment/GetCommentsForPost/Comments/" + idPost.ToString();
            var response = await client.GetAsync(link);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // Access the response field
                var responseData = jsonObject?.response;
                var errorData = jsonObject?.errorMessage;

                if (responseData != null)
                {
                    // Deserialize the response field into a CommentDTO object
                    var comment = JsonConvert.DeserializeObject<List<CommentDTO>>(responseData.ToString());
                    return Ok(comment);
                }
                else
                {
                    // Deserialize the errorMessage field into an ErrorMessage object
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                    return BadRequest(error);
                }
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                var errorData = jsonObject?.errorMessage;
                var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                return BadRequest(error);
            }
        }
    }

    /// <summary>
    /// This method implements the Create operation (C from CRUD) of a post. 
    /// </summary>
    [Authorize]
    [HttpPost] // This attribute will make the controller respond to a HTTP POST request on the route /api/User/Add.
    public async Task<ActionResult<RequestResponse>> Add([FromBody] CommentAddDTO Comment)
    {
        using (HttpClient client = new HttpClient())
        {
            var link = "http://localhost:5000/api/Comment/Add";
            var response = await client.PostAsJsonAsync(link, Comment);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // Access the response field
                var responseData = jsonObject?.response;
                var errorData = jsonObject?.errorMessage;

                if (responseData != null)
                {
                    return Ok("Comment added successfully");
                }
                else
                {
                    // Deserialize the errorMessage field into an ErrorMessage object
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                    return BadRequest(error);
                }
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                var errorData = jsonObject?.errorMessage;
                var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                return BadRequest(error);
            }
        }
    }

    // Add update post
    [Authorize]
    [HttpPut] // This attribute will make the controller respond to a HTTP PUT request on the route /api/User/Update.
    public async Task<ActionResult<RequestResponse>> Update([FromBody] CommentUpdateDTO Comment) // The FromBody attribute indicates that the parameter is deserialized from the JSON body.
    {
        using (HttpClient client = new HttpClient())
        {
            var link = "http://localhost:5000/api/Comment/Update";
            var response = await client.PutAsJsonAsync(link, Comment);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // Access the response field
                var responseData = jsonObject?.response;
                var errorData = jsonObject?.errorMessage;

                if (responseData != null)
                {
                    // Deserialize the response field into a CommentDTO object
                    return Ok("Comment updated successfully");
                }
                else
                {
                    // Deserialize the errorMessage field into an ErrorMessage object
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                    return BadRequest(error);
                }
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                var errorData = jsonObject?.errorMessage;
                var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                return BadRequest(error);
            }
        }
    }

    /// <summary>
    /// This method implements the Delete operation (D from CRUD) on a user.
    /// Note that in the HTTP RFC you cannot have a body for DELETE operations.
    /// </summary>
    [Authorize]
    [HttpDelete("{id}/{idUser}/{idPost}")] // This attribute will make the controller respond to a HTTP DELETE request on the route /api/User/Delete/<some_guid>.
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id, [FromRoute] Guid idUser, [FromRoute] Guid idPost) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            var link = "http://localhost:5000/api/Comment/Delete/" + id.ToString() + "/" + idUser.ToString() + "/" + idPost.ToString();
            var response = await client.DeleteAsync(link);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // Access the response field
                var responseData = jsonObject?.response;
                var errorData = jsonObject?.errorMessage;

                if (responseData != null)
                {
                    // Deserialize the response field into a CommentDTO object
                    return Ok("Comment deleted successfully");
                }
                else
                {
                    // Deserialize the errorMessage field into an ErrorMessage object
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                    return BadRequest(error);
                }
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an object
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                var errorData = jsonObject?.errorMessage;
                var error = JsonConvert.DeserializeObject<ErrorMessage>(errorData?.ToString());
                return BadRequest(error);
            }
        }
    }
}