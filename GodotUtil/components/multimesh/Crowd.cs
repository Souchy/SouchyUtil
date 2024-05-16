using Godot;
using Godot.NativeInterop;
using Godot.Sharp.Extras;
using souchyutil.godot.animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace souchyutil.godot.components.multimesh;

public partial class Crowd : Node3D
{
    #region Nodes
    [NodePath]
    public MultiMeshInstance3D MultiMeshInstance3D { get; set; }
    #endregion

    #region Properties
    public Dictionary<int, List<AnimationHeader>> AnimationHeaders { get; set; } = new();
    private List<InstanceData> Instances { get; set; } = new();
    #endregion

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        //MultiMeshInstance3D = new();
        //MultiMeshInstance3D.Multimesh = new();
        MultiMeshInstance3D.Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        MultiMeshInstance3D.Multimesh.UseCustomData = true;
    }

    public void SetMesh(Mesh mesh)
    {
        MultiMeshInstance3D.Multimesh.Mesh = mesh;
        //var mat = (ShaderMaterial) MultiMeshInstance3D.MaterialOverride;
        //var meshMat = mesh.SurfaceGetMaterial(0);
        // combine meshMat's fragment shader + our vertex shader
    }

    public void SetAnimationLibrary(float[] data) //string libraryPath)
    {
        //var floats = CrowdUtils.LoadAnimationData(libraryPath);
        AnimationHeaders = CrowdUtils.LoadAnimationHeaders(data);
        var mat = (ShaderMaterial) MultiMeshInstance3D.MaterialOverride;
        var tex = CrowdUtils.CreateAnimationDataTexture(data);
        mat.SetShaderParameter("animation_data", tex);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (AnimationHeaders.Count > 0)
        {
            for (int i = 0; i < Instances.Count; i++)
            {
                var instance = Instances[i];
                instance.Process(delta);
                if(instance.CurrentAnimation != null)
                {
                    MultiMeshInstance3D.Multimesh.SetInstanceCustomData(i,
                        new Color(instance.CurrentAnimation.LibraryId, instance.CurrentAnimation.AnimationId, instance.Time, 0)
                    );
                }
                MultiMeshInstance3D.Multimesh.SetInstanceTransform(i, instance.Transform3D);
            }
        }
    }

    public void AddInstances(params InstanceData[] instances)
    {
        Instances.AddRange(instances);
        MultiMeshInstance3D.Multimesh.InstanceCount = Instances.Count;
    }

    public void RemoveInstances(params InstanceData[] instances)
    {
        foreach (var inst in instances)
            Instances.Remove(inst);
        MultiMeshInstance3D.Multimesh.InstanceCount = Instances.Count;
    }

}


public static class CrowdUtils
{
    //public static float[] LoadAnimationData(string path)
    //{
    //    if (!libraryPath.Contains("res://"))
    //        libraryPath = Paths.animations + libraryPath;
    //    var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
    //    return (float[]) file.GetVar();
    //}
    public static Texture2D CreateAnimationDataTexture(float[] floats)
    {
        var pixelCount = floats.Length / 4;
        var texWidth = 2048;
        var texHeight = pixelCount / texWidth + 1;
        var texSize = texWidth * texHeight;
        System.Array.Resize(ref floats, texSize * 4);

        // create a byte array and copy the floats into it...
        var bytes = new byte[floats.Length * 4];
        Buffer.BlockCopy(floats, 0, bytes, 0, bytes.Length);
        //var bytes = ((Variant) floats).AsByteArray();
        var img = Image.CreateFromData(texWidth, texHeight, false, Image.Format.Rgbaf, bytes);
        var texture = ImageTexture.CreateFromImage(img);
        return texture;
    }
    public static Dictionary<int, List<AnimationHeader>> LoadAnimationHeaders(float[] data)
    {
        Dictionary<int, List<AnimationHeader>> animationHeaders = new();
        int libCount = (int) data[0];

        //int libIndex = 0;
        int readIndex = 0;
        for (int l = 0; l < libCount; l++)
        {
            int animCount = (int) data[readIndex + 1];
            int boneCount = (int) data[readIndex + 2];
            //float empty = data[3];
            readIndex += 4; // read lib
            animationHeaders.Add(l, new());

            for (int i = 0; i < animCount; i++)
            {
                float length = data[readIndex];
                int loopMode = (int) data[readIndex + 1];
                int frameCount = (int) data[readIndex + 2];
                int index = (int) data[readIndex + 3];
                var anim = new AnimationHeader(l, i, length, (Animation.LoopModeEnum) loopMode, frameCount, index);
                //animationHeaders.Add(anim);
                animationHeaders[l].Add(anim);
                readIndex += 4 + frameCount * boneCount * 12; // read animation + bones * frames
            }
        }
        return animationHeaders;
    }
}