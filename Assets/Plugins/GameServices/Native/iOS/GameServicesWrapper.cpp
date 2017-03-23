#include <map>
#include <string>
#include <vector>

#include "PlaySession.h"
#include "Achievements.h"
#include "Leaderboards.h"
#define EXPORT __attribute__((visibility("default")))


using namespace::GameServices;


extern "C" EXPORT void PlaySessionSendEvent(const char** sessionEvent_keys, float* sessionEvent_values, int sessionEvent_length, const char** tags_p, int tags_length)
{
    std::map<std::string, float> sessionEvent;
    for (int i = 0; i < sessionEvent_length; i++)
    {
        sessionEvent[sessionEvent_keys[i]] = sessionEvent_values[i];
    }
    std::vector<std::string> tags;
    for (int i = 0; i < tags_length; i++)
    {
        tags.push_back(tags_p[i]);
    }
    PlaySession::SendEvent(sessionEvent, tags);
}

extern "C" EXPORT void PlaySessionSendEvent1(const char* key, float value, const char** tags_p, int tags_length)
{
    std::vector<std::string> tags;
    for (int i = 0; i < tags_length; i++)
    {
        tags.push_back(tags_p[i]);
    }
    PlaySession::SendEvent(key, value, tags);
}

extern "C" EXPORT void PlaySessionBegin()
{
    PlaySession::Begin();
}

extern "C" EXPORT void PlaySessionEnd()
{
    PlaySession::End();
}

extern "C" EXPORT void PlaySessionCancel()
{
    PlaySession::Cancel();
}

extern "C" EXPORT void PlaySessionActivateTag(const char* key)
{
    PlaySession::ActivateTag(key);
}

extern "C" EXPORT void PlaySessionDeactivateTag(const char* key)
{
    PlaySession::DeactivateTag(key);
}

extern "C" EXPORT void PlaySessionPause()
{
    PlaySession::Pause();
}

extern "C" EXPORT void PlaySessionResume()
{
    PlaySession::Resume();
}

extern "C" EXPORT bool PlaySessionIsActive()
{
    return PlaySession::IsActive();
}

extern "C" EXPORT bool PlaySessionIsPaused()
{
    return PlaySession::IsPaused();
}

extern "C" EXPORT int AchievementsGetAchievementDefinitionsCount()
{
    return Achievements::GetAchievementDefinitionsCount();
}

struct AchievementDefinition_bridge
{
    const char* id;
    const char* platformId;
    const char* name;
    const char* description;
    bool permitLaterClaim;
    int status;
    int displayOrder;
};

extern "C" EXPORT AchievementDefinition_bridge * AchievementsGetAchievementDefinition(int index)
{
    AchievementDefinition* definition = Achievements::GetAchievementDefinition(index);
    if (!definition)
        return NULL;
    AchievementDefinition_bridge* brdige = new AchievementDefinition_bridge();
    //TODO: dealloc strings???
    brdige->id = definition->id.c_str();
    brdige->platformId = definition->platformId.c_str();
    brdige->name = definition->name.c_str();
    brdige->description = definition->description.c_str();
    brdige->permitLaterClaim = definition->permitLaterClaim;
    brdige->status = definition->status;
    brdige->displayOrder = definition->displayOrder;
    return brdige;
}

extern "C" EXPORT void AchievementsSetAchievementUnlockedCallback(AchievementUnlockedCallback unlockedCallback)
{
    Achievements::SetAchievementUnlockedCallback(unlockedCallback);
}

struct AchievementStatus_bridge
{
    const char* id;
    float progress;
    float maxProgress;
    bool unlocked;
    bool claimed;
};

extern "C" EXPORT AchievementStatus_bridge * AchievementsGetStatus(const char* id)
{
    AchievementStatus_bridge* bridge = new AchievementStatus_bridge();
    AchievementStatus status = Achievements::GetStatus(id);
    bridge->id = status.id.c_str();
    bridge->progress = status.progress;
    bridge->maxProgress = status.maxProgress;
    bridge->unlocked = status.unlocked;
    bridge->claimed = status.claimed;
    return bridge;
}

extern "C" EXPORT void AchievementsClaimAchievement(const char* id)
{
    Achievements::ClaimAchievement(id);
}

extern "C" EXPORT int LeaderboardsGetLeaderboardDefinitionsCount()
{
    return Leaderboards::GetLeaderboardDefinitionsCount();
}

