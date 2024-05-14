using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace souchyutil.godot.components.multimesh;

public partial class Crowd : Node3D
{
    #region Nodes
    public MultiMeshInstance3D MultiMeshInstance3D { get; set; }
    public Array<Area3D> Collisions { get; set; }
    #endregion

    #region Properties
    public List<InstanceData> Instances { get; set; } = new();
    #endregion

    public override void _Ready()
    {
        base._Ready();
        MultiMeshInstance3D = new();
        MultiMeshInstance3D.Multimesh = new();
        MultiMeshInstance3D.Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        MultiMeshInstance3D.Multimesh.UseCustomData = true;
    }

    public void SetMesh(Mesh mesh)
    {
        MultiMeshInstance3D.Multimesh.Mesh = mesh;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        for(int i = 0; i < Instances.Count; i++)
        {
            var instance = Instances[i];
            instance.Process(delta);
            MultiMeshInstance3D.Multimesh.SetInstanceCustomData(i, 
                new Color(instance.CurrentAnimation.Id, instance.Time, 0, 0)
            );
            //MultiMeshInstance3D.Multimesh.SetInstanceTransform(i, new Transform3D());
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public void AddInstances(params InstanceData[] instances)
    {
        Instances.AddRange(instances);
        MultiMeshInstance3D.Multimesh.InstanceCount = Instances.Count;
    }

}



