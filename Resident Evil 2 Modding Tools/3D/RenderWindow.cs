using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

//using OpenTK;
//using OpenTK.Input;
//using OpenTK.Graphics;
//using SmoothGL.Graphics;

namespace Resident_Evil_2_Modding_Tools._3D
{
    /*public class RenderWindow : GameWindow
    {
        protected float NearPlane = 0.1f;
        protected float FarPlane = 1000.0f;

        private Matrix4 _projection;
        private float _elapsedTime;
        private float _cameraPitch;
        private float _cameraYaw;
        private float _cameraDistance;
        private bool _drag;

        /// <summary>
        /// Creates a new sample window with fixed size and specified title. A graphics context is
        /// initialized for OpenGL version 3.3.
        /// </summary>
        /// <param name="title"></param>
        public RenderWindow(string title)
           : base(1024, 768, GraphicsMode.Default, title, GameWindowFlags.Default, DisplayDevice.Default, 3, 3, GraphicsContextFlags.Default)
        {
            _elapsedTime = 0.0f;
            _cameraPitch = 0.0f;
            _cameraYaw = 0.0f;
            _cameraDistance = 5.0f;
            _drag = false;

            UpdateProjection();
        }

        public float speed = 1.5f;
        public Vector3 position = new Vector3(0.0f, 0.0f, 0f);
        public Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.W:
                    position += front * speed * 1;
                    break;
                case Key.S:
                    position -= front * speed * 1;
                    break;
                case Key.A:
                    position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * 1;
                    break;
                case Key.D:
                    position += Vector3.Normalize(Vector3.Cross(front, up)) * speed * 1;
                    break;
                case Key.Space:
                    position += (up * speed * 1);
                    break;
                case Key.C:
                    position -= (up * speed * 1);
                    break;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                _drag = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                _drag = false;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouse = Mouse.GetState();

            if (_drag)
            {
                _cameraPitch += 0.01f * e.YDelta;
                _cameraYaw += 0.01f * e.XDelta;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _cameraDistance = MathHelper.Clamp(_cameraDistance - 0.2f * e.Delta, 1.0f, 10.0f);
        }

        MouseState mouse;
        float deltaTime = 0;
        Vector2 lastPos = new Vector2(0, 0);
        float yaw = 0;
        float pitch = 0;
        float sensitivity = 1;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            deltaTime = (float)e.Time;
            _elapsedTime += (float)e.Time;

            float deltaX = mouse.X - lastPos.X;
            float deltaY = mouse.Y - lastPos.Y;
            lastPos = new Vector2(mouse.X, mouse.Y);
            yaw += deltaX * sensitivity;

            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            front = Vector3.Normalize(front);
        }

        protected override void OnResize(EventArgs e)
        {
            UpdateProjection();
            UpdateViewport();
        }

        /// <summary>
        /// Re-calculates the camera's projection matrix, which is required when the aspect ratio of the
        /// window changes.
        /// </summary>
        private void UpdateProjection()
        {
            float aspectRatio = (float)Width / (float)Height;
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), aspectRatio, NearPlane, FarPlane);
        }

        /// <summary>
        /// Sets the size of the default frame buffer to the size of the window, which is required when
        /// the window is resized.
        /// </summary>
        private void UpdateViewport()
        {
            FrameBufferTarget.Default.Viewport = new Rectangle(0, 0, Width, Height);
        }

        /// <summary>
        /// Gets the camera's projection matrix.
        /// </summary>
        protected Matrix4 CameraProjection
        {
            get
            {
                return _projection;
            }
        }

        /// <summary>
        /// Gets the camera's view matrix. The camera angle and zoom can be adjusted with mouse controls.
        /// </summary>
        /*protected Matrix4 CameraView
        {
            get
            {
                //return Matrix4.CreateTranslation(camPos);
                return Matrix4.CreateRotationY(_cameraYaw) * Matrix4.CreateRotationX(_cameraPitch) * Matrix4.CreateTranslation(0, -_cameraDistance, 0);
            }
        }

        /// <summary>
        /// Gets the time since window initialization in seconds, useful for time-based animations.
        /// </summary>
        protected float ElapsedTime
        {
            get
            {
                return _elapsedTime;
            }
        }
    }*/
}
