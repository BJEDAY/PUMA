using System.Diagnostics;
using System.Drawing;
using DearImGui;
using DearImGui.OpenTK;
using DearImGui.OpenTK.Extensions;
using DearImPlot;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace SampleApplication.OpenTK;

struct ViewPerspectiveSettings
{
    public float fov, f, n;
    public ViewPerspectiveSettings(float Fov, float F, float N) { fov = Fov; f = F; n = N; }
}

struct GlobalPumaLengths
{
    public float l1, l2, l3, l4;
    public GlobalPumaLengths(float val) {  l1 = val; l2 = val; l3 = val;l4 = val; }
}
// TESTING THINGS
internal sealed class MyGameWindow : GameWindowBaseWithDebugContext
{
    private readonly ImGuiController Controller;

    private readonly ImPlotContext ImPlotContext;

    private Color4 Color1 = Color4.Crimson;

    private Color4 Color2 = Color4.DeepSkyBlue;

    private bool ShowImGuiDemo = true;

    private bool ShowImPlotDemo = true;

    private bool ShowDockingDemo = true;

    bool openPR = false;

    Shader shader; Camera camera; ViewPerspectiveSettings perspectiveSettings; Line line; Shader phongShader; Cylinder cylinder, cylinder2; Vector2 prev_mouse; Vector3 lightColor; Vector3 lightPos;
    Grid grid; Shader gridShader; Pumon pumaLeft; Pumon pumaRight; Rect seperatingLine; CoordinateSystem startCoord; CoordinateSystem endCoord; SimulationController simulationController;
    GlobalPumaLengths GlobalPumaLengths; OrbitCamera OrbitCamera; 
    public MyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        Controller = new ImGuiController(this, "Roboto-Regular.ttf", 20.0f);

        ImPlotContext = ImPlot.CreateContext();

        ImPlot.SetCurrentContext(ImPlotContext);

        ImPlot.SetImGuiContext(Controller.Context);

