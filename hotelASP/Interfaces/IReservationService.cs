using hotelASP.Entities;
using hotelASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace hotelASP.Interfaces
{
    public interface IReservationService
    {
        Task<(bool Success, string ErrorMessage)> CreateAsync(Reservation reservation);
    }
}