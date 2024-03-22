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
        Cylinder c1,c2,c3;
        public Matrix4 TranslationC1;
        public Matrix4 RotationC1;
        public Matrix4 TranslationC2;
        public Matrix4 RotationC2;
        public Matrix4 TranslationC3;
        public Matrix4 RotationC3;

        public Pumon() 
        {
            c1 = new Cylinder();
            TranslationC1 = Matrix4.CreateTranslation(0,0,0);
            RotationC1 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(0));

            c2 = new();
            TranslationC2 = Matrix4.CreateTranslation(0, 0, c1.Height);
            RotationC2 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90));

            c3 = new();
            TranslationC3 = Matrix4.CreateTranslation(0, -c2.Height, c1.Height);
            RotationC3 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90))* Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(45));
        }


        public void Render(Shader shader, Matrix4 view, Matrix4 perspective, Vector3 cameraPos)
        {
            c1.Render(shader,c1.ModelMatrix* RotationC1*TranslationC1, view, perspective, cameraPos, new Vector3(1.0f,0.0f,0.0f));
            c2.Render(shader,c2.ModelMatrix*RotationC2*TranslationC2,view,perspective, cameraPos, new Vector3(1.0f,0.0f,1.0f));
            c3.Render(shader,c3.ModelMatrix*RotationC3*TranslationC3,view,perspective, cameraPos, new Vector3(0.0f,1.0f,1.0f));
            
        }
    }
}
