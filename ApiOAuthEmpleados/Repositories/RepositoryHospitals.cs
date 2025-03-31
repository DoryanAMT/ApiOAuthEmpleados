using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ApiOAuthEmpleados.Repository
{
    public class RepositoryHospitals
    {
        private HospitalContext context;
        public RepositoryHospitals(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            return await this.context.Empleados.ToListAsync();
        }
        public async Task<Empleado> FindEmpleadoAsync
            (int idEmpleado)
        {
            return await this.context.Empleados
                .FirstOrDefaultAsync(x => x.IdEmpleado == idEmpleado);
        }
        public async Task<Empleado> LogInEmpledoAsync
            (string apellido, int idEmpleado)
        {
            return await this.context.Empleados
                .FirstOrDefaultAsync(x => x.IdEmpleado == idEmpleado
                && x.Apellido == apellido);
        }

        public async Task<List<Empleado>> GetCompisEmpleadosAsync
            (int idDepartamento)
        {
            return await this.context.Empleados
                .Where(x => x.IdDepartamento == idDepartamento)
                .ToListAsync();
        }
        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Empleados
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }
        public async Task<List<Empleado>> GetEmpleadosByOficiosAsync(List<string> oficios)
        {
            var consulta = from datos in this.context.Empleados
                           where oficios.Contains(datos.Oficio)
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task IncrementarSalariosAsync
            (int incremento, List<string> oficios)
        {
            List<Empleado> empleados = await
                this.GetEmpleadosByOficiosAsync(oficios);
            foreach (Empleado emp in empleados)
            {
                emp.Salario += incremento;
            }
            await this.context.SaveChangesAsync();
        }
    }
}