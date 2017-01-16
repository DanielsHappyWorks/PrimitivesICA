using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GDApp;

namespace GDLibrary
{
    public class PlayerCollidablePrimitiveObject : CollidablePrimitiveObject
    {
        #region Fields
        private float moveSpeed, rotationSpeed;
        private Keys[] moveKeys;
        #endregion

        #region Properties
        #endregion


        public PlayerCollidablePrimitiveObject(string id, ActorType actorType,
            Transform3D transform, Effect effect, IVertexData vertexData,
            Texture2D texture, Color color, float alpha, StatusType statusType,
             ICollisionPrimitive collisionPrimitive, 
            Keys[] moveKeys, float moveSpeed, float rotationSpeed)
            : base(id, actorType, transform, effect, vertexData,
            texture, color, alpha, statusType, collisionPrimitive)
        {
            this.moveKeys = moveKeys;
            this.moveSpeed = moveSpeed;
            this.rotationSpeed = rotationSpeed;
        }

        public override void Update(GameTime gameTime)
        {
            //read any input and store suggested increments
            HandleInput(gameTime);

            //have we collided with something?
            this.Collidee = CheckCollisions(gameTime);

            //how do we respond to this collidee e.g. pickup?
            HandleCollisionResponse(this.Collidee);

            //if no collision then move
            if (this.Collidee == null)
                ApplyInput(gameTime);

            //reset translate and rotate and update primitive
            base.Update(gameTime);
        }


        protected override void HandleCollisionResponse(Actor collidee)
        {
            if(collidee is SimpleZoneObject)
            {
               //do something...
            }
            else if(collidee is CollidablePrimitiveObject)
            {
                if(collidee.ActorType == GDLibrary.ActorType.Pickup)
                {
                    //do a cast to the specific type e.g. CollidablePrimitiveObject
                    PickupCollidablePrimitiveObject pickupObject 
                        = collidee as PickupCollidablePrimitiveObject;

                    if(pickupObject.GetID() == "goodpickup")
                    {
                        game.progressController.incrementProgressBar(10);
                        game.Score += 100;
                        game.TextScoreObject.Text = "Score: " + game.Score;
                        game.SoundManager.PlayCue("goodpickup");
                    }
                    else if (pickupObject.GetID() == "badpickup")
                    {
                        game.progressController.incrementProgressBar(-10);
                        game.Score -= 50;
                        game.TextScoreObject.Text = "Score: " + game.Score;
                        game.SoundManager.PlayCue("badpickup");
                    }
                    else if (pickupObject.GetID() == "winobject")
                    {
                        //win event
                        EventDispatcher.Publish(new EventData("winobj", this, EventActionType.OnWin, EventCategoryType.MainMenu));
                        game.Score += 200;
                        game.TextScoreObject.Text = "Score: " + game.Score;
                        game.TextLevelObject.Text = "LeveL: " + (game.level + 2);
                        game.SoundManager.PlayCue("win");
                    }

                    //remove?
                    game.ObjectManager.Remove(pickupObject);

                }
            }

         //   base.HandleCollisionResponse(collidee);
        }

        protected override void HandleInput(GameTime gameTime)
        {
            if (game.KeyboardManager.IsKeyDown(this.moveKeys[AppData.IndexRotateLeft])) //Forward
            {
                if (game.CameraManager.ActiveCameraLayout == "rail")
                {
                    game.CameraManager.SetActiveCameraLayout("1x1");
                }

                this.Transform3D.TranslateIncrement
                    += -this.Transform3D.Look * gameTime.ElapsedGameTime.Milliseconds
                            * this.moveSpeed;
            }
            else if (game.KeyboardManager.IsKeyDown(this.moveKeys[AppData.IndexRotateRight])) //Backward
            {
                if (game.CameraManager.ActiveCameraLayout == "rail")
                {
                    game.CameraManager.SetActiveCameraLayout("1x1");
                }

                this.Transform3D.TranslateIncrement
                   += this.Transform3D.Look * gameTime.ElapsedGameTime.Milliseconds
                           * this.moveSpeed;
            }

            if (game.KeyboardManager.IsKeyDown(this.moveKeys[AppData.IndexMoveForward])) //Left
            {
                if (game.CameraManager.ActiveCameraLayout == "rail")
                {
                    game.CameraManager.SetActiveCameraLayout("1x1");
                }

                this.Transform3D.TranslateIncrement
                   += -this.Transform3D.Right * gameTime.ElapsedGameTime.Milliseconds
                           * this.moveSpeed;
            }
            else if (game.KeyboardManager.IsKeyDown(this.moveKeys[AppData.IndexMoveBackward])) //Right
            {
                if (game.CameraManager.ActiveCameraLayout == "rail")
                {
                    game.CameraManager.SetActiveCameraLayout("1x1");
                }

                this.Transform3D.TranslateIncrement
                   += this.Transform3D.Right * gameTime.ElapsedGameTime.Milliseconds
                           * this.moveSpeed;
            }

       //     base.HandleInput(gameTime);
        }
    }
}
