using GDApp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public class CollidablePrimitiveObject : TexturedPrimitiveObject
    {
        #region Variables
        //the skin used to wrap the object
        private ICollisionPrimitive collisionPrimitive;

        //the object that im colliding with
        private Actor collidee;

        #endregion

        #region Properties
        public Actor Collidee
        {
            get
            {
                return collidee;
            }
            set
            {
                collidee = value;
            }
        }
        public ICollisionPrimitive CollisionPrimitive
        {
            get
            {
                return collisionPrimitive;
            }
            set
            {
                collisionPrimitive = value;
            }
        }
        #endregion

        //used to draw collidable primitives that have a texture i.e. use VertexPositionColor vertex types only
        public CollidablePrimitiveObject(string id, ActorType actorType,
            Transform3D transform, Effect effect, IVertexData vertexData,
            Texture2D texture, Color color, float alpha, StatusType statusType,
             ICollisionPrimitive collisionPrimitive)
            : base(id, actorType, transform, effect, vertexData, texture, color, alpha, statusType)
        {
            this.collisionPrimitive = collisionPrimitive;
        }


        public override void Update(GameTime gameTime)
        {
            //reset collidee to prevent colliding with the same object in the next update
            collidee = null;

            //reset any movements applied in the previous update from move keys
            this.Transform3D.TranslateIncrement = Vector3.Zero;
            this.Transform3D.RotateIncrement = 0;

            //update collision primitive with new object position
            if (collisionPrimitive != null)
                collisionPrimitive.Update(gameTime, this.Transform3D);

            base.Update(gameTime);
        }

        //read and store movement suggested by keyboard input
        protected virtual void HandleInput(GameTime gameTime)
        {

        }

        //define what happens when a collision occurs
        protected virtual void HandleCollisionResponse(Actor collidee)
        {

        }

        private Actor actor;
        protected virtual Actor CheckCollisions(GameTime gameTime)
        {
            for (int i = 0; i < game.ObjectManager.Size; i++)
            {
                
                actor = game.ObjectManager[i];
                if (this != actor)
               {
                   if (actor is CollidablePrimitiveObject)
                    {
                        CollidablePrimitiveObject collidableObject = actor as CollidablePrimitiveObject;
                        if (this.CollisionPrimitive.Intersects(collidableObject.CollisionPrimitive, this.Transform3D.TranslateIncrement))
                            return collidableObject;
                    }
                    else if (actor is SimpleZoneObject)
                    {
                        SimpleZoneObject zoneObject = actor as SimpleZoneObject;
                        if (this.CollisionPrimitive.Intersects(zoneObject.CollisionPrimitive, this.Transform3D.TranslateIncrement))
                            return zoneObject;
                    }
                }
            }
            return null;
        }

        //apply suggested movement since no collision will occur if the player moves to that position
        protected virtual void ApplyInput(GameTime gameTime)
        {
            //was a move/rotate key pressed, if so then these values will be > 0 in dimension
            if (this.Transform3D.TranslateIncrement != Vector3.Zero)
                this.Transform3D.TranslateBy(this.Transform3D.TranslateIncrement);

            if (this.Transform3D.RotateIncrement != 0)
                this.Transform3D.RotateAroundYBy(this.Transform3D.RotateIncrement);
        }

        public new object Clone()
        {
            if(this.CollisionPrimitive is BoxCollisionPrimitive)
            return new CollidablePrimitiveObject(this.ID, this.ActorType,
                (Transform3D)this.Transform3D.Clone(), 
                this.Effect, this.VertexData,
                this.Texture, this.Color, this.Alpha, 
                this.StatusType,
                (BoxCollisionPrimitive)this.CollisionPrimitive.Clone());
            else
                return new CollidablePrimitiveObject(this.ID, this.ActorType,
                    (Transform3D)this.Transform3D.Clone(),
                    this.Effect, this.VertexData,
                    this.Texture, this.Color, this.Alpha,
                    this.StatusType,
                    (SphereCollisionPrimitive)this.CollisionPrimitive.Clone());
        }
    }
}
