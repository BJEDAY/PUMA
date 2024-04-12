using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Vector3 = OpenTK.Mathematics.Vector3;

namespace SampleApplication.OpenTK
{
    struct Config
    {
        public float alfa, beta, gamma, sigma, delta;
        public float arm2;
    }
    public class SimulationController
    {
        // Shader is used inside GetPositions function to draw current generated positions (could be removed from end version)
        Shader lineShader;
        public bool run, pause, stop;
        public float deltaTime, animationTime;
        public float currentTime;
        Pumon pumaLeft;
        Pumon pumaRight;
        CoordinateSystem start, end;

        // Interpolation Animation
        Config startConfig;
        Config endConfig;


        // Information for PUMA
        bool firstFrame;


        // Inversed Kinematic Animaton

        public SimulationController(ref Pumon pumaL,  ref Pumon pumaR, CoordinateSystem s, CoordinateSystem e, Shader shader)
        {
            run = false;
            pause = false;
            stop = false;
            currentTime = 0;
            deltaTime = 0;
            animationTime = 10;
            start = s;
            end = e;
            lineShader = shader;
            pumaLeft = pumaL;
            pumaRight = pumaR;
        }

        public void UpdateConfigs()
        {
            var startPositions = pumaRight.GetPositions(start, lineShader, firstFrame);
            var startAngles = pumaRight.GetAnglesFromPositions(startPositions.p1,startPositions.p3,startPositions.p4,startPositions.p5,start, true);
            FillConfig(ref startConfig, startPositions.p1, startPositions.p3, startAngles);

            var endPositions = pumaRight.GetPositions(end, lineShader, firstFrame);
            var endAngles = pumaRight.GetAnglesFromPositions(endPositions.p1,endPositions.p3,endPositions.p4,endPositions.p5,end, true);

            FillConfig(ref endConfig, endPositions.p1, endPositions.p3, endAngles);
            Console.WriteLine("Before adjusting end angles:");
            WriteConsoleInfo();

            endAngles.alfa = FindShortestAngle(startAngles.alfa, endAngles.alfa);
            endAngles.beta = FindShortestAngle(startAngles.beta, endAngles.beta);
            endAngles.gamma = FindShortestAngle(startAngles.gamma, endAngles.gamma);
            endAngles.sigma = FindShortestAngle(startAngles.sigma, endAngles.sigma);
            endAngles.delta = FindShortestAngle(startAngles.delta, endAngles.delta);

            FillConfig(ref endConfig, endPositions.p1, endPositions.p3, endAngles);
            Console.WriteLine("After adjusting end angles:");
            WriteConsoleInfo();
        }

        public void WriteConsoleInfo()
        {
            Console.WriteLine("Start Config: ");
            Console.WriteLine($"Alfa: {MathHelper.RadiansToDegrees(startConfig.alfa)}");
            Console.WriteLine($"Beta: {MathHelper.RadiansToDegrees(startConfig.beta)}");
            Console.WriteLine($"Gamma: {MathHelper.RadiansToDegrees(startConfig.gamma)}");
            Console.WriteLine($"Sigma: {MathHelper.RadiansToDegrees(startConfig.sigma)}");
            Console.WriteLine($"Delta: {MathHelper.RadiansToDegrees(startConfig.delta)}");

            Console.WriteLine("End Config: ");
            Console.WriteLine($"Alfa: {MathHelper.RadiansToDegrees(endConfig.alfa)}");
            Console.WriteLine($"Beta: {MathHelper.RadiansToDegrees(endConfig.beta)}");
            Console.WriteLine($"Gamma: {MathHelper.RadiansToDegrees(endConfig.gamma)}");
            Console.WriteLine($"Sigma: {MathHelper.RadiansToDegrees(endConfig.sigma)}");
            Console.WriteLine($"Delta: {MathHelper.RadiansToDegrees(endConfig.delta)}");
        }

