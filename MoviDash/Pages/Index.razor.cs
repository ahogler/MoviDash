using Microsoft.AspNetCore.Components;
using MoviDash.Services;
using MudBlazor;
using System;
using System.Net.Http.Json;

namespace MoviDash.Pages
{
    public partial class Index
    {
        public List<Ticket> TicketsAbertosHoje { get; set; }
        public List<Ticket> TicketsTabela { get; set; }
        public List<Ticket> TicketsTotalEmAberto { get; set; }
        public int AbertosHoje { get; set; }
        public int TotalEmAberto { get; set; }
        public int AbertosEsseMes { get; set; }
        public int AbertosHojeAindaAberto { get; set; }
        public DateTime UltimaAtualizacao { get; set; }

        private System.Threading.Timer? timer;

        private bool _loading;

        [Inject]
        public ITicketService service { get; set; }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            UltimaAtualizacao = DateTime.Now;

            timer = new System.Threading.Timer(async (object? stateInfo) =>
            {
                _loading = true;
                StateHasChanged();

                TicketsAbertosHoje = (await service.AbertosHoje()).ToList();
                TicketsTabela = TicketsAbertosHoje.OrderByDescending(x => x.id).ToList();

                AbertosHoje = TicketsAbertosHoje.Count();

                AbertosHojeAindaAberto = TicketsAbertosHoje.Where(x => x.status != "Resolvido" && x.status != "Fechado").ToList().Count();

                TicketsTotalEmAberto = (await service.TotalAberto()).ToList();
                TotalEmAberto = TicketsTotalEmAberto.Count();

                AbertosEsseMes = TicketsTotalEmAberto.Where(x => x.createdDate.Date.Month == DateTime.Now.Date.Month).ToList().Count();
                UltimaAtualizacao = DateTime.Now;

                _loading = false;
                StateHasChanged(); // NOTE: MUST CALL StateHasChanged() BECAUSE THIS IS TRIGGERED BY A TIMER INSTEAD OF A USER EVENT
            }, new System.Threading.AutoResetEvent(false), 30000, 30000); // fire every 2000 milliseconds
        }
    }
    public class Ticket
    {
        public string status { get; set; }
        public string category { get; set; }
        public DateTime createdDate { get; set; }
        public string subject { get; set; }
        public int id { get; set; }

        public string TicketLink
        {
            get { return "https://mhtec.movidesk.com/Ticket/Edit/" + id; }
        }
        public Color CorStatus
        {
            get {
                if (status != "Resolvido" && status != "Fechado")
                    return Color.Error;
                else
                    return Color.Success;
            }
        }

    }

}
