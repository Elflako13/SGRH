using SGRH.Web.Models;

namespace SGRH.Web.Services
{
    public interface IDisponibilidadService
    {
        Task<(List<HotelResultado> Resultados, string? Error)> PorHotelAsync(DateOnly entrada, DateOnly salida, int huespedes, string? ciudad = null);
    }
}