        SetupShaders();
        SetupCamera();
        SetupGL();
        SetupObjects();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Controller.Dispose();
            ImPlot.DestroyContext(ImPlotContext);
        }

        base.Dispose(disposing);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        SampleExtraFontImpl();

        Controller.Update((float)args.Time);

        // TODO: SimulationController deltaTime = args.Time
        simulationController.deltaTime = (float)args.Time;
    }

    protected void SetupObjects()
    {
        GlobalPumaLengths = new GlobalPumaLengths(3f);
        grid = new Grid();
        line = new Line();
        seperatingLine = new Rect();
        cylinder = new Cylinder();
        startCoord = new CoordinateSystem(new Vector3(6,0,6), new Vector3(0,0,0));
        endCoord = new CoordinateSystem(new Vector3(6, -2, 6), new Vector3(0, 0, 0));
        pumaLeft = new Pumon();
        pumaRight = new Pumon();
        simulationController = new SimulationController(ref pumaLeft, ref pumaRight, startCoord, endCoord, shader);
    }
    protected void SetupShaders()
    {
        // instead of using path "Shaders/ShaderVerts.glsl and using option "copy to output directory" the path is given directly to the source of shaders (every change gonna be instant)
        shader = new Shader("../../../Shaders/ShaderVert.glsl", "../../../Shaders/ShaderFrag.glsl");

        gridShader = new Shader("../../../Shaders/GridShaderVert.glsl", "../../../Shaders/GridShaderFrag.glsl");

        gridShader.Use();
        var backColor = Color.CornflowerBlue.ToVector4();
        gridShader.SetVec4("backgroundColor", new Vector4(backColor.X, backColor.Y, backColor.Z, backColor.W)); // new Vector4(0.66f, 0.66f, 0.66f, 1f));
        gridShader.SetVec4("gridColor", new Vector4(0f, 1f, 1f, 1f));


        phongShader = new Shader("../../../Shaders/ShaderPhongVert.glsl", "../../../Shaders/ShaderPhongFrag.glsl");
        lightColor = new Vector3(1f, 1f, 1f);
        lightPos = new Vector3(-10, -20, 20);

        //phong shader light position and color are setup once and used globally for diffrent objects.
        //Althought viewPos is changed for all objects in every frame and objectColor every frame for every object using phong.
        phongShader.Use();
        phongShader.SetVec3("lightPos", lightPos);            //uniform vec3 lightPos;
        phongShader.SetVec3("lightColor", lightColor);        //uniform vec3 lightColor;


        // this needs to be set up in every frame while rendering specific object
        //shader.SetMatrix4("model", ModelMatrix);
        //shader.SetMatrix4("view", View);
        //shader.SetMatrix4("projection", Perspective);
        //shader.SetVec3("objectColor", ObjectColor);  //uniform vec3 objectColor;
        //shader.SetVec3("viewPos", cameraPos);
        
    }

    protected void SetupCamera()
    {
        //Camera initialization
        camera = new Camera();
        perspectiveSettings = new ViewPerspectiveSettings(45.0f, 30.0f, 0.5f);
        camera.UpdateProjectionMatrix((float)ClientSize.X/2, (float)ClientSize.Y, perspectiveSettings.fov, perspectiveSettings.n, perspectiveSettings.f);

        OrbitCamera = new OrbitCamera();
        var aspect = ((float)ClientSize.X / 2) / ((float)ClientSize.Y);
        OrbitCamera.UpdateProj(aspect);
    }

    protected void SetupGL()
    {
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
    }


    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.ClearColor(Color.CornflowerBlue);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        simulationController.Run();

        //// left screen
        //GL.Viewport(0, 0, ClientSize.X / 2, ClientSize.Y);
        //grid.Draw(gridShader, camera.viewMatrix, camera.projectionMatrix);

        ////startCoord.RenderUsingEuler(phongShader, camera.viewMatrix,camera.projectionMatrix,camera.cameraPosition);
        //startCoord.RenderUsingQuat(phongShader, camera.viewMatrix, camera.projectionMatrix, camera.cameraPosition);
        //endCoord.RenderUsingQuat(phongShader, camera.viewMatrix, camera.projectionMatrix, camera.cameraPosition);

        //pumaLeft.Render(phongShader, camera.viewMatrix, camera.projectionMatrix, camera.cameraPosition);
        //seperatingLine.moveRight = true;
        //seperatingLine.Render(shader);
        ////coord.Render(phongShader, camera.viewMatrix, camera.projectionMatrix, camera.cameraPosition);

        //// right screen
        //GL.Viewport(ClientSize.X / 2, 0, ClientSize.X / 2, ClientSize.Y);
        //grid.Draw(gridShader, camera.viewMatrix, camera.projectionMatrix);

        //startCoord.RenderUsingQuat(phongShader, camera.viewMatrix, camera.projectionMatrix, camera.cameraPosition);
        //endCoord.RenderUsingQuat(phongShader, camera.viewMatrix, camera.projectionMatrix, camera.cameraPosition);

        //pumaRight.Render(phongShader, camera.viewMatrix, camera.projectionMatrix, camera.cameraPosition);
        //seperatingLine.moveRight = false;
        //seperatingLine.Render(shader);


        // left screen
        GL.Viewport(0, 0, ClientSize.X / 2, ClientSize.Y);
        grid.Draw(gridShader, OrbitCamera.viewMatrix, OrbitCamera.projectionMatrix);

        //startCoord.RenderUsingEuler(phongShader, camera.viewMatrix,camera.projectionMatrix,camera.cameraPosition);
        startCoord.RenderUsingQuat(phongShader, OrbitCamera.viewMatrix, OrbitCamera.projectionMatrix, OrbitCamera.pos);
        endCoord.RenderUsingQuat(phongShader, OrbitCamera.viewMatrix, OrbitCamera.projectionMatrix, OrbitCamera.pos);

        pumaLeft.Render(phongShader, OrbitCamera.viewMatrix, OrbitCamera.projectionMatrix, OrbitCamera.pos);
        seperatingLine.moveRight = true;
        seperatingLine.Render(shader);
        //coord.Render(phongShader, camera.viewMatrix, camera.projectionMatrix, camera.cameraPosition);

        // right screen
        GL.Viewport(ClientSize.X / 2, 0, ClientSize.X / 2, ClientSize.Y);
        grid.Draw(gridShader, OrbitCamera.viewMatrix, OrbitCamera.projectionMatrix);

        startCoord.RenderUsingQuat(phongShader, OrbitCamera.viewMatrix, OrbitCamera.projectionMatrix, OrbitCamera.pos);
        endCoord.RenderUsingQuat(phongShader, OrbitCamera.viewMatrix, OrbitCamera.projectionMatrix, OrbitCamera.pos);

        pumaRight.Render(phongShader, OrbitCamera.viewMatrix, OrbitCamera.projectionMatrix, OrbitCamera.pos);
        seperatingLine.moveRight = false;
        seperatingLine.Render(shader);

        // whole screen
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

        ImGui.SetNextWindowSize(new System.Numerics.Vector2(400, 250), ImGuiCond.Once);
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(400, 0));

        ImGui.PushStyleColor(ImGuiCol.WindowBg, Color.DarkSlateGray.ToVector4());
        if (ImGui.Begin("Puma Settings", ref openPR, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            ImGui.BeginDisabled(simulationController.run);
            ImGui.SliderAngle("Alfa", ref pumaRight.a1,-180,180);
            if(ImGui.SliderAngle("Beta", ref pumaRight.a2,-180,180))
            {
                //Console.WriteLine($"Angle a is {90 - MathHelper.RadiansToDegrees(pumaLeft.a2)}");
                //Console.WriteLine($"Current end is {pumaLeft.currentEnd}");
            }
            ImGui.SliderAngle("Gamma", ref pumaRight.a3,-180,180);
            ImGui.SliderAngle("Sigma", ref pumaRight.a4, 0, 360);
            ImGui.SliderAngle("Delta", ref pumaRight.a5, 0, 360);

            if(ImGui.DragFloat("L1",ref GlobalPumaLengths.l1,0.0f,10.0f))
            {
                pumaRight.len1 = GlobalPumaLengths.l1;
                pumaLeft.len1 = GlobalPumaLengths.l1;
                pumaRight.UpdateLen(1);
                pumaLeft.UpdateLen(1);
            }
            if (ImGui.DragFloat("L2", ref pumaRight.len2, 0.0f, 10.0f))
            {
                if (!simulationController.run)
                {
                    GlobalPumaLengths.l2 = pumaRight.len2;
                    pumaLeft.len2 = GlobalPumaLengths.l2;
                    pumaRight.UpdateLen(2);
                    pumaLeft.UpdateLen(2);
                }
            }
            if (ImGui.DragFloat("L3", ref GlobalPumaLengths.l3, 0.0f, 10.0f))
            {
                pumaRight.len3 = GlobalPumaLengths.l3;
                pumaLeft.len3 = GlobalPumaLengths.l3;
                pumaRight.UpdateLen(3);
                pumaLeft.UpdateLen(3);
            }
            if (ImGui.DragFloat("L4", ref GlobalPumaLengths.l4, 0.0f, 10.0f))
            {
                pumaRight.len4 = GlobalPumaLengths.l4;
                pumaLeft.len4 = GlobalPumaLengths.l4;
                pumaRight.UpdateLen(4);
                pumaLeft.UpdateLen(4);
            }
            ImGui.EndDisabled();
        }
        ImGui.PopStyleColor();
        ImGui.End();

        ImGui.SetNextWindowSize(new System.Numerics.Vector2(400, 250), ImGuiCond.Once);
        ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero);   
        ImGui.PushStyleColor(ImGuiCol.WindowBg, Color.DarkSlateGray.ToVector4());
        if (ImGui.Begin("Config Settings", ref openPR, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            if(ImGui.TreeNode("Start"))
            {
                ImGui.DragFloat3("Position", startCoord.Position, 0.1f, -10,10);
                if(ImGui.DragFloat3("Euler", startCoord.EulerAngle,0.5f, -180, 180))
                {
                    var data = HelpConverters.ConvertEulerToQuaternion(startCoord.Euler);
                    startCoord.QuatData = new Vector4(data.X, data.Y, data.Z, data.W);
                    startCoord.UpdateQuaternionData();
                }
                if(ImGui.DragFloat4("Quaternion", startCoord.Quaternion,0.01f, -10, 10))
                {
                    startCoord.UpdateQuaternionData();
                    var data = HelpConverters.ConvertQuaternionToEuler(startCoord.Quat);
                    startCoord.Euler = data;
                }
                ImGui.TreePop();
            }
            if (ImGui.TreeNode("End"))
            {
                ImGui.DragFloat3("Position", endCoord.Position, 0.1f, -10, 10);
                if (ImGui.DragFloat3("Euler", endCoord.EulerAngle,0.5f, -180, 180))
                {
                    var data = HelpConverters.ConvertEulerToQuaternion(endCoord.Euler);
                    endCoord.QuatData = new Vector4(data.X, data.Y, data.Z, data.W);
                    endCoord.UpdateQuaternionData();
                }
                if (ImGui.DragFloat4("Quaternion", endCoord.Quaternion,0.01f, -10, 10))
                {
                    endCoord.UpdateQuaternionData();
                    var data = HelpConverters.ConvertQuaternionToEuler(endCoord.Quat);
                    endCoord.Euler = data;
                }
                ImGui.TreePop();
            }
            if(ImGui.TreeNode("Buttons"))
            {
                if (ImGui.Button("Generate positions out of start coord"))
                {
                    pumaLeft.GetPositions(startCoord, shader, true);
                    pumaRight.GetPositions(startCoord, shader, true);
                }
                if (ImGui.Button("Get start coord to current pumaLeft pos"))
                {
                    startCoord.Pos = pumaRight.currentEnd.Xyz;
                    startCoord.Quat = pumaRight.currentRot;
                    startCoord.QuatData = new Vector4(startCoord.Quat.X, startCoord.Quat.Y, startCoord.Quat.Z, startCoord.Quat.W);
                }
                if (ImGui.Button("Move PUMA to current start cooord"))
                {
                    pumaLeft.MovePumaToCurrentCoord(startCoord, shader, true);
                    pumaRight.MovePumaToCurrentCoord(endCoord, shader, true);
                }
                ImGui.TreePop();
            }
            if (ImGui.TreeNode("Simulation"))
            {
                ImGui.Text("Simulation: "); ImGui.SameLine(); ImGui.Text(simulationController.run.ToString());
                ImGui.Text("Time: "); ImGui.SameLine(); ImGui.Text(simulationController.currentTime.ToString());
                if (ImGui.Button("Start"))
                {
                    //SimulationController.TestInstance(new Vector2(0, MathHelper.DegreesToRadians(70)));
                    //SimulationController.path = space.path;
                    simulationController.Start();
                    //SimulationController.endNextFrame = false;
                }
                ImGui.SameLine();
                if (ImGui.Button("Pause")) { simulationController.pause = true; simulationController.run = false; }
                ImGui.SameLine();
                if (ImGui.Button("Stop")) { simulationController.Stop(); }
                ImGui.SliderFloat("Time", ref simulationController.animationTime, 0.01f, 10.0f);
                //ImGui.SliderFloat("Speed", ref SimulationSettings.SimulationSpeed, 0.01f, 1.0f);
                ImGui.TreePop();
            }
        }
        ImGui.PopStyleColor();
        ImGui.End();



        Controller.Render();

        SwapBuffers();
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Keys.F5) simulationController.Start();
        if (e.Key == Keys.F6) { simulationController.pause = true; simulationController.run = false; }
        if (e.Key == Keys.F7) { simulationController.Stop(); }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        if (this.MouseState[MouseButton.Right])
        {
            var delta = e.Delta.Y;
            camera.ChangeDistance((float)(delta * 0.01f));

            OrbitCamera.ChangeDist((float)(delta * 0.001f));
        }

        if (this.MouseState[MouseButton.Middle])
        {
            double speed = 0.002;
            var pos = e.Position;
            double deltaY = pos.Y - prev_mouse.Y;
            double deltaX = pos.X - prev_mouse.X;
            camera.UpdateRotation((float)(deltaY * speed), (float)(deltaX * speed));

            OrbitCamera.RotateX((float)(deltaY * speed));
            OrbitCamera.RotateY((float)(deltaX * speed));
        }


        prev_mouse = e.Position;
    }

    #region SampleDocking

    private ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags.None;

    void DrawDockSpaceOptionsBar(ref bool p_open)
    {
        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), dockspace_flags);
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Options"))
            {
                if (ImGui.MenuItem("Enable Docking", "IO.ConfigFlags.DockingEnable", ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable))) {
                    ImGui.GetIO().ConfigFlags ^= ImGuiConfigFlags.DockingEnable;
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Require Shift For Docking", "IO.ConfigDockingWithShift",
                    ImGui.GetIO().ConfigDockingWithShift)) { ImGui.GetIO().ConfigDockingWithShift = !ImGui.GetIO().ConfigDockingWithShift; }

                ImGui.Separator();

                if (ImGui.MenuItem("Flag: NoSplit", "", (dockspace_flags & ImGuiDockNodeFlags.NoSplit) != 0)) {  dockspace_flags ^= ImGuiDockNodeFlags.NoSplit; }
                if (ImGui.MenuItem("Flag: NoResize", "", (dockspace_flags & ImGuiDockNodeFlags.NoResize) != 0)) { dockspace_flags ^= ImGuiDockNodeFlags.NoResize; }
                if (ImGui.MenuItem("Flag: NoDockingInCentralNode", "", (dockspace_flags & ImGuiDockNodeFlags.NoDockingInCentralNode) != 0)) { dockspace_flags ^= ImGuiDockNodeFlags.NoDockingInCentralNode; }
                if (ImGui.MenuItem("Flag: AutoHideTabBar", "", (dockspace_flags & ImGuiDockNodeFlags.AutoHideTabBar) != 0)) { dockspace_flags ^= ImGuiDockNodeFlags.AutoHideTabBar; }
                if (ImGui.MenuItem("Flag: PassthruCentralNode", "", (dockspace_flags & ImGuiDockNodeFlags.PassthruCentralNode) != 0)) { dockspace_flags ^= ImGuiDockNodeFlags.PassthruCentralNode; }
                ImGui.EndMenu();
            }
            HelpMarker(
                @"When docking is enabled, you can ALWAYS dock MOST window into another! Try it now!
    
- Drag from window title bar or their tab to dock/undock.
    
- Drag from window menu button (upper-left button) to undock an entire node (all windows).
    
- Hold SHIFT to disable docking (if io.ConfigDockingWithShift == false, default)
    
- Hold SHIFT to enable docking (if io.ConfigDockingWithShift == true)");

            ImGui.EndMainMenuBar();
        }

        static void HelpMarker(string desc)
        {
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
    }

    

    #endregion

    #region SampleExtraFont

    private bool? SampleExtraFontFlag;

    private unsafe void SampleExtraFontImpl()
    {
        if (SampleExtraFontFlag is false)
            return;

        SampleExtraFontFlag = false;

        var io = ImGui.GetIO();

        var fonts = io.Fonts;

        var ranges = fonts.GlyphRangesDefault;

        var size = Controller.GetDpiScaledFontSize(12.0f);

        var font = fonts.AddFontFromFileTTF("Roboto-Regular.ttf", size, null, ref *ranges);

        Debug.WriteLine(font);

        fonts.Build();

        Controller.UpdateFontsTextureAtlas();
    }

    private void SampleExtraFontDemo()
    {
        ImGui.SetNextWindowSize(new  System.Numerics.Vector2(400, 100), ImGuiCond.Once);

        if (ImGui.Begin("Sample: extra font"))
        {
            if (SampleExtraFontFlag is false)
            {
                ImGui.Text("Change font from Tools/Style Editor.");
            }
            else
            {
                if (ImGui.Button("Click to add an extra font"))
                {
                    SampleExtraFontFlag = true;
                }
            }
        }

        ImGui.End();
    }

    #endregion
}