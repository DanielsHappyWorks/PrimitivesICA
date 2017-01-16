/*
Function: 		Provide mouse input functions
Author: 		NMCG
Version:		1.0
Date Updated:	26/9/16
Bugs:			None
Fixes:			None
*/

using GDApp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDLibrary
{
    /// <summary>
    /// Provides methods to determine the state of the mouse.
    /// </summary>
    /// 

    public class MouseManager : GameComponent
    {
        #region Variables
        private Main game;
        private MouseState newState, oldState;
        #endregion

        #region Properties
        public Microsoft.Xna.Framework.Rectangle Bounds
        {
            get
            {
                return new Microsoft.Xna.Framework.Rectangle(this.newState.X, this.newState.Y, 1, 1);
            }
        }
        public Vector2 Position
        {
            get
            {
                return new Vector2(this.newState.X, this.newState.Y);
            }
        }
        #endregion

        public MouseManager(Main game, bool isVisible)
            : base(game)
        {

            this.game = game;
            game.IsMouseVisible = isVisible;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }
     
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //store the old state
            this.oldState = newState;

            //get the new state
            this.newState = Mouse.GetState();

            base.Update(gameTime);
        }

        public bool HasMoved()
        {
            float deltaPositionLength = new Vector2(newState.X - oldState.X, 
                newState.Y - oldState.Y).Length();

            return (deltaPositionLength > AppData.MouseSensitivity) ? true : false;
        }
        public bool IsLeftButtonClickedOnce()
        {
            return ((newState.LeftButton.Equals(ButtonState.Pressed)) && (!oldState.LeftButton.Equals(ButtonState.Pressed)));
        }
        public bool IsLeftButtonClicked()
        {
            return (newState.LeftButton.Equals(ButtonState.Pressed));
        }
        public bool IsRightButtonClickedOnce()
        {
            return ((newState.RightButton.Equals(ButtonState.Pressed)) && (!oldState.RightButton.Equals(ButtonState.Pressed)));
        }
        public bool isRightButtonClicked()
        {
            return (newState.RightButton.Equals(ButtonState.Pressed));
        }

        //Calculates the mouse pointer distance (in X and Y) from a user-defined position
        public Vector2 GetDeltaFromPosition(Vector2 position)
        {
            Vector2 delta;
            if (this.Position != position) //e.g. not the centre
            {
                if (game.CameraManager.ActiveCamera.View.Up.Y == -1)
                {
                    delta.X = 0;
                    delta.Y = 0;
                }
                else
                {
                    delta.X = this.Position.X - position.X;
                    delta.Y = this.Position.Y - position.Y;
                }
                SetPosition(position);
                return delta;
            }
            return Vector2.Zero;
        }

        //Calculates the mouse pointer distance from the screen centre
        public Vector2 GetDeltaFromCentre()
        {
            return new Vector2(this.newState.X - this.game.ScreenCentre.X,
                        this.newState.Y - this.game.ScreenCentre.Y);
        }

        //has the mouse state changed since the last update?
        public bool IsStateChanged()
        {
            return (this.newState.Equals(oldState)) ? false : true;
        }

        //did the mouse move above the limits of precision from centre position
        public bool IsStateChangedOutsidePrecision(float mousePrecision)
        {
            return ((Math.Abs(newState.X - oldState.X) > mousePrecision) || (Math.Abs(newState.Y - oldState.Y) > mousePrecision));
        }


        //how much has the scroll wheel been moved since the last update?
        public int GetDeltaFromScrollWheel()
        {
            if (IsStateChanged()) //if state changed then return difference
                return newState.ScrollWheelValue - oldState.ScrollWheelValue;

            return 0;
        }

        public void SetPosition(Vector2 position)
        {
            Mouse.SetPosition((int)position.X, (int)position.Y);
        }

        //tests if mouse on the screen vertical screen edge
        public bool IsMouseOnScreenEdgeVertical(float activationSensitivity, ref Vector2 mouseDelta)
        {
            //left
            if (this.newState.X <= (game.ScreenRectangle.Width * (1 - activationSensitivity)))
            {
                mouseDelta += Vector2.UnitY;
                return true;
            }
            //right
            else if (this.newState.X >= (game.ScreenRectangle.Width * activationSensitivity))
            {
                mouseDelta += -Vector2.UnitY;
                return true;
            }

            return false;
        }

        //tests if mouse on the screen horizontal screen edge
        public bool IsMouseOnScreenEdgeHorizontal(float activationSensitivity, ref Vector2 mouseDelta)
        {          
            //top
            if (this.newState.Y <= (game.ScreenRectangle.Height * (1 - activationSensitivity)))
            {
                mouseDelta += Vector2.UnitX;
                return true;
            }
            //bottom
            else if (this.newState.Y >= (game.ScreenRectangle.Height * activationSensitivity))
            {
                mouseDelta += -Vector2.UnitX;
                return true;
            }

            return false;
        }



        #region Ray Picking
        //get a ray positioned at the mouse's location on the screen - used for picking 
        public Microsoft.Xna.Framework.Ray GetMouseRay(Camera3D camera)
        {
            //get the positions of the mouse in screen space
            Vector3 near = new Vector3(this.newState.X, this.Position.Y, 0);

            //convert from screen space to world space
            near = camera.Viewport.Unproject(near, camera.ProjectionParameters.Projection, camera.View, Matrix.Identity);

            return GetMouseRayFromNearPosition(camera, near);
        }

        //get a ray from a user-defined near position in world space and the mouse pointer
        public Microsoft.Xna.Framework.Ray GetMouseRayFromNearPosition(Camera3D camera, Vector3 near)
        {
            //get the positions of the mouse in screen space
            Vector3 far = new Vector3(this.newState.X, this.Position.Y, 1);

            //convert from screen space to world space
            far = camera.Viewport.Unproject(far, camera.ProjectionParameters.Projection, camera.View, Matrix.Identity);

            //generate a ray to use for intersection tests
            return new Microsoft.Xna.Framework.Ray(near, Vector3.Normalize(far - near));
        }

        //get a ray positioned at the scnree position - used for picking when we have a centred reticule
        public Vector3 GetMouseRayDirection(Camera3D camera, Vector2 screenPosition)
        {
            //get the positions of the mouse in screen space
            Vector3 near = new Vector3(screenPosition.X, screenPosition.Y, 0);
            Vector3 far = new Vector3(this.Position, 1);

            //convert from screen space to world space
            near = camera.Viewport.Unproject(near, camera.ProjectionParameters.Projection, camera.View, Matrix.Identity);
            far = camera.Viewport.Unproject(far, camera.ProjectionParameters.Projection, camera.View, Matrix.Identity);

 
            //generate a ray to use for intersection tests
            return Vector3.Normalize(far - near);
        }

        public Vector3 GetMouseRayDirection(Camera3D camera)
        {
            return GetMouseRayDirection(camera, new Vector2(this.newState.X, this.newState.Y));
        }
        #endregion
    }
}