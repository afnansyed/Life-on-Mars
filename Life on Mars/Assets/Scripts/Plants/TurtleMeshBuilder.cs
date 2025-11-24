using System.Collections.Generic;
using UnityEngine;

public class TurtleMeshBuilder
{
    public struct Segment { public Vector3 a, b; public float thickness; }
    public struct LeafPoint { public Vector3 position; public Quaternion rotation; }

    public static (List<Segment> segments, List<LeafPoint> leaves) Build(
        string lString,
        float step,
        float angleDeg,
        float baseThickness,
        int leafDepthThreshold)
    {
        var segs = new List<Segment>();
        var leaves = new List<LeafPoint>();
        Stack<(Vector3 pos, Quaternion rot, int depth)> stack = new();

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        int depth = 0;

        // REF: https://paulbourke.net/fractals/lsys/
        // (with added 3D rotation symbols)
        foreach (char c in lString)
        {
            switch (c)
            {
                case 'F':
                    {
                        float thickness = baseThickness * Mathf.Pow(0.7f, depth);
                        Vector3 start = pos;
                        Vector3 end = pos + rot * Vector3.up * step;

                        segs.Add(new Segment { a = start, b = end, thickness = thickness });
                        pos = end;

                        // add leaf at branch tips (based on depth)
                        if (depth >= leafDepthThreshold)
                        {
                            leaves.Add(new LeafPoint { position = end, rotation = rot });
                        }
                        break;
                    }

                case 'L': // leaf symbol (optional, also adds based on depth)
                    leaves.Add(new LeafPoint { position = pos, rotation = rot });
                    break;

                case '+': rot = rot * Quaternion.Euler(0, angleDeg, 0); break;
                case '-': rot = rot * Quaternion.Euler(0, -angleDeg, 0); break;
                case '&': rot = rot * Quaternion.Euler(angleDeg, 0, 0); break; // Pitch down
                case '^': rot = rot * Quaternion.Euler(-angleDeg, 0, 0); break; // Pitch up
                case '\\': rot = rot * Quaternion.Euler(0, 0, angleDeg); break; // Roll right
                case '/': rot = rot * Quaternion.Euler(0, 0, -angleDeg); break; // Roll left

                case '[':
                    stack.Push((pos, rot, depth));
                    depth++;
                    break;

                case ']':
                    var st = stack.Pop();
                    pos = st.pos;
                    rot = st.rot;
                    depth = st.depth;
                    break;
            }
        }

        return (segs, leaves);
    }
}