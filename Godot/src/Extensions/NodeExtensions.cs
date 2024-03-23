namespace Markwardt.Godot;

public static class NodeExtensions
{
    public static void AddChildDeferred(this Node node, Node child)
        => node.CallDeferred("add_child", child);
    
    public static void RemoveChildDeferred(this Node node, Node child)
        => node.CallDeferred("remove_child", child);

    public static Vector3<float> GetLocalPosition(this Node3D node)
        => node.Position.ToGeneral();

    public static void SetLocalPosition(this Node3D node, Vector3<float> value)
        => node.Position = value.ToGodot();

    public static Vector4<float> GetLocalRotation(this Node3D node)
        => node.Quaternion.ToGeneral();

    public static void SetLocalRotation(this Node3D node, Vector4<float> value)
        => node.Quaternion = value.ToGodot();

    public static Vector3<float> GetLocalScale(this Node3D node)
        => node.Scale.ToGeneral();

    public static void SetLocalScale(this Node3D node, Vector3<float> value)
        => node.Scale = value.ToGodot();

    public static Vector3<float> GetGlobalPosition(this Node3D node)
        => node.GlobalPosition.ToGeneral();

    public static void SetGlobalPosition(this Node3D node, Vector3<float> value)
        => node.GlobalPosition = value.ToGodot();

    public static Vector4<float> GetGlobalRotation(this Node3D node)
        => new Quaternion(node.GlobalTransform.Basis).ToGeneral();

    public static void SetGlobalRotation(this Node3D node, Vector4<float> value)
        => node.GlobalTransform = new Transform3D(new Basis(value.ToGodot()), node.GlobalTransform.Origin);

    public static Vector3<float> GetGlobalScale(this Node3D node)
        => node.GlobalTransform.Basis.GetEuler().ToGeneral();

    public static void SetGlobalScale(this Node3D node, Vector3<float> value)
        => node.GlobalScale(value.ToGodot());
}