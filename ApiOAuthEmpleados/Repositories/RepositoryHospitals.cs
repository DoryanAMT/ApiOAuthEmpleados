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

        public async Task <List<Empleado>> GetEmpleadosAsync()
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
    }
}
