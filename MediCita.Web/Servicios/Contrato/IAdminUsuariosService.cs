using MediCita.Web.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediCita.Web.Servicios.Contrato
{
    public interface IAdminUsuariosService
    {
        // --- CONSULTAS ---
        Task<List<Especialidad>> ListarEspecialidades();
        Task<List<Usuario>> ListarUsuarios();
        Task<List<Medico>> ListarMedicos();
        Task<Medico?> ObtenerMedicoPorId(int idMedico);

        // --- VALIDACIONES ---
        Task<string?> ValidarDatosMedicoProcedure(
            string? dni,
            string correo,
            string cmp,
            int? idMedico = null
        );

        // --- PACIENTES ---
        Task<int> CrearPaciente(
            string nombre,
            string? dni,
            string correo,
            string clave,
            bool activo = true
        );

        Task<bool> EditarPaciente(
            int idUsuario,
            string nombre,
            string? dni,
            string correo,
            bool activo
        );

        // --- MÉDICOS ---
        Task<int> CrearMedico(
            string nombreCompleto,
            string? dni,
            string correo,
            string clave,
            int idEspecialidad,
            string cmp,
            string? rne,
            string? telefono,
            decimal precioConsulta,
            int duracionMinutos,
            string? tipoServicio,
            string? imagenUrl,   
            bool activo
        );

        Task<int> ActualizarMedico(
            int idMedico,
            string nombreCompleto,
            string? dni,
            string correo,
            int idEspecialidad,
            string cmp,
            string? rne,
            string? telefono,
            decimal precioConsulta,
            int duracionMinutos,
            string? tipoServicio,
            string? imagenUrl,   
            bool activo
        );

        // --- SEGURIDAD ---
        Task<bool> CambiarClave(int idUsuario, string nuevaClave);
    }
}
