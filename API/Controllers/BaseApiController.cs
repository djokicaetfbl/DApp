using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // zahavljujuci ovom ApiController atributu ne moramo vise govoriti unutar HttpPOST metode FromBody, sam zna da uradi binding, isto tako uradit ce validaciju prije nego se udje u kontroler,
    // zato je dobro mjesto za validaciju DTO model
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController] 
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
    }
}
