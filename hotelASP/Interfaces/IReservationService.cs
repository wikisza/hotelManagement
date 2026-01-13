using hotelASP.Entities;
using hotelASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace hotelASP.Interfaces
{
    public interface IReservationService
    {
        Task<(bool Success, string ErrorMessage)> CreateAsync(Reservation reservation);
        Task<(bool Success, string ErrorMessage)> EditAsync(Reservation reservation);
        Task<ReservationViewModel> FindReservation(int id);
        Task<(bool Success, string ErrorMessage)> DeleteConfirmed(int id);
        List<ReservationViewModel> CurrentReservations();
        List<ReservationViewModel> HistoryReservations();
        Task<List<object>> GetReservations();
        Task<List<object>> GetOldReservations();
        Task<List<int>> GetAvailableRoomIds(DateTime dateFrom, DateTime dateTo);
    }
}