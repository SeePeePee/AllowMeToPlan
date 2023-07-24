using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace AllowMeToPlan
{
    [StaticConstructorOnStartup]
    public class Main : Mod
    {
        public static Harmony HarmonyInstance { get; private set; }

        public Main(ModContentPack content) : base(content)
        {
        }

        static Main()
        {
            HarmonyInstance = new Harmony("SeePeePee.AllowMeToPlan");
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Designator_Build), nameof(Designator_Build.ProcessInput))]
    internal class Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand.ToString() == "System.Boolean godMode")
                {
                    if (found)
                        throw new("Oops, this is not supposed to happen");
                    codes[i].opcode = OpCodes.Ldc_I4_1;
                    codes[i].operand = null;
                    found = true;
                    Log.Message($"[AMTP] Patched instruction at {i}");
                }
            }
            if (found)
                Log.Message("[AMTP] Patching complete");
            else
                Log.Error("[AMTP] Patching failed");

            return codes.AsEnumerable();
        }
    }
}
