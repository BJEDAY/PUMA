using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApplication.OpenTK
{
    public class Grid
    {
        private int GridNum = 100;
        private float GridSize = 0.5f;

        public int VertexArrayObject { get; set; }
        public int VertexBufferObject { get; set; }
        public int ElementBufferObject { get; set; }

        public int IndicesCount { get; set; }
        public Grid()
        {
            GenerateVAO();
        }
        public (float[] vertices, int[] indices) MakeMesh()
        {
            List<float> vertices = new List<float>();
            List<int> indices = new List<int>();
            float offset = GridNum * GridSize * 0.5f;

            for (int i = 0; i < GridNum; i++)   //row
            {
                for (int j = 0; j < GridNum; j++)   //col
                {
                    vertices.Add(j * GridSize - offset);   // position x
                    vertices.Add(i * GridSize - offset);   // position y
                    vertices.Add(0);    // position z
                }
            }

            for (int i = 0; i < GridNum; i++)   //row
            {
                for (int j = 0; j < GridNum; j++)   //col
                {
                    int elem_num = i * GridNum + j;
                    int next_right = elem_num + 1;
                    int next_up = elem_num + GridNum;
                    if (i == GridNum - 1 && j == GridNum - 1)   //last elemement in top right corner
                    {
                        continue;
                    }
                    else if (i == GridNum - 1) // last row, nothing up
                    {
                        indices.Add(elem_num); indices.Add(next_right);
                    }
                    else if (j == GridNum - 1) //last col, nothing on right
                    {
                        indices.Add(elem_num); indices.Add(next_up);
                    }
                    else //there is element to connect up and right
                    {
                        indices.Add(elem_num); indices.Add(next_right);
                        indices.Add(elem_num); indices.Add(next_up);
                    }
                }
            }
            return (vertices.ToArray(), indices.ToArray());
        }

        public void GenerateVAO() //generates Vertex Array Object used for drawing it on screen
        {
            var (vertices, indices) = MakeMesh();
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            VertexBufferObject = GL.GenBuffer(); //tworzy bufor na karcie graficznej i daje uchwyt dla VertexBufferObject
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject); //przypisujemy bufor do danego typu
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw); //copies the previously defined vertex data into the buffer's memory
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            IndicesCount = indices.Length;
        }

        public void Draw(Shader shader, Matrix4 view, Matrix4 perspective)
        {
            shader.Use();
            shader.SetMatrix4("persp", perspective);
            shader.SetMatrix4("view", view);
            //shader.SetMatrix4("transform", Matrix4.Identity);
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Lines, IndicesCount, DrawElementsType.UnsignedInt, 0);
        }
    }
}
