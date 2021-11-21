using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using FSH.BlazorWebAssembly.Shared.Identity;
using FSH.BlazorWebAssembly.Shared.Requests.Identity;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Identity.Roles;
public class RoleService : IRoleService
{
    private readonly HttpClient _httpClient;

    public RoleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult<string>> DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"{RolesEndpoints.Delete}/{id}");
        return await response.ToResult<string>();
    }

    public async Task<IResult<List<RoleDto>>> GetRolesAsync()
    {
        var response = await _httpClient.GetAsync(RolesEndpoints.GetAll);
        return await response.ToResult<List<RoleDto>>();
    }

    public async Task<IResult<string>> SaveAsync(RoleRequest role)
    {
        var response = await _httpClient.PostAsJsonAsync(RolesEndpoints.Save, role);
        return await response.ToResult<string>();
    }

    // public async Task<IResult<PermissionResponse>> GetPermissionsAsync(string roleId)
    // {
    //    var response = await _httpClient.GetAsync(Routes.RolesEndpoints.GetPermissions + roleId);
    //    return await response.ToResult<PermissionResponse>();
    // }

    // public async Task<IResult<string>> UpdatePermissionsAsync(PermissionRequest request)
    // {
    //    var response = await _httpClient.PutAsJsonAsync(Routes.RolesEndpoints.UpdatePermissions, request);
    //    return await response.ToResult<string>();
    // }
}