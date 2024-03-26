using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication.OpenTK
{
    public class CoordinateSystem
    {
        Cylinder Up, Down, Right;

        public Matrix4 TranslationMatrix;
        public Matrix4 RotationMatrix;

        //public Vector3 Translation;
        //public OpenTK.Mathematics.Quaternion Quat;

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

    }
}
