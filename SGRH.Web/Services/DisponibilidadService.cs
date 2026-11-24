using SGRH.BL.Services;
using SGRH.Web.Models;

namespace SGRH.Web.Services
{
    public class DisponibilidadService : IDisponibilidadService
    {
        private readonly IHotelService hotelService;
        private readonly IHabitacionService habitacionService;

        public DisponibilidadService(IHotelService hotelService, IHabitacionService habitacionService)
        {
            this.hotelService = hotelService;
            this.habitacionService = habitacionService;
        }

        public async Task<(List<HotelResultado> Resultados, string? Error)> PorHotelAsync(DateOnly entrada, DateOnly salida, int huespedes, string? ciudad = null)
        {
            var resultados = new List<HotelResultado>();

            var hoteles = ((await hotelService.GetAllAsync()).Data ?? [])
                .Where(h => h.Estatus)
                .Where(h => string.IsNullOrEmpty(ciudad) || h.Ciudad == ciudad)
                .OrderByDescending(h => h.Estrellas);

            foreach (var hotel in hoteles)
            {
                var disponibles = await habitacionService.GetDisponiblesAsync(hotel.HotelId, entrada, salida, huespedes);
                if (!disponibles.Success)
                    return (resultados, Formato.Limpiar(disponibles.Message));

                var lista = disponibles.Data!;
                resultados.Add(new HotelResultado
                {
                    Hotel = hotel,
                    Disponibles = lista.Count,
                    DesdePorNoche = lista.Count > 0 ? lista.Min(h => h.TarifaPorNoche) : null,
                    TotalDesde = lista.Count > 0 ? lista.Min(h => h.CostoEstimado) : null
                });
            }

            return (resultados.OrderByDescending(r => r.Disponibles > 0).ThenBy(r => r.TotalDesde).ToList(), null);
        }
    }
}
