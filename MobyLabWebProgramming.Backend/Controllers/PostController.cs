using System.Reflection.Metadata.Ecma335;
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
public class PostController: ControllerBase // Here we use the AuthorizedController as the base class because it derives ControllerBase and also has useful methods to retrieve user information.
{
    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public IPostService postService { get; set; }
    public string root = "http://localhost:5000";
    public PostController(IPostService _postService) // Also, you may pass constructor parameters to a base class constructor and call as specific constructor from the base class.
    {
        postService = _postService;
    }

    /// <summary>
    /// This method implements the Read operation (R from CRUD) on a user. 
    /// </summary>
    [Authorize] // You need to use this attribute to protect the route access, it will return a Forbidden status code if the JWT is not present or invalid, and also it will decode the JWT token.
    [HttpGet("{id:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetById/<some_guid>.
    public async Task<ActionResult<RequestResponse<PostDTO>>> GetById([FromRoute] Guid id) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/Post/GetById/" + id.ToString();
            var link = "http://localhost:5000/api/Post/GetById/" + id.ToString();
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
                    var comment = JsonConvert.DeserializeObject<PostDTO>(responseData.ToString());
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
    /// This method implements the Read operation (R from CRUD) on page of users.
    /// Generally, if you need to get multiple values from the database use pagination if there are many entries.
    /// It will improve performance and reduce resource consumption for both client and server.
    /// </summary>
    [Authorize]
    [HttpGet] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetPage.
    public async Task<ActionResult<RequestResponse<PagedResponse<PostDTO>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination) // The FromQuery attribute will bind the parameters matching the names of
                                                                                                                                         // the PaginationSearchQueryParams properties to the object in the method parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/Post/GetPage?"  + "Search=" + pagination.Search + "&Page=" + pagination.Page + "&PageSize=" + pagination.PageSize;
            // var link = "http://localhost:5000/api/Post/GetPage?" + pagination.ToQueryString();
            var link = root + "/api/Post/GetPage?" + "Search=" + pagination.Search + "&Page=" + pagination.Page + "&PageSize=" + pagination.PageSize;
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
                    // Deserialize the response field into a PagedResponse object
                    var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<PostDTO>>(responseData.ToString());
                    return Ok(pagedResponse);
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
    public async Task<ActionResult<RequestResponse>> Add([FromBody] PostAddDTO post)
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/Post/Add";
            var link = root + "/api/Post/Add";
            var response = await client.PostAsJsonAsync(link, post);
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
                    // Deserialize the response field into a RequestResponse object
                    return Ok("Post added successfully");
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
    /// This method implements the Update operation (U from CRUD) on a user. 
    /// </summary>
    [Authorize]
    [HttpPut] // This attribute will make the controller respond to a HTTP PUT request on the route /api/User/Update.
    public async Task<ActionResult<RequestResponse>> Update([FromBody] PostUpdateDTO post) // The FromBody attribute indicates that the parameter is deserialized from the JSON body.
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/Post/Update";
            var link = root + "/api/Post/Update";
            var response = await client.PutAsJsonAsync(link, post);
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
                    // Deserialize the response field into a RequestResponse object
                    return Ok("Post updated successfully");
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
    [HttpDelete("{id}/{idUserCreator}")] // This attribute will make the controller respond to a HTTP DELETE request on the route /api/User/Delete/<some_guid>.
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id, [FromRoute] Guid idUserCreator) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/Post/Delete/" + id.ToString() + "/" + idUserCreator.ToString();
            var link = root + "/api/Post/Delete/" + id.ToString() + "/" + idUserCreator.ToString();
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
                    // Deserialize the response field into a RequestResponse object
                    return Ok("Post deleted successfully");
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