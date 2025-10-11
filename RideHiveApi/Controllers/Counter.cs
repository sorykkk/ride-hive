using Microsoft.AspNetCore.Mvc;
using Counter.Data;
using Counter.Model;
using Microsoft.EntityFrameworkCore;

namespace Counter.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CounterController : ControllerBase
    {
        private readonly AppDbContext context; //Pentru accesul bazei de date

        public CounterController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult<CounterClass>> CreateVal([FromQuery] int i)
        {
            
            string res = $"A{i}";
            CounterClass outVal = new CounterClass
            {
                Id = i,
                Val = res
            };

            this.context.CounterClass.Add(outVal); // adauga userul

            await this.context.SaveChangesAsync(); //salveaza in baza de date

            return Ok(outVal);; //raspuns cu 201 si outVal
        }
    }
}
