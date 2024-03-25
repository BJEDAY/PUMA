using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication.OpenTK
{
    public class Pumon
    {
        Cylinder c1,c2,c3,c4;
        public Matrix4 TranslationC1;
        public Matrix4 RotationC1;
        public Matrix4 TranslationC2;
        public Matrix4 RotationC2;
        public Matrix4 TranslationC3;
        public Matrix4 RotationC3;

        public Vector4 currentEnd;

        public float a1, a2, a3, a4, a5;
        public float len1, len2, len3, len4;

        Cylinder block;
        float blockHeight = 0.22f;
        float blockRadius = 0.15f;
        public Pumon() 
        {
            len1 = 3f;
            len2 = 3f;
            len3 = 3f;
            len4 = 3f;

            c1 = new Cylinder();
            //TranslationC1 = Matrix4.CreateTranslation(0,0,0);
            //RotationC1 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(0));

            c2 = new();
            //TranslationC2 = Matrix4.CreateTranslation(0, 0, c1.Height);
            //RotationC2 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90));

            c3 = new();
            //TranslationC3 = Matrix4.CreateTranslation(0, -c2.Height, c1.Height);
            //RotationC3 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90))* Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(45));

            c4 = new();
            c4.Rot = new Vector3(180, 90, 0);
            c4.UpdateModelMatrix();

            a1 = MathHelper.DegreesToRadians(45);
            a2 = MathHelper.DegreesToRadians(90);
            a3 = MathHelper.DegreesToRadians(-90);
            a4 = MathHelper.DegreesToRadians(0);

            block = new(blockHeight, blockRadius);
        }

        public void UpdateLen(int num)
        {
            if (num == 1) 
            {
                c1.Height = len1;
                c1.UpdateVAO();
            }
            if (num == 2)
            {
                c2.Height = len2;
                c2.UpdateVAO();
            }
            if (num == 3)
            {
                c3.Height = len3;
                c3.UpdateVAO();
            }
            if (num == 4)
            {
                c4.Height = len4;
                c4.UpdateVAO();
            }
        }
        public void Render(Shader shader, Matrix4 view, Matrix4 perspective, Vector3 cameraPos)
        {
            //c1.Render(shader,c1.ModelMatrix* RotationC1*TranslationC1, view, perspective, cameraPos, new Vector3(1.0f,0.0f,0.0f));
            //c2.Render(shader,c2.ModelMatrix*RotationC2*TranslationC2,view,perspective, cameraPos, new Vector3(1.0f,0.0f,1.0f));
            //c3.Render(shader,c3.ModelMatrix*RotationC3*TranslationC3,view,perspective, cameraPos, new Vector3(0.0f,1.0f,1.0f));

            var transform = Matrix4.CreateRotationZ(a1);
            var blockTransform = Matrix4.CreateTranslation(0, 0, -blockHeight / 2) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90));
            block.Render(shader, block.ModelMatrix, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c1.Render(shader, c1.ModelMatrix*transform, view, perspective, cameraPos, new Vector3(1.0f,0.0f,0.0f));

            //transform =  Matrix4.CreateRotationX(a2)  * Matrix4.CreateTranslation(0, 0, c1.Height) * transform;
            transform = Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1)* Matrix4.CreateTranslation(0, 0, c1.Height);
            block.Render(shader, block.ModelMatrix * blockTransform* transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c2.Render(shader,c2.ModelMatrix* transform, view, perspective, cameraPos, new Vector3(1.0f, 1.0f, 0.0f));

            // to teraz tak, defaultowo dla 0 stopni c2 leci pionowo do góry. aby wyznaczyć punkt położenia jej aktualnego końca (i wiedzieć gdzie przesunąć c3)
            // trzeba kierunek (wektor) będący pionową krechą tak samo potraktować przekstałceniami, a potem z tego wyznaczyć przesunięcie z końca c1 na koniec c2

            var currentDir = new Vector4(0, 0, 1,0);
            currentDir*= transform;
            currentEnd = new Vector4(0,0,c1.Height,1);
            currentEnd += currentDir * c2.Height;

            transform = Matrix4.CreateRotationX(a3)* Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1) * Matrix4.CreateTranslation(currentEnd.X, currentEnd.Y, currentEnd.Z); //Matrix4.CreateRotationX(a3)  *
            block.Render(shader, block.ModelMatrix * blockTransform * transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c3.Render(shader, c3.ModelMatrix * transform, view, perspective, cameraPos, new Vector3(0.0f, 1.0f, 1.0f));

            currentDir = new Vector4(0, 0, 1, 0);
            currentDir *= transform;
            currentEnd += currentDir * c3.Height;

            blockTransform = Matrix4.CreateTranslation(0, 0, -blockHeight / 2);
            transform = Matrix4.CreateRotationZ(a4) * Matrix4.CreateRotationX(a3) * Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1) * Matrix4.CreateTranslation(currentEnd.X, currentEnd.Y, currentEnd.Z);
            block.Render(shader, block.ModelMatrix * blockTransform * transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c4.Render(shader, c4.ModelMatrix * transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 1.0f));

            
        }
    }
}
