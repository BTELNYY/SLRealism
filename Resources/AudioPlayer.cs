using SCPSLAudioApi.AudioCore;
using VoiceChat;

namespace SLRealism
{
    public class AudioPlayer : AudioPlayerBase
    {
        public static AudioPlayer Get(ReferenceHub hub)
        {
            if (AudioPlayers.TryGetValue(hub, out AudioPlayerBase player))
            {
                if (player is AudioPlayer scp575Player1)
                    return scp575Player1;
            }

            var scp575Player = hub.gameObject.AddComponent<AudioPlayer>();
            scp575Player.Owner = hub;
            scp575Player.BroadcastChannel = VoiceChatChannel.Proximity;

            AudioPlayers.Add(hub, scp575Player);
            return scp575Player;
        }
    }
}