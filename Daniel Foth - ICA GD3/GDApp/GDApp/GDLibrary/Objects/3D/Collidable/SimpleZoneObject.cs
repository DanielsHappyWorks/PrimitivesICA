using Microsoft.Xna.Framework;

namespace GDLibrary
{
    /// <summary>
    /// Represents an area that can detect collisions similat to ZoneObject but using only a simple
    /// BoundingSphere or BoundingBox. It does NOT have an associated model. We can use this class 
    /// to create activation zones e.g. for camera switching or event generation
    /// </summary>
    public class SimpleZoneObject : Actor3D
    {

        #region Variables
        private ICollisionPrimitive collisionPrimitive;
        #endregion

        #region Properties
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

        public SimpleZoneObject(string id, ActorType actorType,
                            Transform3D transform, StatusType statusType, ICollisionPrimitive collisionPrimitive)
            : base(id, actorType, transform, statusType)
        {
            this.collisionPrimitive = collisionPrimitive;
        }

        public override void Update(GameTime gameTime)
        {
            //update collision primitive with new object position
            if (collisionPrimitive != null)
                collisionPrimitive.Update(gameTime, this.Transform3D);
        }
    }
}
