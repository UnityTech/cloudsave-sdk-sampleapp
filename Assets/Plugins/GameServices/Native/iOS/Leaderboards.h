#pragma once
#include <string>
#include <vector>

namespace GameServices
{
    struct LeaderboardEntry
    {
        char userId[32];
        char username[32];
        char region[32];
        float score;
        int rank;
        LeaderboardEntry()
        {
            userId[0] = 0;
            region[0] = 0;
            username[0] = 0;
            score = 0;
            rank = -1;
        }
    };

    class Leaderboard;
    class LeaderboardsPrivate;
    
    typedef void (*LeaderboardsCallback)(const Leaderboard* leaderboards, int count, void* context);
    typedef void (*LeaderboardCallback)(const Leaderboard* leaderboard, void* context);
    typedef void (*LeaderboardEntryCallback)(const Leaderboard* leaderboard, const LeaderboardEntry* entries, int count, void* context);
    typedef void (*LeaderboardPositionCallback)(const Leaderboard* leaderboard, int totalEntries, int position, void* context);
    typedef void (*LeaderboardValueUpdatedCallback)(const Leaderboard* leaderboard, float value);

    class Leaderboard
    {
    public:
        static void GetLeaderboards(LeaderboardsCallback callback, void* context);
        static void GetLeaderboard(const std::string& name, LeaderboardCallback callback, void* context);
        static void GetLeaderboardWithId(const std::string& id, LeaderboardCallback callback, void* context);
        void GetScores(int index, int count, LeaderboardEntryCallback callback, void* context) const;
        void GetFriendsScores(int index, int count, LeaderboardEntryCallback callback, void* context) const;
        void GetPosition(LeaderboardPositionCallback callback, void* context) const;
        static void SetValueUpdatedCallback(LeaderboardValueUpdatedCallback callback);
        float GetValue() const;
        const std::string& GetId() const;
        const std::string& GetName() const;
        const std::string& GetDescription() const;

    private:
        friend class LeaderboardsPrivate;
        Leaderboard(const Leaderboard&);
        Leaderboard& operator=(const Leaderboard&);
        Leaderboard();
        ~Leaderboard();
        std::string m_Id;
        std::string m_Name;
        std::string m_PlatformId;
        std::string m_Description;
    };
}
