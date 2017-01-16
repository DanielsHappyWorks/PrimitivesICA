using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace GDApp
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private BasicEffect wireframePrimitiveEffect, texturedPrimitiveEffect;
        private Effect billboardPrimitiveEffect;

        private ObjectManager objectManager;
        private MouseManager mouseManager;
        private KeyboardManager keyboardManager;
        private CameraManager cameraManager;
        private UIManager uiManager;
        private MenuManager menuManager;
        private SoundManager soundManager;

        private GenericDictionary<string, Texture2D> textureDictionary;
        private GenericDictionary<string, IVertexData> vertexDictionary;
        private GenericDictionary<string, DrawnActor3D> objectDictionary;
        private GenericDictionary<string, SpriteFont> fontDictionary;
        private GenericDictionary<string, Transform3DCurve> curveDictionary;
        private GenericDictionary<string, RailParameters> railDictionary;
        private GenericDictionary<string, Video> videoDictionary;

        private EventDispatcher eventDispatcher;
        private Vector2 screenCentre;
        private Rectangle screenRectangle;
        //debug only
     //   private PrimitiveDebugDrawer primitiveDebugDrawer;

        //temp vars
        private PrimitiveObject triangleObject;
        private AudioEmitter audioEmitter;
        private BasicEffect litPrimitiveEffect;
        private PlayerCollidablePrimitiveObject player;
        public UIProgressController progressController;
        public int level = 0;
        private int score = 0;
        private UITextObject textObject;
        private TexturedPrimitiveObject cloneTexturedPrimitiveObject;

        #endregion

        #region Properties
        public BasicEffect LitPrimitiveEffect
        {
            get
            {
                return this.litPrimitiveEffect;
            }
        }

        public BasicEffect TexturedPrimitiveEffect
        {
            get
            {
                return this.texturedPrimitiveEffect;
            }
        }

        public UITextObject TextScoreObject
        {
            get
            {
                return this.textObject;
            } 
            set
            {
                this.textObject = value;
            }
        }

        public int Score
        {
            get
            {
                return this.score;
            }
            set
            {
                this.score = value;
            }
        }

        public BasicEffect WireframePrimitiveEffect
        {
            get
            {
                return this.wireframePrimitiveEffect;
            }
        }

        public GenericDictionary<string, Texture2D> TextureDictionary
        {
            get
            {
                return this.textureDictionary;
            }
        }
        public EventDispatcher EventDispatcher
        {
            get
            {
                return this.eventDispatcher;
            }
        }
        public SpriteBatch SpriteBatch
        {
            get
            {
                return this.spriteBatch;
            }
        }
        public GraphicsDeviceManager Graphics
        {
            get
            {
                return this.graphics;
            }
        }
        public Vector2 ScreenCentre
        {
            get
            {
                return this.screenCentre;
            }
        }

        public Microsoft.Xna.Framework.Rectangle ScreenRectangle
        {
            get
            {
                if(this.screenRectangle == Microsoft.Xna.Framework.Rectangle.Empty)
                    this.screenRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, 
                        (int)graphics.PreferredBackBufferWidth,
                        (int)graphics.PreferredBackBufferHeight);

                return this.screenRectangle;
            }
        }

        public SoundManager SoundManager
        {
            get
            {
                return this.soundManager;
            }
        }
        public MouseManager MouseManager
        {
            get
            {
                return this.mouseManager; 
            }
        }
        public KeyboardManager KeyboardManager
        {
            get
            {
                return this.keyboardManager; 
            }
        }
        public CameraManager CameraManager
        {
            get
            {
                return this.cameraManager;
            }
        }
        public ObjectManager ObjectManager
        {
            get
            {
                return this.objectManager;
            }
        }

        public UITextObject TextLevelObject { get; private set; }
        #endregion

        #region Initialization
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }  
        
        protected override void Initialize()
        {
            int width = 1024, height = 768;

            InitializeEventDispatcher();
            InitializeStaticReferences();
            InitializeGraphics(width, height);
            InitializeEffects();

            InitializeDictionaries();

            LoadFonts();
            LoadTextures(); 
            LoadVertices();
            LoadPrimitiveArchetypes();
            LoadVideos();

            InitializeManagers();

            InitializeCollidablePlayer();
            LoadGame();
            InitializeNonCollidableGround(1000);
            InitializeCameraTracks();
            InitializeCameraRails();
            InitializeCameras();
            

            InitializeUI();

            #region Event Handling
            this.eventDispatcher.MenuChanged += EventDispatcher_MenuChanged;
            #endregion

            this.soundManager.PlayCue("background");

            base.Initialize();
        }

        private void EventDispatcher_MenuChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnExit)
                this.Exit();
            else if (eventData.EventType == EventActionType.OnRestart)
                this.RestartGame();
            else if (eventData.EventType == EventActionType.OnWin)
                this.NextLevel();
            else if (eventData.EventType == EventActionType.OnLose)
                this.ShowLoseScreen();
        }

        private void LoadGame()
        {
            List<Actor> actorList = null;

            LevelLoader levelLoader = new LevelLoader(this);
            if (this.level == 0)
            {
                actorList = levelLoader.Load(this.textureDictionary["level1"], 10, 10);
            }
            else if (this.level == 1)
            {
                actorList = levelLoader.Load(this.textureDictionary["level2"], 10, 10);
                this.menuManager.HideMenu();

            }
            else if (this.level == 2)
            {
                actorList = levelLoader.Load(this.textureDictionary["level3"], 10, 10);
                this.menuManager.HideMenu();

            }
            else if (this.level == 3)
            {
                actorList = levelLoader.Load(this.textureDictionary["level4"], 10, 10);
                this.menuManager.HideMenu();

            }
            else if(this.level == 4)
            {
                ShowWinGame();
                actorList = levelLoader.Load(this.textureDictionary["level1"], 10, 10);
            }

            if (actorList != null)
            {
                foreach (Actor actor in actorList)
                    this.objectManager.Add(actor);
            }


        }

        private void InitializeCollidableProps()
        {
            PrimitiveObject primitiveObject = null;
            Transform3D transform3D = null;
            IVertexData vertexData = null;

            #region Coloured Triangle
            //step 1 - make the vertices
            vertexData = new VertexData<VertexPositionColor>(
                VertexFactory.GetColoredTriangle(), PrimitiveType.TriangleStrip, 1);

            //step 2 - transform
            transform3D = new Transform3D(new Vector3(0, 10, 0), 2 * Vector3.One);
            //step 3 - texture - not this time because its VertexPositionColor      
      
            //step 4 - make primitive
            primitiveObject = new PrimitiveObject("1st triangle", ActorType.Decorator,
                transform3D, this.wireframePrimitiveEffect, vertexData,
                Color.Red, 1, StatusType.Drawn | StatusType.Updated);

            this.objectManager.Add(primitiveObject);
            #endregion

            #region Textured Quad
            //Add a textured quad and put a texture on the quad, attach a controller
            TexturedPrimitiveObject texturedPrimitiveObject = null;

            vertexData = new VertexData<VertexPositionColorTexture>(
                VertexFactory.GetTextureQuadVertices(), PrimitiveType.TriangleStrip, 2);

            transform3D = new Transform3D(new Vector3(20, 20, 0), 3 * Vector3.One);

            texturedPrimitiveObject = new TexturedPrimitiveObject("1st quad", ActorType.Decorator,
                transform3D, this.texturedPrimitiveEffect, vertexData,
                this.textureDictionary["ml"], Color.Pink, 1,
                StatusType.Drawn | StatusType.Updated);

            this.objectManager.Add(texturedPrimitiveObject);
            #endregion
 

        }

        private void InitializeNonCollidableBillboards()
        {
            BillboardPrimitiveObject billboardArchetypeObject = null, cloneBillboardObject = null;

            //archetype - clone from this
            billboardArchetypeObject = new BillboardPrimitiveObject("billboard", ActorType.Billboard,
                Transform3D.Zero, //transform reset in clones
                this.billboardPrimitiveEffect,
                this.vertexDictionary["textured_quad"],
                this.textureDictionary["white"],
                Color.White, 1,
                StatusType.Drawn | StatusType.Updated,
                BillboardType.Normal); //texture reset in clones

            #region Normal
            cloneBillboardObject = (BillboardPrimitiveObject)billboardArchetypeObject.Clone();
            cloneBillboardObject.BillboardType = BillboardType.Normal;
            cloneBillboardObject.Transform3D = new Transform3D(new Vector3(0, 25, -10), Vector3.Zero, 4 * Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            cloneBillboardObject.Texture = this.textureDictionary["chevron1"];
            this.objectManager.Add(cloneBillboardObject);
            #endregion


            #region Normal Scrolling
            cloneBillboardObject = (BillboardPrimitiveObject)billboardArchetypeObject.Clone();
            cloneBillboardObject.Transform3D = new Transform3D(new Vector3(-15, 25, -10), new Vector3(45, 0, 0), new Vector3(16, 10, 1), Vector3.UnitX, Vector3.UnitY);
            cloneBillboardObject.Alpha = 0.4f; //remember we can set alpha
            cloneBillboardObject.Texture = this.textureDictionary["ml"];
            cloneBillboardObject.BillboardParameters.SetScrolling(true);
            cloneBillboardObject.BillboardParameters.SetScrollRate(new Vector2(0, -50));
            this.objectManager.Add(cloneBillboardObject);
            #endregion

            #region Normal Animated
            cloneBillboardObject = (BillboardPrimitiveObject)billboardArchetypeObject.Clone();
            cloneBillboardObject.Transform3D = new Transform3D(new Vector3(15, 25, -10), new Vector3(0, 0, 0), new Vector3(4, 4, 1), Vector3.UnitX, Vector3.UnitY);
            cloneBillboardObject.Texture = this.textureDictionary["alarm2"];
            cloneBillboardObject.Color = Color.Red;
            cloneBillboardObject.BillboardParameters.SetAnimated(true);
            cloneBillboardObject.BillboardParameters.SetAnimationRate(4, 1, 0);
            this.objectManager.Add(cloneBillboardObject);
            #endregion

            #region Normal Scrolling - Snow
            cloneBillboardObject = (BillboardPrimitiveObject)billboardArchetypeObject.Clone();
            cloneBillboardObject.Transform3D = new Transform3D(new Vector3(50, -25, -10), new Vector3(0, 0, 0), new Vector3(25, 25, 1), Vector3.UnitX, Vector3.UnitY);
            cloneBillboardObject.Texture = this.textureDictionary["snow1"];
            cloneBillboardObject.BillboardParameters.SetScrolling(true);
            cloneBillboardObject.BillboardParameters.SetScrollRate(new Vector2(5, -15));
            this.objectManager.Add(cloneBillboardObject);

            cloneBillboardObject = (BillboardPrimitiveObject)billboardArchetypeObject.Clone();
            cloneBillboardObject.Transform3D = new Transform3D(new Vector3(50, -25, -15), new Vector3(0, 30, 0), new Vector3(25, 25, 1), Vector3.UnitX, Vector3.UnitY);
            cloneBillboardObject.Texture = this.textureDictionary["snow1"];
            cloneBillboardObject.BillboardParameters.SetScrolling(true);
            cloneBillboardObject.BillboardParameters.SetScrollRate(new Vector2(3, -10));
            this.objectManager.Add(cloneBillboardObject);

            cloneBillboardObject = (BillboardPrimitiveObject)billboardArchetypeObject.Clone();
            cloneBillboardObject.Transform3D = new Transform3D(new Vector3(50, -25, -20), new Vector3(0, -15, 0), new Vector3(25, 25, 1), Vector3.UnitX, Vector3.UnitY);
            cloneBillboardObject.Texture = this.textureDictionary["snow1"];
            cloneBillboardObject.BillboardParameters.SetScrolling(true);
            cloneBillboardObject.BillboardParameters.SetScrollRate(new Vector2(1, -6));
            this.objectManager.Add(cloneBillboardObject);
            #endregion

            #region Normal Text To Texture
            cloneBillboardObject = (BillboardPrimitiveObject)billboardArchetypeObject.Clone();
            cloneBillboardObject.Transform3D = new Transform3D(new Vector3(-60, 30f, 20), new Vector3(0, 0, 0),
                new Vector3(20, 5, 1), Vector3.UnitX, Vector3.UnitY);
            cloneBillboardObject.Texture = this.textureDictionary["white"];
            cloneBillboardObject.AttachController(new TextRendererController("trc1", 
                ControllerType.TextRenderer,
                this.fontDictionary["ui"], "Press V to play video", Color.Black,
                new Color(255, 0, 255, 0)));
            this.objectManager.Add(cloneBillboardObject);
            #endregion

            #region Normal Video
            cloneBillboardObject = (BillboardPrimitiveObject)billboardArchetypeObject.Clone();
            cloneBillboardObject.Transform3D = new Transform3D(new Vector3(-60, 20, 20), new Vector3(0, 0, 0),
                new Vector3(8, 8, 1), Vector3.UnitX, Vector3.UnitY);
            cloneBillboardObject.Texture = this.textureDictionary["white"];
            cloneBillboardObject.AttachController(new Video3DController("vc1",
                ControllerType.Video3D,
                  this.textureDictionary["white"], 
                  this.videoDictionary["sample"], "sample.wmv", 0.1f, 0.5f));
            this.objectManager.Add(cloneBillboardObject);
            #endregion
        }

        //adds an emitter at the origin that can be heard in 3D by thr F4 (1st person collidable camera)
        private void Initialize3DAudioEmitter()
        {
            this.audioEmitter = new AudioEmitter();
            audioEmitter.Position = new Vector3(0, 0, 0);
            audioEmitter.Up = Vector3.UnitY;
            audioEmitter.Forward = Vector3.UnitZ;
            audioEmitter.DopplerScale = 1;
        }

        private void RestartGame()
        {
            
            this.menuManager.ShowMenu();
            this.objectManager.Dispose();


            bool bShowCDCRSurfaces = false; //show wireframe CD-CR surfaces
            bool bShowZones = true; //show zone wireframe CD-CR surfaces
            this.objectManager = new ObjectManager(this, "gameObjects", bShowCDCRSurfaces, bShowZones);
            Components.Add(this.objectManager);
            this.objectManager.DrawOrder = 0;

            this.score = 0;
            this.TextScoreObject.Text = "Score: " + this.Score;
            this.level = 0;
            LoadGame();

            this.objectManager.Add(cloneTexturedPrimitiveObject);

            this.player.Transform3D = new Transform3D(new Vector3(20, 10, 20),
                Vector3.Zero, Vector3.One * 5, Vector3.UnitX, Vector3.UnitY);
            this.objectManager.Add(this.player);

            this.progressController.CurrentValue = 100;

            this.cameraManager.SetActiveCameraLayout("rail");
            this.cameraManager.ActiveCamera.DetachController("rail");
            this.cameraManager.ActiveCamera.AttachController(new TrackController("rail", ControllerType.Track,
            this.curveDictionary["camera_rail"], StatusType.Play));
        }

        //figure out issue with menu
        public void NextLevel()
        {
            this.menuManager.ShowMenu();
            this.objectManager.Dispose();

            
            bool bShowCDCRSurfaces = false; //show wireframe CD-CR surfaces
            bool bShowZones = true; //show zone wireframe CD-CR surfaces
            this.objectManager = new ObjectManager(this, "gameObjects", bShowCDCRSurfaces, bShowZones);
            Components.Add(this.objectManager);
            this.objectManager.DrawOrder = 0; 

            this.level++;
            LoadGame();

            this.objectManager.Add(cloneTexturedPrimitiveObject);

            this.progressController.CurrentValue = 100;

            this.player.Transform3D = new Transform3D(new Vector3(20, 10, 20),
                Vector3.Zero, Vector3.One * 5, Vector3.UnitX, Vector3.UnitY);
            this.objectManager.Add(this.player);
        }
        public void ShowLoseScreen()
        {
            
            RestartGame();
            progressController.Stop();
            this.TextLevelObject.Text = "LeveL: 1";
            this.TextScoreObject.Text = "Score: " + this.Score;
            this.menuManager.ShowLoseMenuScreen();
        }
        public void ShowWinGame()
        {
            this.score = 0;
            progressController.Stop();
            this.TextScoreObject.Text = "Score: " + this.Score;
            this.TextLevelObject.Text = "LeveL: 1";
            this.menuManager.ShowWinMenuScreen();
            this.level = 0;

        }

        private void InitializeUI()
        {
            InitializeUIHelpDialog();
            InitializeUIProgress();
            InitializeUIScore();
            InitializeUILevel();
        }

        private void InitializeUIHelpDialog()
        {
            Transform2D transform = null;
            SpriteFont font = null;
            Texture2D texture = null;

            //text
            font = this.fontDictionary["ui"];
            String text = "Rules of The Game:\nW A S and D to move the player\nReach the end of the maze in time\nCollect the lighter pickups\nAnd avoid the darker ones!";
            Vector2 dimensions = font.MeasureString(text);
            transform = new Transform2D(new Vector2(50, 600), 0, Vector2.One, Vector2.Zero, new Integer2(dimensions));
            UITextObject textObject = new UITextObject("test1",
                ActorType.UIText,
                StatusType.Drawn | StatusType.Updated, 
                transform, new Color(15, 15, 15, 200), SpriteEffects.None, 0, text, font);
            this.uiManager.Add(textObject);

            //texture
            texture = this.textureDictionary["white"];
            transform = new Transform2D(new Vector2(40, 590), 0, new Vector2(3.8f, 10), Vector2.Zero, new Integer2(texture.Width, texture.Height));
            UITextureObject texture2DObject = new UITextureObject("texture1",
                 ActorType.UITexture,
                StatusType.Drawn | StatusType.Updated, 
                transform, new Color(127, 127, 127, 50),
                SpriteEffects.None, 1, texture);
            this.uiManager.Add(texture2DObject);
        }
        private void InitializeUIProgress()
        {

            Transform2D transform = null;
            Texture2D texture = null;
            UITextureObject textureObject = null;
            Vector2 position = Vector2.Zero;
            Vector2 scale = Vector2.Zero;
            float verticalOffset = 10;
            float horizontalOffset = 10;

            scale = new Vector2(2, 2f);
            position = new Vector2(horizontalOffset, verticalOffset);
            texture = this.textureDictionary["bar_outline"];
            transform = new Transform2D(position, 0, scale, Vector2.Zero, new Integer2(texture.Width, texture.Height));

            
            textureObject = new UITextureObject("background",
                    ActorType.UITexture,
                    StatusType.Drawn | StatusType.Updated,
                    transform, Color.White,
                    SpriteEffects.None,
                    0,
                    texture);
            this.uiManager.Add(textureObject);

            this.progressController = new UIProgressController("anxiety", ControllerType.UIProgressController, 100, 100);
            texture = this.textureDictionary["bar_colour"];
            textureObject = new UITextureObject("colour",
                    ActorType.UITexture,
                    StatusType.Drawn | StatusType.Updated,
                    transform, Color.Green,
                    SpriteEffects.None,
                    0.1f,
                    texture);
            textureObject.AttachController(this.progressController);
            this.uiManager.Add(textureObject);

            texture = this.textureDictionary["bar_white"];
            textureObject = new UITextureObject("foreground",
                    ActorType.UITexture,
                    StatusType.Drawn | StatusType.Updated,
                    transform, Color.White,
                    SpriteEffects.None,
                    0.2f,
                    texture);

            this.uiManager.Add(textureObject);
        }

        private void InitializeUIScore()
        {
            Transform2D transform = null;
            SpriteFont font = null;

            //text
            string text = "Score: 0";
            font = this.fontDictionary["ui"];
            Vector2 dimensions = font.MeasureString(text);
            transform = new Transform2D(new Vector2(15, 55), 0, Vector2.One, Vector2.Zero, new Integer2(dimensions));
            this.textObject = new UITextObject("Score",
                ActorType.UIText,
                StatusType.Drawn | StatusType.Updated,
                transform, new Color(15, 15, 15, 200), SpriteEffects.None, 0, text, font);
            this.uiManager.Add(textObject);
        }

        private void InitializeUILevel()
        {
            Transform2D transform = null;
            SpriteFont font = null;

            //text
            string text = "Level: 1";
            font = this.fontDictionary["ui"];
            Vector2 dimensions = font.MeasureString(text);
            transform = new Transform2D(new Vector2(15, 40), 0, Vector2.One, Vector2.Zero, new Integer2(dimensions));
            this.TextLevelObject = new UITextObject("Level",
                ActorType.UIText,
                StatusType.Drawn | StatusType.Updated,
                transform, new Color(15, 15, 15, 200), SpriteEffects.None, 0, text, font);
            this.uiManager.Add(TextLevelObject);
        }

        private void InitializeEventDispatcher()
        {
            this.eventDispatcher = new EventDispatcher(this, 10);
            Components.Add(this.eventDispatcher);
        }
        private void InitializeStaticReferences()
        {
            Actor.game = this;
            Camera3D.game = this;
            Controller.game = this;
        }
        private void InitializeGraphics(int width, int height)
        {
            this.graphics.PreferredBackBufferWidth = width;
            this.graphics.PreferredBackBufferHeight = height;
            this.graphics.ApplyChanges();

            //or we can set full screen
            //   this.graphics.IsFullScreen = true;
            //    this.graphics.ApplyChanges();

            //records screen centre point - used by mouse to see how much the mouse pointer has moved
            this.screenCentre = new Vector2(this.graphics.PreferredBackBufferWidth / 2.0f,
                this.graphics.PreferredBackBufferHeight / 2.0f);
        }
        private void InitializeEffects()
        {
            //used for wirframe primitives
            this.wireframePrimitiveEffect = new BasicEffect(graphics.GraphicsDevice);
            this.wireframePrimitiveEffect.VertexColorEnabled = true;

            //used for textured (i.e. solid) primitives
            this.texturedPrimitiveEffect = new BasicEffect(graphics.GraphicsDevice);
            this.texturedPrimitiveEffect.VertexColorEnabled = true;
            this.texturedPrimitiveEffect.TextureEnabled = true;

            //used for billboards
            this.billboardPrimitiveEffect = Content.Load<Effect>("Assets/Effects/Billboard");

            this.litPrimitiveEffect = new BasicEffect(graphics.GraphicsDevice);
            this.litPrimitiveEffect.TextureEnabled = true;
            this.litPrimitiveEffect.LightingEnabled = true;
            this.litPrimitiveEffect.EnableDefaultLighting();
            this.litPrimitiveEffect.PreferPerPixelLighting = true;


        }
        private void InitializeManagers()
        {
            this.mouseManager = new MouseManager(this, true);
            this.mouseManager.SetPosition(this.ScreenCentre);
            Components.Add(this.mouseManager);

            this.keyboardManager = new KeyboardManager(this);
            Components.Add(this.KeyboardManager);

            this.cameraManager = new CameraManager(this);
            Components.Add(this.cameraManager);

            this.soundManager = new SoundManager(this,
               "Content\\Assets\\Audio\\Demo2DSound.xgs",
               "Content\\Assets\\Audio\\WaveBank1.xwb",
              "Content\\Assets\\Audio\\SoundBank1.xsb");
            Components.Add(this.soundManager);

            bool bShowCDCRSurfaces = false; //show wireframe CD-CR surfaces
            bool bShowZones = true; //show zone wireframe CD-CR surfaces
            this.objectManager = new ObjectManager(this, "gameObjects", bShowCDCRSurfaces, bShowZones);
            this.objectManager.DrawOrder = 0; //first to draw
            Components.Add(this.objectManager);

            Texture2D[] menuTextures = { 
                        this.textureDictionary["mainmenu"],
                        this.textureDictionary["exitmenu"],
                        this.textureDictionary["winmenu"],
                        this.textureDictionary["losemenu"],
                                        };
         
            this.uiManager = new UIManager(this, "ui manager", 10, true);
            this.uiManager.DrawOrder = 1; //next to draw, on top of object
            Components.Add(this.uiManager);

            this.menuManager = new MenuManager(this, menuTextures,
             this.fontDictionary["menu"], Integer2.Zero, Color.White);
            this.menuManager.DrawOrder = 2; //last to draw, on top of object and UI
            Components.Add(this.menuManager);

            //draws CDCR surfaces for boxes and spheres
         //   this.primitiveDebugDrawer = new PrimitiveDebugDrawer(this);
         //   Components.Add(this.primitiveDebugDrawer);


            //set drawable component draw order
        }
        private void InitializeDictionaries()
        {
            //"grass", grass.png
            this.textureDictionary = new GenericDictionary<string, Texture2D>("texture dictionary");            
            this.vertexDictionary = new GenericDictionary<string, IVertexData>("vertex dictionary");           
            this.objectDictionary = new GenericDictionary<string, DrawnActor3D>("object dictionary");
            this.fontDictionary = new GenericDictionary<string, SpriteFont>("font dictionary");
            this.curveDictionary = new GenericDictionary<string, Transform3DCurve>("curve dictionary");
            this.railDictionary = new GenericDictionary<string, RailParameters>("rail dictionary");
            this.videoDictionary = new GenericDictionary<string, Video>("vidoes");

        }
        #endregion

        #region Camera
        private void InitializeCameraRails()
        {
            RailParameters rail = null;

            rail = new RailParameters("rail1", new Vector3(-100, 50, 100), new Vector3(100, 100, 100));
            this.railDictionary.Add(rail.ID, rail);

            rail = new RailParameters("rail2", new Vector3(100, 50, 100), new Vector3(100, 50, -100));
            this.railDictionary.Add(rail.ID, rail);
        }
        private void InitializeCameraTracks()
        {
            Transform3DCurve curve = null;

            #region Curve1
            curve = new Transform3DCurve(CurveLoopType.Linear);
            curve.Add(new Vector3(120, 40, 60), -Vector3.UnitY, -Vector3.UnitZ, 0);

            curve.Add(new Vector3(70, 40, 60), -Vector3.UnitY, -Vector3.UnitZ, 2);

            curve.Add(new Vector3(65, 40, 20), -Vector3.UnitY, -Vector3.UnitZ, 4.5f);

            curve.Add(new Vector3(20, 50, 20), -Vector3.UnitY, -Vector3.UnitZ, 6);

            curve.Add(new Vector3(20, 80, 20), -Vector3.UnitY, -Vector3.UnitZ, 8);

            this.curveDictionary.Add("camera_rail", curve);
            #endregion
            
        }
        private void InitializeCameras()
        {
            Transform3D transform = null;
            Transform3D transform2 = null;
            Camera3D camera = null;
            string cameraLayout = "";

            #region Layout 1x1
            cameraLayout = "1x1";

            #region First Person Camera
            //changed look and up to point camera down
            transform = new Transform3D(new Vector3(20, 80, 20), -Vector3.UnitY, -Vector3.UnitZ);
            camera = new Camera3D("Player", ActorType.Camera, transform,
                ProjectionParameters.StandardMediumSixteenNine,
                new Viewport(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            camera.AttachController(new CameraFollowController("camerafollow", ControllerType.ThirdPerson, this.player));

            //add the new camera to the approriate K, V pair in the camera manager dictionary i.e. where key is "1x2"
            this.cameraManager.Add(cameraLayout, camera);
            #endregion
            #endregion

            //finally, set the active layout
            this.cameraManager.SetActiveCameraLayout("1x1");

            transform2 = new Transform3D(new Vector3(20, 80, 20), -Vector3.UnitY, -Vector3.UnitZ);
            camera = new Camera3D("Rail", ActorType.Camera, transform2,
                ProjectionParameters.StandardMediumSixteenNine,
                new Viewport(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

            camera.AttachController(new TrackController("rail", ControllerType.Track,
            this.curveDictionary["camera_rail"], StatusType.Play));


            this.cameraManager.Add("rail", camera);
            this.cameraManager.SetActiveCameraLayout("rail");
        }
        #endregion
        private void InitializeCollidablePlayer()
        {
            Transform3D transform = null;
            IVertexData vertexData = null;
            PrimitiveType primitiveType;
            int primitiveCount;

            transform = new Transform3D(new Vector3(20, 10, 20),
                Vector3.Zero, Vector3.One*5, Vector3.UnitX, Vector3.UnitY);

            VertexPositionNormalTexture[] vertices
              = VertexFactory.GetVerticesPositionNormalTexturedCube(1,
                out primitiveType, out primitiveCount);

            vertexData
                = new BufferedVertexData<VertexPositionNormalTexture>(
                    graphics.GraphicsDevice, vertices, primitiveType,
                    primitiveCount);

            BoxCollisionPrimitive collisionPrimitive
                = new BoxCollisionPrimitive(transform);

            this.player = new PlayerCollidablePrimitiveObject("player",
                    ActorType.Player,
                    transform, this.litPrimitiveEffect, vertexData, 
                     this.textureDictionary["ml"], Color.White, 1, StatusType.Drawn | StatusType.Updated ,collisionPrimitive,
                    AppData.CameraMoveKeys, AppData.CameraMoveSpeed/4, AppData.CameraRotationSpeed/4);

            this.objectManager.Add(this.player);
        }
        #region Assets
        private void LoadVideos()
        {
            this.videoDictionary.Add("sample", Content.Load<Video>("Assets/Video/sample"));
        }

        private void LoadFonts()
        {
            this.fontDictionary.Add("ui", Content.Load<SpriteFont>("Assets/Fonts/ui"));
            this.fontDictionary.Add("menu", Content.Load<SpriteFont>("Assets/Fonts/menu"));
            this.fontDictionary.Add("mouse", Content.Load<SpriteFont>("Assets/Fonts/mouse"));
        }
       
        private void LoadTextures()
        {
            #region debug
            this.textureDictionary.Add("ml",
               Content.Load<Texture2D>("Assets/Textures/Debug/ml"));
            this.textureDictionary.Add("checkerboard",
                Content.Load<Texture2D>("Assets/Textures/Debug/checkerboard"));
            this.textureDictionary.Add("abs",
                Content.Load<Texture2D>("Assets/Textures/Abstract/pink"));
            this.textureDictionary.Add("wall",
                Content.Load<Texture2D>("Assets/Textures/Debug/wall"));
            this.textureDictionary.Add("floor",
                Content.Load<Texture2D>("Assets/Textures/Debug/floor"));
            this.textureDictionary.Add("backdrop",
                Content.Load<Texture2D>("Assets/Textures/Debug/backdrop"));
            #endregion 

            #region UI Reticule
            this.textureDictionary.Add("reticuleDefault", Content.Load<Texture2D>("Assets/Textures/UI/Reticule/reticuleDefault"));
            this.textureDictionary.Add("reticuleOpen", Content.Load<Texture2D>("Assets/Textures/UI/Reticule/reticuleOpen"));
            this.textureDictionary.Add("reticuleClosed", Content.Load<Texture2D>("Assets/Textures/UI/Reticule/reticuleClosed"));
            this.textureDictionary.Add("mouseicons", Content.Load<Texture2D>("Assets/Textures/UI/mouseicons"));
            #endregion

            #region billboards
            this.textureDictionary.Add("billboardtexture", Content.Load<Texture2D>("Assets/Textures/Billboards/billboardtexture"));
            this.textureDictionary.Add("snow1", Content.Load<Texture2D>("Assets/Textures/Billboards/snow1"));
            this.textureDictionary.Add("chevron1", Content.Load<Texture2D>("Assets/Textures/Billboards/chevron1"));
            this.textureDictionary.Add("chevron2", Content.Load<Texture2D>("Assets/Textures/Billboards/chevron2"));
            this.textureDictionary.Add("alarm1", Content.Load<Texture2D>("Assets/Textures/Billboards/alarm1"));
            this.textureDictionary.Add("alarm2", Content.Load<Texture2D>("Assets/Textures/Billboards/alarm2"));
            this.textureDictionary.Add("tv", Content.Load<Texture2D>("Assets/Textures/Props/tv"));
            #endregion

            #region menu
            this.textureDictionary.Add("mainmenu", Content.Load<Texture2D>("Assets/Textures/Menu/mainmenu"));
            this.textureDictionary.Add("exitmenu", Content.Load<Texture2D>("Assets/Textures/Menu/exitmenu"));
            this.textureDictionary.Add("winmenu", Content.Load<Texture2D>("Assets/Textures/Menu/winmenu"));
            this.textureDictionary.Add("losemenu", Content.Load<Texture2D>("Assets/Textures/Menu/losemenu"));
            #endregion

            #region UI
            this.textureDictionary.Add("white", Content.Load<Texture2D>("Assets/Textures/UI/white"));
            this.textureDictionary.Add("bar_white", Content.Load<Texture2D>("Assets/Textures/UI/white"));
            this.textureDictionary.Add("bar_colour", Content.Load<Texture2D>("Assets/Textures/UI/colour"));
            this.textureDictionary.Add("bar_outline", Content.Load<Texture2D>("Assets/Textures/UI/outline"));
            #endregion

            #region Levels
            this.textureDictionary.Add("level1", Content.Load<Texture2D>("Assets/Textures/Level/level1"));
            this.textureDictionary.Add("level2", Content.Load<Texture2D>("Assets/Textures/Level/level2"));
            this.textureDictionary.Add("level3", Content.Load<Texture2D>("Assets/Textures/Level/level3"));
            this.textureDictionary.Add("level4", Content.Load<Texture2D>("Assets/Textures/Level/level4"));
            #endregion
        }
        private void LoadVertices()
        {
            #region Factory Based Approach
            #region Textured Quad
            this.vertexDictionary.Add("textured_quad", 
                new VertexData<VertexPositionColorTexture>(
                VertexFactory.GetTextureQuadVertices(),
                PrimitiveType.TriangleStrip, 2));
            #endregion
            #endregion

            #region Old User Defines Vertices Approach
            VertexPositionColor[] verticesPositionColor = null;
            IVertexData vertexData = null;
            float halfLength = 0.5f;

            #region Textured Cube
            int primitiveCount = 0;
            PrimitiveType primitiveType;
            this.vertexDictionary.Add("textured_cube", 
                new VertexData<VertexPositionColorTexture>(VertexFactory.GetVerticesPositionTexturedCube(1, out primitiveType, out primitiveCount),
                primitiveType, primitiveCount));
            #endregion

            #region Wireframe Origin Helper
            verticesPositionColor = new VertexPositionColor[6];

            //x-axis
            verticesPositionColor[0] = new VertexPositionColor(new Vector3(-halfLength, 0, 0), Color.Red);
            verticesPositionColor[1] = new VertexPositionColor(new Vector3(halfLength, 0, 0), Color.Red);
            //y-axis
            verticesPositionColor[2] = new VertexPositionColor(new Vector3(0, halfLength, 0), Color.Green);
            verticesPositionColor[3] = new VertexPositionColor(new Vector3(0, -halfLength, 0), Color.Green);
            //z-axis
            verticesPositionColor[4] = new VertexPositionColor(new Vector3(0, 0, halfLength), Color.Blue);
            verticesPositionColor[5] = new VertexPositionColor(new Vector3(0, 0, -halfLength), Color.Blue);

            vertexData = new VertexData<VertexPositionColor>(verticesPositionColor, Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList, 3);            
            this.vertexDictionary.Add("wireframe_origin_helper", vertexData);
            #endregion

            #region Wireframe Triangle
            verticesPositionColor = new VertexPositionColor[3];

            verticesPositionColor[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
            verticesPositionColor[1] = new VertexPositionColor(new Vector3(1, 0, 0), Color.Green);
            verticesPositionColor[2] = new VertexPositionColor(new Vector3(-1, 0, 0), Color.Blue);

            vertexData = new VertexData<VertexPositionColor>(verticesPositionColor, Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 1);
            this.vertexDictionary.Add("wireframe_triangle", vertexData);
            #endregion
            #endregion
        }
        private void LoadPrimitiveArchetypes()
        {
            Transform3D transform = null;
            TexturedPrimitiveObject texturedQuad = null;

            #region Textured Quad Archetype
            transform = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            texturedQuad = new TexturedPrimitiveObject("textured quad archetype", ActorType.Decorator,
                     transform, this.texturedPrimitiveEffect, this.vertexDictionary["textured_quad"],
                     this.textureDictionary["checkerboard"], Color.White, 1, StatusType.Drawn | StatusType.Updated); //or  we can leave texture null since we will replace it later

            this.objectDictionary.Add("textured_quad", texturedQuad);
            #endregion
        }
        #endregion

        #region Non-collidable
        private void InitializeNonCollidableGround(int worldScale)
        {
            TexturedPrimitiveObject archTexturedPrimitiveObject = null;
            

            #region Archetype
            //we need to do an "as" typecast since the dictionary holds DrawnActor3D types
            archTexturedPrimitiveObject = this.objectDictionary["textured_quad"] as TexturedPrimitiveObject;
            archTexturedPrimitiveObject.Transform3D.Scale *= 150;
            #endregion

            #region Grass
            cloneTexturedPrimitiveObject = (TexturedPrimitiveObject)archTexturedPrimitiveObject.Clone();
            cloneTexturedPrimitiveObject.ID = "ground";
            cloneTexturedPrimitiveObject.Transform3D.Translation = new Vector3(20, 0, 20);
            cloneTexturedPrimitiveObject.Transform3D.Rotation = new Vector3(-90, 0, 0);
            cloneTexturedPrimitiveObject.Texture = this.textureDictionary["backdrop"];
            cloneTexturedPrimitiveObject.AttachController(new CameraFollowController("camerafollow", ControllerType.ThirdPerson, this.player));
            this.objectManager.Add(cloneTexturedPrimitiveObject);
            #endregion
        }
        #endregion

        #region Collidable
        #endregion

        #region Game Loop & Content
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        protected override void UnloadContent()
        {
            //release all assets - see GenericDictionary::Dispose()
            //this.fontDictionary.Dispose();
           //this.textureDictionary.Dispose();
            //this.vertexDictionary.Dispose();
            this.objectDictionary.Dispose();
            this.railDictionary.Dispose();
            this.curveDictionary.Dispose();
            this.videoDictionary.Dispose();

            
            this.cameraManager.Dispose();
            this.objectManager.Dispose();
            this.uiManager.Dispose();
            this.mouseManager.Dispose();
            this.keyboardManager.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach(Camera3D camera in this.cameraManager)
            {
                //set the viewport based on the current camera
                graphics.GraphicsDevice.Viewport = camera.Viewport;
                base.Draw(gameTime);

                //set which is the active camera (remember that our objects use the CameraManager::ActiveCamera property to access View and Projection for rendering
                this.cameraManager.ActiveCameraIndex++;
            }
        }
        #endregion

    }
}