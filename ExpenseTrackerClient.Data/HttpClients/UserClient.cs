using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using ExpenseTrackerClient.Data.HttpClients.Contracts;
using ExpenseTrackerClient.Data.Models;
using ExpenseTrackerClient.Data.Models.Dtos;
using Newtonsoft.Json;

namespace ExpenseTrackerClient.Data.HttpClients;

public class UserClient : IUserClient
{
    private const string CONNECTION_STRING = "https://localhost:44388/";
    private readonly HttpClient _httpClient;

    public UserClient()
    {
        
        _httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }

        })
        {
            
            BaseAddress = new Uri(CONNECTION_STRING)
        };
    }

    public async Task<Guid> RegisterUserAsync(string username, string password, Guid bankAccountId)
    {
        var registerUserDto = new RegisterUserDto
        {
            Username = username,
            Password = password,
            BankAccountId = bankAccountId
        };
        if (!registerUserDto.IsValid())
            return Guid.Empty;
        
        var response = await _httpClient.PostAsJsonAsync("register/user", registerUserDto);

        if (!response.IsSuccessStatusCode) return Guid.Empty;
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Guid>(content);

    }

    // YES, it returns BAid, it is the WORST solution and ik i
    public async Task<Guid> LoginUserAsync(string username, string password)
    {
        var loginUserDto = new LoginUserDto
        {
            Username = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("login/user", loginUserDto);

        if (!response.IsSuccessStatusCode) return Guid.Empty;
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Guid>(content);

    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"get/user/{id}");

        if (!response.IsSuccessStatusCode) return null;
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<User>(content);

    }
}