using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HolidayEngine.Drawing
{
    public class PrimManager
    {
        /// <summary>
        /// Holds a reference to the current graphic device.
        /// </summary>
        private GraphicsDevice device;

        /// <summary>
        /// The effect used for all drawing.
        /// </summary>
        public MyEffect myEffect;


        /// <summary>
        /// The matrix for world transformations.
        /// </summary>
        public Matrix worldMatrix;

        /// <summary>
        /// The matrix for the view transformations.
        /// </summary>
        public Matrix viewMatrix;

        /// <summary>
        /// The matrix for the projection transformations.
        /// </summary>
        public Matrix projectionMatrix;


        /// <summary>
        /// The matrix used for orthographic views.
        /// </summary>
        public Matrix worldMatrixOrtho;

        /// <summary>
        /// The matrix used for orthographic views.
        /// </summary>
        private Matrix viewMatrixOrtho;

        /// <summary>
        /// The matrix used for orthographic views.
        /// </summary>
        private Matrix projectionMatrixOrtho;


        /// <summary>
        /// Graphic card state settings for the rasterizer.
        /// Used to set culling.
        /// </summary>
        private RasterizerState rasterizerState;

        /// <summary>
        /// Graphic card state settings for the texture sampler.
        /// </summary>
        private SamplerState samplerState;

        /// <summary>
        /// Graphic card settings for when the depth buffer is off.
        /// </summary>
        private DepthStencilState depthOffState;


        /// <summary>
        /// Constructs a new primitive manager.
        /// </summary>
        /// <param name="engine">The current game engine.</param>
        /// <param name="effect">The effect file to load into MyEffect.</param>
        public PrimManager(Engine engine, Effect effect)
        {
            // Gets the graphic device.
            this.device = engine.GraphicsDevice;

            // Sets the 3D matrix transformations.
            worldMatrix = Matrix.Identity;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 3, device.Viewport.AspectRatio, 5.0f, 800.0f);
            viewMatrix = Matrix.CreateLookAt(new Vector3(-8, -8, 8), Vector3.Zero, new Vector3(0, 0, 1));

            // Sets the orthographic matrix transformations.
            worldMatrixOrtho = Matrix.Identity;
            projectionMatrixOrtho = Matrix.CreateOrthographicOffCenter(0, engine.windowSize.X, -engine.windowSize.Y, 0, -512f, 512f);
            viewMatrixOrtho = new Matrix(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);

            // Sets the sampler states.
            // NOTE: that these settings are overrided in the effects file.
            samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            samplerState.AddressW = TextureAddressMode.Clamp;
            samplerState.Filter = TextureFilter.Point;
            device.SamplerStates[0] = samplerState;

            // Sets the rasterizer states, used to set culling.
            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            device.RasterizerState = rasterizerState;

            // Creates a depth buffer set off.
            depthOffState = new DepthStencilState();
            depthOffState.DepthBufferEnable = false;
            device.DepthStencilState = DepthStencilState.Default;

            // Creates and sets the effect settings.
            myEffect = new MyEffect(engine, effect);
            myEffect.World = worldMatrix;
            myEffect.Projection = projectionMatrix;
            myEffect.View = viewMatrix;
        }


        /// <summary>
        /// Sets the camera view.
        /// </summary>
        public void SetView(Vector3 CameraPosition, Vector3 LookAtPosition)
        {
            viewMatrix = Matrix.CreateLookAt(CameraPosition, LookAtPosition, new Vector3(0, 0, 1));
            myEffect.View = viewMatrix;
        }


        /// <summary>
        /// Sets to render orthographically.
        /// </summary>
        public void SetOrthographic()
        {
            myEffect.World = worldMatrixOrtho;
            myEffect.Projection = projectionMatrixOrtho;
            myEffect.View = viewMatrixOrtho;
        }


        /// <summary>
        /// Sets to render in regular 3D.
        /// </summary>
        public void SetPerspective()
        {
            myEffect.World = worldMatrix;
            myEffect.Projection = projectionMatrix;
            myEffect.View = viewMatrix;
        }


        /// <summary>
        /// Sets the depth buffer on or off.
        /// </summary>
        public void SetDepthBuffer(bool DepthBuffer)
        {
            if (DepthBuffer)
                device.DepthStencilState = DepthStencilState.Default;
            else
                device.DepthStencilState = depthOffState;
        }


        /// <summary>
        /// Call this after a spriteBatch call to return to 3D properly.
        /// </summary>
        public void Reset3D(Engine engine)
        {
            device.SamplerStates[0] = samplerState;
            engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }


        /// <summary>
        /// Draws the vertices using current settings.
        /// </summary>
        public void DrawVertices(VertexIndexData vertexIndexData, Texture2D texture)
        {
            myEffect.Texture = texture;
            DrawVertices(vertexIndexData.VertexArray, vertexIndexData.IndexArray);
        }

        /// <summary>
        /// Draws the vertices using current settings.
        /// </summary>
        public void DrawVertices(VertexPositionNormalTexture[] vertices, short[] indices, Texture2D texture)
        {
            myEffect.Texture = texture;
            DrawVertices(vertices, indices);
        }

        /// <summary>
        /// Draws the vertices using current settings.
        /// </summary>
        private void DrawVertices(VertexPositionNormalTexture[] vertices, short[] indices)
        {
            foreach (EffectPass pass in myEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives
                    <VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    vertices, 0, vertices.Length,
                    indices, 0, indices.Length / 3
                    );
            }
        }


        /// <summary>
        /// Draws lines.
        /// </summary>
        public void DrawLines(LineData lineData)
        {
            DrawLines(lineData.VertexArray, lineData.IndexArray);
        }

        /// <summary>
        /// Draws lines.
        /// </summary>
        private void DrawLines(VertexPositionColor[] vertices, short[] indices)
        {
            myEffect.SetTechnique = Technique.Color;
            foreach (EffectPass pass in myEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives
                    <VertexPositionColor>(
                    PrimitiveType.LineList,
                    vertices, 0, vertices.Length,
                    indices, 0, indices.Length / 2
                    );
            }
            myEffect.SetTechnique = Technique.Point;
        }
    }
}
