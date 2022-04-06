using MoviDash.Pages;
using System.Net.Http.Json;

namespace MoviDash.Services
{
    public class TicketService : ITicketService
    {
        const string URL_BASE = "https://api.movidesk.com/public/v1/tickets?token=65a837f3-8d4c-4dc8-b755-536dcf00dbab&$select=id,subject,createdDate,category,status";
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
            var url = MontarUrl(StatusEmAberto());
            return await httpClient.GetFromJsonAsync<IEnumerable<Ticket>>(url);
        }

        public async Task<IEnumerable<Ticket>> TotalAberto()
        {
            var url = MontarUrl(StatusEmAberto());
            return await httpClient.GetFromJsonAsync<IEnumerable<Ticket>>(url);
        }
        public static string Hoje()
        {
            return $"createdDate gt {DateTime.Today.ToUniversalTime().ToString("o")}";
        }
        public static string StatusEmAberto()
        {
            return "(status eq 'Não concluído' or status eq 'Em atendimento' or status eq 'Novo' or status eq 'Parado')";
        }
        private static string MontarUrl(string filtros)
        {
            return $"{URL_BASE}&$filter={filtros}&$expand=owner,customFieldValues($expand=items),clients($expand=organization)";
        }
    }
}
