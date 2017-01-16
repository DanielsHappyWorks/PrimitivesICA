using GDApp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    //Draws the bounding volume for your primitive objects
    public class PrimitiveDebugDrawer : DrawableGameComponent
    {
        private Main game;
        private bool bPaused = false;
        private BasicEffect wireframeEffect;

        //temp vars
        private IVertexData vertexData = null;
        private SphereCollisionPrimitive coll;
        private Matrix world;

        public PrimitiveDebugDrawer(Main game)
            : base(game)
        {
            this.game = game;
        }

        public override void Initialize()
        {
            //used to draw bounding volumes
            this.wireframeEffect = new BasicEffect(this.game.GraphicsDevice);
            this.wireframeEffect.VertexColorEnabled = true;

            //register for the menu events
            this.game.EventDispatcher.MainMenuChanged += EventDispatcher_MainMenu;
            base.Initialize();
        }


        #region Event Handling
        //handle the relevant menu events
        public virtual void EventDispatcher_MainMenu(EventData eventData)
        {
            if ((eventData.EventType == EventType.OnPlay) || (eventData.EventType == EventType.OnRestart))
                this.bPaused = false;
            else if (eventData.EventType == EventType.OnPause)
                this.bPaused = true;
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            if (!bPaused)
            {
                //set so we dont see the bounding volume through the object is encloses - disable to see result
                this.game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                 
                for (int i = 0; i < this.game.ObjectManager.Size; i++)
                {
                   if (this.game.ObjectManager[i] is CollidablePrimitiveObject)
                       DrawBoundingPrimitive(gameTime, (this.game.ObjectManager[i] as CollidablePrimitiveObject).CollisionPrimitive, Color.White); //collidable object volumes are White
                    else if (this.game.ObjectManager[i] is SimpleZoneObject)
                        DrawBoundingPrimitive(gameTime, (this.game.ObjectManager[i] as SimpleZoneObject).CollisionPrimitive, Color.Red);        //collidable zone volumes are red
                }
                base.Draw(gameTime);
            }
        }

        private void DrawBoundingPrimitive(GameTime gameTime, ICollisionPrimitive collisionPrimitive, Color color)
        {
            if (collisionPrimitive is SphereCollisionPrimitive)
            {
               // vertexData = PrimitiveFactory.GetVertexDataInstance(this.GraphicsDevice, PrimitiveFactory.GeometryType.WireframeSphere, PrimitiveFactory.StorageType.Buffered);
                coll = collisionPrimitive as SphereCollisionPrimitive;
                world = Matrix.Identity * Matrix.CreateScale(coll.BoundingSphere.Radius) * Matrix.CreateTranslation(coll.BoundingSphere.Center);
                this.wireframeEffect.World = world;
                this.wireframeEffect.View = this.game.CameraManager.ActiveCamera.View;
                this.wireframeEffect.Projection = this.game.CameraManager.ActiveCamera.ProjectionParameters.Projection;
                this.wireframeEffect.DiffuseColor = color.ToVector3();
                this.wireframeEffect.CurrentTechnique.Passes[0].Apply();
                vertexData.Draw(gameTime, this.wireframeEffect);
            }
            else 
            {
                BoxCollisionPrimitive coll = collisionPrimitive as BoxCollisionPrimitive;
                BoundingBoxBuffers buffers = BoundingBoxDrawer.CreateBoundingBoxBuffers(coll.BoundingBox, this.GraphicsDevice);
                BoundingBoxDrawer.DrawBoundingBox(buffers, this.wireframeEffect, this.GraphicsDevice, this.game.CameraManager.ActiveCamera);
            }
        }

        
    }
}
