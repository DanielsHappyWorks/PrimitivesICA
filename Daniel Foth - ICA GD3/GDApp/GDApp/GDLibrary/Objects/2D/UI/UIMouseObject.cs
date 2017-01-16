using GDApp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public class UIMouseObject : UITextureObject
    {

        #region Variables
        private string text;
        private SpriteFont spriteFont;
        private Vector2 textOffsetPosition;
        private Color textColor;
        private Vector2 textDimensions;
        private Vector2 textOrigin;

        #endregion

        #region Properties
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.textDimensions = this.spriteFont.MeasureString(this.text);
                this.textOrigin = new Vector2(this.textDimensions.X / 2, this.textDimensions.Y / 2);
            }
        }
        public SpriteFont SpriteFont
        {
            get
            {
                return this.spriteFont;
            }
            set
            {
                this.spriteFont = value;
            }
        }
        #endregion

        public UIMouseObject(string id, ActorType actorType, StatusType statusType, Transform2D transform,
        Color color, SpriteEffects spriteEffects, float layerDepth, Texture2D texture)
            : this(id, actorType, statusType, transform, color, spriteEffects, 
                  null, null, Vector2.Zero, Color.White, layerDepth, texture, 
                new Rectangle(0, 0, texture.Width, texture.Height), new Vector2(texture.Width / 2.0f, texture.Height / 2.0f))
        {

        }

        //backward compatability for mouse objects with no text
        public UIMouseObject(string id, ActorType actorType, StatusType statusType, Transform2D transform,
            Color color, SpriteEffects spriteEffects, float layerDepth, Texture2D texture, Rectangle sourceRectangle, Vector2 origin)
            : this(id, actorType, statusType, transform, color, spriteEffects,
                  null, null, Vector2.Zero, Color.White, layerDepth, texture, sourceRectangle, origin)
        {

        }

        public UIMouseObject(string id, ActorType actorType, StatusType statusType, Transform2D transform,
            Color color, SpriteEffects spriteEffects, SpriteFont spriteFont, 
            string text, Vector2 textOffsetPosition, Color textColor,
            float layerDepth, Texture2D texture, Rectangle sourceRectangle, Vector2 origin)
            : base(id, actorType, statusType, transform, color, spriteEffects, layerDepth, texture, sourceRectangle, origin)
        {
            this.spriteFont = spriteFont;
            this.Text = text;
            this.textOffsetPosition = textOffsetPosition;
            this.textColor = textColor;

            this.Transform2D.Translation = game.ScreenCentre;
        }

        public override void Draw(GameTime gameTime)
        {
            //draw icon
            game.SpriteBatch.Draw(this.Texture, this.Transform2D.Translation, //bug - 22/4/16
                this.SourceRectangle, this.Color, this.Transform2D.Rotation, this.Origin, 
                    this.Transform2D.Scale, this.SpriteEffects, this.LayerDepth);

            //draw any additional text
            if (this.text != null)
                game.SpriteBatch.DrawString(this.spriteFont, this.text,
                    ((this.Transform2D.Translation - this.textOrigin) - this.textOffsetPosition), this.textColor);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateMouseObject(gameTime);
            DoMousePick(gameTime);
            base.Update(gameTime);
        }

        private void UpdateMouseObject(GameTime gameTime)
        {
            //move the texture for the mouse object to be where the mouse pointer is
            this.Transform2D.Translation = game.MouseManager.Position;
        }

        private Actor actor;
        public virtual void DoMousePick(GameTime gameTime)
        {
            if (game.CameraManager.ActiveCamera != null)
            {
                //to do..
                //for loop through all the objects in the object manager
                    //is this CollidablePrimitiveObject?
                        //cast as CollidablePrimitiveObject

                //get mouse ray 
             Ray ray 
    = game.MouseManager.GetMouseRay(game.CameraManager.ActiveCamera);

                if(ray.Intersects(new BoundingBox()) != -1)
                {
                    //im mousing over a value
                }
            }
        }
    }
}
