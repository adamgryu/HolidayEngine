using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace HolidayEngine.Drawing
{
    /// <summary>
    /// This class holds all the information needed to run an animation.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// The list of sub-animations this animation contains.
        /// </summary>
        public List<List<TextureData>> AniList = new List<List<TextureData>>();

        /// <summary>
        /// The index of the current sub-animation running.
        /// </summary>
        private short AniNumber = 0;

        /// <summary>
        /// A reference to the texture with the spritesheet on it.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// The current frame of the current animation.
        /// </summary>
        short Frame = 0;

        /// <summary>
        /// The speed at which frames switch.
        /// This is dynamic and can be assigned different values for different animations.
        /// The relationship is actually inverse, the greater the speed, the slower the animation moves.
        /// </summary>
        public short Speed = 6;

        /// <summary>
        /// The default speed at which the animation runs.
        /// This is used when the speed is not specified.
        /// </summary>
        private short DefaultSpeed = 6;

        /// <summary>
        /// A counter for the animation.
        /// </summary>
        short Counter = 0;


        /// <summary>
        /// Constructs an animation with default settings.
        /// </summary>
        public Animation(Texture2D Texture)
        {
            this.Texture = Texture;
        }


        /// <summary>
        /// Constructs an animation with custom settings.
        /// </summary>
        public Animation(Texture2D Texture, short DefaultSpeed, short StartingAnimationIndex)
        {
            this.AniNumber = StartingAnimationIndex;
            this.DefaultSpeed = Speed;
            this.Speed = DefaultSpeed;
            this.Texture = Texture;
        }


        /// <summary>
        /// Updates the animation frame at a steady speed.
        /// </summary>
        public void Update()
        {
            Counter++;
            if (Counter >= Speed)
            {
                Counter = 0;
                Frame++;
                if (Frame >= AniList[AniNumber].Count)
                    Frame = 0;
            }
        }

        /// <summary>
        /// Sets the animation index safely.
        /// </summary>
        public void SetAnimation(int index)
        {
            Speed = DefaultSpeed;
            AniNumber = (short)index;
            if (Frame >= AniList[AniNumber].Count)
                Frame = 0;
        }


        /// <summary>
        /// Sets the animation index safely with speed settings.
        /// </summary>
        public void SetAnimation(int num, int speed)
        {
            Speed = (short)speed;
            AniNumber = (short)num;
            if (Frame >= AniList[AniNumber].Count)
                Frame = 0;
        }


        /// <summary>
        /// Gets the current animation texture data for drawing.
        /// </summary>
        public TextureData texture
        {
            get { return AniList[AniNumber][Frame]; }
        }
    }
}
