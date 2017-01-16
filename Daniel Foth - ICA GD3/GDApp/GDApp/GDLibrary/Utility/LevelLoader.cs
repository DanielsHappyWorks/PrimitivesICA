using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using GDApp;

namespace GDLibrary
{
    public class LevelLoader
    {
        private static readonly Color ColorLevelLoaderIgnore = Color.White;

        private Main game;
        public LevelLoader(Main game)
        {
            this.game = game;
        }

        public List<Actor> Load(Texture2D texture, int tileWidth, int tileHeight)
        {
            List<Actor> list = new List<Actor>();
            Color[] colorData = new Color[texture.Height * texture.Width];
            texture.GetData<Color>(colorData);

            Color color;
            Vector2 position;
            List<Actor> actorlist;

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    color = colorData[x + y * texture.Width];

                    if (!color.Equals(ColorLevelLoaderIgnore))
                    {
                        position = new Vector2(x * tileWidth, y * tileHeight);
                        actorlist = getObjectFromColor(color, position, tileWidth, tileHeight);

                        if (actorlist != null)
                            list.AddRange(actorlist);
                    }
                } //end for x
            } //end for y
            return list;
        }

        private Random rand = new Random();
        private List<Actor> getObjectFromColor(Color color, Vector2 position, int tileWidth, int tileHeight)
        {
            IVertexData vertexData = null;
            PrimitiveType primitiveType;
            int primitiveCount;
            Transform3D transform3D = null;

            List<Actor> actorList = new List<Actor>();

            if (color.Equals(new Color(0, 255, 0)))   //start
            {
                vertexData = new VertexData<VertexPositionNormalTexture>(
                    VertexFactory.GetVerticesPositionNormalTexturedCube(1,
                    out primitiveType, out primitiveCount),
                    primitiveType, primitiveCount);

                transform3D = new Transform3D(new Vector3(position.X, 0, position.Y), new Vector3(10, 10, 10));

                BoxCollisionPrimitive collisionPrimitive
               = new BoxCollisionPrimitive(new Transform3D(new Vector3(position.X, 0, position.Y),
               Vector3.Zero, new Vector3(10, 10, 10), Vector3.UnitX, Vector3.UnitY));

                CollidablePrimitiveObject collidablePrimitiveObject
                     = new CollidablePrimitiveObject("floor",
                    ActorType.Decorator, transform3D,
                    this.game.LitPrimitiveEffect,
                    vertexData,
                    this.game.TextureDictionary["floor"],
                    Color.White, 1, StatusType.Drawn | StatusType.Updated, collisionPrimitive);

                actorList.Add(collidablePrimitiveObject);
            }
            else if (color.Equals(new Color(255, 0, 0)))   //finish
            {
                vertexData = new VertexData<VertexPositionNormalTexture>(
                    VertexFactory.GetVerticesPositionNormalTexturedCube(1,
                    out primitiveType, out primitiveCount),
                    primitiveType, primitiveCount);

                transform3D = new Transform3D(new Vector3(position.X, 0, position.Y), new Vector3(10, 10, 10));

                BoxCollisionPrimitive collisionPrimitive
               = new BoxCollisionPrimitive(new Transform3D(new Vector3(position.X, 0, position.Y),
               Vector3.Zero, new Vector3(10, 10, 10), Vector3.UnitX, Vector3.UnitY));

                CollidablePrimitiveObject collidablePrimitiveObject
                     = new CollidablePrimitiveObject("floor",
                    ActorType.Decorator, transform3D,
                    this.game.LitPrimitiveEffect,
                    vertexData,
                    this.game.TextureDictionary["floor"],
                    Color.White, 1, StatusType.Drawn | StatusType.Updated, collisionPrimitive);

                actorList.Add(collidablePrimitiveObject);

                //win Object
                vertexData = new VertexData<VertexPositionColor>(
                    VertexFactory.GetColoredTriangle(),
                    PrimitiveType.TriangleList, 1);

                transform3D = new Transform3D(new Vector3(position.X, 15, position.Y), new Vector3(0, 90, 90), new Vector3(4, 4, 4), Vector3.UnitX, Vector3.UnitY);

                collisionPrimitive = new BoxCollisionPrimitive(new Transform3D(new Vector3(position.X, 15, position.Y),
                        Vector3.Zero, new Vector3(5, 5, 5), Vector3.UnitX, Vector3.UnitY));

                PickupCollidablePrimitiveObject collidablePrimitiveObjectPickup
                     = new PickupCollidablePrimitiveObject("winobject",
                    ActorType.Pickup, transform3D,
                    this.game.WireframePrimitiveEffect,
                    vertexData,
                    this.game.TextureDictionary["abs"],
                    Color.LimeGreen, 1, StatusType.Drawn | StatusType.Updated, collisionPrimitive, 10);
                //collidablePrimitiveObjectPickup.AttachController(new RotationController("rotate", ControllerType.Rotation, new Vector3(0, 0, 0.25f)));
                actorList.Add(collidablePrimitiveObjectPickup);
            }
            else if (color.Equals(new Color(255, 255, 0)))   //floor
            {
                vertexData = new VertexData<VertexPositionNormalTexture>(
                    VertexFactory.GetVerticesPositionNormalTexturedCube(1,
                    out primitiveType, out primitiveCount),
                    primitiveType, primitiveCount);

                transform3D = new Transform3D(new Vector3(position.X, 0, position.Y), new Vector3(10, 10, 10));

                BoxCollisionPrimitive collisionPrimitive
               = new BoxCollisionPrimitive(new Transform3D(new Vector3(position.X, 0, position.Y),
               Vector3.Zero, new Vector3(10, 10, 10), Vector3.UnitX, Vector3.UnitY));

                CollidablePrimitiveObject collidablePrimitiveObject
                     = new CollidablePrimitiveObject("floor",
                    ActorType.Decorator, transform3D,
                    this.game.LitPrimitiveEffect,
                    vertexData,
                    this.game.TextureDictionary["floor"],
                    Color.White, 1, StatusType.Drawn | StatusType.Updated, collisionPrimitive);

                actorList.Add(collidablePrimitiveObject);
            }
            else if (color.Equals(new Color(100, 100, 100)))   //walls
            {
                vertexData = new VertexData<VertexPositionNormalTexture>(
                    VertexFactory.GetVerticesPositionNormalTexturedCube(1,
                    out primitiveType, out primitiveCount),
                    primitiveType, primitiveCount);

                transform3D = new Transform3D(new Vector3(position.X, 10, position.Y), new Vector3(10, 15, 10));

                BoxCollisionPrimitive collisionPrimitive
               = new BoxCollisionPrimitive(new Transform3D(new Vector3(position.X, 10, position.Y),
               Vector3.Zero, new Vector3(10, 15, 10), Vector3.UnitX, Vector3.UnitY));

                CollidablePrimitiveObject collidablePrimitiveObject
                     = new CollidablePrimitiveObject("wall",
                    ActorType.Decorator, transform3D,
                    this.game.LitPrimitiveEffect,
                    vertexData,
                    this.game.TextureDictionary["wall"],
                    Color.Green, 1, StatusType.Drawn | StatusType.Updated, collisionPrimitive);

                    actorList.Add(collidablePrimitiveObject);
            }
            else if (color.Equals(new Color(0, 255, 255)))   //pickup positive
            {
                vertexData = new VertexData<VertexPositionNormalTexture>(
                    VertexFactory.GetVerticesPositionNormalTexturedCube(1,
                    out primitiveType, out primitiveCount),
                    primitiveType, primitiveCount);

                transform3D = new Transform3D(new Vector3(position.X, 0, position.Y), new Vector3(10, 10, 10));

                BoxCollisionPrimitive collisionPrimitive
                = new BoxCollisionPrimitive(new Transform3D(new Vector3(position.X, 0, position.Y),
                Vector3.Zero, new Vector3(10, 10, 10), Vector3.UnitX, Vector3.UnitY));

                CollidablePrimitiveObject collidablePrimitiveObject
                     = new CollidablePrimitiveObject("floor",
                    ActorType.Decorator, transform3D,
                    this.game.LitPrimitiveEffect,
                    vertexData,
                    this.game.TextureDictionary["floor"],
                    Color.LightGreen, 1, StatusType.Drawn | StatusType.Updated, collisionPrimitive);

                actorList.Add(collidablePrimitiveObject);


                //pickup
                vertexData = new VertexData<VertexPositionColorTexture>(
                    VertexFactory.GetVerticesPositionColorTexturedDiamond(1,
                    out primitiveType, out primitiveCount),
                    primitiveType, primitiveCount);

                transform3D = new Transform3D(new Vector3(position.X, 15, position.Y), new Vector3(90, 0, 0), new Vector3(4, 4, 4), Vector3.UnitX, Vector3.UnitY);

                collisionPrimitive = new BoxCollisionPrimitive(new Transform3D(new Vector3(position.X, 15, position.Y),
                        Vector3.Zero, new Vector3(5, 5, 5), Vector3.UnitX, Vector3.UnitY));

                PickupCollidablePrimitiveObject collidablePrimitiveObjectPickup
                     = new PickupCollidablePrimitiveObject("goodpickup",
                    ActorType.Pickup, transform3D,
                    this.game.TexturedPrimitiveEffect,
                    vertexData,
                    this.game.TextureDictionary["abs"],
                    Color.White, 1, StatusType.Drawn | StatusType.Updated, collisionPrimitive, -10);
                collidablePrimitiveObjectPickup.AttachController(new RotationController("rotate", ControllerType.Rotation, new Vector3(0, 0, 0.25f)));
                actorList.Add(collidablePrimitiveObjectPickup);
            }
            else if (color.Equals(new Color(255, 20, 255)))   //pickup negative
            {
                vertexData = new VertexData<VertexPositionNormalTexture>(
                    VertexFactory.GetVerticesPositionNormalTexturedCube(1,
                    out primitiveType, out primitiveCount),
                    primitiveType, primitiveCount);

                transform3D = new Transform3D(new Vector3(position.X, 0, position.Y), new Vector3(10, 10, 10));

                BoxCollisionPrimitive collisionPrimitive
               = new BoxCollisionPrimitive(new Transform3D(new Vector3(position.X, 0, position.Y),
               Vector3.Zero, new Vector3(10, 10, 10), Vector3.UnitX, Vector3.UnitY));

                CollidablePrimitiveObject collidablePrimitiveObject
                     = new CollidablePrimitiveObject("floor",
                    ActorType.Pickup, transform3D,
                    this.game.LitPrimitiveEffect,
                    vertexData,
                    this.game.TextureDictionary["floor"],
                    Color.LightPink, 1, StatusType.Drawn | StatusType.Updated, collisionPrimitive);

                actorList.Add(collidablePrimitiveObject);

                //pickup
                vertexData = new VertexData<VertexPositionColorTexture>(
                    VertexFactory.GetVerticesPositionColorTexturedDiamond(1,
                    out primitiveType, out primitiveCount),
                    primitiveType, primitiveCount);

                transform3D = new Transform3D(new Vector3(position.X, 15, position.Y), new Vector3(90, 0, 0), new Vector3(4, 4, 4), Vector3.UnitX, Vector3.UnitY);

                collisionPrimitive = new BoxCollisionPrimitive(new Transform3D(new Vector3(position.X, 15, position.Y),
                        Vector3.Zero, new Vector3(5, 5, 5), Vector3.UnitX, Vector3.UnitY));

                PickupCollidablePrimitiveObject collidablePrimitiveObjectPickup
                     = new PickupCollidablePrimitiveObject("badpickup",
                    ActorType.Pickup, transform3D,
                    this.game.TexturedPrimitiveEffect,
                    vertexData,
                    this.game.TextureDictionary["abs"],
                    Color.LightBlue, 1, StatusType.Drawn | StatusType.Updated, collisionPrimitive, -10);
                collidablePrimitiveObjectPickup.AttachController(new RotationController("rotate", ControllerType.Rotation, new Vector3(0,0,-0.5f)));
                actorList.Add(collidablePrimitiveObjectPickup);
            }

            return actorList;
        }
    }
}
