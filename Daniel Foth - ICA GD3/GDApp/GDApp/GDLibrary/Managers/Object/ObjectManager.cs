/*
Function: 		Supports rendering primitives only. A single draw list.
Author: 		NMCG
Version:		1.0
Date Updated:	15/12/16
Bugs:			None
Fixes:			None
*/


using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GDApp;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public class ObjectManager : GenericDrawableManager
    {
        #region Variables
        private bool bDebugMode, bShowZones;
        private RasterizerState rasterizerStateOpaque, rasterizerStateTransparent;
        #endregion

        #region Properties
        public bool ShowZones
        {
            get
            {
                return this.bShowZones;
            }
            set
            {
                this.bShowZones = value;
            }
        }
        public bool DebugMode
        {
            get
            {
                return this.bDebugMode;
            }
            set
            {
                this.bDebugMode = value;
            }
        }
        #endregion

        //backward compatability with versions pre 3.9
        public ObjectManager(Main game, string name, bool bDebugMode)
            : this(game, name, 10, bDebugMode, true)
        {
        }

        public ObjectManager(Main game, string name, bool bDebugMode, bool bShowZones)
            : this(game, name, 10, bDebugMode, bShowZones)
        {
        }
        public ObjectManager(Main game, string name, int initialSize, bool bDebugMode, bool bShowZones)
            : base(game, name, initialSize)
        {
            this.bDebugMode = bDebugMode;
            this.bShowZones = bShowZones;
            InitializeGraphicsStateObjects();
        }

        private void InitializeGraphicsStateObjects()
        {
            //opaque objects
            this.rasterizerStateOpaque = new RasterizerState();
            this.rasterizerStateOpaque.CullMode = CullMode.CullCounterClockwiseFace;

            //transparent objects
            this.rasterizerStateTransparent = new RasterizerState();
            this.rasterizerStateTransparent.CullMode = CullMode.None;
        }
        private void SetGraphicsStateObjects(bool isOpaque)
        {
            //Remember this code from our initial aliasing problems with the Sky box?
            //enable anti-aliasing along the edges of the quad i.e. to remove jagged edges to the primitive
            this.Game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            if (isOpaque)
            {
                //set the appropriate state for opaque objects
                this.Game.GraphicsDevice.RasterizerState = this.rasterizerStateOpaque;

                //disable to see what happens when we disable depth buffering - look at the boxes
                this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            else
            {
                //set the appropriate state for transparent objects
                this.Game.GraphicsDevice.RasterizerState = this.rasterizerStateTransparent;

                //enable alpha blending for transparent objects i.e. trees
                this.Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

                //disable to see what happens when we disable depth buffering - look at the boxes
                this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            }


        }

        public override void Draw(GameTime gameTime)
        {
            //if you want to see game around menu edges then disable this if()
            if (!this.PauseDraw)
            {
                SetGraphicsStateObjects(true);
                foreach (IActor actor in this.OpaqueDrawList)
                {
                    DrawObjectByType(gameTime, actor as Actor3D);
                }

                SetGraphicsStateObjects(false);
                foreach (IActor actor in this.TransparentDrawList)
                {
                    DrawObjectByType(gameTime, actor as Actor3D);
                }
            }
        }

        private void DrawObjectByType(GameTime gameTime, Actor3D actor)
        {
            //was the drawn enum value set?
            if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
            {
                if (actor is BillboardPrimitiveObject)
                {
                    DrawObject(gameTime, actor as BillboardPrimitiveObject);
                }
                else if (actor is TexturedPrimitiveObject)
                {
                    DrawObject(gameTime, actor as TexturedPrimitiveObject);
                }
                else if (actor is PrimitiveObject)
                {
                    DrawObject(gameTime, actor as PrimitiveObject);
                }

                DrawCollisionSkin(actor);
            }
        }

        //draw a NON-TEXTURED primitive i.e. vertices (and possibly indices) defined by the user
        private void DrawObject(GameTime gameTime, PrimitiveObject primitiveObject)
        {
            BasicEffect effect = primitiveObject.Effect as BasicEffect;

            //W, V, P, Apply, Draw
            effect.World = primitiveObject.GetWorldMatrix();
            effect.View = (this.Game as Main).CameraManager.ActiveCamera.View;
            effect.Projection = (this.Game as Main).CameraManager.ActiveCamera.ProjectionParameters.Projection;

            effect.DiffuseColor = primitiveObject.Color.ToVector3();
            effect.Alpha = primitiveObject.Alpha;

            effect.CurrentTechnique.Passes[0].Apply();
            primitiveObject.VertexData.Draw(gameTime, effect);
        }

        //draw a NON-TEXTURED primitive i.e. vertices (and possibly indices) defined by the user
        private void DrawObject(GameTime gameTime, TexturedPrimitiveObject texturedPrimitiveObject)
        {
            BasicEffect effect = texturedPrimitiveObject.Effect as BasicEffect;

            //W, V, P, Apply, Draw
            effect.World = texturedPrimitiveObject.GetWorldMatrix();
            effect.View = (this.Game as Main).CameraManager.ActiveCamera.View;
            effect.Projection = (this.Game as Main).CameraManager.ActiveCamera.ProjectionParameters.Projection;

            if (texturedPrimitiveObject.Texture != null) //e.g. VertexPositionColor vertices will have no UV coordinates - so no texture
                effect.Texture = texturedPrimitiveObject.Texture;

            effect.DiffuseColor = texturedPrimitiveObject.Color.ToVector3();
            effect.Alpha = texturedPrimitiveObject.Alpha;

            effect.CurrentTechnique.Passes[0].Apply();
            texturedPrimitiveObject.VertexData.Draw(gameTime, effect);
        }

       
        BillboardParameters billboardParameters;
        private void DrawObject(GameTime gameTime, BillboardPrimitiveObject billboardPrimitiveObject)
        {
            Effect effect = billboardPrimitiveObject.Effect as Effect;
            billboardParameters = billboardPrimitiveObject.BillboardParameters;

            //W, V, P, Apply, Draw
            effect.CurrentTechnique = effect.Techniques[billboardParameters.Technique];
            effect.Parameters["World"].SetValue(billboardPrimitiveObject.Transform3D.World);
            effect.Parameters["View"].SetValue((this.Game as Main).CameraManager.ActiveCamera.View);
            effect.Parameters["Projection"].SetValue((this.Game as Main).CameraManager.ActiveCamera.ProjectionParameters.Projection);
            effect.Parameters["Up"].SetValue(billboardPrimitiveObject.Transform3D.Up);
            effect.Parameters["DiffuseTexture"].SetValue(billboardPrimitiveObject.Texture);
            effect.Parameters["Alpha"].SetValue(billboardPrimitiveObject.Alpha);

            if (billboardParameters.BillboardType == BillboardType.Normal)
            {
                effect.Parameters["Right"].SetValue(billboardPrimitiveObject.Transform3D.Right);
            }

            if (billboardParameters.IsScrolling)
            {
                effect.Parameters["IsScrolling"].SetValue(billboardParameters.IsScrolling);
                effect.Parameters["scrollRate"].SetValue(billboardParameters.scrollValue);
            }
            else
            {
                effect.Parameters["IsScrolling"].SetValue(false);
            }

            if (billboardParameters.IsAnimated)
            {
                effect.Parameters["IsAnimated"].SetValue(billboardParameters.IsAnimated);
                effect.Parameters["InverseFrameCount"].SetValue(billboardParameters.inverseFrameCount);
                effect.Parameters["CurrentFrame"].SetValue(billboardParameters.currentFrame);
            }
            else
            {
                effect.Parameters["IsAnimated"].SetValue(false);
            }


            effect.CurrentTechnique.Passes[0].Apply();
            billboardPrimitiveObject.VertexData.Draw(gameTime, effect);
        }


        //debug method to draw collision skins for collidable objects and zone objects
        private void DrawCollisionSkin(IActor actor)
        {
           
        }
    }
}
