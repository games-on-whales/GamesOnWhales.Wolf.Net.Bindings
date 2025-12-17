namespace GamesOnWhales;

public interface IWolfApi
{
    GeneratedApiClient GeneratedClient { get; }
    
    Task<ICollection<App>> GetAppsAsync();
    Task<ICollection<PairedClient>> GetClientsAsync();
    Task<ImageInspect> GetDockerImagesInspectAsync(string imageName);
    Task<ICollection<Lobby>> GetLobbiesAsync();
    Task<ICollection<PendingPairClient>> GetPendingPairRequestsAsync();
    Task<ICollection<Profile>> GetProfilesAsync();
    Task<ICollection<StreamSession>> GetSessionsAsync();
    Task<Stream?> GetUtilsGetIconAsync(string iconPath);
    Task<GenericSuccessResponse> PostAppsAddAsync(App app);
    Task<GenericSuccessResponse> PostAppsDeleteAsync(App app);
    Task<GenericSuccessResponse> PostAppsDeleteAsync(string appId);
    Task<GenericSuccessResponse> PostClientSettingsAsync(UpdateClientSettingsRequest clientSettings);
    Task PostDockerImagesPullAsync(string imageName);
    Task<LobbyCreateResponse> PostLobbiesCreate(CreateLobbyRequest lobby);
    Task<GenericSuccessResponse> PostLobbiesJoinAsync(JoinLobbyEvent lobby);
    Task<GenericSuccessResponse> PostLobbiesLeaveAsync(LeaveLobbyEvent lobby);
    Task<GenericSuccessResponse> PostLobbiesStopAsync(StopLobbyEvent lobby);
    Task<GenericSuccessResponse> PostPairClientAsync(PairRequest pair);
    Task<GenericSuccessResponse> PostProfilesAddAsync(Profile profile);
    Task<GenericSuccessResponse> PostProfilesRemoveAsync(ProfileRemoveRequest profile);
    Task<GenericSuccessResponse> PostRunnerStartAsync(RunnerStartRequest runner);
    Task<StreamSessionCreated> PostSessionsAddAsync(StreamSession session);
    Task<GenericSuccessResponse> PostSessionsInputAsync(StreamSessionHandleInputRequest input);
    Task<GenericSuccessResponse> PostSessionsPauseAsync(StreamSessionPauseRequest session);
    Task<GenericSuccessResponse> PostSessionsStartAsync(StreamSessionStartRequest session);
    Task<GenericSuccessResponse> PostSessionsStopAsync(StreamSessionStopRequest session);
    Task<GenericSuccessResponse> PostUnpairClientAsync(UnpairClientRequest unpair);
}