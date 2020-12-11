using OptionalUI;
using UnityEngine;

namespace NoLizardBiteRNG
{
    public class Config : OptionInterface
	{
		public Config() : base(NoLizardBiteRNG.Instance) { }

        public override bool Configuable() => true;

        public override void ConfigOnChange()
        {
            NoLizardBiteRNG.BiteDamageMultiplier = int.Parse(config["D1"]) / 10;
            NoLizardBiteRNG.Cooldown = int.Parse(config["D2"]);
            if (NoLizardBiteRNG.Cooldown < 0)
                NoLizardBiteRNG.Cooldown = int.MaxValue;
        }

        public override void Initialize()
		{
			base.Initialize();

            Tabs = new OpTab[] { new OpTab("Config") };

            const string desc = 
                "+ When bit by a lizard, instead of randomly killing the player, the player takes damage." +
                "\n+ Too much damage equals death." +
                "\n+ After some time being conscious and without being bit, damage is reset. " +
                "\n+ The amount of damage taken per bite is based off how likely the lizard is to kill you normally, so by default a 1/3 chance to kill becomes a consistent 3 bites to kill.";

            var labelAuthor = new OpLabel(20, 600 - 30, "by Dual", true);
            var labelVersion = new OpLabel(20, 600 - 30 - 40, "github.com/Dual-Iron/");
            var labelNote = new OpLabel(200, 600 - 30 - 20, "Testing the waters with this mod. It's been pleasant so far.");

            var pos = new Vector2(300, 300 - 250 / 2);
            var rectDescription = new OpRect(pos, new Vector2(250, 200)) { description = "Description" };
            Tabs[0].AddItems(rectDescription);

            var labelDescription = new OpLabelLong(pos + new Vector2(10, 0), rectDescription.size - Vector2.one * 20, desc, true, FLabelAlignment.Left);

            var top = 300;
            var labelDmgMul = new OpLabel(20, 600 - top, "Lizard damage multiplier");
            var draggerDmgMul = new OpDragger(new Vector2(220, 600 - top), "D1", 10)
            {
                description = "Makes you take more/less damage per lizard bite (value is divided by 10).",
                min = 0,
                max = 50
            };
            var labelDmgRegen = new OpLabel(20, 600 - top - 30, "Lizard damage regen cooldown");
            var draggerDmgRegen = new OpDragger(new Vector2(220, 600 - top - 30), "D2", -1)
            {
                description = "The time being conscious it takes to reset damage after being bit by a lizard (value is in seconds). Value -1 means damage is only reset at the start of a cycle.",
                min = -1,
                max = 60,
            };

            Tabs[0].AddItems(
                labelDescription,
                labelAuthor,
                labelVersion,
                labelNote,
                labelDmgMul,
                draggerDmgMul,
                labelDmgRegen,
                draggerDmgRegen
                );
		}
	}
}
