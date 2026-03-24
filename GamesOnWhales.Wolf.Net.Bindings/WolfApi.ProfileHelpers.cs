using System.Collections.Concurrent;

namespace GamesOnWhales;

public partial class WolfApi
{
    private readonly object _profileQueueLock = new();
    
    // default is false, set 1 for true.
    private int _threadSafeBoolBackValue = 1;
    private bool IsProfileStale
    {
        get => (Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 1) == 1);
        set
        {
            if (value) Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 0);
            else Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 0, 1);
        }
    }
    
    private readonly ConcurrentQueue<Profile> _profileQueue = new();
    public ConcurrentQueue<Profile> ProfileQueue
    {
        get
        {
            lock (_profileQueueLock)
            {
                if (!IsProfileStale) return _profileQueue;
                var profiles = GetProfilesAsync().Result;
                _profileQueue.Clear();
                foreach (var profile in profiles)
                {
                    _profileQueue.Enqueue(profile);
                }
                IsProfileStale = false;
                Emit(ProfilesUpdated, profiles);
                return _profileQueue;
            }
        }
    }
    
    public async Task<GenericSuccessResponse> AddProfile(Profile profile)
    {
        var val = await GeneratedClient.Add2Async(profile);
        IsProfileStale = true;
        return val;
    }

    public async Task<GenericSuccessResponse> DeleteProfile(Profile profile)
        => await DeleteProfile(profile.Id);

    public async Task<GenericSuccessResponse> DeleteProfile(string id)
    {
        var val = await GeneratedClient.RemoveAsync(new ProfileRemoveRequest()
        {
            Id = id
        });
        IsProfileStale = true;
        return val;
    }

    public async Task<GenericSuccessResponse> UpdateProfile(string id, Profile profile)
    {
        if(id != profile.Id) throw new ArgumentException("id must be equal to profile id", nameof(id));
        var del = await GeneratedClient.RemoveAsync(new ProfileRemoveRequest()
        {
            Id = id
        });
        if (!del.Success) return del;
        var add = await GeneratedClient.Add2Async(profile);
        IsProfileStale = true;
        return add;
    }
    
    public event Func<object, ICollection<Profile>, Task>? ProfilesUpdated;
    protected virtual Task OnProfilesUpdatedEvent(ICollection<Profile> profiles) => Task.CompletedTask;
}