using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

[RoutePrefix("api/npi")]
public class NpiController : ApiController
{
    private readonly NpiService _npi = new NpiService();

    [HttpGet, Route("{number}")]
    public async Task<IHttpActionResult> Get(string number)
    {
        if (number.Length != 10 || !number.All(char.IsDigit))
            return BadRequest("NPI must be 10 digits.");

        var result = await _npi.LookupByNumberAsync(number);
        if (result?.Errors != null && result.Errors.Count > 0) return BadRequest("NPI registry returned an error.");
        if (result == null || result.ResultCount == 0) return NotFound();
        return Ok(result.Results[0]);
    }

    [HttpGet, Route("search")]
    public async Task<IHttpActionResult> Search(string firstName = null, string lastName = null, string state = null)
    {
        var result = await _npi.SearchAsync(firstName, lastName, state);
        return Ok(result);
    }
}