        private void FillConfig(ref Config config, Vector3 p1, Vector3 p3, (float alfa, float beta, float gamma, float sigma, float delta) angles)
        {
            config.alfa = angles.alfa;
            config.beta = angles.beta;
            config.gamma = angles.gamma;
            config.sigma = angles.sigma;
            config.delta = angles.delta;
            var armLen = Math.Abs(Vector3.Distance(p1, p3));
            config.arm2 = armLen;
        }

        public void Start()
        {
            UpdateConfigs();
            if (!pause) currentTime = 0;
            run = true;
            pause = false;
            stop = false;
            firstFrame = true;
        }

        public void Stop()
        {
            run = false;
            pause = false;
            stop = true;
            currentTime = 0;

            pumaLeft.a1 = startConfig.alfa;
            pumaLeft.a2 = startConfig.beta;
            pumaLeft.a3 = startConfig.gamma;
            pumaLeft.a4 = startConfig.sigma;
            pumaLeft.a5 = startConfig.delta;
            pumaLeft.len2 = startConfig.arm2;
            pumaLeft.UpdateLen(2);

            pumaRight.a1 = startConfig.alfa;
            pumaRight.a2 = startConfig.beta;
            pumaRight.a3 = startConfig.gamma;
            pumaRight.a4 = startConfig.sigma;
            pumaRight.a5 = startConfig.delta;
            pumaRight.len2 = startConfig.arm2;
            pumaRight.UpdateLen(2);
        }

        float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        Vector3 Lerp(Vector3 a,  Vector3 b, float by)
        {
            return a * (1 - by) + b * by;
        }

        private float FindShortestAngle(float start, float end)
        {
            float alternativeEnd;

            if(end > 0) alternativeEnd = end - (float)Math.PI * 2;
            else alternativeEnd = end + (float)Math.PI * 2;

            float l1 = Math.Abs(alternativeEnd - start);
            float l2 = Math.Abs(end- start);
            if (l2 < l1) return end;
            else return alternativeEnd;
        }

        public void Run()
        {
            if(run)
            {
                currentTime += deltaTime;

                if (currentTime > animationTime) 
                {
                    currentTime = animationTime;
                    run = false;
                } 

                // Classic animation interpolation - tutaj na podstawie początkowego i końcowego ułożenia interpoluję wyznaczone kąty
                var currentAlfa = Lerp(startConfig.alfa, endConfig.alfa, (float)currentTime / animationTime);
                var currentBeta = Lerp(startConfig.beta, endConfig.beta, (float)currentTime / animationTime);
                var currentGamma = Lerp(startConfig.gamma, endConfig.gamma, (float)currentTime / animationTime);
                var currentSigma = Lerp(startConfig.sigma, endConfig.sigma, (float)currentTime / animationTime);
                var currentDelta = Lerp(startConfig.delta, endConfig.delta, (float)currentTime / animationTime);
                var currentLen = Lerp(startConfig.arm2, endConfig.arm2, (float)currentTime / animationTime);

                pumaLeft.a1 = currentAlfa;
                pumaLeft.a2 = currentBeta;
                pumaLeft.a3 = currentGamma;
                pumaLeft.a4 = currentSigma;
                pumaLeft.a5 = currentDelta;
                pumaLeft.len2 = currentLen;
                pumaLeft.UpdateLen(2);

                // Inversed kinematic animation - tutaj na podstawie początkowego i końcowego ustawienia układu współrzednych generuję nowy układ współrzednych poprzez interpolację
                // kwaternionu i pozycji, a następnie dla nowego coord wyznaczam nowe rozwiązanie PUMY i wyświetlam

                var currentQuat = HelpConverters.QuaternionSlerp(start.Quat,end.Quat,(float)currentTime / animationTime);
                var currentPos = Lerp(start.Pos, end.Pos, (float)currentTime /animationTime);
                var currentCoord = new CoordinateSystem(currentPos,currentQuat);
                pumaRight.MovePumaToCurrentCoord(currentCoord, lineShader, firstFrame);
                if(firstFrame) firstFrame = false;
            }
        }
    }
}
