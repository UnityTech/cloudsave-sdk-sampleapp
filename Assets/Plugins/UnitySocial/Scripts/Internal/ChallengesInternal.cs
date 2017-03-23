using System;
using System.Collections.Generic;
using UnityEngine;
using UnitySocial.Entities;

namespace UnitySocial
{
namespace Internal
{
    internal partial class UnitySocialBridge : MonoBehaviour
    {
        private User DeserializeUser(object userObject)
        {
            var user = new User();
            var userDictionary = userObject as Dictionary<string, object>;

            user.id = userDictionary["id"] as string;
            user.username = userDictionary["username"] as string;
            user.avatarURL = userDictionary["avatar_url"] as string;

            return user;
        }

        private ChallengeScore GetChallengeScoreFromDictionary(Dictionary<string, object> dict, string key)
        {
            ChallengeScore score = new ChallengeScore();
            if (dict["best_score"] != null && UnitySocial.Tools.DictionaryExtensions.TryGetValue(dict, key, out score.value))
            {
                return score;
            }

            return null;
        }

        private ChallengeOpponent[] DeserializeChallengeParticipants(object opponentsObject)
        {
            var rawParticipants = opponentsObject as List<object>;
            var participants = new List<ChallengeOpponent>();

            foreach (var rawParticipant in rawParticipants)
            {
                var participant = new ChallengeOpponent();
                Dictionary<string, object> participantDictionary = rawParticipant as Dictionary<string, object>;
                Dictionary<string, object> stateDictionary = participantDictionary["state"] as Dictionary<string, object>;
                participant.score = GetChallengeScoreFromDictionary(stateDictionary, "best_score");
                participant.user = DeserializeUser(participantDictionary["user"]);

                participants.Add(participant);
            }

            return participants.ToArray();
        }

        private ChallengeStatus DeserializeChallengeStatus(object challengeObject, object metadataObject)
        {
            Dictionary<string, object> challengeDictionary = challengeObject as Dictionary<string, object>;
            Dictionary<string, object> metadataDictionary = metadataObject as Dictionary<string, object>;

            var status = new ChallengeStatus();
            status.id = challengeDictionary["id"] as string;
            status.name = challengeDictionary["template_name"] as string;
            status.description = challengeDictionary["template_description"] as string;
            status.templateImageURL = challengeDictionary["template_image"] as string;
            status.metadata = metadataDictionary;
            status.opponents = DeserializeChallengeParticipants(challengeDictionary["participants"]);
            status.extra = challengeDictionary["extra"] as Dictionary<string, object>;

            return status;
        }

        private void UnitySocialChallengeStarted(string challengeAndMetadata)
        {
            try
            {
                Dictionary<string, object> dictionary = UnitySocial.Tools.DictionaryExtensions.JsonToDictionary(challengeAndMetadata);
                var status = DeserializeChallengeStatus(dictionary["challenge"], dictionary["metadata"]);
                Challenges.onChallengeStarted.Invoke(status);
            }
            catch (Exception)
            {
                // received invalid data - don't crash
                return;
            }
        }
    }
}
}
