using OpenTK.Mathematics;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace SampleApplication.OpenTK
{
    public class Pumon
    {
        Cylinder c1,c2,c3,c4;
        CoordinateSystem coord;
        public Matrix4 TranslationC1;
        public Matrix4 RotationC1;
        public Matrix4 TranslationC2;
        public Matrix4 RotationC2;
        public Matrix4 TranslationC3;
        public Matrix4 RotationC3;

        public bool GenLines;
        Line normLine;
        Line ramie1;
        Line ramie2;
        Line ramie3;
        Line ramie4;
        Line crossVec;
        Matrix4 transform;

        Shader lineShader;

        public Vector4 currentEnd;
        public Quaternion currentRot
        {
            get
            {
                return transform.ExtractRotation();
            }
        }

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

            coord = new();

            a1 = MathHelper.DegreesToRadians(0);
            a2 = MathHelper.DegreesToRadians(90);
            a3 = MathHelper.DegreesToRadians(-90);
            a4 = MathHelper.DegreesToRadians(0);
            a5 = MathHelper.DegreesToRadians(0);

            block = new(blockHeight, blockRadius);
            GenLines = false;
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

            transform = Matrix4.CreateRotationZ(a1) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90));
            var blockTransform = Matrix4.CreateTranslation(0, 0, -blockHeight / 2) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90));
            block.Render(shader, block.ModelMatrix, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c1.Render(shader, c1.ModelMatrix*transform, view, perspective, cameraPos, new Vector3(1.0f,0.0f,0.0f));

            //transform =  Matrix4.CreateRotationX(a2)  * Matrix4.CreateTranslation(0, 0, c1.Height) * transform;
            transform = Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90)) * Matrix4.CreateTranslation(0, 0, c1.Height);
            block.Render(shader, block.ModelMatrix * blockTransform* transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c2.Render(shader,c2.ModelMatrix* transform, view, perspective, cameraPos, new Vector3(1.0f, 1.0f, 0.0f));

            // to teraz tak, defaultowo dla 0 stopni c2 leci pionowo do góry. aby wyznaczyć punkt położenia jej aktualnego końca (i wiedzieć gdzie przesunąć c3)
            // trzeba kierunek (wektor) będący pionową krechą tak samo potraktować przekstałceniami, a potem z tego wyznaczyć przesunięcie z końca c1 na koniec c2

            var currentDir = new Vector4(0, 0, 1,0);
            currentDir*= transform;
            currentEnd = new Vector4(0,0,c1.Height,1);
            currentEnd += currentDir * c2.Height;

            transform = Matrix4.CreateRotationX(a3)* Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90)) * Matrix4.CreateTranslation(currentEnd.X, currentEnd.Y, currentEnd.Z); //Matrix4.CreateRotationX(a3)  *
            block.Render(shader, block.ModelMatrix * blockTransform * transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c3.Render(shader, c3.ModelMatrix * transform, view, perspective, cameraPos, new Vector3(0.0f, 1.0f, 1.0f));

            currentDir = new Vector4(0, 0, 1, 0);
            currentDir *= transform;
            currentEnd += currentDir * c3.Height;


            // in that last case of alfa5 the block should not be rotated by it's value (that looks weird) - that's why RotationY is seperate from transform
            blockTransform = Matrix4.CreateTranslation(0, 0, -blockHeight / 2);
            transform = Matrix4.CreateRotationZ(a4) * Matrix4.CreateRotationX(a3) * Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90)) * Matrix4.CreateTranslation(currentEnd.X, currentEnd.Y, currentEnd.Z);
            block.Render(shader, block.ModelMatrix * blockTransform * transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c4.Render(shader, c4.ModelMatrix * Matrix4.CreateRotationY(a5) * transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 1.0f));

            currentDir = new Vector4(0,-1,0,0);
            currentDir *= transform;
            currentEnd += currentDir * (c4.Height+0.02f);

            transform = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(270)) * Matrix4.CreateRotationY(a5) * Matrix4.CreateRotationZ(a4) * Matrix4.CreateRotationX(a3) * Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90)) * Matrix4.CreateTranslation(currentEnd.X, currentEnd.Y, currentEnd.Z);
            //transform = Matrix4.Identity;
            coord.Render(shader, view, perspective, cameraPos, transform);

            transform.ExtractRotation();

            if (GenLines) 
            {
                //normLine.Draw(lineShader, view * perspective);
                ramie1.Draw(lineShader, view * perspective);
                ramie4.Draw(lineShader, view * perspective);
                ramie3.Draw(lineShader, view * perspective);
                ramie2.Draw(lineShader, view * perspective);
            } 
        }

        public void GetPositions(CoordinateSystem coord, Shader line_shader)
        {
            Vector3 p0 = new Vector3(0, 0, 0);
            Vector3 p1 = p0 + new Vector3(0, 0, c1.Height);
            Vector3 p5 = coord.Pos;
            Vector3 x5 = (new Vector4(1, 0, 0, 0) * coord.GetRotationMatrix).Xyz;
            x5.Normalize();
            
            Vector3 p4 = p5 - x5 * c4.Height;

            Console.WriteLine($"Vector x5 len: {x5.Length}");
            Console.WriteLine($"Vector x5: {x5}");
            Console.WriteLine($"Postion p0: {p0}");
            Console.WriteLine($"Postion p1: {p1}");
            Console.WriteLine($"Postion p4: {p4}");
            Console.WriteLine($"Postion p5: {p5}");

            var normal = Vector3.Cross(p1 - p0, p4 - p0);
            normal.Normalize();

            Console.WriteLine($"Normal is : {normal}");
            Console.WriteLine($"");


            lineShader = line_shader;
            Vector3 normOrigin = new Vector3(2, -2, 2);
            normLine = new Line(normOrigin, normOrigin + normal);
            Vector3 offset = new Vector3(0.2f, 0.2f, 0.2f); // line are gonna be near the robot, not inside
            ramie1 = new Line(p0+ offset, p1 + offset);
            ramie4 = new Line(p4 + offset, p5 + offset);




            // test w która stronę leci vec odbędzie się na podstawie sprawdzenia lokalnego układu
            Vector3 up = new Vector3(0, 0, 1);
            var test = Vector3.Cross(up, x5);
            Console.WriteLine($"Cross up i x5 to: {test}");
            
            
            var vectorRamie3 = Vector3.Cross(p5 - p4, normal);

            vectorRamie3.Normalize();
            Console.WriteLine($"Cross normalki i ramienia4: {vectorRamie3}");
            // almost always works
            //if (vectorRamie3.Z >= 0) vectorRamie3 = -vectorRamie3;
            //if(vectorRamie3.X >0 && vectorRamie3.Y<0 && vectorRamie3.Z<0) vectorRamie3 = - vectorRamie3;
            //if(vectorRamie3.X >0 && vectorRamie3.Y>0 && vectorRamie3.Z>0) vectorRamie3 = - vectorRamie3;

            Vector3 p3 = p4 + vectorRamie3 * c3.Height;
            Vector3 alt_p3 = p4 - vectorRamie3 * c3.Height;
            var test1 = Math.Abs(Vector3.Distance(p3, p1));
            var test2 = Math.Abs(Vector3.Distance(alt_p3, p1));

            //var VecLen = (Vector3 a) => { return Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z); };

            // the result is chosen the way that lenght of second arm is lowest possible 
            // in the future there could be option to choose alternative option

            if (test1 > test2) 
            {
                p3 = alt_p3;
            }

            ramie3 = new Line(p3 + offset, p4 + offset);

            ramie2 = new Line(p1+offset,p3+ offset);    
            GenLines = true;
        }
    }
}
