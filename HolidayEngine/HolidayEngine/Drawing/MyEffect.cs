using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using HolidayEngine.Level;

namespace HolidayEngine.Drawing
{
    public enum Technique
    {
        Simple,
        Point,
        Color,
        ShadowMap
    }

    public class MyEffect : Effect
    {
        private Matrix worldMatrix;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        private Texture2D texture;

        private float alpha;
        private float ambience;
        private float lightIntensity;
        private Vector3 lightDirection;
        private Vector3 lightPosition;

        private RenderTarget2D renderTarget;
        public Texture2D shadowMap;

        public MyEffect(Engine engine, Effect SourceEffect)
            : base(SourceEffect)
        {
            this.CurrentTechnique = this.Techniques["Simple"];
            this.LightDirection = new Vector3(0.5f, 0.8f, -0.8f);
            this.LightIntensity = 0.6f;
            this.Ambience = 0.6f;
            this.Alpha = 1f;

            PresentationParameters pp = engine.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(engine.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, engine.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        }

        public void UpdateShadowMap(Engine engine)
        {
            Matrix ShadowMatrix = Matrix.Identity;
            ShadowMatrix *= Matrix.CreateLookAt(lightPosition, lightPosition + lightDirection, new Vector3(0, 0, 1));
            ShadowMatrix *= Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 2, 1.0f, 5.0f, 800.0f);
            this.Parameters["xShadowMapWorldViewProj"].SetValue(ShadowMatrix);

            this.SetTechnique = Technique.ShadowMap;
            engine.GraphicsDevice.SetRenderTarget(renderTarget);
            engine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            engine.room.DrawRoom(engine);

            engine.GraphicsDevice.SetRenderTarget(null);
            shadowMap = (Texture2D)renderTarget;
            this.Parameters["xShadowMap"].SetValue(shadowMap);
            this.SetTechnique = Technique.Point;
        }

        public void PlaceLightUsingDirection(Engine engine, int BlockDistance)
        {
            LightPosition = Vector3.UnitX * (engine.room.Width / 2) * Block.Size
                            + Vector3.UnitZ * (engine.room.Height / 2) * Block.Size
                            + Vector3.UnitY * Block.Size * (engine.room.Depth / 2)
                            - LightDirection * (BlockDistance) * Block.Size;
        }

        public Technique SetTechnique
        {
            set
            {
                switch (value)
                {
                    case Technique.Simple:
                        this.CurrentTechnique = this.Techniques["Simple"];
                        break;
                    case Technique.Point:
                        this.CurrentTechnique = this.Techniques["Point"];
                        break;
                    case Technique.Color:
                        this.CurrentTechnique = this.Techniques["Color"];
                        break;
                    case Technique.ShadowMap:
                        this.CurrentTechnique = this.Techniques["ShadowMap"];
                        break;
                }
            }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; this.Parameters["xTexture"].SetValue(value); }
        }

        public Matrix World
        {
            get { return worldMatrix; }
            set { worldMatrix = value; this.Parameters["xWorld"].SetValue(value); }
        }

        public Matrix View
        {
            get { return viewMatrix; }
            set { viewMatrix = value; this.Parameters["xView"].SetValue(value); }
        }

        public Matrix Projection
        {
            get { return projectionMatrix; }
            set { projectionMatrix = value; this.Parameters["xProjection"].SetValue(value); }
        }

        public float Alpha
        {
            get { return alpha; }
            set { alpha = value; this.Parameters["xAlpha"].SetValue(value); }
        }

        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = Vector3.Normalize(value); this.Parameters["xLightDirection"].SetValue(-Vector3.Normalize(value)); }
        }

        public Vector3 LightPosition
        {
            get { return lightPosition; }
            set { lightPosition = value; this.Parameters["xLightPosition"].SetValue(value); }
        }

        public float Ambience
        {
            get { return ambience; }
            set { ambience = value; this.Parameters["xAmbience"].SetValue(value); }
        }

        public float LightIntensity
        {
            get { return lightIntensity; }
            set { lightIntensity = value; this.Parameters["xLightIntensity"].SetValue(value); }
        }
    }
}
