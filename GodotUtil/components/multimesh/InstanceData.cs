using Godot;
using souchyutil.godot.animation;

namespace souchyutil.godot.components.multimesh;

/// <summary>
/// 
/// </summary>
public class InstanceData
{
    //public int Id { get; set; }
    public AnimationHeader CurrentAnimation { get; private set; }
    public AnimationHeader LoopAnimation { get; private set; }
    public float Time { get; private set; }
    public Transform3D Transform3D { get; set; }

    public void SetCurrentAnimation(AnimationHeader anim)
    {
        CurrentAnimation = anim;
        Time = 0;
    }

    public void SetLoopAnimation(AnimationHeader anim)
    {
        LoopAnimation = anim;
    }

    public void PlayLoopAnimation(AnimationHeader anim)
    {
        SetLoopAnimation(anim);
        SetCurrentAnimation(anim);
    }

    public void Process(double delta)
    {
        if (CurrentAnimation != null)
        {
            Time += (float) delta;
            if (Time > CurrentAnimation.Length)
                SetCurrentAnimation(LoopAnimation);
        }
    }

}