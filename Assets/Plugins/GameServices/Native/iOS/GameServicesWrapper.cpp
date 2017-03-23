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

typedef void (* LeaderboardCallback_bridge)(const Leaderboard* leaderboardPointer, int callbackId);

struct LeaderboardCallbackContext
{
    LeaderboardCallback_bridge callback;
    int managedCallbackId;
};

static void OnLeaderboardLoaded(const Leaderboard* leaderboard, void* contextPtr)
{
    LeaderboardCallbackContext* context = (LeaderboardCallbackContext*)contextPtr;
    context->callback(leaderboard, context->managedCallbackId);
    delete context;
}

extern "C" EXPORT void LeaderboardGetLeaderboardWithName(const char* name, LeaderboardCallback_bridge callback, int callbackId)
{
    LeaderboardCallbackContext* context = new LeaderboardCallbackContext();
    context->callback = callback;
    context->managedCallbackId = callbackId;
    Leaderboard::GetLeaderboard(name, OnLeaderboardLoaded, context);
}

extern "C" EXPORT void LeaderboardGetLeaderboardWithId(const char* id, LeaderboardCallback_bridge callback, int callbackId)
{
    LeaderboardCallbackContext* context = new LeaderboardCallbackContext();
    context->callback = callback;
    context->managedCallbackId = callbackId;
    Leaderboard::GetLeaderboardWithId(id, OnLeaderboardLoaded, context);
}

typedef void (* LeaderboardsCallback_bridge)(const Leaderboard* leaderboardsPointer, int count, int callbackId);

struct LeaderboardsCallbackContext
{
    LeaderboardsCallback_bridge callback;
    int managedCallbackId;
};

static void OnLeaderboardsLoaded(const Leaderboard* leaderboards, int count, void* contextPtr)
{
    LeaderboardsCallbackContext* context = (LeaderboardsCallbackContext*)contextPtr;
    context->callback(leaderboards, count, context->managedCallbackId);
    delete context;
}

extern "C" EXPORT void LeaderboardGetLeaderboards(LeaderboardsCallback_bridge callback, int callbackId)
{
    LeaderboardsCallbackContext* context = new LeaderboardsCallbackContext();
    context->callback = callback;
    context->managedCallbackId = callbackId;
    Leaderboard::GetLeaderboards(OnLeaderboardsLoaded, context);
}

typedef void (* LeaderboardEntryCallback_bridge)(const Leaderboard* leaderboard, const LeaderboardEntry* entries, int count, int callbackId);

struct LeaderboardEntryCallbackContext
{
    LeaderboardEntryCallback_bridge callback;
    int managedCallbackId;
};

static void OnLeaderboardScoresLoaded(const Leaderboard* leaderboard, const LeaderboardEntry* entries, int count, void* contextPtr)
{
    LeaderboardEntryCallbackContext* context = (LeaderboardEntryCallbackContext*)contextPtr;
    context->callback(leaderboard, entries, count, context->managedCallbackId);
    delete context;
}

extern "C" EXPORT void LeaderboardGetScores(const Leaderboard* leaderboard, int index, int count, LeaderboardEntryCallback_bridge callback, int callbackId)
{
    LeaderboardEntryCallbackContext* context = new LeaderboardEntryCallbackContext();
    context->callback = callback;
    context->managedCallbackId = callbackId;
    leaderboard->GetScores(index, count, OnLeaderboardScoresLoaded, context);
}

extern "C" EXPORT void LeaderboardGetFriendsScores(const Leaderboard* leaderboard, int index, int count, LeaderboardEntryCallback_bridge callback, int callbackId)
{
    LeaderboardEntryCallbackContext* context = new LeaderboardEntryCallbackContext();
    context->callback = callback;
    context->managedCallbackId = callbackId;
    leaderboard->GetFriendsScores(index, count, OnLeaderboardScoresLoaded, context);
}

typedef void (* LeaderboardPositionCallback_bridge)(const Leaderboard* leaderboard, int totalEntries, int position, int callbackId);

struct LeaderboardPositionCallbackContext
{
    LeaderboardPositionCallback_bridge callback;
    int managedCallbackId;
};

static void OnLeaderboardPositionLoaded(const Leaderboard* leaderboard, int totalEntries, int position, void* contextPtr)
{
    LeaderboardPositionCallbackContext* context = (LeaderboardPositionCallbackContext*)contextPtr;
    context->callback(leaderboard, totalEntries, position, context->managedCallbackId);
    delete context;
}

extern "C" EXPORT void LeaderboardGetPosition(const Leaderboard* leaderboard, LeaderboardPositionCallback_bridge callback, int callbackId)
{
    LeaderboardPositionCallbackContext* context = new LeaderboardPositionCallbackContext();
    context->callback = callback;
    context->managedCallbackId = callbackId;
    leaderboard->GetPosition(OnLeaderboardPositionLoaded, context);
}

static LeaderboardValueUpdatedCallback s_LeaderboardValueUpdatedCallback = NULL;

static void OnLeaderboardValueUpdated(const Leaderboard* leaderboard, float value)
{
    if (s_LeaderboardValueUpdatedCallback)
        s_LeaderboardValueUpdatedCallback(leaderboard, value);
}

extern "C" EXPORT void LeaderboardSetValueUpdatedCallback(LeaderboardValueUpdatedCallback callback)
{
    s_LeaderboardValueUpdatedCallback = callback;
    Leaderboard::SetValueUpdatedCallback(OnLeaderboardValueUpdated);
}

extern "C" EXPORT float LeaderboardGetValue(const Leaderboard* leaderboard)
{
    return leaderboard->GetValue();
}

extern "C" EXPORT const char* LeaderboardGetId(const Leaderboard * leaderboard)
{
    return strdup(leaderboard->GetId().c_str());
}

extern "C" EXPORT const char* LeaderboardGetName(const Leaderboard * leaderboard)
{
    return strdup(leaderboard->GetName().c_str());
}

extern "C" EXPORT const char* LeaderboardGetDescription(const Leaderboard * leaderboard)
{
    return strdup(leaderboard->GetName().c_str());
}
