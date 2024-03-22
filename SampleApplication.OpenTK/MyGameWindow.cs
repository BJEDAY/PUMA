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
// TESTING THINGS
internal sealed class MyGameWindow : GameWindowBaseWithDebugContext
{
    private static readonly double[] SampleData1 = Enumerable.Range(0, 256).Select(s => Math.Cos(s / 2.0d / Math.PI)).ToArray();

    private static readonly double[] SampleData2 = Enumerable.Range(0, 256).Select(s => Math.Sin(s / 2.0d / Math.PI)).ToArray();

    private readonly ImGuiController Controller;

    private readonly ImPlotContext ImPlotContext;

    private Color4 Color1 = Color4.Crimson;

    private Color4 Color2 = Color4.DeepSkyBlue;

    private bool ShowImGuiDemo = true;

    private bool ShowImPlotDemo = true;

    private bool ShowDockingDemo = true;

    Shader shader; Camera camera; ViewPerspectiveSettings perspectiveSettings; Line line; Shader phongShader; Cylinder cylinder; Vector2 prev_mouse; Vector3 lightColor; Vector3 lightPos;
     //Grid grid; //Shader gridShader;
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
    }

    protected void SetupObjects()
    {
        line = new Line();
        cylinder = new Cylinder();
        //grid = new Grid();
    }
    protected void SetupShaders()
    {
        // instead of using path "Shaders/ShaderVerts.glsl and using option "copy to output directory" the path is given directly to the source of shaders (every change gonna be instant)
        //shader = new Shader("../../../Shaders/ShaderVert.glsl", "../../../Shaders/ShaderFrag.glsl");

        //gridShader = new Shader("../../../Shaders/GridShaderVert.glsl", "../../../Shaders/GridShaderFrag.glsl");

        //gridShader.Use();
        //gridShader.SetVec4("backgroundColor", new Vector4(0.66f, 0.66f, 0.66f, 1f));
        //gridShader.SetVec4("gridColor", new Vector4(1f, 0f, 0f, 1f));


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
        camera.UpdateProjectionMatrix((float)ClientSize.X, (float)ClientSize.Y, perspectiveSettings.fov, perspectiveSettings.n, perspectiveSettings.f);
    }

    protected void SetupGL()
    {
        GL.Enable(EnableCap.CullFace);
        //GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.ClearColor(Color.CornflowerBlue);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        //line.Draw(shader, camera.viewMatrix, camera.projectionMatrix);


        ImGui.SetNextWindowSize(new System.Numerics.Vector2(800, 500), ImGuiCond.Once);

        if (ShowDockingDemo)
        {
            //DrawDockSpaceOptionsBar(ref ShowDockingDemo);
        }


        if (ImGui.Begin("Hello, world!"))
        {
            if (ImPlot.BeginPlot("Sample plot"))
            {
                ImPlot.SetupAxes("X", "Y");

                ImPlot.SetNextLineStyle(Color1.ToVector4());
                ImPlot.PlotLine("Sample data 1", ref SampleData1[0], SampleData1.Length);

                ImPlot.SetNextLineStyle(Color2.ToVector4());
                ImPlot.PlotLine("Sample data 2", ref SampleData2[0], SampleData2.Length);

                ImPlot.EndPlot();
            }

            ImGui.ColorEdit4("Color 1", Color1.AsSpan(), ImGuiColorEditFlags.NoInputs);
            ImGui.ColorEdit4("Color 2", Color2.AsSpan(), ImGuiColorEditFlags.NoInputs);

            ImGui.Checkbox("Show ImGui Demo", ref ShowImGuiDemo);
            ImGui.Checkbox("Show ImPlot Demo", ref ShowImPlotDemo);
            ImGui.Checkbox("Show Docking Demo", ref ShowDockingDemo);
        }

        ImGui.End();

        //SampleExtraFontDemo();

        if (ShowImGuiDemo)
        {
            //ImGui.ShowDemoWindow(ref ShowImGuiDemo);
        }

        if (ShowImPlotDemo)
        {
            //ImPlot.ShowDemoWindow(ref ShowImPlotDemo);
        }

        Controller.Render();

        //grid.Draw(gridShader, camera.viewMatrix, camera.projectionMatrix);
        cylinder.Render(phongShader, camera.viewMatrix, camera.projectionMatrix, camera.cameraPosition, new Vector3(1.0f, 0.0f, 0.0f));

        SwapBuffers();
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
        }

        if (this.MouseState[MouseButton.Middle])
        {
            double speed = 0.2;
            var pos = e.Position;
            double deltaY = pos.Y - prev_mouse.Y;
            double deltaX = pos.X - prev_mouse.X;
            camera.UpdateRotation((float)(deltaY * speed), (float)(deltaX * speed));
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