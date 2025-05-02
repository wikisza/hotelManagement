using hotelASP.Entities;
using Microsoft.AspNetCore.Mvc;

namespace hotelASP.Interfaces
{
    public interface IReservationsController
    {
        IActionResult Calendar();
        IActionResult Create();
        Task<IActionResult> Create([Bind(new[] { "Id_reservation,Date_from, Date_to, First_name, Last_name, IdRoom" })] Reservation reservation);
        Task<IActionResult> CurrentReservations();
    }
}