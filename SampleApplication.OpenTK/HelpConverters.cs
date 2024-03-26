using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication.OpenTK
{
    public static class HelpConverters
    {
        private static Quaternion QuaternionSlerp(Quaternion q1, Quaternion q2, float t)
        {
            return Quaternion.Slerp(q1, q2, t);
        }

        public static Quaternion ConvertEulerToQuaternion(Vector3 Eualer)
        {
            Vector3 RadEuler = new Vector3(MathHelper.DegreesToRadians(Eualer.X), MathHelper.DegreesToRadians(Eualer.Y), MathHelper.DegreesToRadians(Eualer.Z));
            return Quaternion.FromEulerAngles(RadEuler);
        }

        public static Vector3 ConvertQuaternionToEuler(Quaternion Quater)
        {
            return Quater.ToEulerAngles();
        }

        private static Quaternion QuaternionLerp(Quaternion q1, Quaternion q2, float t)
        {
            var x = MathHelper.Lerp(q1.X, q2.X, t);
            var y = MathHelper.Lerp(q1.Y, q2.Y, t);
            var z = MathHelper.Lerp(q1.Z, q2.Z, t);
            var w = MathHelper.Lerp(q1.W, q2.W, t);
            var res = new Quaternion(x, y, z, w);
            return res.Normalized();
        }

        private static Vector3 Lerp(Vector3 e1, Vector3 e2, float t)
        {
            var x = MathHelper.Lerp(e1.X, e2.X, t);
            var y = MathHelper.Lerp(e1.Y, e2.Y, t);
            var z = MathHelper.Lerp(e1.Z, e2.Z, t);
            return new Vector3(x, y, z);
        }

        public static Matrix4 GetRotationMarixFromQuaternion(Quaternion quat)
        {
            var Matrix = new Matrix4();
            Matrix4.CreateFromQuaternion(quat, out Matrix);
            return Matrix;
        }

        public static Matrix4 GetRotationMarixFromEuler(Vector3 Euler)
        {  
            return Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Euler.Z)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Euler.Y)) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Euler.X));
        }

        public static Matrix4 GetTranslationMatrix(Vector3 Position)
        {
            return Matrix4.CreateTranslation(Position);
        }
    }
}
