using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class PickupCollidablePrimitiveObject : CollidablePrimitiveObject
    {
        #region Variables
        private int value;
        #endregion

        #region Properties
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
        #endregion

        //used to draw collidable primitives that a value associated with them e.g. health
        public PickupCollidablePrimitiveObject(string id, ActorType actorType,
            Transform3D transform, Effect effect, IVertexData vertexData,
            Texture2D texture, Color color, float alpha, StatusType statusType, ICollisionPrimitive collisionPrimitive, int value)
            : base(id, actorType, transform, effect, vertexData, texture, color, alpha, statusType, collisionPrimitive)
        {
            this.value = value;
        }

        public new object Clone()
        {
            return new PickupCollidablePrimitiveObject(this.ID, this.ActorType,
            (Transform3D)this.Transform3D.Clone(), this.Effect,
            this.VertexData, this.Texture,
            this.Color, this.Alpha, this.StatusType,
            this.CollisionPrimitive,
            this.Value);
        }
    }
}
