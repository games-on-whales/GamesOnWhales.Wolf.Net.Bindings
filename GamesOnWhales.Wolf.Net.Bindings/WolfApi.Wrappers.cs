namespace GamesOnWhales;

public partial class WolfApi
{
    public async Task<ICollection<App>> GetAppsAsync() 
        => (await GeneratedClient.AppsAsync()).Apps ?? Array.Empty<App>();
    
    public async Task<ICollection<PairedClient>> GetClientsAsync() 
        => (await GeneratedClient.ClientsAsync()).Clients ?? Array.Empty<PairedClient>();

    public async Task<ImageInspect> GetDockerImagesInspectAsync(string imageName)
        => await GeneratedClient.ImageInspectAsync(imageName);
    
    public async Task<ICollection<Lobby>> GetLobbiesAsync() 
        => (await GeneratedClient.LobbiesAsync()).Lobbies ?? Array.Empty<Lobby>();
    
    public async Task<ICollection<PendingPairClient>> GetPendingPairRequestsAsync() 
        => (await GeneratedClient.PendingAsync()).Requests ?? Array.Empty<PendingPairClient>();
    
    public async Task<ICollection<Profile>> GetProfilesAsync() 
        => (await GeneratedClient.ProfilesAsync()).Profiles ?? Array.Empty<Profile>();
    
    public async Task<ICollection<StreamSession>> GetSessionsAsync()
        => (await GeneratedClient.SessionsAsync()).Sessions ?? Array.Empty<StreamSession>();

    public async Task<Stream?> GetUtilsGetIconAsync(string iconPath)
        => await GetIcon(iconPath);
    
    public async Task<GenericSuccessResponse> PostAppsAddAsync(App app)
        => await GeneratedClient.AddAsync(app);

    public async Task<GenericSuccessResponse> PostAppsDeleteAsync(App app)
        => await GeneratedClient.DeleteAsync(new AppDeleteRequest(){Id = app.Id});
    
    public async Task<GenericSuccessResponse> PostAppsDeleteAsync(string appId)
        => await GeneratedClient.DeleteAsync(new AppDeleteRequest(){Id = appId});

    public async Task<GenericSuccessResponse> PostClientSettingsAsync(
        UpdateClientSettingsRequest clientSettings)
        => await GeneratedClient.SettingsAsync(clientSettings);

    public async Task PostDockerImagesPullAsync(string imageName)
        => PullImage(imageName);
    
    public async Task<LobbyCreateResponse> PostLobbiesCreate(CreateLobbyRequest lobby)
        => await GeneratedClient.CreateAsync(lobby);

    public async Task<GenericSuccessResponse> PostLobbiesJoinAsync(JoinLobbyEvent lobby)
        => await GeneratedClient.JoinAsync(lobby);
    
    public async Task<GenericSuccessResponse> PostLobbiesLeaveAsync(LeaveLobbyEvent lobby)
        => await GeneratedClient.LeaveAsync(lobby);

    public async Task<GenericSuccessResponse> PostLobbiesStopAsync(StopLobbyEvent lobby)
        => await GeneratedClient.StopAsync(lobby);

    public async Task<GenericSuccessResponse> PostPairClientAsync(PairRequest pair)
        => await GeneratedClient.ClientAsync(pair);
    
    public async Task<GenericSuccessResponse> PostProfilesAddAsync(Profile profile)
        => await GeneratedClient.Add2Async(profile);
    
    public async Task<GenericSuccessResponse> PostProfilesRemoveAsync(ProfileRemoveRequest profile)
        => await GeneratedClient.RemoveAsync(profile);
    
    public async Task<GenericSuccessResponse> PostRunnerStartAsync(RunnerStartRequest runner)
        => await GeneratedClient.StartAsync(runner);
    
    public async Task<StreamSessionCreated> PostSessionsAddAsync(StreamSession session)
        => await GeneratedClient.Add3Async(session);

    public async Task<GenericSuccessResponse> PostSessionsInputAsync(
        StreamSessionHandleInputRequest input)
        => await GeneratedClient.InputAsync(input);
    
    public async Task<GenericSuccessResponse> PostSessionsPauseAsync(StreamSessionPauseRequest session)
        => await GeneratedClient.PauseAsync(session);
    
    public async Task<GenericSuccessResponse> PostSessionsStartAsync(StreamSessionStartRequest session)
        => await GeneratedClient.Start2Async(session);
    
    public async Task<GenericSuccessResponse> PostSessionsStopAsync(StreamSessionStopRequest session)
        => await GeneratedClient.Stop2Async(session);
    
    public async Task<GenericSuccessResponse> PostUnpairClientAsync(UnpairClientRequest unpair)
        => await GeneratedClient.Client2Async(unpair);
}