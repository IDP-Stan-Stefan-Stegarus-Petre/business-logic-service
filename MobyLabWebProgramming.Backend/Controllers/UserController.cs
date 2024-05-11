using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Configurations;
using Newtonsoft.Json;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController] // This attribute specifies for the framework to add functionality to the controller such as binding multipart/form-data.
[Route("api/[controller]/[action]")] // The Route attribute prefixes the routes/url paths with template provides as a string, the keywords between [] are used to automatically take the controller and method name.
public class
    UserController : ControllerBase // Here we use the AuthorizedController as the base class because it derives ControllerBase and also has useful methods to retrieve user information.
{
    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    private readonly DbReadWriteServiceConfiguration _dbReadWriteServiceConfiguration;

    public UserController(IOptions<DbReadWriteServiceConfiguration> dbReadWriteServiceConfiguration)
    {
        _dbReadWriteServiceConfiguration = dbReadWriteServiceConfiguration.Value;
    }

    /// <summary>
    /// This method implements the Read operation (R from CRUD) on a user. 
    /// </summary>
    [Authorize] // You need to use this attribute to protect the route access, it will return a Forbidden status code if the JWT is not present or invalid, and also it will decode the JWT token.
    [HttpGet("{id:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetById/<some_guid>.
    public async Task<ActionResult<RequestResponse<UserDTO>>> GetById([FromRoute] Guid id) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/User/GetById/" + id.ToString();
            var link = _dbReadWriteServiceConfiguration.BaseUrl + "/api/User/GetById/" + id.ToString();
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
                    UserDTO user = new UserDTO()
                    {
                        Id = Guid.Parse(responseData.id.ToString()),
                        Name = responseData.name.ToString(),
                        Email = responseData.email.ToString(),
                        PhoneNumber = responseData.phoneNumber.ToString()
                    };
                    if (responseData.role == "Admin")
                    {
                        user.Role = UserRoleEnum.Admin;
                    }
                    else if (responseData.role == "User")
                    {
                        user.Role = UserRoleEnum.User;
                    }

                    return Ok(user);
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
    public async Task<ActionResult<RequestResponse<PagedResponse<UserDTO>>>>
        GetPage([FromQuery] PaginationSearchQueryParams pagination) // The FromQuery attribute will bind the parameters matching the names of
        // the PaginationSearchQueryParams properties to the object in the method parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/User/GetPage?" + "Search=" + pagination.Search + "&Page=" + pagination.Page + "&PageSize=" + pagination.PageSize;
            var link = _dbReadWriteServiceConfiguration.BaseUrl + "/api/User/GetPage?" + "Search=" + pagination.Search + "&Page=" + pagination.Page + "&PageSize=" +
                       pagination.PageSize;
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
                    List<UserDTO> users = new List<UserDTO>();
                    PagedResponse<UserDTO> res = new PagedResponse<UserDTO>((uint)responseData.page, (uint)responseData.pageSize, (uint)responseData.totalCount, users);
                    foreach (var item in responseData.data)
                    {
                        UserDTO user = new UserDTO()
                        {
                            Id = Guid.Parse(item.id.ToString()),
                            Name = item.name.ToString(),
                            Email = item.email.ToString(),
                            PhoneNumber = item.phoneNumber.ToString()
                        };
                        if (item.role == "Admin")
                        {
                            user.Role = UserRoleEnum.Admin;
                        }
                        else if (item.role == "User")
                        {
                            user.Role = UserRoleEnum.User;
                        }

                        users.Add(user);
                    }

                    return Ok(res);
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
    /// This method implements the Create operation (C from CRUD) of a user. 
    /// </summary>
    [Authorize]
    [HttpPost] // This attribute will make the controller respond to a HTTP POST request on the route /api/User/Add.
    public async Task<ActionResult<RequestResponse>> Add([FromBody] UserAddDTO user)
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/User/Add";
            var link = _dbReadWriteServiceConfiguration.BaseUrl + "/api/User/Add";
            var response = await client.PostAsJsonAsync(link, user);
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
                    return Ok("User added successfully");
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
    public async Task<ActionResult<RequestResponse>>
        Update([FromBody] UserUpdateDTO user) // The FromBody attribute indicates that the parameter is deserialized from the JSON body.
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/User/Update";
            var link = _dbReadWriteServiceConfiguration.BaseUrl + "/api/User/Update";
            var response = await client.PutAsJsonAsync(link, user);
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
                    return Ok("User updated successfully");
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
    [HttpDelete("{id:guid}")] // This attribute will make the controller respond to a HTTP DELETE request on the route /api/User/Delete/<some_guid>.
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        using (HttpClient client = new HttpClient())
        {
            // var link = "http://localhost:5000/api/User/Delete/" + id.ToString();
            var link = _dbReadWriteServiceConfiguration.BaseUrl + "/api/User/Delete/" + id.ToString();
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
                    return Ok("User deleted successfully");
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
