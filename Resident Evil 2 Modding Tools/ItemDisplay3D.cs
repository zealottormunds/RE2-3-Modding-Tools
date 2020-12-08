using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

/*using OpenTK;
using SmoothGL.Graphics;
using SmoothGL.Content;
using Resident_Evil_2_Modding_Tools._3D;

using System.IO;
using System.Globalization;

namespace Resident_Evil_2_Modding_Tools
{
    /// <summary>
    /// This sample shows how different, advanced rendering techniques can be achieved using SmoothGL.
    /// Resources are loaded relying on the content manager, multiple instances of the same geometry are drawn
    /// using hardware instancing and a skybox is rendered to demonstrate how cube mapping works. Furthermore,
    /// the scene is drawn to a custom frame buffer off-screen as a basis for post-processing effects.
    /// </summary>
    public class ItemDisplay3D : RenderWindow
    {
        private ContentManager _contentManager;
        private VertexArray _vertexArrayTorus;
        private VertexArray _vertexArrayRPD;
        private VertexArray _vertexArraySkybox;
        private ShaderProgram _shaderTorus;
        private ShaderProgram _shaderSky;
        private ShaderProgram _shaderPostProcessing;
        private ShaderProgram _shaderTest;
        private VertexBuffer _instanceBuffer;
        private Quad _quad;

        private FrameBuffer _frameBuffer;
        private ColorTexture2D _frameBufferColorTexture;
        private DepthStencilTexture2D _frameBufferDepthTexture;

        public List<Vector3> items = new List<Vector3>();

        public ItemDisplay3D()
            : base("Advanced Techniques Sample")
        {
            // A content manager is required to load resources such as meshes, textures and shaders from disk.
            // We specify that file paths are relative to the "Content" directory which is the location
            // where our content files will be stored. The default content reader for cube textures is
            // overwritten to load images with the provided layout.
            _contentManager = ContentManager.CreateDefault("Content");
            _contentManager.SetContentReader<TextureCube>(new TextureCubeReader(TextureFilterMode.Default, false, CubeTextureLayout.HorizontalCross));
        }

        protected override void OnLoad(EventArgs e)
        {
            int rendereditems = items.Count + 1;

            // Setup of the skybox. A cube builder is used to create cube geometry in memory. The faces
            // are flipped inwards since the skybox should surround the scene, seen from inside. From
            // this mesh data, a vertex buffer and vertex array is created as a corresponding
            // representation on the GPU that can be drawn later on.
            MeshData meshDataSkybox = (new CubeBuilder()).Build().GetFlippedTriangleOrientation();
            VertexBuffer vertexBufferSkybox = meshDataSkybox.ToVertexBuffer(MeshData.VertexPositionSelector, VertexPosition.VertexDeclaration);

            _vertexArraySkybox = new VertexArray(vertexBufferSkybox);

            // Adds the created graphics resources to the content manager. This makes sure that the
            // resources are disposed when unloading the content manager, avoiding manual cleanup.
            _contentManager.Add(vertexBufferSkybox);
            _contentManager.Add(_vertexArraySkybox);

            // Defines the layout of data that is passed to the vertex shader per instance. Since we
            // would like to draw a number of instances of the same model with different transformations,
            // we store a separate world matrix per instance, encoded as four float vectors. From
            // this declaration, a buffer holding information for nine instances is created.
            VertexDeclaration instanceDeclaration = new VertexDeclaration(
                new VertexElementFloat(3, 4),
                new VertexElementFloat(4, 4),
                new VertexElementFloat(5, 4),
                new VertexElementFloat(6, 4)
            );

            _instanceBuffer = new VertexBuffer(rendereditems, instanceDeclaration, BufferUsage.Dynamic);

            MeshData d = new MeshData(
                new Vector3[]
                {
                    new Vector3(0, -0.5f, -0.5f),
                    new Vector3(1, -0.5f, -0.5f),

                    new Vector3(0, 0.5f, -0.5f),
                    new Vector3(1, 0.5f, -0.5f),

                    new Vector3(0, -0.5f, 0.5f),
                    new Vector3(1, -0.5f, 0.5f),

                    new Vector3(0, 0.5f, 0.5f),
                    new Vector3(1, 0.5f, 0.5f),
                },
                new Vector3[]
                {
                    Vector3.Zero,
                    Vector3.Zero,

                    Vector3.Zero,
                    Vector3.Zero,

                    Vector3.Zero,
                    Vector3.Zero,

                    Vector3.Zero,
                    Vector3.Zero,
                },
                new Vector2[]
                {
                    Vector2.Zero,
                    Vector2.Zero,

                    Vector2.Zero,
                    Vector2.Zero,

                    Vector2.Zero,
                    Vector2.Zero,
                },
                new uint[]
                {
                    0, 1, 2,
                    1, 3, 2,

                    4, 0, 2,
                    2, 4, 6,

                    5, 6, 7,
                    4, 5, 6,

                    1, 3, 5,
                    3, 5, 7,

                    2, 3, 7,
                    2, 6, 7,

                    0, 1, 5,
                    0, 4, 5
                }
            );
            VertexBuffer vertexBufferTorus = d.ToVertexBuffer();
            ElementBuffer elementBufferTorus = d.ToElementBuffer();
            _vertexArrayTorus = new VertexArray(vertexBufferTorus, _instanceBuffer, elementBufferTorus);

            _contentManager.Add(vertexBufferTorus);
            _contentManager.Add(elementBufferTorus);
            _contentManager.Add(_vertexArrayTorus);
            //_contentManager.Add(_instanceBuffer);

            MeshData meshDataRPD = new MeshData(
                new Vector3[]
                {
                    new Vector3(-40, -1, -40),
                    new Vector3(-40, -1, 40),
                    new Vector3(40, -1, -40),
                    new Vector3(40, -1, 40),
                },
                new Vector3[]
                {
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 0),
                },
                new Vector2[]
                {
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),

                },
                new uint[]
                {
                    0, 2, 3,
                    0, 3, 1
                }
            );
            VertexBuffer vertexBufferRPD = meshDataRPD.ToVertexBuffer();
            ElementBuffer elementBufferRPD = meshDataRPD.ToElementBuffer();
            _vertexArrayRPD = new VertexArray(vertexBufferRPD, _instanceBuffer, elementBufferRPD);

            _contentManager.Add(vertexBufferRPD);
            _contentManager.Add(elementBufferRPD);
            _contentManager.Add(_vertexArrayRPD);

            _contentManager.Add(_instanceBuffer);

            // Loads different shaders that are required to draw the torus model, the skybox and
            // to apply a post-processing step to the entire screen after rendering the scene.
            // By default, the content manager expects a simple XML file that lists the paths
            // to the GLSL source code per shader stage.
            _shaderTorus = _contentManager.Load<ShaderProgram>("ShaderTorus.xml");
            _shaderSky = _contentManager.Load<ShaderProgram>("ShaderSky.xml");
            _shaderPostProcessing = _contentManager.Load<ShaderProgram>("ShaderPostProcessing.xml");

            // Sets the values of shader uniforms that do not change during the main loop. Color and
            // cube textures can be assigned to uniform samplers like any other primitive value.
            _shaderPostProcessing.Uniform("NearPlane").Value = NearPlane;
            _shaderPostProcessing.Uniform("FarPlane").Value = FarPlane;
            _shaderSky.Uniform("TextureSkybox").Value = _contentManager.Load<TextureCube>("Skybox.png");
            _shaderTorus.Uniform("Texture").Value = _contentManager.Load<ColorTexture2D>("Texture.png");

            // Creates a quad, which is essentially a vertex array with two triangles spanning the
            // whole screen. This is required for post-processing effects: First, the scene is rendered
            // off-screen and the resulting textures are then drawn to the quad with a post-processing
            // shader, which allows for modifications per pixel.
            _quad = new Quad();
            _contentManager.Add(_quad);

            // Creates a frame buffer, which is used as a target for off-screen rendering. Besides a
            // texture with normal RGBA color channels, a depth-stencil texture is also attached to
            // the frame buffer (from which, however, only the depth component is used). Information
            // on the rendered scene will then be stored in these textures, accessible by the
            // post-processing shader.
            _frameBuffer = new FrameBuffer(Width, Height);
            _frameBufferColorTexture = new ColorTexture2D(Width, Height, TextureColorFormat.Rgba32, TextureFilterMode.None);
            _frameBufferDepthTexture = new DepthStencilTexture2D(Width, Height);

            _frameBuffer.Attach(
                _frameBufferDepthTexture.CreateFrameBufferAttachment(),
                _frameBufferColorTexture.CreateFrameBufferAttachment()
            );

            _contentManager.Add(_frameBuffer);
            _contentManager.Add(_frameBufferColorTexture);
            _contentManager.Add(_frameBufferDepthTexture);
        }

        protected override void OnUnload(EventArgs e)
        {
            // Unloads the content manager. This is the only thing required for cleanup, since
            // all created graphics resources were added to the content manager for automatic
            // disposal. Resources directly loaded by the content manager were added implicitly.
            _contentManager.Unload();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Targets the custom frame buffer for off-screen rendering. All subsequent draw
            // operations do not affect the screen directly, but are instead forwarded to the
            // textures attached to this frame buffer. As normal, the old data is cleared
            // before starting to render the scene.
            _frameBuffer.SetAsTarget();
            _frameBuffer.Clear(TargetOptions.All, Color.CornflowerBlue, 1.0f, 0);

            // Draws the skybox. First, depth testing is disabled to prevent the skybox from
            // appearing in front of any object drawn later on. The corresponding shader is
            // then supplied with recent values for the camera view and projection before
            // being used to draw the skybox geometry.
            DepthState.None.Apply();

            Matrix4 view = Matrix4.LookAt(position, position + front, up);

            _shaderSky.Uniform("Projection").Value = CameraProjection;
            _shaderSky.Uniform("View").Value = view;
            _shaderSky.Use();
            _vertexArraySkybox.Draw(Primitive.Triangles);

            // Draws multiple instances of the torus object. Depth testing is enabled again
            // to ensure a plausible visual representation of solid geometry. After setting up
            // the shader, a unique world matrix is created for each of the nine instances of
            // the torus we would like to draw. The instance buffer is then updated to contain
            // these matrices. Finally, the vertex array of the torus is drawn multiple times
            // with a single rendering call.
            DepthState.Default.Apply();

            _shaderTorus.Uniform("Projection").Value = CameraProjection;
            _shaderTorus.Uniform("View").Value = view;
            _shaderTorus.Use();

            Matrix4[] instanceData = new Matrix4[items.Count + 1];
            for (int i = 0; i < items.Count; ++i)
            {
                Vector3 thisp = items[i];
 
                instanceData[i] = Matrix4.CreateScale(0.6f) *
                                  Matrix4.CreateRotationY(0) *
                                  Matrix4.CreateRotationX(0) *
                                  Matrix4.CreateTranslation(thisp);
            }

            _instanceBuffer.SetData(instanceData);
            _vertexArrayTorus.DrawMultiple(Primitive.Triangles, items.Count);

            instanceData[items.Count] = Matrix4.CreateScale(1) *
                Matrix4.CreateRotationY(0) *
                Matrix4.CreateRotationX(0) *
                Matrix4.CreateTranslation(new Vector3(0, 0, 0));

            _instanceBuffer.SetData(instanceData);
            _vertexArrayRPD.Draw(Primitive.Triangles);

            // Since we targeted the custom frame buffer when drawing the scene, nothing has
            // been displayed on the screen yet. In this final step, the post-processing
            // shader is provided access to the textures which hold color and depth data
            // of the previously rendered scene. Using this shader, the quad is drawn to
            // cover every pixel of the screen and therefore allowing to modify every pixel
            // arbitrarily in the fragment shader.
            FrameBufferTarget.Default.SetAsTarget();
            FrameBufferTarget.Default.Clear(TargetOptions.All, Color.CornflowerBlue, 1.0f, 0);

            _shaderPostProcessing.Uniform("TextureColor").Value = _frameBufferColorTexture;
            _shaderPostProcessing.Uniform("TextureDepth").Value = _frameBufferDepthTexture;
            _shaderPostProcessing.Use();
            _quad.Draw();

            // As usual, the back and the front frame buffers need to be swapped to present the
            // result on the display device.
            SwapBuffers();
        }
    }
}
*/