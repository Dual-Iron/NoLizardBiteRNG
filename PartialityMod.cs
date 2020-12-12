using CompletelyOptional;
using OptionalUI;
using Partiality.Modloader;
using RWCustom;
using System;
using System.Collections.Generic;

namespace NoLizardBiteRNG
{
    public class NoLizardBiteRNG : PartialityMod
    {
        public NoLizardBiteRNG()
        {
            ModID = "Consistent Lizard Chomps";
            author = "Dual";
            Version = "1.0.0";
            Instance = this;
        }

        public static OptionInterface LoadOI() => new Config();

        public static NoLizardBiteRNG Instance { get; private set; }

        private readonly Dictionary<int, LizardDamage> players = new Dictionary<int, LizardDamage>();

        public static float BiteDamageMultiplier;
        public static float Cooldown;

        public override void OnEnable()
        {
            base.OnEnable();

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
                cooldown -= 1f / self.room.game.framesPerSecond;
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
                // Bite instant-kill behavior should never happen.
                var temp = self.lizardParams.biteDamageChance;
                self.lizardParams.biteDamageChance = 0;
                orig(self, chunk);
                self.lizardParams.biteDamageChance = temp;
                
                // If the lizard has a chance to do damage...
                if (temp > 0)
                {
                    // Calculate the damage for the player based on the % chance biting would kill the player.
                    var damage = temp / player.Template.baseDamageResistance;
                    if (player.Template.damageRestistances[(int)Creature.DamageType.Bite, 0] > 0)
                    {
                        damage /= player.Template.damageRestistances[(int)Creature.DamageType.Bite, 0];
                    }

                    // Apply that damage.
                    if (!players.ContainsKey(player.playerState.playerNumber))
                    {
                        players.Add(player.playerState.playerNumber, new LizardDamage());
                    }

                    players[player.playerState.playerNumber].Damage += damage * BiteDamageMultiplier;
                    players[player.playerState.playerNumber].RegenCooldown = Cooldown;

                    // If the player's accumulated damage is greater than their death limit, then die.
                    if (players[player.playerState.playerNumber].Damage >= player.Template.instantDeathDamageLimit)
                    {
                        player.Die();
                    }
                }
            }
        }
    }
}
