using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GDApp;


namespace GDLibrary
{
    public class MenuManager: DrawableGameComponent
    {
        #region Variables
        private List<MenuItem> menuItemList;
        private Main game;
        private SpriteFont menuFont;
        private Color menuTextureBlendColor;

        private Texture2D[] menuTextures;
        private Rectangle textureRectangle;

        private MenuItem menuPlay, menuRestart, menuExit;
        private MenuItem menuExitYes, menuExitNo;
        private MenuItem menuBackToMain, menuGoToExit;

        protected int currentMenuTextureIndex = 0; //0 = main, 1 = volume
        private bool bPaused = true;
        private bool buttonSoundPlayed;
        #endregion

        #region Properties
        public Color MenuTextureBlendColor
        {
            get
            {
                return menuTextureBlendColor;
            }
            set
            {
                menuTextureBlendColor = value;
            }
        }
        public bool Pause
        {
            get
            {
                return bPaused;
            }
            set
            {
                bPaused = value;
            }
        }
        #endregion

        #region Core menu manager - No need to change this code
        public MenuManager(Main game, Texture2D[] menuTextures, 
            SpriteFont menuFont, Integer2 textureBorderPadding,
            Color menuTextureBlendColor) 
            : base(game)
        {
            this.game = game;

            //load the textures
            this.menuTextures = menuTextures;

            //background blend color for the menu
            this.menuTextureBlendColor = menuTextureBlendColor;

            //menu font
            this.menuFont = menuFont;

            //stores all menu item (e.g. Save, Resume, Exit) objects
            this.menuItemList = new List<MenuItem>();
               
            //set the texture background to occupy the entire screen dimension, less any padding
            this.textureRectangle = game.ScreenRectangle;

            //deflate the texture rectangle by the padding required
            this.textureRectangle.Inflate(-textureBorderPadding.X, -textureBorderPadding.Y);

           InitialiseMenuOptions();

           ShowMenu();

        }
                                                                                                                               
        public void Add(MenuItem theMenuItem) 
        {
            menuItemList.Add(theMenuItem);
        }

        public void Remove(MenuItem theMenuItem)
        {
            menuItemList.Remove(theMenuItem);
        }

        public void RemoveAll()
        {
            menuItemList.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            TestIfPaused();

            //menu is not paused so show and process
            if (!bPaused)
                ProcessMenuItemList();

         
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!this.bPaused)
            {
                //enable alpha blending on the menu objects
                this.game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.Default, null);
                //draw whatever background we expect to see based on what menu or sub-menu we are viewing
                game.SpriteBatch.Draw(menuTextures[currentMenuTextureIndex], textureRectangle, this.menuTextureBlendColor);

                //draw the text on top of the background
                for (int i = 0; i < menuItemList.Count; i++)
                {
                    menuItemList[i].Draw(game.SpriteBatch, menuFont);
                }

                game.SpriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void TestIfPaused()
        {
            //if menu is pause and we press the show menu button then show the menu
            if(this.game.KeyboardManager.IsFirstKeyPress(AppData.KeyPauseShowMenu))
            {
                if (this.bPaused)
                    ShowMenu();
                else
                    HideMenu();
            }

        }

        public void ShowMenu()
        {
            //show the menu screen
            ShowMainMenuScreen();

            //show the menu by setting pause to false
            bPaused = false;

            //generate an event to tell the object manager to...
            EventDispatcher.Publish(new EventData("pause", this, EventActionType.OnPause, EventCategoryType.MainMenu));

            //if the mouse is invisible then show it
            if (!this.game.IsMouseVisible)
                this.game.IsMouseVisible = true;

        }

        public void HideMenu()
        {
            //generate an event to tell the object manager to...
            EventDispatcher.Publish(new EventData("play", this, EventActionType.OnPlay, EventCategoryType.MainMenu));

            //hide the menu by setting pause to true
            bPaused = true;

            //if the mouse is invisible then show it
            if (this.game.IsMouseVisible)
                this.game.IsMouseVisible = false;

            this.game.MouseManager.SetPosition(this.game.ScreenCentre);
        }

        private void RestartGame()
        {
            //generate an event to tell the object manager to...
            EventDispatcher.Publish(new EventData("restart", this, EventActionType.OnRestart, EventCategoryType.MainMenu));

            //hide the menu by setting pause to true
            bPaused = true;

            //if the mouse is invisible then show it
            if (this.game.IsMouseVisible)
                this.game.IsMouseVisible = false;
        }

        private void ExitGame()
        {
            //generate an event to tell the object manager to...
            EventDispatcher.Publish(new EventData("exit", this, EventActionType.OnExit, EventCategoryType.MainMenu));
        }

