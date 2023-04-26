using Microsoft.AspNetCore.Components;
using MoviDash.Services;
using MudBlazor;
using System;
using System.Net.Http.Json;
using static MudBlazor.Colors;

namespace MoviDash.Pages
{
    public partial class Index
    {
        MudTabs tabs;
        public List<PorAgente> TicketsPorAgente { get; set; }
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

            TicketsPorAgente = new List<PorAgente>();
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
                TicketsTotalEmAberto = (await service.TotalAberto()).ToList();

                var listaPrincipal = TicketsTotalEmAberto.Where(x => x.status != "Resolvido" && x.status != "Fechado").OrderByDescending(x => x.id).ToList();
                TicketsTabela = listaPrincipal.Where(x => x.justification == "Aguardando retorno do cliente" 
                    || x.justification == "Em análise pelo suporte" || string.IsNullOrEmpty(x.justification))
                        .OrderByDescending(x => x.id).ToList();

                AbertosHoje = TicketsAbertosHoje.Count();

                AbertosHojeAindaAberto = TicketsAbertosHoje.Where(x => x.status != "Resolvido" && x.status != "Fechado").ToList().Count();

                TotalEmAberto = TicketsTotalEmAberto.Count();

                TicketsConcluidosHoje = (await service.ConcluidosHoje()).ToList();
                TotalConcluidosHoje = TicketsConcluidosHoje.Count();

                var totalEmAbertoComSuporte = TicketsTotalEmAberto.Where(x => x.justification == "Aguardando retorno do cliente"
                    || x.justification == "Em análise pelo suporte" || string.IsNullOrEmpty(x.justification)).ToList();


                TicketsPorAgente = totalEmAbertoComSuporte
                    .GroupBy(a => a.owner?.businessName)
                    .Select(g => new PorAgente
                    {
                        Nome = g.Key,
                        TotalEmAberto = g.Count()
                    })
                    .ToList();

                TicketsAugusto = totalEmAbertoComSuporte.Where(x => x.owner?.id == "820261949").ToList();
                TicketsRodrigo = totalEmAbertoComSuporte.Where(x => x.owner?.id == "2146209211").ToList();
                TicketsAni = totalEmAbertoComSuporte.Where(x => x.owner?.id == "145414126").ToList();

                AbertosEsseMes = TicketsTotalEmAberto.Where(x => x.createdDate.Date.Month == DateTime.Now.Date.Month).ToList().Count();
                UltimaAtualizacao = DateTime.Now;

                _loading = false;
                StateHasChanged(); // NOTE: MUST CALL StateHasChanged() BECAUSE THIS IS TRIGGERED BY A TIMER INSTEAD OF A USER EVENT
            }, new System.Threading.AutoResetEvent(false), 25000, 25000); // fire every 2000 milliseconds

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
            }, new AutoResetEvent(false), 15000, 15000); 

        }
        void Activate(int index)
        {
            tabs.ActivatePanel(index);
        }
    }

    public class PorAgente
    {
        public string Nome { get; set; }

        public string PrimeiroNome
        {
            get {
                var spaceIndex = Nome.IndexOf(" ");
                return Nome.Substring(0, spaceIndex);
            }
        }

        public string UrlFoto
        {
            get
            {
                if (PrimeiroNome.ToUpper().Contains("ANIELE"))
                    return "https://s3.amazonaws.com/movidesk-files/E61AFD4077A934F7E56AF8A2B20A8349";
                if (PrimeiroNome.ToUpper().Contains("AUGUSTO"))
                    return "https://s3.amazonaws.com/movidesk-files/09AE32BFEBF21D5A690390378956707F";
                if (PrimeiroNome.ToUpper().Contains("ADENILSON"))
                    return "https://s3.amazonaws.com/movidesk-files/5FF543E6E90E40722927260CCFAC6E07";
                if (PrimeiroNome.ToUpper().Contains("EVERTON"))
                    return "https://s3.amazonaws.com/movidesk-files/4DEF0ABD48D70B0CCD1CA5CEC237C2E6";
                if (PrimeiroNome.ToUpper().Contains("ALEXANDRE"))
                    return "https://s3.amazonaws.com/movidesk-files/5934A3F1BA2D646F9608EEB2BDCE171E";
                if (PrimeiroNome.ToUpper().Contains("ALEXANDRE"))
                    return "https://s3.amazonaws.com/movidesk-files/FB0352F292C47974FE3B7ABA96AF2F16";
                
                return "";
            }
        }

        public int ResolvidosHoje { get; set; } 
        public int TotalEmAberto { get; set; } 
    }
    public class Ticket
    {
        public string status { get; set; }
        public string category { get; set; }
        public DateTime createdDate { get; set; }

        public DateTime DataCriacao
        {
            get { return createdDate.AddHours(-3); }
        }

        public TimeSpan TempoAbertura
        {
            get { 
                return (DateTime.Now - DataCriacao); 
            }
        }

        public string TempoAberturaTexto
        {
            get
            {
                return $"{TempoAbertura.ToString(@"hh\:mm")}hrs";
            }
        }

        public string subject { get; set; }
        public string justification { get; set; }
        public int id { get; set; }
        public Owner owner { get; set; }
        public string urgency { get; set; }
        public string ownerTeam { get; set; }
        public DateTime lastActionDate { get; set; }

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

        public int DiaSemAcao
        {
            get
            {
                var dias = DateTime.Now - lastActionDate;
                return Convert.ToInt32(dias.TotalDays);
            }
        }
        public string DiaSemAcaoTxt
        {
            get
            {
                return DiaSemAcao.ToString() + " dias sem ação";
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
