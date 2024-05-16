using Godot;
using System.Collections.Generic;

namespace souchyutil.godot.animation;

public class AnimationBaker
{

    public Skeleton3D Skeleton3D {  get; set; }
    public MeshInstance3D MeshInstance3D {  get; set; }
    public AnimationPlayer AnimationPlayer {  get; set; }
    public AnimationLibrary AnimationLibrary { get; set; }

    public float[] pixels {  get; set; }

    public void Load(string gltf, string animationLibrary)
    {

    }

    public void Save()
    {
        var file = Godot.FileAccess.Open("", Godot.FileAccess.ModeFlags.Write);
        file.StoreVar(pixels);
        file.Close();
        GD.Print("Baked " + AnimationLibrary.ResourceName);
    }

    public float[] Bake()
    {
        List<float> floats = new()
        {
            AnimationLibrary.GetAnimationList().Count,
            Skeleton3D.GetBoneCount(),
            0,
            0
        };
        foreach(var name in AnimationLibrary.GetAnimationList())
        {
            var anim = AnimationLibrary.GetAnimation(name);
            Skeleton3D.ResetBonePoses();
            AnimationPlayer.CurrentAnimation = AnimationLibrary.ResourceName + "/" + anim;
            var framerate = 15;
            var frameCount = anim.Length * framerate;
            var adjustedTimePerFrame = anim.Length / (frameCount - 1);

            var animPixelIndex = floats.Count;
            floats.Add(anim.Length);
            floats.Add((int) anim.LoopMode);
            floats.Add(frameCount);
            floats.Add(animPixelIndex);

            float time = 0;
            AnimationPlayer.Advance(0);
            for(int f  = 0; f < frameCount; f++)
            {
                for(int b = 0; b < Skeleton3D.GetBoneCount(); b++)
                {
                    var globalTransform = Skeleton3D.GetBoneGlobalPose(b);
                    var skinTransform = MeshInstance3D.Skin.GetBindPose(b);
                    var transform = globalTransform * skinTransform;
                    floats.Add(transform.Basis.X.X);
                    floats.Add(transform.Basis.Y.X);
                    floats.Add(transform.Basis.Z.X);
                    floats.Add(transform.Origin.X);
                    floats.Add(transform.Basis.X.Y);
                    floats.Add(transform.Basis.Y.Y);
                    floats.Add(transform.Basis.Z.Y);
                    floats.Add(transform.Origin.Y);
                    floats.Add(transform.Basis.X.Z);
                    floats.Add(transform.Basis.Y.Z);
                    floats.Add(transform.Basis.Z.Z);
                    floats.Add(transform.Origin.Z);
                }
                time += adjustedTimePerFrame;
                AnimationPlayer.Advance(adjustedTimePerFrame);
            }
        }
        pixels = floats.ToArray();
        return pixels;
    }
}
