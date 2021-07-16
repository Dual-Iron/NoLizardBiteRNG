using BepInEx;
using RWCustom;
using System.Collections.Generic;

namespace NoLizardBiteRNG
{
    [BepInPlugin("com.github.dual.nolizardbiterng", "NoLizardBiteRNG", "1.2.0")]
    public class NoLizardBiteRNGPlugin : BaseUnityPlugin
    {
        public static object LoadOI() => new Config();

        public static NoLizardBiteRNGPlugin? Instance { get; private set; }

        private readonly Dictionary<int, LizardDamage> players = new();

        public static float BiteDamageMultiplier = 1;
        public static float Cooldown = int.MaxValue;

        public void OnEnable()
        {
            Instance = this;

            On.Lizard.Bite += Lizard_Bite;
            On.Player.Update += Player_Update;
            On.ProcessManager.SwitchMainProcess += ProcessManager_SwitchMainProcess;
        }

        private void ProcessManager_SwitchMainProcess(On.ProcessManager.orig_SwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID ID)
        {
            orig(self, ID);
            players.Clear();
        }

        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            // Decrease accumulated lizard damage
            if (self.grabbedBy?.Count == 0 && players.ContainsKey(self.playerState.playerNumber))
            {
                ref var cooldown = ref players[self.playerState.playerNumber].RegenCooldown;
                cooldown -= 1 / 40f;
                if (cooldown <= 0)
                {
                    players.Remove(self.playerState.playerNumber);
                }
            }
        }

        private void Lizard_Bite(On.Lizard.orig_Bite orig, Lizard self, BodyChunk chunk)
        {
            if (chunk.owner is Player player)
            {
                // Apply that damage.
                if (!players.ContainsKey(player.playerState.playerNumber))
                {
                    players.Add(player.playerState.playerNumber, new LizardDamage());
                }

                // Bite instant-kill behavior should never happen.
                var temp = self.lizardParams.biteDamageChance;

                players[player.playerState.playerNumber].Damage += temp * BiteDamageMultiplier;
                players[player.playerState.playerNumber].RegenCooldown = Cooldown;

                self.lizardParams.biteDamageChance = players[player.playerState.playerNumber].Damage >= 1 ? 1 : 0;
                orig(self, chunk);
                self.lizardParams.biteDamageChance = temp;


            }
            else orig(self, chunk);
        }
    }
}
