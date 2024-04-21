namespace Markwardt;

public static class VectorExtensions
{
    public static Vector3<float> ToGeneral(this Vector3 vector)
        => new(vector.X, vector.Y, vector.Z);
    
    public static Vector4<float> ToGeneral(this Quaternion quaternion)
        => new(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
    
    public static Vector3 ToGodot(this Vector3<float> vector)
        => new(vector.X, vector.Y, vector.Z);
    
    public static Quaternion ToGodot(this Vector4<float> quaternion)
        => new(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
}