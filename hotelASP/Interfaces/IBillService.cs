using hotelASP.Entities;

namespace hotelASP.Interfaces
{
    public interface IBillService
    {
        Task<Bill> CreateBillForReservationAsync(int reservationId);
    }
}
