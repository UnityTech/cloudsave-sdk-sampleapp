using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnitySocial
{
    /// <summary>
    /// Represents a challenge and its status
    /// </summary>
    public class ChallengeStatus
    {
        /// <summary>
        /// Unique identifier of the challenge
        /// </summary>
        public string id;

        /// <summary>
        /// Name of the challenge, as assigned in the challenge template
        /// </summary>
        public string name;

        /// <summary>
        /// Description of the challenge, as assigned in the challenge template
        /// </summary>
        public string description;
        /// <summary>
        /// Unique identifier of the challenge template
        /// </summary>
        public string templateId;

        /// <summary>
        /// The thumbnail image of the challenge, as assigned in the challenge template
        /// </summary>
        public string templateImageURL;

        /// <summary>
        /// Additional free-form data assigned to the challenge template
        /// </summary>
        public Dictionary<string, object> metadata;

        /// <summary>
        /// An array of the opponents of the current user in the challenge
        /// </summary>
        public ChallengeOpponent[] opponents;

        /// <summary>
        /// The current user's best score in the challenge
        /// </summary>
        public ChallengeScore bestScore;

        /// <summary>
        /// Extra information about the challenge
        /// </summary>
        public Dictionary<string, object> extra;
    }

    /// <summary>
    /// Represents an opponent in a challenge
    /// </summary>
    public class ChallengeOpponent
    {
        /// <summary>
        /// The user object of the opponent
        /// </summary>
        public User user;

        /// <summary>
        /// The score of the opponent
        /// </summary>
        public ChallengeScore score;
    }

    /// <summary>
    /// Represents a score in a challenge
    /// </summary>
    public class ChallengeScore
    {
        /// <summary>
        /// The actual score value
        /// </summary>
        public double value;

        /// <summary>
        /// Not available yet
        /// </summary>
        public object payload;
    }

    public static class Challenges
    {
        private static ChallengeStartedEvent s_ChallengeStarted = new ChallengeStartedEvent();

        /// <summary>
        /// Occurs when a new challenge should start
        /// </summary>
        public static ChallengeStartedEvent onChallengeStarted { get { return s_ChallengeStarted; } }

        /// <summary>
        /// <see cref="UnityEvent"/> callback for notfying about new challenge to be started
        /// </summary>
        public class ChallengeStartedEvent : UnityEvent<ChallengeStatus> {}
    }
}

namespace UnitySocialInternal
{
    internal partial class UnitySocialBridge : MonoBehaviour
    {
        private UnitySocial.ChallengeOpponent[] FilterOutCurrentUser(UnitySocial.ChallengeOpponent[] opponents)
        {
            var currentUserID = UnitySocial.Identity.DefaultProvider().currentCredential.userId;
            var result = new List<UnitySocial.ChallengeOpponent>();

            foreach (var opponent in opponents)
            {
                if (opponent.user.id != currentUserID)
                {
                    result.Add(opponent);
                }
            }

            return result.ToArray();
        }

        private UnitySocial.User DeserializeUser(object userObject)
        {
            var user = new UnitySocial.User();
            var userDictionary = userObject as Dictionary<string, object>;

            user.id = userDictionary["id"] as string;
            user.username = userDictionary["username"] as string;
            user.avatarURL = userDictionary["avatar_url"] as string;

            return user;
        }

        private UnitySocial.ChallengeScore GetChallengeScoreFromDictionary(Dictionary<string, object> dict, string key)
        {
            UnitySocial.ChallengeScore score = new UnitySocial.ChallengeScore();
            if (dict["best_score"] != null && UnitySocialTools.DictionaryExtensions.TryGetValue(dict, key, out score.value))
            {
                return score;
            }

            return null;
        }

        private UnitySocial.ChallengeOpponent[] DeserializeChallengeParticipants(object opponentsObject)
        {
            var rawParticipants = opponentsObject as List<object>;
            var participants = new List<UnitySocial.ChallengeOpponent>();

            foreach (var rawParticipant in rawParticipants)
            {
                var participant = new UnitySocial.ChallengeOpponent();
                Dictionary<string, object> participantDictionary = rawParticipant as Dictionary<string, object>;
                Dictionary<string, object> stateDictionary = participantDictionary["state"] as Dictionary<string, object>;
                participant.score = GetChallengeScoreFromDictionary(stateDictionary, "best_score");
                participant.user = DeserializeUser(participantDictionary["user"]);

                participants.Add(participant);
            }

            return participants.ToArray();
        }

        private UnitySocial.ChallengeStatus DeserializeChallengeStatus(object challengeObject, object metadataObject)
        {
            Dictionary<string, object> challengeDictionary = challengeObject as Dictionary<string, object>;
            Dictionary<string, object> metadataDictionary = metadataObject as Dictionary<string, object>;

            var status = new UnitySocial.ChallengeStatus();
            status.id = challengeDictionary["id"] as string;
            status.name = challengeDictionary["template_name"] as string;
            status.description = challengeDictionary["template_description"] as string;
            status.templateImageURL = challengeDictionary["template_image"] as string;
            status.metadata = metadataDictionary;
            status.opponents = FilterOutCurrentUser(DeserializeChallengeParticipants(challengeDictionary["participants"]));
            status.bestScore = GetChallengeScoreFromDictionary(challengeDictionary, "best_score");
            status.extra = challengeDictionary["extra"] as Dictionary<string, object>;

            return status;
        }

        private void UnitySocialChallengeStarted(string challengeAndMetadata)
        {
            try
            {
                Dictionary<string, object> dictionary = UnitySocialTools.DictionaryExtensions.JsonToDictionary(challengeAndMetadata);
                var status = DeserializeChallengeStatus(dictionary["challenge"], dictionary["metadata"]);
                UnitySocial.Challenges.onChallengeStarted.Invoke(status);
            }
            catch (Exception)
            {
                // received invalid data - don't crash
                return;
            }
        }
    }
}
