using hotelASP.Entities;
using hotelASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace hotelASP.Interfaces
{
    public interface IReservationService
    {
        Task<(bool Success, string ErrorMessage)> CreateAsync(Reservation reservation);

        Task<(bool Success, string ErrorMessage)> EditAsync(Reservation reservation);

        Task<(bool Success, string ErrorMessage)> FindReservation(int id);
        Task<(bool Success, string ErrorMessage)> DeleteConfirmed(int id);
    }
}