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

        Matrix4 transform;

        Vector3 lastVec3;

        Vector3 prevP3;
        float prevAlfa, prevBeta;

        Shader lineShader;

        public Vector4 currentEnd;
        public Quaternion currentRot
        {
            get
            {
                return transform.ExtractRotation();
            }
        }

        public float a1, a2, a3, a4, a5;    //alfa, beta, gamma, sigma, delta
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
            c2 = new();
            c3 = new();
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
            //GenLines = false;
            prevP3 = Vector3.Zero;
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
            transform = Matrix4.CreateRotationZ(a1) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90));
            var blockTransform = Matrix4.CreateTranslation(0, 0, -blockHeight / 2) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90));
            block.Render(shader, block.ModelMatrix, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c1.Render(shader, c1.ModelMatrix*transform, view, perspective, cameraPos, new Vector3(1.0f,0.0f,0.0f));

            transform = Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90)) * Matrix4.CreateTranslation(0, 0, c1.Height);
            block.Render(shader, block.ModelMatrix * blockTransform* transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c2.Render(shader,c2.ModelMatrix* transform, view, perspective, cameraPos, new Vector3(1.0f, 1.0f, 0.0f));


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



            blockTransform = Matrix4.CreateTranslation(0, 0, -blockHeight / 2);
            transform = Matrix4.CreateRotationZ(a4) * Matrix4.CreateRotationX(a3) * Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90)) * Matrix4.CreateTranslation(currentEnd.X, currentEnd.Y, currentEnd.Z);
            block.Render(shader, block.ModelMatrix * blockTransform * transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 0.0f));
            c4.Render(shader, c4.ModelMatrix * Matrix4.CreateRotationY(a5) * transform, view, perspective, cameraPos, new Vector3(1.0f, 0.0f, 1.0f));

            

            currentDir = new Vector4(0,-1,0,0);
            currentDir *= transform;
            currentEnd += currentDir * (c4.Height+0.02f);

            transform = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(270)) * Matrix4.CreateRotationY(a5) * Matrix4.CreateRotationZ(a4) * Matrix4.CreateRotationX(a3) * Matrix4.CreateRotationX(a2) * Matrix4.CreateRotationZ(a1) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90)) * Matrix4.CreateTranslation(currentEnd.X, currentEnd.Y, currentEnd.Z);
            coord.Render(shader, view, perspective, cameraPos, transform);
        }

        public (Vector3 p1, Vector3 p3, Vector3 p4, Vector3 p5) GetPositions(CoordinateSystem coord, Shader line_shader, bool firstFrame)
        {
            //verticalUp = false;
            lineShader = line_shader;
            Vector3 p0 = new Vector3(0, 0, 0);
            Vector3 p1 = p0 + new Vector3(0, 0, c1.Height);
            Vector3 p5 = coord.Pos;
            Vector3 x5 = (new Vector4(1, 0, 0, 0) * coord.GetRotationMatrix).Xyz;
            x5.Normalize();
            Vector3 p3 = Vector3.Zero;
            Vector3 alt_p3 = Vector3.Zero; 
                                   
            Vector3 p4 = p5 - x5 * c4.Height;
            var normal = Vector3.Cross(p1 - p0, p4 - p0);       
            normal.Normalize();

            // jak jest pionowo do góry to wtedy normalka jest 0,0,0 (bo p1,p0 i p4 leżą na tej samej lini)

            var vectorRamie3 = Vector3.Cross(p5 - p4, normal);      
            vectorRamie3.Normalize();

            if (float.IsNaN(vectorRamie3.X))
            {
                if (firstFrame)
                {
                    p3 = new Vector3(p4.X, p4.Y, p4.Z - len3);
                    alt_p3 = new Vector3(p4.X, p4.Y, p4.Z - len3);

                    //if (p4.X == p1.X && p4.Y == p1.Y)        //jak taki case to jest pionowo do góry puma
                    //{
                    //    p3.Z = p5.Z - len3;
                    //    alt_p3.Z = p5.Z - len3;
                    //}
                    lastVec3 = new Vector3(0, 0, p1.Z - p3.Z);
                    lastVec3.Normalize();
                }
                else
                {
                    p3 = p4 + lastVec3 * c3.Height;
                    alt_p3 = p4 - lastVec3 * c3.Height;
                }
            }
            else
            {
                p3 = p4 + vectorRamie3 * c3.Height;
                alt_p3 = p4 - vectorRamie3 * c3.Height;
                lastVec3 = vectorRamie3;
            }
            
            if (!firstFrame)
            {
                // choose closest solution to the last one
                var test1 = Math.Abs(Vector3.Distance(p3, prevP3));
                var test2 = Math.Abs(Vector3.Distance(alt_p3, prevP3));
                if (test1 > test2)
                {
                    p3 = alt_p3;
                }
                prevP3 = p3;
            }
            else
            {
                var test1 = Math.Abs(Vector3.Distance(p3, p1));
                var test2 = Math.Abs(Vector3.Distance(alt_p3, p1));
                if (test1 > test2)
                {
                    p3 = alt_p3;
                }
                prevP3 = p3;  
            }            

            return (p1, p3, p4, p5);
        }

        public (float alfa, float beta, float gamma, float sigma, float delta) GetAnglesFromPositions(Vector3 p1, Vector3 p3, Vector3 p4, Vector3 p5, CoordinateSystem coord, bool firstFrame)
        {
            // Pierwsze wyznaczenie kątów będzie z ogarniczeniem dla bety od 0 do 180 stopni
            // W kolejnych klatkach animacji kąty będą wyznaczane tak, aby były jak najbliżej rozwiązania z poprzedniej klatki

            Vector3 arm2 = p3 - p1;
            Vector3 arm3 = p4 - p3;
            Vector3 arm4 = p5 - p4;

            // Wyznaczanie alfy
            var alfa = Math.Atan2(arm2.Y, arm2.X);      // kiedy y i x 0 to osobliwość

            // jak alfa chce przeskoczyc z 0 na 180 (lub 180 na 0) to trzeba odbić betę na minus
            
            // Wyznaczanie bety
            Vector3 up = Vector3.UnitZ;
            var beta = Vector3.CalculateAngle(up, arm2);

            if(float.IsNaN(beta)) beta = 0; 

            // jak odbite to niech tak zostanie
            if (!firstFrame && prevBeta < 0)
            {
                beta = -beta;

                // alfa musi zostać odpowiednio poprawione (obrócone o 180 stopni, bo z atan2 wyjdzie inny wynik)
                if (alfa < 0) alfa += Math.PI;
                else alfa -= Math.PI;
            }

            // samo odbicie
            if (!firstFrame && Math.Abs((float)alfa - prevAlfa) > Math.PI / 2) // jak nagle alfa przeskakuje o ponad 90 stopni (ogólnie chodzi o przypadek gdy to jest 180 przeskok)
            {
                // beta się odbija
                beta = -beta;

                // alfa musi zostać odpowiednio poprawione (obrócone o 180 stopni, bo z atan2 wyjdzie inny wynik)
                if (alfa < 0) alfa += Math.PI;
                else alfa -= Math.PI;
            }


            prevAlfa = (float)alfa; prevBeta = beta; 

            // Wyznaczanie gammy
            Vector4 GammaTesterVec = new Vector4(-Vector3.UnitY,0);
            GammaTesterVec *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-180)) * Matrix4.CreateRotationX(beta) * Matrix4.CreateRotationZ((float)alfa) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90));
            var gamma = Vector3.CalculateAngle(arm2, arm3);
            var dotGamma = Vector3.Dot(GammaTesterVec.Xyz, arm3);
            if (dotGamma > -0.001f) gamma = -gamma;
            if (float.IsNaN(gamma)) gamma = 0;

            // Wyznaczanie sigmy
            Vector4 right = new Vector4(-Vector3.UnitY, 0);
            var currentTransform = Matrix4.CreateRotationX(gamma) * Matrix4.CreateRotationX(beta) * Matrix4.CreateRotationZ((float)alfa) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90));
            right *= currentTransform;
            Vector4 SigmaTesterVec = new Vector4(Vector3.UnitX, 0);
            SigmaTesterVec *= currentTransform;
            var sigma = Vector3.CalculateAngle(right.Xyz, arm4);
            var dotSigma = Vector3.Dot(SigmaTesterVec.Xyz, arm4);           
            if (dotSigma < -0.001f) sigma = (float)(Math.PI * 2 - sigma);
            if (float.IsNaN(sigma)) sigma = 0;

            // Wyznaczanie delty
            Vector4 yVec = (new Vector4(0, 1, 0, 0) * coord.GetRotationMatrix);
            yVec.Normalize();
            Vector4 rotatedArm5New = new Vector4(1, 0, 0, 0);
            Vector4 DeltaTesterVec = new Vector4(0, 0, -1, 0);
            currentTransform = Matrix4.CreateRotationZ(sigma) * Matrix4.CreateRotationX(gamma) * Matrix4.CreateRotationX(beta) * Matrix4.CreateRotationZ((float)alfa) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90));
            rotatedArm5New *= currentTransform;
            DeltaTesterVec *= currentTransform;
            var delta = Vector3.CalculateAngle(yVec.Xyz, rotatedArm5New.Xyz);
            var dotDelta = Vector3.Dot(DeltaTesterVec.Xyz, yVec.Xyz);
            if (dotDelta < -0.001f) delta = (float)(Math.PI * 2 - delta);

            return ((float)alfa,beta,gamma,sigma,delta);  
        }

        public void MovePumaToCurrentCoord(CoordinateSystem coord, Shader line_shader, bool firstFrame)
        {
            var positions = GetPositions(coord,line_shader, firstFrame);

            // Set up arm 2 lenght based on p1 and p3
            var arm2Dist = Math.Abs(Vector3.Distance(positions.p1, positions.p3));
            this.c2.Height = arm2Dist;
            this.len2 = arm2Dist;
            c2.UpdateVAO();

            var angles = GetAnglesFromPositions(positions.p1, positions.p3, positions.p4, positions.p5, coord, firstFrame);
            this.a1 = angles.alfa;
            this.a2 = angles.beta;
            this.a3 = angles.gamma;
            this.a4 = angles.sigma;
            this.a5 = angles.delta;
        }







        //public bool Flipped(Vector3 a, Vector3 b)
        //{
        //    var res = true;
        //    var deltaX = Math.Abs(a.X - b.X);
        //    var deltaY = Math.Abs(a.Y - b.Y);
        //    var deltaZ = Math.Abs(a.Z - b.Z);
        //    if (deltaX < 0.1 && deltaY < 0.1 && deltaZ < 0.1) res = false;
        //    return res;
        //}


        //// Funckcja jest do kitu
        //public bool CheckGamma(Vector3 p3, Vector3 p4)  // jak true to gamma na minusie, jak false to na plusie
        //{
        //    var res = false;
        //    var betaAngle = MathHelper.RadiansToDegrees(a2);
        //    var up = (p4 - p3).Z;
        //    // jeśli beta jest od 0 do 180, a punkt p4-p3 leci do góry to sigma jest ujemna
        //    if (betaAngle>=0 && betaAngle<=180)
        //    {
        //        if (up > 0) res = true;
        //        else res = false;
        //    }
        //    else
        //    {
        //        if (up > 0) res = false;
        //        else res = true;
        //    }

        //    return res;
        //}



        //Vector3 normOrigin = p1;

        //Vector3 offset = new Vector3(0.2f, 0.2f, 0.2f); 
        //ramie1 = new Line(p0+ offset, p1 + offset);
        //ramie4 = new Line(p4 + offset, p5 + offset);
        //normLine.Draw(lineShader, view * perspective);
        //normLine2.Draw(lineShader, view * perspective);
        //normLine3.Draw(lineShader, view * perspective);
        //newLine1.Draw(lineShader, view * perspective);
        //newLine2.Draw(lineShader, view * perspective);
        //newLine3.Draw(lineShader, view * perspective);
        //ramie1.Draw(lineShader, view * perspective);
        //ramie4.Draw(lineShader, view * perspective);
        //ramie3.Draw(lineShader, view * perspective);
        //ramie2.Draw(lineShader, view * perspective);

        //ramie3 = new Line(p3 + offset, p4 + offset);

        //ramie2 = new Line(p1+offset,p3+ offset);    
        //GenLines = true;

        //var normal2 = Vector3.Cross(p1 - p0, p3 - p1);
        //normal2.Normalize();
        ////Console.WriteLine($"Normalka pierwsze to: {normal2}");

        //var normal3 = Vector3.Cross(p3 - p1, p4 - p3);
        //normal3.Normalize();
        //normLine = new Line(normOrigin, normOrigin + normal2);
        //normOrigin = p3;
        //normLine2 = new Line(normOrigin, normOrigin + normal3);

        //var normal4 = Vector3.Cross(p1 - p0,p4 - p0);
        //normal4.Normalize();
        //normOrigin = p1 + p3 / 2;
        //normLine3 = new Line(normOrigin, normOrigin + normal4);



        //GetAnglesFromPositions(p1, p3, p4, p5, coord);


        //Console.WriteLine("Calculated Angles:");
        //Console.WriteLine($"Alfa: {Math.Round(MathHelper.RadiansToDegrees(alfa),1)}");
        //Console.WriteLine($"Beta: {Math.Round(MathHelper.RadiansToDegrees(beta), 1)}");
        //Console.WriteLine($"Gamma: {Math.Round(MathHelper.RadiansToDegrees(gamma), 1)}");
        //Console.WriteLine($"Sigma: {Math.Round(MathHelper.RadiansToDegrees(sigma), 1)}");
        //Console.WriteLine($"Delta: {Math.Round(MathHelper.RadiansToDegrees(delta), 1)}");

        //var offset = new Vector3(0, 0, 0.5f);
        //newLine1 = new Line(p4 + offset, p4 + right.Xyz + offset);      
        //newLine1 = new Line(p5 + offset, p5 + DeltaTesterVec.Xyz + offset);      
        //newLine2 = new Line(p3 + offset, p3 + GammaTesterVec.Xyz + offset);
        //newLine3 = new Line(p4 + offset, p4 + SigmaTesterVec.Xyz + offset);
        //newLine2 = new Line(p5 + offset, p5 + yVec.Xyz + offset);
        //newLine3 = new Line(p5 + offset, p5 + rotatedArm5New.Xyz + offset);


        //Console.WriteLine($"Cross normalki i ramienia4: {vectorRamie3}");
        // almost always works
        //if (vectorRamie3.Z >= 0) vectorRamie3 = -vectorRamie3;
        //if(vectorRamie3.X >0 && vectorRamie3.Y<0 && vectorRamie3.Z<0) vectorRamie3 = - vectorRamie3;
        //if(vectorRamie3.X >0 && vectorRamie3.Y>0 && vectorRamie3.Z>0) vectorRamie3 = - vectorRamie3;

        //var VecLen = (Vector3 a) => { return Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z); };
        // the result is chosen the way that lenght of second arm is lowest possible 
        // in the future there could be option to choose alternative option


        ////transform.ExtractRotation();

        //if (GenLines) 
        //{

        //} 


        //c1.Render(shader,c1.ModelMatrix* RotationC1*TranslationC1, view, perspective, cameraPos, new Vector3(1.0f,0.0f,0.0f));
        //c2.Render(shader,c2.ModelMatrix*RotationC2*TranslationC2,view,perspective, cameraPos, new Vector3(1.0f,0.0f,1.0f));
        //c3.Render(shader,c3.ModelMatrix*RotationC3*TranslationC3,view,perspective, cameraPos, new Vector3(0.0f,1.0f,1.0f));

        //// test w która stronę leci vec odbędzie się na podstawie sprawdzenia lokalnego układu
        //Vector3 up = new Vector3(0, 0, 1);
        //var test = Vector3.Cross(up, x5);
        ////Console.WriteLine($"Cross up i x5 to: {test}");
        ///
        //TranslationC1 = Matrix4.CreateTranslation(0,0,0);
        //RotationC1 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(0));

        //public bool GenLines;
        //Line normLine;
        //Line normLine2;
        //Line normLine3;
        //Line ramie1;
        //Line ramie2;
        //Line ramie3;
        //Line ramie4;
        //Line crossVec;

        //Line newLine1, newLine2, newLine3;
    }
}
