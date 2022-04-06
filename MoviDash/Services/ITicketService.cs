using MoviDash.Pages;

namespace MoviDash.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> AbertosHoje();
        Task<IEnumerable<Ticket>> ConcluidosHoje();
        Task<IEnumerable<Ticket>> TotalAberto();
    }
}
