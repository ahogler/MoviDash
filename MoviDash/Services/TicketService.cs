using MoviDash.Pages;
using System.Net.Http.Json;

namespace MoviDash.Services
{
    public class TicketService : ITicketService
    {
        const string URL_BASE = "https://api.movidesk.com/public/v1/tickets?token=65a837f3-8d4c-4dc8-b755-536dcf00dbab&$select=id,subject,createdDate,category,status,urgency,justification,ownerTeam";
        private readonly HttpClient httpClient;

        public TicketService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Ticket>> AbertosHoje()
        {
            var url = MontarUrl(Hoje());
            return await httpClient.GetFromJsonAsync<IEnumerable<Ticket>>(url);
        }

        public async Task<IEnumerable<Ticket>> ConcluidosHoje()
        {
            var url = MontarUrl(FiltroConcluidosHoje());
            return await httpClient.GetFromJsonAsync<IEnumerable<Ticket>>(url);
        }

        public async Task<IEnumerable<Ticket>> TotalAberto()
        {
            var url = MontarUrl(StatusEmAberto());
            return await httpClient.GetFromJsonAsync<IEnumerable<Ticket>>(url);
        }
        public static string Hoje()
        {
            return $"(urgency ne 'Projetos') and createdDate gt {DateTime.Today.ToUniversalTime().ToString("o")}";
        }

        public static string FiltroConcluidosHoje()
        {
            return $"(urgency ne 'Projetos') and resolvedIn gt {DateTime.Today.ToUniversalTime().ToString("o")} and (status eq 'Resolvido' or status eq 'Fechado')";
        }

        public static string StatusEmAberto()
        {
            return "(urgency ne 'Projetos') and (status eq 'Não concluído' or status eq 'Em atendimento' or status eq 'Novo' or status eq 'Parado')";
        }
        private static string MontarUrl(string filtros)
        {
            return $"{URL_BASE}&$filter={filtros}&$expand=owner,customFieldValues($expand=items),clients($expand=organization)";
        }
    }
}
