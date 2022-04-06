using Microsoft.AspNetCore.Components;
using MoviDash.Services;
using System.Net.Http.Json;

namespace MoviDash.Pages
{
    public partial class Index
    {
        public List<Ticket> TicketsAbertosHoje { get; set; }
        public List<Ticket> TicketsTotalEmAberto { get; set; }
        public int AbertosHoje { get; set; }
        public int TotalEmAberto { get; set; }

        [Inject]
        public ITicketService service { get; set; }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            TicketsAbertosHoje = (await service.AbertosHoje()).ToList();
            AbertosHoje = TicketsAbertosHoje.Count();

            TicketsTotalEmAberto = (await service.TotalAberto()).ToList();
            TotalEmAberto = TicketsTotalEmAberto.Count();
        }
    }
    public class Ticket
    {
        public string status { get; set; }
        public string category { get; set; }
        public DateTime createdDate { get; set; }
        public string subject { get; set; }
        public int id { get; set; }
    }

}
