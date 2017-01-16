using System;
using Microsoft.Xna.Framework;


namespace GDLibrary
{
    public class MenuData
    {
        #region Main Menu Strings
        //all the strings shown to the user through the menu
        public static String Game_Title = "Space Invaders";
        public static String Menu_Play = "Play";
        public static string StringMenuRestart = "Restart";
        public static String StringMenuSave = "Save";
        public static String StringMenuExit = "Exit";

        public static string StringMenuExitYes = "Yes";
        public static string StringMenuExitNo = "No";

        public static string StringToMainMenu = "Main Menu";
        public static string StringToExit = "Exit";
        #endregion

        #region Colours, Padding, Texture transparency , Array Indices and Bounds
        public static Integer2 MenuTexturePadding = new Integer2(10, 10);
        public static Color MenuTextureColor = new Color(1, 1, 1, 0.9f);

        //the hover colours for menu items
        public static Color ColorMenuInactive = new Color(15, 216,91);
        public static Color ColorMenuActive = Color.White;

        //the position of the texture in the array of textures provided to the menu manager
        public static int TextureIndexMainMenu = 0;
        public static int TextureIndexExitMenu = 1;
        public static int TextureIndexWinMenu = 2;
        public static int TextureIndexLoseMenu = 3;

        //bounding rectangles used to detect mouse over
        public static Rectangle BoundsMenuPlay = new Rectangle(460, 420, 70, 40); //x, y, width, height
        public static Rectangle BoundsMenuRestart = new Rectangle(440, 470, 120, 40);
        public static Rectangle BoundsMenuExit = new Rectangle(460, 520, 90, 40);

        public static Rectangle BoundsMenuExitYes = new Rectangle(400, 500, 50, 40);
        public static Rectangle BoundsMenuExitNo = new Rectangle(600, 500, 35, 40);

        public static Rectangle BoundsToMainMenu = new Rectangle(350, 500, 160, 40);
        public static Rectangle BoundsToExit = new Rectangle(600, 500, 90, 40);
        #endregion

        #region UI Menu
        public static String UI_Menu_AddHouse = "Add House";
        public static string UI_Menu_AddBarracks = "Add Barracks";
        public static string UI_Menu_AddFence = "Add Fence";

        public static Rectangle UI_Menu_AddHouse_Bounds = new Rectangle(40, 380, 90, 20);
        public static Rectangle UI_Menu_AddBarracks_Bounds = new Rectangle(40, 400, 120, 20);
        public static Rectangle UI_Menu_AddFence_Bounds = new Rectangle(40, 420, 90, 20);


        #endregion

    }
}
