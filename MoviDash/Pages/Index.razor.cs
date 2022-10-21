using Microsoft.AspNetCore.Components;
using MoviDash.Services;
using MudBlazor;
using System;
using System.Net.Http.Json;

namespace MoviDash.Pages
{
    public partial class Index
    {
        MudTabs tabs;
        public List<Ticket> TicketsAugusto { get; set; }
        public List<Ticket> TicketsRodrigo { get; set; }
        public List<Ticket> TicketsAndre { get; set; }
        public List<Ticket> TicketsAni { get; set; }
        public List<Ticket> TicketsAde { get; set; }
        public List<Ticket> TicketsEverton { get; set; }
        public List<Ticket> TicketsConcluidosHoje { get; set; }
        public List<Ticket> TicketsAbertosHoje { get; set; }
        public List<Ticket> TicketsTabela { get; set; }
        public List<Ticket> TicketsTotalEmAberto { get; set; }
        public int AbertosHoje { get; set; }
        public int TotalEmAberto { get; set; }
        public int AbertosEsseMes { get; set; }
        public int AbertosHojeAindaAberto { get; set; }
        public int TotalConcluidosHoje { get; set; }
        public DateTime UltimaAtualizacao { get; set; }

        private System.Threading.Timer? timer;
        private System.Threading.Timer? timerTabs;
        private int tabIndex;

        private bool _loading;

        [Inject]
        public ITicketService service { get; set; }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            UltimaAtualizacao = DateTime.Now;

            TicketsAugusto = new List<Ticket>();
            TicketsRodrigo = new List<Ticket>();
            TicketsAndre = new List<Ticket>();
            TicketsAni = new List<Ticket>();
            TicketsAde = new List<Ticket>();
            TicketsEverton = new List<Ticket>();
            TicketsConcluidosHoje = new List<Ticket>();

            timer = new System.Threading.Timer(async (object? stateInfo) =>
            {
                _loading = true;
                StateHasChanged();

                TicketsAbertosHoje = (await service.AbertosHoje()).ToList();
                TicketsTabela = TicketsAbertosHoje.Where(x => x.status != "Resolvido" && x.status != "Fechado").OrderByDescending(x => x.id).ToList();

                AbertosHoje = TicketsAbertosHoje.Count();

                AbertosHojeAindaAberto = TicketsAbertosHoje.Where(x => x.status != "Resolvido" && x.status != "Fechado").ToList().Count();

                TicketsTotalEmAberto = (await service.TotalAberto()).ToList();
                TotalEmAberto = TicketsTotalEmAberto.Count();

                TicketsConcluidosHoje = (await service.ConcluidosHoje()).ToList();
                TotalConcluidosHoje = TicketsConcluidosHoje.Count();

                TicketsAugusto = TicketsTotalEmAberto.Where(x => x.owner?.id == "820261949").ToList();
                TicketsRodrigo = TicketsTotalEmAberto.Where(x => x.owner?.id == "2146209211").ToList();
                TicketsAndre = TicketsTotalEmAberto.Where(x => x.owner?.id == "1184814735").ToList();
                TicketsAde = TicketsTotalEmAberto.Where(x => x.owner?.id == "1360469349").ToList();
                TicketsEverton = TicketsTotalEmAberto.Where(x => x.owner?.id == "1933648929").ToList();
                TicketsAni = TicketsTotalEmAberto.Where(x => x.owner?.id == "145414126").ToList();

                AbertosEsseMes = TicketsTotalEmAberto.Where(x => x.createdDate.Date.Month == DateTime.Now.Date.Month).ToList().Count();
                UltimaAtualizacao = DateTime.Now;

                _loading = false;
                StateHasChanged(); // NOTE: MUST CALL StateHasChanged() BECAUSE THIS IS TRIGGERED BY A TIMER INSTEAD OF A USER EVENT
            }, new System.Threading.AutoResetEvent(false), 20000, 20000); // fire every 2000 milliseconds

            tabIndex = 0;
            timerTabs = new Timer(async (object? stateInfo) =>
            {
                Activate(tabIndex);

                if (tabIndex == 0)
                    tabIndex = 1;
                else
                    tabIndex = 0;
                Activate(tabIndex);

                StateHasChanged();
            }, new AutoResetEvent(false), 10000, 10000); // fire every 2000 milliseconds

        }
        void Activate(int index)
        {
            tabs.ActivatePanel(index);
        }
    }
    public class Ticket
    {
        public string status { get; set; }
        public string category { get; set; }
        public DateTime createdDate { get; set; }
        public string subject { get; set; }
        public string justification { get; set; }
        public int id { get; set; }
        public Owner owner { get; set; }
        public string urgency { get; set; }
        public string ownerTeam { get; set; }

        public string TicketLink
        {
            get { return "https://mhtec.movidesk.com/Ticket/Edit/" + id; }
        }
        public Color CorStatus
        {
            get {
                if (status == "Novo")
                    return Color.Error;
                else
                    return Color.Success;
            }
        }

        public List<client> clients { get; set; }

        public string Empresa
        {
            get
            {
                if (clients == null || clients[0].organization == null) return "";
                return clients[0].organization.businessName;
            }
        }

    }

    public class client
    {
        public string businessName { get; set; }
        public empresa organization { get; set; }
        public class empresa
        {
            public string businessName { get; set; }
        }
    }

    public class Owner
    {
        public string id { get; set; }
        public int personType { get; set; }
        public int profileType { get; set; }
        public string businessName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string pathPicture { get; set; }

    }

}
