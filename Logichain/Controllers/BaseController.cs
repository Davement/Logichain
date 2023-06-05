using Microsoft.AspNetCore.Mvc;

namespace Logichain.Controllers;

public class BaseController : Controller
{
    protected CreatedResult Created(object value)
    {
        var locationUri = new UriBuilder
        {
            Scheme = Request.Scheme,
            Host = Request.Host.Host,
            Path = Request.Path.ToString()
        }.Uri.ToString();
        return Created(locationUri, value);
    }
}