struct LeaderboardDefinition_bridge
{
    const char* id;
    const char* platformId;
    const char* name;
    const char* description;
};

extern "C" EXPORT LeaderboardDefinition_bridge * LeaderboardsGetLeaderboardDefinition(int index)
{
    LeaderboardDefinition* definition = Leaderboards::GetLeaderboardDefinition(index);
    if (!definition)
        return NULL;
    LeaderboardDefinition_bridge* bridge = new LeaderboardDefinition_bridge();
    bridge->id = definition->id.c_str();
    bridge->platformId = definition->id.c_str();
    bridge->name = definition->name.c_str();
    bridge->description = definition->description.c_str();
    return bridge;
}

struct LeaderboardEntry_bridge
{
    const char* userId;
    const char* region;
    float score;
    int rank;
};

struct Leaderboard_bridge
{
    const char* id;
    LeaderboardEntry* entries;
    int numEntries;
};

static Leaderboard_bridge* CreateLeaderboardBridge(Leaderboard* leaderboard)
{
    Leaderboard_bridge* bridge = new Leaderboard_bridge();
    bridge->entries = new LeaderboardEntry[leaderboard->entries.size()];
    bridge->numEntries = leaderboard->entries.size();
    bridge->id = leaderboard->id.c_str();
    for (int i = 0; i < leaderboard->entries.size(); i++)
    {
        bridge->entries[i] = *leaderboard->entries[i];
    }
    return bridge;
}

typedef void (* LeadeboardBrdigeCallback)(Leaderboard_bridge*);
typedef void (* LeaderboardBridgeErrorCallback)(const char* leaderboardId, int statusCode);
typedef void (* LeaderboardBrdigeValueUpdatedCallback)(const char* leaderboardId, float value);

static LeadeboardBrdigeCallback s_LeaderboardBridgeLoaded = NULL;
static void OnLeaderboardLoaded(Leaderboard* leaderboard)
{
    if (!leaderboard || !s_LeaderboardBridgeLoaded)
        return;
    s_LeaderboardBridgeLoaded(CreateLeaderboardBridge(leaderboard));
}

static LeaderboardBridgeErrorCallback s_LeaderboardBridgeError = NULL;
static void OnLeaderboardError(const std::string& leaderboardId, int statusCode)
{
    if (s_LeaderboardBridgeError)
        s_LeaderboardBridgeError(leaderboardId.c_str(), statusCode);
}

static LeaderboardBrdigeValueUpdatedCallback s_LeaderboardBridgeValueUpdated = NULL;
static void OnLeaderboardValueUpdated(const std::string& leaderboardId, float value)
{
    if (s_LeaderboardBridgeValueUpdated)
        s_LeaderboardBridgeValueUpdated(leaderboardId.c_str(), value);
}

extern "C" EXPORT void LeaderboardsSetLeaderboardCallbacks(LeadeboardBrdigeCallback leaderboardLoadedCallback, LeaderboardPositionCallback positionCallback, LeaderboardBridgeErrorCallback leaderboardErrorCallback, LeaderboardBrdigeValueUpdatedCallback leaderboardValueUpdatedCallback)
{
    s_LeaderboardBridgeLoaded = leaderboardLoadedCallback;
    s_LeaderboardBridgeError = leaderboardErrorCallback;
    s_LeaderboardBridgeValueUpdated = leaderboardValueUpdatedCallback;
    Leaderboards::SetLeaderboardCallbacks(OnLeaderboardLoaded, positionCallback, OnLeaderboardError, OnLeaderboardValueUpdated);
}

extern "C" EXPORT void LeaderboardsGetLeaderboardAsync(const char* leaderboardId, int startIndex, int endIndex)
{
    Leaderboards::GetLeaderboardAsync(leaderboardId, startIndex, endIndex);
}

extern "C" EXPORT void LeaderboardsGetFriendsLeaderboardAsync(const char* leaderboardId, int startIndex, int endIndex)
{
    Leaderboards::GetFriendsLeaderboardAsync(leaderboardId, startIndex, endIndex);
}

extern "C" EXPORT void LeaderboardsGetPositionInLeaderboardAsync(const char* leaderboardId)
{
    Leaderboards::GetPositionInLeaderboardAsync(leaderboardId);
}

extern "C" EXPORT float LeaderboardsGetLeaderboardValue(const char* leaderboardId)
{
    return Leaderboards::GetLeaderboardValue(leaderboardId);
}
