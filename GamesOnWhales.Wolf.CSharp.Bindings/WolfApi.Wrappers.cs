namespace GamesOnWhales;

public partial class WolfApi
{
    public async Task<ICollection<NSwagWolfApi.App>> GetAppsAsync() 
        => (await GeneratedApiBindings.AppsAsync()).Apps ?? Array.Empty<NSwagWolfApi.App>();
    
    public async Task<ICollection<NSwagWolfApi.PairedClient>> GetClientsAsync() 
        => (await GeneratedApiBindings.ClientsAsync()).Clients ?? Array.Empty<NSwagWolfApi.PairedClient>();

    public async Task<NSwagDocker.ImageInspect> GetDockerImagesInspectAsync(string imageName)
        => await DockerApi.ImageInspectAsync(imageName);
    
    public async Task<ICollection<NSwagWolfApi.Lobby>> GetLobbiesAsync() 
        => (await GeneratedApiBindings.LobbiesAsync()).Lobbies ?? Array.Empty<NSwagWolfApi.Lobby>();
    
    public async Task<ICollection<NSwagWolfApi.PendingPairClient>> GetPendingPairRequestsAsync() 
        => (await GeneratedApiBindings.PendingAsync()).Requests ?? Array.Empty<NSwagWolfApi.PendingPairClient>();
    
    public async Task<ICollection<NSwagWolfApi.Profile>> GetProfilesAsync() 
        => (await GeneratedApiBindings.ProfilesAsync()).Profiles ?? Array.Empty<NSwagWolfApi.Profile>();
    
    public async Task<ICollection<NSwagWolfApi.StreamSession>> GetSessionsAsync()
        => (await GeneratedApiBindings.SessionsAsync()).Sessions ?? Array.Empty<NSwagWolfApi.StreamSession>();

    public async Task<Stream?> GetUtilsGetIconAsync(string iconPath)
        => await GetIcon(iconPath);
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostAppsAddAsync(NSwagWolfApi.App app)
        => await GeneratedApiBindings.AddAsync(app);

    public async Task<NSwagWolfApi.GenericSuccessResponse> PostAppsDeleteAsync(NSwagWolfApi.App app)
        => await GeneratedApiBindings.DeleteAsync(new NSwagWolfApi.AppDeleteRequest(){Id = app.Id});
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostAppsDeleteAsync(string appId)
        => await GeneratedApiBindings.DeleteAsync(new NSwagWolfApi.AppDeleteRequest(){Id = appId});

    public async Task<NSwagWolfApi.GenericSuccessResponse> PostClientSettingsAsync(
        NSwagWolfApi.UpdateClientSettingsRequest clientSettings)
        => await GeneratedApiBindings.SettingsAsync(clientSettings);

    public async Task PostDockerImagesPullAsync(string imageName)
        => PullImage(imageName);
    
    public async Task<NSwagWolfApi.LobbyCreateResponse> PostLobbiesCreate(NSwagWolfApi.CreateLobbyRequest lobby)
        => await GeneratedApiBindings.CreateAsync(lobby);

    public async Task<NSwagWolfApi.GenericSuccessResponse> PostLobbiesJoinAsync(NSwagWolfApi.JoinLobbyEvent lobby)
        => await GeneratedApiBindings.JoinAsync(lobby);
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostLobbiesLeaveAsync(NSwagWolfApi.LeaveLobbyEvent lobby)
        => await GeneratedApiBindings.LeaveAsync(lobby);

    public async Task<NSwagWolfApi.GenericSuccessResponse> PostLobbiesStopAsync(NSwagWolfApi.StopLobbyEvent lobby)
        => await GeneratedApiBindings.StopAsync(lobby);

    public async Task<NSwagWolfApi.GenericSuccessResponse> PostPairClientAsync(NSwagWolfApi.PairRequest pair)
        => await GeneratedApiBindings.ClientAsync(pair);
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostProfilesAddAsync(NSwagWolfApi.Profile profile)
        => await GeneratedApiBindings.Add2Async(profile);
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostProfilesRemoveAsync(NSwagWolfApi.ProfileRemoveRequest profile)
        => await GeneratedApiBindings.RemoveAsync(profile);
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostRunnerStartAsync(NSwagWolfApi.RunnerStartRequest runner)
        => await GeneratedApiBindings.StartAsync(runner);
    
    public async Task<NSwagWolfApi.StreamSessionCreated> PostSessionsAddAsync(NSwagWolfApi.StreamSession session)
        => await GeneratedApiBindings.Add3Async(session);

    public async Task<NSwagWolfApi.GenericSuccessResponse> PostSessionsInputAsync(
        NSwagWolfApi.StreamSessionHandleInputRequest input)
        => await GeneratedApiBindings.InputAsync(input);
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostSessionsPauseAsync(NSwagWolfApi.StreamSessionPauseRequest session)
        => await GeneratedApiBindings.PauseAsync(session);
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostSessionsStartAsync(NSwagWolfApi.StreamSessionStartRequest session)
        => await GeneratedApiBindings.Start2Async(session);
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostSessionsStopAsync(NSwagWolfApi.StreamSessionStopRequest session)
        => await GeneratedApiBindings.Stop2Async(session);
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> PostUnpairClientAsync(NSwagWolfApi.UnpairClientRequest unpair)
        => await GeneratedApiBindings.Client2Async(unpair);
}