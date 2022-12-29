using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpentkControl
{
    class OptkControl:GLControl
    {
        //Test FilePath
        string filePath = "E:/JHL-SYD/Code/STLTestFile/Cobra  Single Head.stl";
        Mesh mesh;

        //Mouse Control Setting
        float moveSpeed = 0.05f;
        float zoomSpeed = 0.01f;
        float sensitivityRotate = 0.8f;
        Vector2 lastPos;

        //Euler Angle Setting
        float yaw = 0.0f;
        float pitch = -90.0f;
        float fov = 45.0f;

        //Mouse Button Flag
        bool leftButtonDown = false;
        bool rightButtonDown = false;

        //View Matrix Setting
        Vector3 position, up, front;

        //Light Position
        private readonly Vector3 _lightPos = new Vector3(10.0f, 10.0f, 30.0f);

        //Shader Program
        Shader _shader;

        //VAO VBO EBO
        int VertexBufferObject;
        int VertexArrayObject;
        int ElementBufferObject;

        public OptkControl():base(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 4))
        {
            this.Load += OnLoad;
            this.Paint += OnPaint;
            this.Resize += OnResize;

            position = new Vector3(0.0f, 0.0f, 20.0f);
            up = new Vector3(0.0f, 1.0f, 0.0f);
            front = new Vector3(0.0f, 0.0f, -1.0f);

            Loader loader = new Loader();
            mesh = loader.LoadSTL(filePath);
        }
        private void OnLoad(object sender, System.EventArgs e)
        {
            // 在此处初始化OpenGL状态
            this.MakeCurrent();
            //配置shader program
            _shader = new Shader("shader.vert", "shader.frag");

            //清屏颜色
            GL.ClearColor(0.9f, 0.9f, 0.9f, 1.0f);

            //创建VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            //创建VBO
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.vertices.Length * sizeof(float), mesh.vertices, BufferUsageHint.StaticDraw);

            //创建EBO
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.indices.Length * sizeof(uint), mesh.indices, BufferUsageHint.StaticDraw);

            //设置顶点属性并启用
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // OptkControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.Name = "OptkControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);

        }

        private void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            this.MakeCurrent();

            // 在此处绘制3D图形            
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            //设置MVP矩阵
            float centerX = (mesh.maxX + mesh.minX) * 0.5f;
            float centerY = (mesh.maxY + mesh.minY) * 0.5f;
            float centerZ = (mesh.maxZ + mesh.minZ) * 0.5f;
            Matrix4 model = Matrix4.CreateTranslation(-centerX, -centerY, -centerZ) *
                                        Matrix4.CreateScale(0.1f) *
                                        Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(yaw)) *
                                        Matrix4.CreateRotationX(MathHelper.DegreesToRadians(pitch));
            Matrix4 view = Matrix4.LookAt(position, position + front, up);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov),Width / (float)Height, 0.1f, 100.0f);
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetVector3("lightPos", _lightPos);
            _shader.SetVector3("viewPos", position);
            _shader.SetVector3("material.ambient", new Vector3(0.60f, 0.60f, 0.60f));
            _shader.SetVector3("material.diffuse", new Vector3(0.68f, 0.68f, 0.68f));
            _shader.SetVector3("material.specular", new Vector3(0.95f, 0.95f, 0.95f));
            _shader.SetFloat("material.shininess", 32.0f);
            _shader.SetVector3("light.ambient", new Vector3(0.4f, 0.4f, 0.4f));
            _shader.SetVector3("light.diffuse", new Vector3(0.5f, 0.5f, 0.5f)); 
            _shader.SetVector3("light.specular", new Vector3(0.8f, 0.8f, 0.8f));

            //绘制图形
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, mesh.indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            //交换缓存
            this.SwapBuffers();
            //清除深度缓存
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            // 在此处处理窗口大小更改
            this.MakeCurrent();
            GL.Viewport(0, 0, (int)Width, (int)Height);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _shader.Dispose();
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (leftButtonDown)
            {
                float deltaX = e.X - lastPos.X;
                float deltaY = e.Y - lastPos.Y;
                lastPos = new Vector2(e.X, e.Y);
                yaw = (yaw + deltaX * sensitivityRotate) % 360;
                pitch = (pitch + deltaY * sensitivityRotate) % 360; // reversed since y-coordinates range from bottom to top
                this.Invalidate();
            }
            if (rightButtonDown)
            {
                float deltaX = e.X - lastPos.X;
                float deltaY = e.Y - lastPos.Y;
                lastPos = new Vector2(e.X, e.Y);
                position += (-Vector3.Normalize(Vector3.Cross(front, up)) * deltaX + up * deltaY) * moveSpeed;
                this.Invalidate();
            }
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                leftButtonDown = true;
                lastPos = new Vector2(e.X, e.Y);
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                rightButtonDown = true;
                lastPos = new Vector2(e.X, e.Y);
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                leftButtonDown = false;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                rightButtonDown = false;
            }
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            position += front * e.Delta * zoomSpeed;
            this.Invalidate();
        }
    }
}
