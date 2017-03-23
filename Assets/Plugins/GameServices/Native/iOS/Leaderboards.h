#pragma once
#include <string>
#include <vector>
#include "GameServicesDefinition.h"

namespace GameServices
{
    struct LeaderboardDefinition : GameServicesDefinition {};

    struct LeaderboardEntry
    {
        std::string userId;
        std::string region;
        float score;
        int rank;
    };

    struct Leaderboard
    {
        std::string id;
        std::vector<LeaderboardEntry*> entries;
        ~Leaderboard();
    };

    typedef void (* LeaderboardCallback)(Leaderboard* leaderboard);
    typedef void (* LeaderboardErrorCallback)(const std::string& leaderboardId, int statusCode);
    typedef void (* LeaderboardPositionCallback)(int totalEntries, int position);
    typedef void (* LeaderboardValueUpdatedCallback)(const std::string& leaderboardId, float value);

    class Leaderboards
    {
    public:

        /*PROP*/
        static int GetLeaderboardDefinitionsCount();
        static LeaderboardDefinition* GetLeaderboardDefinition(int index);
        static void SetLeaderboardCallbacks(LeaderboardCallback leaderboardLoadedCallback, LeaderboardPositionCallback positionCallback, LeaderboardErrorCallback leaderboardErrorCallback, LeaderboardValueUpdatedCallback leaderboardValueUpdatedCallback);
        static void GetLeaderboardAsync(const std::string& leaderboardId, int startIndex, int endIndex);
        static void GetFriendsLeaderboardAsync(const std::string& leaderboardId, int startIndex, int endIndex);
        static void GetPositionInLeaderboardAsync(const std::string& leaderboardId);
        static float GetLeaderboardValue(const std::string& leaderboardId);
    };
}
