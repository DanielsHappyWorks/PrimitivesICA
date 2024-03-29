﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public class PrimitiveObject : DrawnActor3D, ICloneable
    {
        #region Variables
        private IVertexData vertexData;
        #endregion 

        #region Properties
        public IVertexData VertexData
        {
            get
            {
                return this.vertexData;
            }
            set
            {
                this.vertexData = value;
            }
        }
        #endregion

        public PrimitiveObject(string id, ActorType actorType,
            Transform3D transform, Effect effect, IVertexData vertexData, Color color, 
            float alpha, StatusType statusType)
            : base(id, actorType, transform, effect, color, alpha, statusType)
        {
            this.vertexData = vertexData;
        }

        public new object Clone()
        {
            return new PrimitiveObject("clone - " + ID, //deep
               this.ActorType, //deep
               (Transform3D)this.Transform3D.Clone(), //deep
               this.Effect, //shallow - its ok if objects refer to the same effect
               this.vertexData, //shallow - its ok if objects refer to the same vertices
               this.Color, //deep
               this.Alpha, //deep
               this.StatusType); //deep
        }
    }
}
