﻿using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryHospitals repo;
        //  CUANDO GENERAMO SEL TOKEN DEBEMOS INTEGRAR
        //  ALGUNOS DATOS COMO ISSUER Y DEMAS
        private HelperActionServicesOAuth helper;
        public AuthController(RepositoryHospitals repo,
            HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> LogIn
            (LoginModel model)
        {
            Empleado empleado = await this.repo.LogInEmpledoAsync(model.UserName, int.Parse(model.Password));
            if (empleado == null)
            {
                return Unauthorized();
            }
            else
            {
                //  DEBEMOS CREAR UNAS CREDENCIALES PARA
                //  INCLUIRLAS DENTRO DEL TOKEN Y QUE ESTARAN
                //  COMPUESTAS POR EL SECRET KEY CIFRADO Y EL 
                //  TIPO DE CIFRADO QUE INCLUIREMOS EN EL TOKEN
                SigningCredentials credentials =
                    new SigningCredentials(
                        this.helper.GetKeyToken(),
                        SecurityAlgorithms.HmacSha256);
                //  CREAMOS EL OBJEETO MODEL PARA ALAMCENARLO DENTRO DEL TOKEN
                EmpleadoModel modelEmp = new EmpleadoModel();
                modelEmp.IdEmpleado = empleado.IdEmpleado;
                modelEmp.Apellido = empleado.Apellido;
                modelEmp.Oficio = empleado.Oficio;
                modelEmp.IdDepartamento = empleado.IdDepartamento;
                //  CONVERTIMOS A JSON LOS DATOS DEL EMPLEADO
                string jsonEmpleado =
                    JsonConvert.SerializeObject(modelEmp);
                string jsonCifrado =
                    HelperCryptography.EncryptString(jsonEmpleado);
                //  CREAMOS UN ARRAY DE CLAIMS
                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonCifrado),
                    new Claim(ClaimTypes.Role, empleado.Oficio)
                };


                //  EL TOKEN SE GENERA CON UNA CLASE
                //  Y DEBEMOS INDICAR LOS DATOS QUE ALMACENARA EN SU
                //  INTERIOR
                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: informacion,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(20),
                        notBefore: DateTime.UtcNow);
                //  POR ULTIMO, DEVOLVEMOS LA RESPUESTA AFIRMATIVA
                //  CON UN OBJETO QUE CONTENGA EL TOKEN(anonimo)
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler()
                    .WriteToken(token)
                });
            }
        }
    }
}