        //iterate through each menu item and see if it is "highlighted" or "highlighted and clicked upon"
        private void ProcessMenuItemList()
        {
           for(int i = 0; i < menuItemList.Count; i++)
           {
               MenuItem item = menuItemList[i];

               //is the mouse over the item?
               if (this.game.MouseManager.Bounds.Intersects(item.Bounds)) 
               {
                   item.SetActive(true);

                   //is the left mouse button clicked
                   if (game.MouseManager.IsLeftButtonClickedOnce())
                   {
                        buttonSoundPlayed = false;
                        game.SoundManager.PlayCue("menuClick");
                        DoMenuAction(menuItemList[i].Name);
                       break;
                   }
                    else if (buttonSoundPlayed == false)
                    {
                        game.SoundManager.PlayCue("menuButtons");
                        buttonSoundPlayed = true;
                    }
                }
                else
                {
                    if (item.GetActive() == true)
                    {
                        buttonSoundPlayed = false;
                    }
                    item.SetActive(false);
                }
            }
        }

        //to do - dispose, clone
        #endregion

        #region Code specific to your application
        private void InitialiseMenuOptions()
        {
            this.menuPlay = new MenuItem(MenuData.Menu_Play, MenuData.Menu_Play,
                MenuData.BoundsMenuPlay, MenuData.ColorMenuInactive, MenuData.ColorMenuActive);
            this.menuRestart = new MenuItem(MenuData.StringMenuRestart, MenuData.StringMenuRestart,
                MenuData.BoundsMenuRestart, MenuData.ColorMenuInactive, MenuData.ColorMenuActive);
            this.menuExit = new MenuItem(MenuData.StringMenuExit, MenuData.StringMenuExit,
                MenuData.BoundsMenuExit, MenuData.ColorMenuInactive, MenuData.ColorMenuActive);

            this.menuExitYes = new MenuItem(MenuData.StringMenuExitYes, MenuData.StringMenuExitYes,
             MenuData.BoundsMenuExitYes, MenuData.ColorMenuInactive, MenuData.ColorMenuActive);
            this.menuExitNo = new MenuItem(MenuData.StringMenuExitNo, MenuData.StringMenuExitNo,
                MenuData.BoundsMenuExitNo, MenuData.ColorMenuInactive, MenuData.ColorMenuActive);

            this.menuBackToMain = new MenuItem(MenuData.StringToMainMenu, MenuData.StringToMainMenu,
             MenuData.BoundsToMainMenu, MenuData.ColorMenuInactive, MenuData.ColorMenuActive);
            this.menuGoToExit = new MenuItem(MenuData.StringToExit, MenuData.StringToExit,
                MenuData.BoundsToExit, MenuData.ColorMenuInactive, MenuData.ColorMenuActive);
        }
        //perform whatever actions are listed on the menu
        private void DoMenuAction(String name)
        {
            if (name.Equals(MenuData.Menu_Play))
            {
                HideMenu();
                game.progressController.Play();
            }
            else if (name.Equals(MenuData.StringMenuRestart))
            {
                EventDispatcher.Publish(new EventData("restart", this, EventActionType.OnRestart, EventCategoryType.MainMenu));
            }
            else if (name.Equals(MenuData.StringMenuExit))
            {
                ShowExitMenuScreen();       
            }
            else if (name.Equals(MenuData.StringMenuExitYes))
            {
                ExitGame();
            }
            else if (name.Equals(MenuData.StringMenuExitNo) || name.Equals(MenuData.StringToMainMenu))
            {
                ShowMainMenuScreen();
            }

            //add your new menu actions here...
        }

        //add your ShowXMenuScreen() methods here...

        private void ShowMainMenuScreen()
        {
            //remove any items in the menu
            RemoveAll();
            //add the appropriate items
            Add(menuPlay);
            Add(menuRestart);
            Add(menuExit);
            //set the background texture
            currentMenuTextureIndex = MenuData.TextureIndexMainMenu;
        }

        private void ShowExitMenuScreen()
        {
            //remove any items in the menu
            RemoveAll();
            //add the appropriate items
            Add(menuExitYes);
            Add(menuExitNo);
            //set the background texture
            currentMenuTextureIndex = MenuData.TextureIndexExitMenu;
        }

        public void ShowWinMenuScreen()
        {
            //remove any items in the menu
            RemoveAll();
            //add the appropriate items
            Add(menuBackToMain);
            Add(menuGoToExit);
            //set the background texture
            currentMenuTextureIndex = MenuData.TextureIndexWinMenu;
        }

        public void ShowLoseMenuScreen()
        {
            //remove any items in the menu
            RemoveAll();
            //add the appropriate items
            Add(menuBackToMain);
            Add(menuGoToExit);
            //set the background texture
            currentMenuTextureIndex = MenuData.TextureIndexLoseMenu;
        }

        #endregion
    }
}
