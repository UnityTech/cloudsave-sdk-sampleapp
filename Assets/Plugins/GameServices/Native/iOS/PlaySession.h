#pragma once
#include <string>
#include <map>
#include <vector>

namespace GameServices
{
    typedef void (*PlaySessionInitCallback)(bool success);
    class PlaySession
    {

    public:
        static void Init(const std::string& upid, PlaySessionInitCallback callback);
        static void Init(const std::string& upid, std::string bakedAchievements, std::string bakedLeaderboards, PlaySessionInitCallback initCallback);
        
        static void Cleanup();

        static void SendEvent(const std::map<std::string, float>& sessionEvent);
        static void SendEvent(const std::string& key, float value);
        static void SendEvent(const std::map<std::string, float>& sessionEvent, const std::vector<std::string>& tags);
        static void SendEvent(const std::string& key, float value, const std::vector<std::string>& tags);

        
        static void SendEvent(const std::map<std::string, float>& sessionEvent, const std::string& topic);
        static void SendEvent(const std::map<std::string, float>& sessionEvent, const std::string& topic, const std::vector<std::string>& tags);
        
        
        static void FlushEvents();

        static void Begin();
        static void End();
        static void Cancel();
        
        static void ActivateTag(const std::string& key);
        static void DeactivateTag(const std::string& key);

        static void Pause();
        static void Resume();

        static bool IsActive();
        static bool IsPaused();
    };
}
