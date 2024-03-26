using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication.OpenTK
{
    public class Rect
    {
        // Vertex array object, vertex buffer object, element buffer object
        public int VAO { get; set; }
        public int VBO { get; set; }
        public int EBO { get; set; }

        public Matrix4 transform;

        // Basic line vertices and indices
        float[] verts;
        int[] indices;
        public float halfWidth = 0.01f;
        public bool moveRight;
        public Rect()
        {
            //verts = new float[6] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f };
            verts = new float[12] { -halfWidth, -1f, 0f, halfWidth, -1.0f, 0.0f, halfWidth, 1.0f, 0.0f, -halfWidth, 1.0f, 0.0f };
            //indices = new int[2] { 0, 1 };
            //indices = new int[6] { 0, 2, 1, 0, 3, 2 };
            indices = new int[6] { 1, 3, 0, 2, 3, 1 };
            transform = Matrix4.Identity;
            moveRight = true;
            GenerateVAO();
        }

        public void GenerateVAO()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.DynamicDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //// after setting up all data change it to default
            //GL.BindVertexArray(0);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void UpdateVAO()     // in this case propably Update will never be used
        {
            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }


        public void Render(Shader shader)
        {
            shader.Use();
            if(moveRight) shader.SetMatrix4("transform", Matrix4.CreateTranslation(1.0f - halfWidth, 0.0f, 0.0f));
            else shader.SetMatrix4("transform", Matrix4.CreateTranslation(-1.0f + halfWidth, 0.0f, 0.0f));
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

    }
}
