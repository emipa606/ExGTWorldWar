using System.Collections.Generic;
using Verse;

namespace MYDE_ExGTWorldWar;

public static class MYDE_ModFront
{
    public static IEnumerable<IntVec3> GetLine(IntVec3 start, IntVec3 end)
    {
        var num = (int)start.DistanceTo(end);
        for (var i = 0; i <= num; i++)
        {
            yield return Lerp(start, end, i, num);
        }
    }

    private static IntVec3 Lerp(IntVec3 A, IntVec3 B, int t, int max)
    {
        return new IntVec3((int)GenMath.LerpDouble(0f, max, A.x, B.x, t), (int)GenMath.LerpDouble(0f, max, A.y, B.y, t),
            (int)GenMath.LerpDouble(0f, max, A.z, B.z, t));
    }
}