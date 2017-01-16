using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class CameraFollowController : TargetController
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        public CameraFollowController(string id, ControllerType controllerType, Actor targetActor)
            : base(id, controllerType, targetActor)
        {

        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            UpdateParent(gameTime, actor as Actor3D, this.TargetActor as Actor3D);
        }

        private void UpdateParent(GameTime gameTime, Actor3D parentActor, Actor3D targetActor)
        {
            parentActor.Transform3D.Translation = new Vector3((targetActor.Transform3D.Translation.X), parentActor.Transform3D.Translation.Y,targetActor.Transform3D.Translation.Z);
        }
        }
}
