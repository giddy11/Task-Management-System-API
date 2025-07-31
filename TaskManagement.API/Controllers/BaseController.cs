using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Contracts.Persistence;

namespace TaskManagement.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected BaseController(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            Logger = logger;
        }

        public IUnitOfWork UnitOfWork { get; set; }
        public IMapper Mapper { get; set; }
        public ILogger Logger { get; set; }
    }
}
