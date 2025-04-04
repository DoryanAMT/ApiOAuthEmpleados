using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private RepositoryHospitals repo;
        private HelperEmpleadoToken helper;
        public EmpleadosController(
            RepositoryHospitals repo,
            HelperEmpleadoToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }
        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> GetEmpleados()
        {
            return await this.repo.GetEmpleadosAsync();
        }
        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<Empleado> FindEmpleado
            (int id)
        {
            return await this.repo.FindEmpleadoAsync(id);
        }
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<EmpleadoModel>> Perfil()
        {
            EmpleadoModel model = this.helper.GetEmpleado();
            return model;
        }
        [Authorize(Roles = "PRESIDENTE")]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            EmpleadoModel model = this.helper.GetEmpleado();
            return await this.repo.GetCompisEmpleadosAsync(model.IdDepartamento);

        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> Oficios()
        {
            return await this.repo.GetOficiosAsync();
        }
        //  ?oficio=ANALISTA?oficio=DIRECTOR
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> 
            EmpleadosOficio([FromQuery] List<string> oficio)
        {
            return await this.repo.GetEmpleadosByOficiosAsync(oficio);
        }
        [HttpPut]
        [Route("[action]/{incremento}")]
        public async Task<ActionResult> IncrementarSalarios
            (int incremento, [FromQuery]List<string> oficio)
        {
            await this.repo.IncrementarSalariosAsync(incremento, oficio);
            return Ok();
        }
    }
}
