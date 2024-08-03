using HarmonyLib;
using System.Runtime.CompilerServices;

namespace MoqStatic
{
    public class MoqStatic
    {
        public static Harmony Harmony
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
        } = new Harmony(id: "com.github.wdestroier.moqstatic");
    }
}
