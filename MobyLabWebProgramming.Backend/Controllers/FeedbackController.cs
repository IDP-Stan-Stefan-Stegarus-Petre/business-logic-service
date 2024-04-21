using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using Newtonsoft.Json;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController] // This attribute specifies for the framework to add functionality to the controller such as binding multipart/form-data.
[Route("api/[controller]/[action]")] // The Route attribute prefixes the routes/url paths with template provides as a string, the keywords between [] are used to automatically take the controller and method name.
public class FeedbackController : ControllerBase // Here we use the AuthorizedController as the base class because it derives ControllerBase and also has useful methods to retrieve user information.
{
    public string root = "http://localhost:5000";

    public FeedbackController() // Also, you may pass constructor parameters to a base class constructor and call as specific constructor from the base class.
    {
    }

    /// <summary>
    /// This method implements the Read operation (R from CRUD) on a user. 
    /// </summary>
    [Authorize] // You need to use this attribute to protect the route access, it will return a Forbidden status code if the JWT is not present or invalid, and also it will decode the JWT token.
    [HttpGet("{id:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetById/<some_guid>.
    public async Task<ActionResult<RequestResponse<FeedbackDTO>>> GetById([FromRoute] Guid id) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        // return getFeedback from FeedbackService
        // return this.FromServiceResponse(await FeedbackService.GetFeedback(id));
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/Feedback/GetById/" + id.ToString();
            var link = root + "/api/Feedback/GetById/" + id.ToString();
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
                    var comment = JsonConvert.DeserializeObject<FeedbackDTO>(responseData.ToString());
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
    [HttpGet("{idUserInitiator:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetPage.
    public async Task<ActionResult<RequestResponse<PagedResponse<FeedbackDTO>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination, [FromRoute] Guid idUserInitiator) // The FromQuery attribute will bind the parameters matching the names of
                                                                                                                                                                               // the PaginationSearchQueryParams properties to the object in the method parameter.
    {
        // return this.FromServiceResponse(await FeedbackService.GetFeedbacks(pagination, idUserInitiator));
         using (HttpClient client = new HttpClient())
        {
            if (idUserInitiator == Guid.Empty)
                return BadRequest("Invalid user" + idUserInitiator.ToString());

            var link = root + "/api/Feedback/GetPage/"+  idUserInitiator.ToString() + "/?Search=" + pagination.Search + "&Page=" + pagination.Page + "&PageSize=" + pagination.PageSize;
            // var link = "http://localhost:5000/api/Feedback/GetPage/"+  idUserInitiator.ToString() + "/?Search=" + pagination.Search + "&Page=" + pagination.Page + "&PageSize=" + pagination.PageSize;
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
                    var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<FeedbackDTO>>(responseData.ToString());
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
                return BadRequest(errorData);
            }
        }
    
    }

    /// <summary>
    /// This method implements the Create operation (C from CRUD) of a Feedback. 
    /// </summary>
    [Authorize]
    [HttpPost] // This attribute will make the controller respond to a HTTP Feedback request on the route /api/User/Add.
    public async Task<ActionResult<RequestResponse>> Add([FromBody] FeedbackAddDTO Feedback)
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/Feedback/Add";
            var link = root + "/api/Feedback/Add";
            var response = await client.PostAsJsonAsync(link, Feedback);
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
                    return Ok("Feedback added successfully!");
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
    public async Task<ActionResult<RequestResponse>> Update([FromBody] FeedbackUpdateDTO Feedback) // The FromBody attribute indicates that the parameter is deserialized from the JSON body.
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/Feedback/Update";
            var link = root + "/api/Feedback/Update";
            var response = await client.PutAsJsonAsync(link, Feedback);
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
                    return Ok("Feedback updated successfully!");
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
    [HttpDelete("{id}/{idUser}")] // This attribute will make the controller respond to a HTTP DELETE request on the route /api/User/Delete/<some_guid>.
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id, [FromRoute] Guid idUser) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            //var link = "http://localhost:5000/api/Feedback/Delete/" + id.ToString() + "/" + idUser.ToString();
            var link = root + "/api/Feedback/Delete/" + id.ToString() + "/" + idUser.ToString();
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
                    return Ok("Feedback deleted successfully!");
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
