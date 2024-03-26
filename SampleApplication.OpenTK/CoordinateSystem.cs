using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication.OpenTK
{
    public class CoordinateSystem
    {
        Cylinder Up, Down, Right;

        // jednak bez sensu to trzymać w postaci macierzy, lepiej dać bezpośrednio info o pozycji i kątach a z tego wyciągnąć wewnątrz obiektu macierze
        public Matrix4 TranslationMatrix;
        public Matrix4 RotationMatrix;

        public Vector3 Pos;
        public Vector3 Euler;
        public Vector4 QuatData;
        public Quaternion Quat;
        //public Vector3 Translation;
        //public OpenTK.Mathematics.Quaternion Quat;

        public Span<float> Position
        {
            get
            {
                return MemoryMarshal.CreateSpan(ref Pos.X, 3);
            }
        }

        public Span<float> EulerAngle
        {
            get
            {
                return MemoryMarshal.CreateSpan(ref Euler.X, 3);
            }
        }

        public Span<float> Quaternion
        {
            get
            {
                return MemoryMarshal.CreateSpan(ref QuatData.X, 4);
            }
        }

        public CoordinateSystem()
        {
            float scale = 0.17f;
            Up = new Cylinder();
            Up.Rot = new Vector3(90, 0, 0);
            Up.Scale = new Vector3(scale);
            Up.UpdateModelMatrix();

            Down = new Cylinder();
            Down.Rot = new Vector3(0, 90, 0);
            //Down.Translation = new Vector3(0, 0, 0.1f);
            Down.Scale = new Vector3(scale);
            Down.UpdateModelMatrix();

            Right = new Cylinder();
            Right.Rot = new Vector3(0, 0, -90);
            Right.Scale = new Vector3(scale);
            Right.UpdateModelMatrix();

            TranslationMatrix = Matrix4.Identity;
            RotationMatrix = Matrix4.Identity;

            Pos = new Vector3();
            Euler = new Vector3();
            Quat = new Quaternion(0,0,0,1);
        }

        public void Render(Shader PhongShader, Matrix4 view, Matrix4 projection, Vector3 camera_pos)
        {
            Right.Render(PhongShader, Right.ModelMatrix * RotationMatrix * TranslationMatrix, view, projection, camera_pos, new Vector3(1f, 0f, 0f));
            Up.Render(PhongShader, Up.ModelMatrix * RotationMatrix * TranslationMatrix, view, projection, camera_pos, new Vector3(0f, 1f, 0f));
            Down.Render(PhongShader, Down.ModelMatrix * RotationMatrix * TranslationMatrix, view, projection, camera_pos, new Vector3(0f, 0f, 1f));
        }

        public void Render(Shader PhongShader, Matrix4 view, Matrix4 projection, Vector3 camera_pos, Matrix4 transform)
        {
            Right.Render(PhongShader, Right.ModelMatrix * transform, view, projection, camera_pos, new Vector3(1f, 0f, 0f));
            Up.Render(PhongShader, Up.ModelMatrix * transform, view, projection, camera_pos, new Vector3(0f, 1f, 0f));
            Down.Render(PhongShader, Down.ModelMatrix * transform, view, projection, camera_pos, new Vector3(0f, 0f, 1f));
        }

        public void RenderUsingEuler(Shader PhongShader, Matrix4 view, Matrix4 projection, Vector3 camera_pos)
        {
            var Rotation = HelpConverters.GetRotationMarixFromEuler(Euler);
            var Translation = HelpConverters.GetTranslationMatrix(Pos);

            Right.Render(PhongShader, Right.ModelMatrix * Rotation * Translation, view, projection, camera_pos, new Vector3(1f, 0f, 0f));
            Up.Render(PhongShader, Up.ModelMatrix * Rotation * Translation, view, projection, camera_pos, new Vector3(0f, 1f, 0f));
            Down.Render(PhongShader, Down.ModelMatrix * Rotation * Translation, view, projection, camera_pos, new Vector3(0f, 0f, 1f));
        }
        public void RenderUsingQuat(Shader PhongShader, Matrix4 view, Matrix4 projection, Vector3 camera_pos)
        {
            var Rotation = HelpConverters.GetRotationMarixFromQuaternion(Quat);
            var Translation = HelpConverters.GetTranslationMatrix(Pos);

            Right.Render(PhongShader, Right.ModelMatrix * Rotation * Translation, view, projection, camera_pos, new Vector3(1f, 0f, 0f));
            Up.Render(PhongShader, Up.ModelMatrix * Rotation * Translation, view, projection, camera_pos, new Vector3(0f, 1f, 0f));
            Down.Render(PhongShader, Down.ModelMatrix * Rotation * Translation, view, projection, camera_pos, new Vector3(0f, 0f, 1f));
        }

        public void UpdateQuaternionData()
        {
            Quat.X = QuatData.X;Quat.Y = QuatData.Y; Quat.Z = QuatData.Z; Quat.W = QuatData.W;
        }
    }
}
