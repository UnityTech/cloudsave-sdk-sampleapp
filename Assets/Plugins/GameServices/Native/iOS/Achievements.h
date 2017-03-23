#pragma once
#include <string>
#include "GameServicesDefinition.h"

namespace GameServices
{
    struct AchievementDefinition : GameServicesDefinition
    {
        bool permitLaterClaim;
        int status;
        int displayOrder;
    };

    struct AchievementStatus
    {
        std::string id;
        float progress;
        float maxProgress;
        bool unlocked;
        bool claimed;
    };
    
    typedef void (*AchievementUnlockedCallback)(const std::string& id);
    
    class Achievements
    {
    public:
        
        /*PROP*/
        static int GetAchievementDefinitionsCount();
        static AchievementDefinition* GetAchievementDefinition(int index);
        
        static void SetAchievementUnlockedCallback(AchievementUnlockedCallback unlockedCallback);
        static AchievementStatus GetStatus(const std::string& id);
        static void ClaimAchievement(const std::string& id);
    };
}
