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
                "Mod behavior\n" +
                "\n+ When bit by a lizard, instead of randomly killing the player, the player takes damage." +
                "\n+ Too much damage equals death." +
                "\n+ Some time after being released, or after the start of a cycle, damage is reset.";
            const string vanillaDesc = 
                "Vanilla behavior\n" +
                "\n+ When not killed by a bite, slugcat is only grabbed, so you can still escape with your life if your captor drops you before reaching a den." +
                "\n+ The creature will drop you if it is stunned." +
                "\n+ After a moment, it will paralyze slugcat, so hit it with a rock or spear quickly!";

            var labelAuthor = new OpLabel(20, 600 - 30, "by Dual", true);
            var labelVersion = new OpLabel(20, 600 - 30 - 40, "github.com/Dual-Iron/");
            var labelNote = new OpLabel(200, 600 - 30 - 20, "Testing the waters with this mod. It's been pleasant so far.");

            var size = new Vector2(300 - 20, 150);
            var pos = new Vector2(10, 200 - size.y / 2);
            var rectDescription = new OpRect(pos, size) { description = "Mod mechanics" };
            var labelDescription = new OpLabelLong(pos + new Vector2(10, 0), rectDescription.size - Vector2.one * 20, desc, true, FLabelAlignment.Left);

            pos.x = 310;
            var rectVanillaDescription = new OpRect(pos, size) { description = "Tips on creatures grabbing slugcat" };
            var labelVanillaDescription = new OpLabelLong(pos + new Vector2(10, 0), rectDescription.size - Vector2.one * 20, vanillaDesc, true, FLabelAlignment.Left);

            var top = 200;
            var labelDmgMul = new OpLabel(20, 600 - top, "Lizard damage multiplier");
            var labelDecimal = new OpLabel(326, 600 - top + 2, "x 0.1");
            var draggerDmgMul = new OpDragger(new Vector2(300, 600 - top), "D1", 10)
            {
                description = "Makes you take more/less damage per lizard bite. By default (x1.0), a 1/n chance for lizards to kill makes them kill in n bites.",
                min = 0,
                max = 50,
                colorEdge = Color.clear,
                colorText = new Color(122, 216, 255)
            };

            var labelDmgRegen = new OpLabel(20, 600 - top - 30, "Lizard damage reset cooldown");
            var labelSeconds = new OpLabel(326, 600 - top - 30 + 2, "seconds");
            var draggerDmgRegen = new OpDragger(new Vector2(300, 600 - top - 30), "D2", -1)
            {
                description = "The time it takes to reset damage after being released from a grab. Value -1 means damage is only reset at the start of a cycle.",
                min = -1,
                max = 60,
                colorEdge = Color.clear,
                colorText = new Color(122, 216, 255)
            };

            Tabs[0].AddItems(
                rectVanillaDescription,
                labelVanillaDescription,
                rectDescription,
                labelDescription,
                labelAuthor,
                labelVersion,
                labelNote,
                labelDmgMul,
                labelDecimal,
                draggerDmgMul,
                labelDmgRegen,
                labelSeconds,
                draggerDmgRegen
                );
		}
	}
}
