using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class NpiService
{
    // One shared instance; don't create a new HttpClient per request
    private static readonly HttpClient _http;

    static NpiService()
    {
        // Needed on older .NET Framework versions for HTTPS to work
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

        _http = new HttpClient
        {
            BaseAddress = new Uri("https://npiregistry.cms.hhs.gov/api/"),
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    public async Task<NpiResponse> LookupByNumberAsync(string npi)
    {
        return await GetAsync("?version=2.1&number=" + Uri.EscapeDataString(npi));
    }

    public async Task<NpiResponse> SearchAsync(string firstName, string lastName, string state, int limit = 10)
    {
        var qs = new List<string> { "version=2.1", "limit=" + limit };
        if (!string.IsNullOrWhiteSpace(firstName)) qs.Add("first_name=" + Uri.EscapeDataString(firstName));
        if (!string.IsNullOrWhiteSpace(lastName)) qs.Add("last_name=" + Uri.EscapeDataString(lastName));
        if (!string.IsNullOrWhiteSpace(state)) qs.Add("state=" + Uri.EscapeDataString(state));

        return await GetAsync("?" + string.Join("&", qs));
    }

    private async Task<NpiResponse> GetAsync(string query)
    {
        var response = await _http.GetAsync(query);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<NpiResponse>(json);
    }
}