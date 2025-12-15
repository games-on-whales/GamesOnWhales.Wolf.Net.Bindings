namespace GamesOnWhales;

public interface IWolfApi
{
    NSwagDocker.NSwagDocker GeneratedDockerApiBindings { get; }
    NSwagWolfApi.NSwagWolfApi GeneratedApiBindings { get; }
    
    Task<ICollection<NSwagWolfApi.App>> GetAppsAsync();
    Task<ICollection<NSwagWolfApi.PairedClient>> GetClientsAsync();
    Task<NSwagDocker.ImageInspect> GetDockerImagesInspectAsync(string imageName);
    Task<ICollection<NSwagWolfApi.Lobby>> GetLobbiesAsync();
    Task<ICollection<NSwagWolfApi.PendingPairClient>> GetPendingPairRequestsAsync();
    Task<ICollection<NSwagWolfApi.Profile>> GetProfilesAsync();
    Task<ICollection<NSwagWolfApi.StreamSession>> GetSessionsAsync();
    Task<Stream?> GetUtilsGetIconAsync(string iconPath);
    Task<NSwagWolfApi.GenericSuccessResponse> PostAppsAddAsync(NSwagWolfApi.App app);
    Task<NSwagWolfApi.GenericSuccessResponse> PostAppsDeleteAsync(NSwagWolfApi.App app);
    Task<NSwagWolfApi.GenericSuccessResponse> PostAppsDeleteAsync(string appId);
    Task<NSwagWolfApi.GenericSuccessResponse> PostClientSettingsAsync(NSwagWolfApi.UpdateClientSettingsRequest clientSettings);
    Task PostDockerImagesPullAsync(string imageName);
    Task<NSwagWolfApi.LobbyCreateResponse> PostLobbiesCreate(NSwagWolfApi.CreateLobbyRequest lobby);
    Task<NSwagWolfApi.GenericSuccessResponse> PostLobbiesJoinAsync(NSwagWolfApi.JoinLobbyEvent lobby);
    Task<NSwagWolfApi.GenericSuccessResponse> PostLobbiesLeaveAsync(NSwagWolfApi.LeaveLobbyEvent lobby);
    Task<NSwagWolfApi.GenericSuccessResponse> PostLobbiesStopAsync(NSwagWolfApi.StopLobbyEvent lobby);
    Task<NSwagWolfApi.GenericSuccessResponse> PostPairClientAsync(NSwagWolfApi.PairRequest pair);
    Task<NSwagWolfApi.GenericSuccessResponse> PostProfilesAddAsync(NSwagWolfApi.Profile profile);
    Task<NSwagWolfApi.GenericSuccessResponse> PostProfilesRemoveAsync(NSwagWolfApi.ProfileRemoveRequest profile);
    Task<NSwagWolfApi.GenericSuccessResponse> PostRunnerStartAsync(NSwagWolfApi.RunnerStartRequest runner);
    Task<NSwagWolfApi.StreamSessionCreated> PostSessionsAddAsync(NSwagWolfApi.StreamSession session);
    Task<NSwagWolfApi.GenericSuccessResponse> PostSessionsInputAsync(NSwagWolfApi.StreamSessionHandleInputRequest input);
    Task<NSwagWolfApi.GenericSuccessResponse> PostSessionsPauseAsync(NSwagWolfApi.StreamSessionPauseRequest session);
    Task<NSwagWolfApi.GenericSuccessResponse> PostSessionsStartAsync(NSwagWolfApi.StreamSessionStartRequest session);
    Task<NSwagWolfApi.GenericSuccessResponse> PostSessionsStopAsync(NSwagWolfApi.StreamSessionStopRequest session);
    Task<NSwagWolfApi.GenericSuccessResponse> PostUnpairClientAsync(NSwagWolfApi.UnpairClientRequest unpair);
}