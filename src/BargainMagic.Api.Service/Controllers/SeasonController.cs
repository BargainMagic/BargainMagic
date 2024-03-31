using BargainMagic.Api.Service.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BargainMagic.Api.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonController : ControllerBase
    {
        private IDbContextFactory<DataContext> dataContextFactory;

        public SeasonController(IDbContextFactory<DataContext> dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
        }

        [HttpPost]
        public ActionResult Create()
        {
            using var dataContext = dataContextFactory.CreateDbContext();

            var season = new Season
                         {
                             Name = "TestSeason"
                         };

            dataContext.Seasons.Add(season);
            dataContext.SaveChanges();

            return Ok();
        }
    }
}
