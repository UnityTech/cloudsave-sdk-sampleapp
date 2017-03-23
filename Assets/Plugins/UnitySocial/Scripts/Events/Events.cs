using System.Collections.Generic;
using UnityEngine.Events;
using UnitySocial.Entities;

namespace UnitySocial
{
namespace Events
{
    /// <summary>
    /// Generic event callback
    /// </summary>
    public delegate void GenericCallback<TResult>(object err, TResult result);

    /// <summary>
    /// <see cref="UnityEvent"/> callback for notfying about new challenge to be started
    /// </summary>
    public class ChallengeStartedEvent : UnityEvent<ChallengeStatus>
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> callback for notfying that initialization has completed
    /// </summary>
    public class SocialCoreInitializedEvent : UnityEvent<bool>
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> callback for notfying that reward has been claimed.
    /// </summary>
    public class SocialCoreRewardClaimedEvent : UnityEvent<Dictionary<string, object>>
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> callback for when entry point status is updated.
    /// </summary>
    public class EntryPointStateUpdatedEvent : UnityEvent<EntryPointState>
    {
    }
}
}
