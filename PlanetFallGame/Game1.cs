using System;
using System.Collections.Generic;
using FlurryWP8SDK.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace PlanetFall
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        //Function to get random number
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //global random number
        Random r;

        //game variables
        double lander_x;
        double lander_y;
        double lander_y_start = 30;
        int lander_height;
        int lander_width;
        int lander_pixel_factor;
        bool lander_destroyed;
        double thrust_y;
        double thrust_x;
        double velocity_x;
        double velocity_y;
        double gravity = 0.98;
        int level = 0;
        int lives = 3;
        int level_type = 0;
        int level_count = 15;
        int fuel;
        int fuel_max = 500;
        int score;
        int pause_countdown =0; //used to delay keypresses

        //thruster annimation
        bool[] show_thrusters;
        int[] show_thrusters_countdown;
        int[] show_thrusters_animid;
        Texture2D[] thruster;

        //map
        Texture2D map_sprite;
        Texture2D t; //for drawing lines
        int vertical_offset = 120;
        Texture2D[] map_thumbs;
        Rectangle[] map_thumbs_location;

        //menu stuff
        Texture2D menu_close_button;
        bool menu_open = false;
        int menu_id = 0;
        int menu_type = 0; //0=left, 1=center
        string[] menu_items;
        string[] menu_titles;
        Rectangle[] menu_rectangles;
        int menu_item_count;
        Texture2D titleimage;
        Texture2D menu_background;
        Texture2D logo;
        Texture2D logo_text;
        Texture2D logo_final_screen;
        Texture2D whiteRectangle;

        //general game state
        bool game_paused = true;

        //asteroids
        Texture2D[] asteroids_sprite;
        vars._asteroids[] asteroids;
        int asteroid_count = 0;
        int asteroid_direction = 0;


        //actors
        Texture2D lander_sprite;
        Texture2D[] bounties_sprites;
        Texture2D background;
        int background_origin; //middle of background, for parallax scroll
        vars._bounties[] bounties;
        Texture2D[] explosions;
        int explosion_frame;

        int ScreenWidth, ScreenHeight;

        //fonts
        SpriteFont retroFont;
        SpriteFont titleFont;
        SpriteFont titleFont2;
        SpriteFont messageFont;
        SpriteFont myFont;

        //buttons
        Texture2D button_down_texture;
        Texture2D button_up_texture;
        Texture2D button_left_texture;
        Texture2D button_right_texture;
        Texture2D button_cancel;

        Rectangle tap_button_up;
        Rectangle tap_button_down;
        Rectangle tap_button_left;
        Rectangle tap_button_right;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ScreenWidth = GraphicsDevice.Viewport.Width; //770
            ScreenHeight = GraphicsDevice.Viewport.Height; //480
            menu_items = new string[16];
            menu_rectangles = new Rectangle[16];
            menu_titles = new String[16];
            map_thumbs = new Texture2D[level_count + 1];
            map_thumbs_location = new Rectangle[level_count + 1];

            //asteroids
            asteroids_sprite=new Texture2D[10];
            asteroids = new vars._asteroids[10];

            //bounties
            bounties_sprites = new Texture2D[3];

            r = new Random(Guid.NewGuid().GetHashCode());

            thruster = new Texture2D[5]; //animated sprites
            show_thrusters = new bool[3];
            show_thrusters_countdown = new int[3];
            show_thrusters_animid = new int [3];

            explosions = new Texture2D[6];

            gamedata.load_map_coords();

            lander_x = Convert.ToInt32(ScreenWidth / 2);
            lander_y = lander_y_start;
            gravity = vars.gravity_values[0];
            level_type = vars.level_types[level];
            fuel = fuel_max;
            level = 0;
            score = 0;
            thrust_x = 0;
            thrust_y = 0;
            velocity_y = 0;
            menu_open = true;
            game_paused = true;
            vars.arewepaused = game_paused;

            vars.button_down = new vars._myrect();
            vars.button_up = new vars._myrect();
            vars.button_left = new vars._myrect();
            vars.button_right = new vars._myrect();

            vars.button_down.x = Convert.ToInt32(690);
            vars.button_down.y = Convert.ToInt32(399);
            vars.button_down.w = Convert.ToInt32(64);
            vars.button_down.h = Convert.ToInt32(64);

            vars.button_up.x = Convert.ToInt32(600);
            vars.button_up.y = Convert.ToInt32(399);
            vars.button_up.w = Convert.ToInt32(64);
            vars.button_up.h = Convert.ToInt32(64);

            vars.button_left.x = Convert.ToInt32(20);
            vars.button_left.y = Convert.ToInt32(399);
            vars.button_left.w = Convert.ToInt32(64);
            vars.button_left.h = Convert.ToInt32(64);

            vars.button_right.x = Convert.ToInt32(120);
            vars.button_right.y = Convert.ToInt32(399);
            vars.button_right.w = Convert.ToInt32(64);
            vars.button_right.h = Convert.ToInt32(64);

            //set up player movement buttons
            tap_button_down = new Rectangle(vars.button_down.x, vars.button_down.y, vars.button_down.w, vars.button_down.h);
            tap_button_up = new Rectangle(vars.button_up.x, vars.button_up.y, vars.button_up.w, vars.button_up.h);
            tap_button_left = new Rectangle(vars.button_left.x, vars.button_left.y, vars.button_left.w, vars.button_left.h);
            tap_button_right = new Rectangle(vars.button_right.x, vars.button_right.y, vars.button_right.w, vars.button_right.h);

            //load game stuff
            level = 0;
            menu_open = true;
            //menu_id = 13;
            //load_level(level);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("backdrops\\twitter-backgrounds7");
            background_origin = Convert.ToInt32(background.Width / 4);

            logo = Content.Load<Texture2D>("logo\\logo400x300b");
            logo_text = Content.Load<Texture2D>("logo\\logo293x30");
            logo_final_screen = Content.Load<Texture2D>("logo\\logo-final-screen");

            retroFont = Content.Load<SpriteFont>("fonts\\Font");
            titleFont = Content.Load<SpriteFont>("fonts\\titlefont");
            titleFont2 = Content.Load<SpriteFont>("fonts\\titlefont2");
            messageFont = Content.Load<SpriteFont>("fonts\\MessageFont");
            myFont = Content.Load<SpriteFont>("fonts\\myfont");

            //bounties
            bounties_sprites[0] = Content.Load<Texture2D>("bounties\\bounty1");
            bounties_sprites[1] = Content.Load<Texture2D>("bounties\\bounty2");
            bounties_sprites[2] = Content.Load<Texture2D>("bounties\\bounty3");

            menu_close_button = Content.Load<Texture2D>("buttons\\closewindow");
            menu_background = Content.Load<Texture2D>("backdrops\\1-star-background");
            //titleimage = Content.Load<Texture2D>("title\\title3");
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

            //buttons
            button_down_texture = Content.Load<Texture2D>("buttons\\appbar.arrow.down64");
            button_up_texture = Content.Load<Texture2D>("buttons\\appbar.arrow.up64");
            button_left_texture = Content.Load<Texture2D>("buttons\\appbar.back.rest64");
            button_right_texture = Content.Load<Texture2D>("buttons\\appbar.next.rest64");
            button_cancel = Content.Load<Texture2D>("buttons\\appbar.door.leave64");

            //lander
            lander_sprite = Content.Load<Texture2D>("lander\\lander24");
            lander_height = lander_sprite.Height;
            lander_width = lander_sprite.Width;
            lander_pixel_factor = Convert.ToInt32(lander_width / 4) + 1;

            //map
            map_sprite = Content.Load<Texture2D>("maps\\map" + level.ToString());
            //map thumbs
            for (int i = 0; i <= level_count; i++ )
            {
                map_thumbs[i] = Content.Load<Texture2D>("maps\\thumb_map" + i.ToString());
            }

            //dot used for drawing lines
            t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(new Color[] { Color.White });// fill the texture with white

            //thruster
            thruster[0] = Content.Load<Texture2D>("flames\\flame_yellow-0-0");
            thruster[1] = Content.Load<Texture2D>("flames\\flame_yellow-0-1");
            thruster[2] = Content.Load<Texture2D>("flames\\flame_yellow-0-2");
            thruster[3] = Content.Load<Texture2D>("flames\\flame_yellow-0-3");
            thruster[4] = Content.Load<Texture2D>("flames\\flame_yellow-0-4");
            
            explosions[0] = Content.Load<Texture2D>("flames\\enemy_explosion-0-0");
            explosions[1] = Content.Load<Texture2D>("flames\\enemy_explosion-0-1");
            explosions[2] = Content.Load<Texture2D>("flames\\enemy_explosion-0-2");
            explosions[3] = Content.Load<Texture2D>("flames\\enemy_explosion-0-3");
            explosions[4] = Content.Load<Texture2D>("flames\\enemy_explosion-0-4");

            menu_titles[0] = "Main Menu";
            menu_titles[1] = "Level Select";
            menu_titles[2] = "Instructions";

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            base.UnloadContent();
            spriteBatch.Dispose();
        }

        void load_level(int newlevel)
        {
            FlurryWP8SDK.Api.LogEvent("Level #" + newlevel + " loaded");
            map_sprite = Content.Load<Texture2D>("maps\\map" + newlevel.ToString());
            level = newlevel;
            game_paused = true;
            vars.arewepaused = game_paused;
            menu_open = false;
            lander_y = lander_y_start;
            //Random r = new Random(Guid.NewGuid().GetHashCode());
            lander_x = GetRandomNumber(50, ScreenWidth - 50);
            fuel = 300;
            lander_destroyed = false;
            explosion_frame = 0;
            asteroid_count = 0;
            if (level>=4 )
            {
                asteroid_count = Convert.ToInt16(level / 5) + 1;
                for (int i=0;i<=asteroid_count;i++)
                {
                    new_asteroid(i);
                }
            }
        }

        void new_asteroid(int asteroid_id)
        {
            r = new Random(Guid.NewGuid().GetHashCode());
            asteroids[asteroid_id] = new vars._asteroids();
            asteroids[asteroid_id].active = true;
            asteroids[asteroid_id].inplay = false;
            asteroids[asteroid_id].timer = 0;
            asteroid_direction++;
            if (asteroid_direction > 2) { asteroid_direction = 0; }
            if (asteroid_direction == 1)
            {
                //to the left of the escreen
                asteroids[asteroid_id].x = -50;
                asteroids[asteroid_id].y = 20 + GetRandomNumber(1, ScreenHeight - 40);
                asteroids[asteroid_id].velocity_x = (GetRandomNumber(0, 10)) / 8;
                asteroids[asteroid_id].velocity_y = (5 - GetRandomNumber(0, 10)) / 5;
            }
            else if (asteroid_direction == 2)
            {
                //to the right of the screen
                asteroids[asteroid_id].x = ScreenWidth + 50;
                asteroids[asteroid_id].y = 20 + GetRandomNumber(1, ScreenHeight - 40);
                asteroids[asteroid_id].velocity_x = -(GetRandomNumber(0, 10)) / 7;
                asteroids[asteroid_id].velocity_y = (5 - r.Next(0, 10)) / 6;
            }
            else
            {
                //above the screen
                asteroids[asteroid_id].x = GetRandomNumber(0, ScreenWidth);
                asteroids[asteroid_id].y = -50;
                asteroids[asteroid_id].velocity_x = GetRandomNumber(-10, 10);
                asteroids[asteroid_id].velocity_y = (GetRandomNumber(0, 10)) / 5;
            }
            asteroids[asteroid_id].rotation = 0;
            asteroids_sprite[asteroid_id] = Content.Load<Texture2D>("asteroids\\" + GetRandomNumber(1, 9).ToString());     
        }

        string lastGesture;
        int waitabit = 0;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (lives < 1 && menu_open != true)
            {
                // game over man, game over!
                menu_open = true;
                pause_countdown = 50;
                lander_destroyed = false;
                menu_id = 12;
                return;
            }

            // -----------------------------------------------------------------------------------------------
            //                                  PROCESS TOUCH EVENTS
            // -----------------------------------------------------------------------------------------------

            lastGesture = "";
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.DragComplete | GestureType.Flick | GestureType.DoubleTap | GestureType.Hold;
            var gesture = default(GestureSample);
            while (TouchPanel.IsGestureAvailable)
            {
                gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.Tap || gesture.GestureType == GestureType.DoubleTap || gesture.GestureType == GestureType.Hold)
                {

                    waitabit=0;
                    if (waitabit < 0) { waitabit = 0; }
                    if (waitabit == 0)
                    {
                        if (gesture.GestureType == GestureType.Hold) { lastGesture = "Hold"; }
                        if (gesture.GestureType == GestureType.DoubleTap) { lastGesture = "DoubleTap"; }
                        if (gesture.GestureType == GestureType.Tap) { lastGesture = "Tap"; }

                        float x = gesture.Position.X;
                        float y = gesture.Position.Y;

                        if (game_paused == true)
                        {
                            Rectangle r = new Rectangle(120, 120, ScreenWidth - 239, ScreenHeight - 239);
                            if (r.Contains((int)x, (int)y))
                            {
                                game_paused = false;
                                vars.arewepaused = game_paused;
                            }
                        }

                        if (menu_id == 12)  //game over screen
                        {
                            Rectangle r = new Rectangle(1, 1, ScreenWidth - 1, ScreenHeight - 1); //anywhere!
                            if (r.Contains((int)x, (int)y))
                            {
                                //restart game
                                menu_open = true;
                                game_paused = true;
                                vars.arewepaused = false;
                                level = 0;
                                menu_id = 0;
                                return;
                            }
                        }

                        if (pause_countdown > 0) { pause_countdown = pause_countdown - 1; }
                        if (menu_open == true && (menu_id <= 10 || menu_id==13 ))
                        {
                            // -------------- MAIN MENU TOUCH OPTIONS ------------------- //
                            Rectangle r = new Rectangle(ScreenWidth - 74, ScreenHeight - 74, 64, 64);
                            if (r.Contains((int)x, (int)y))
                            {
                                menu_open = false;
                            }
                            if (menu_item_count >= 1)
                            {
                                for (int i = 0; i <= menu_item_count; i++)
                                {
                                    if (menu_rectangles[i].Contains((int)x, (int)y))
                                    {
                                        if (menu_id == 0 && i == 4)
                                        {
                                            //quit  
                                        }
                                        if (menu_id == 0 && i == 0)
                                        {
                                            //new game
                                            score = 0;
                                            lives = 3;
                                            load_level(0);
                                            waitabit = 10;
                                            FlurryWP8SDK.Api.LogEvent("New Game");
                                        }
                                        if (menu_id == 0 && i == 1)
                                        {
                                            //resume game
                                            game_paused = true;
                                            vars.arewepaused = game_paused;
                                            menu_open = false;
                                            waitabit = 10;
                                        }
                                        if (menu_id == 0 && i == 2)
                                        {
                                            //level select
                                            menu_id = 1;
                                            waitabit = 10;
                                            pause_countdown = 3;
                                            FlurryWP8SDK.Api.LogEvent("Level Select");
                                        }
                                        if (menu_id == 0 && i == 3)
                                        {
                                            //instructions
                                            menu_id = 2;
                                            waitabit = 10;
                                            FlurryWP8SDK.Api.LogEvent("Instructions");
                                        }
                                        if (menu_id == 0 && i == 4)
                                        {
                                            //about
                                            pause_countdown = 50;
                                            menu_id = 13;
                                            waitabit = 10;
                                            FlurryWP8SDK.Api.LogEvent("About");
                                        }
                                        if (menu_id == 1 && i == 4)
                                        {
                                            //level select, go back
                                            menu_id = 0;
                                            waitabit = 10;
                                        }

                                        //about menu
                                        if (menu_id == 13 && i == 3)
                                        {
                                            //feedback
                                            FlurryWP8SDK.Api.LogEvent("Feedback");
                                            vars.openfeedback = true;
                                            waitabit = 10;
                                        }
                                        if (menu_id == 13 && i == 5)
                                        {
                                            //level select, go back
                                            menu_id = 0;
                                            waitabit = 10;
                                        }
                                    }
                                }
                            }
                            if (menu_id == 1)
                            {
                                //level select
                                if (pause_countdown > 0) { pause_countdown = pause_countdown - 1; }
                                vars.arewepaused = false; //preempt banner change
                                for (int i = 0; i <= level_count; i++)
                                {
                                    if (pause_countdown ==0 && map_thumbs_location[i].Contains((int)x, (int)y))
                                    {
                                        load_level(i);
                                    }
                                }
                            }
                            if (menu_id == 2) //instructions
                            {
                                if (tap_button_down.Contains((int)x, (int)y))
                                {
                                    // Cancel out of game
                                    menu_id = 0;
                                    menu_open = true;
                                }
                            }
                        }
                        else if (menu_open == true && menu_id == 10 && pause_countdown <= 0)
                        {
                            Rectangle r = new Rectangle(1, 1, ScreenWidth - 1, ScreenHeight - 1); //anywhere!
                            if (r.Contains((int)x, (int)y))
                            {
                                menu_open = false;
                                game_paused = true;
                                vars.arewepaused = false; //preempt banner change
                            }
                        }
                        else if (menu_open == true && menu_id == 11 && pause_countdown <= 0)
                        {
                            Rectangle r = new Rectangle(1, 1, ScreenWidth - 1, ScreenHeight - 1); //anywhere!
                            if (r.Contains((int)x, (int)y))
                            {
                                //restart game
                                menu_open = true;
                                game_paused = true;
                                vars.arewepaused = false; //preempt banner change
                                level = 0;
                                menu_id = 0;
                            }
                        }
                        else
                        {
                            // -------------- MAIN MENU TOUCH OPTIONS ------------------- //
                            // look for movement button taps
                            if (tap_button_down.Contains((int)x, (int)y))
                            {
                                // Cancel out of game
                                menu_id = 0;
                                menu_open = true;
                            }
                            if (tap_button_up.Contains((int)x, (int)y))
                            {
                                velocity_y = velocity_y + 1.5;
                                show_thrusters[0] = true;
                                show_thrusters_countdown[0] = 15;
                                show_thrusters_animid[0] = 0;
                            }
                            if (tap_button_left.Contains((int)x, (int)y))
                            {
                                thrust_x = thrust_x - 0.5;
                                show_thrusters[1] = true;
                                show_thrusters_countdown[1] = 5;
                                show_thrusters_animid[1] = 0;
                            }
                            if (tap_button_right.Contains((int)x, (int)y))
                            {
                                thrust_x = thrust_x + 0.5;
                                show_thrusters[2] = true;
                                show_thrusters_countdown[2] = 5;
                                show_thrusters_animid[2] = 0;
                            }

                            //look for control button taps
                            for (int i = 0; i <= 5; i++)
                            {
                                //Rectangle r = new Rectangle(vars.controlbuttons[i].x, vars.controlbuttons[i].y, vars.controlbuttons[i].w, vars.controlbuttons[i].h);
                                //if (r.Contains((int)x, (int)y))
                                //{
                                //    add_to_messages("Button " + i + " tapped.");
                                //    menu_open = true;
                                //    menu_id = i;
                                //}
                            }
                        }
                    }  //waitabit


                    //lastKeys = "TAP:" + gesture.Position.X + "," + gesture.Position.Y;
                }
            }


            // -----------------------------------------------------------------------------------------------
            //                                  PROCESS TURN-BASED EVENTS
            // -----------------------------------------------------------------------------------------------
            if (menu_open == false)
            {
                if (game_paused != true)
                {
                    if (lander_destroyed==true)
                    {
                        if (explosion_frame > 100)
                        {
                            pause_countdown = 50;
                            game_paused = true;
                            vars.arewepaused = game_paused;
                            lander_destroyed = false;
                            lives = lives - 1;
                            if (lives<1)
                            {
                                // game over man, game over!
                                menu_open = true;
                                pause_countdown = 50;
                                menu_id = 12;
                            }
                            lander_y = lander_y_start;
                            lander_x = Convert.ToInt16(ScreenWidth / 2);
                            fuel = 300;
                            load_level(level);
                        }
                    }
                }
            }

            if (menu_open == false)
            {
                if (game_paused != true)
                {

                    //move asteroids
                    if (asteroid_count > 0)
                    {
                        for (int ii = 0; ii <= asteroid_count; ii++)
                        {
                            if (asteroids[ii].active == true)
                            {
                                asteroids[ii].rotation = asteroids[ii].rotation + 1;
                                if (asteroids[ii].rotation >= 360) { asteroids[ii].rotation = 0; }
                                asteroids[ii].x = asteroids[ii].x + asteroids[ii].velocity_x;
                                asteroids[ii].y = asteroids[ii].y + asteroids[ii].velocity_y;
                                if (asteroids[ii].inplay == true)
                                {
                                    if (asteroids[ii].x < 0 || asteroids[ii].x > ScreenWidth || asteroids[ii].y < 0 || asteroids[ii].y > ScreenHeight)
                                    {
                                        new_asteroid(ii);
                                    }
                                }
                                if (asteroids[ii].timer >= 30 && asteroids[ii].inplay != true) { new_asteroid(ii); }
                            }
                        }
                    }

                    //increase vertical thrust until it reaches the gravity value;
                    thrust_y = thrust_y + 0.05;

                    //work out the thrust/velocity 
                    if (thrust_y >= gravity*1.5) { thrust_y = gravity*1.5; }
                    if (velocity_y >= 0) { velocity_y = velocity_y - 0.1; }


                    // see if lander has hit the ground, if so, stop thrust and velocity - do it 6x times (24pixels wide)
                    bool hit_the_ground = false;
                    int approx_x = Convert.ToInt32(lander_x / 4);
                    if (approx_x >= gamedata.map_cords[level].Length - 1) { approx_x = gamedata.map_cords[level].Length - 1; }
                    for (int i = 0; i <= lander_pixel_factor - 1; i++)
                    {
                        if (approx_x + i <= gamedata.map_cords[level].Length - 1 && approx_x>=0)
                        {
                            if (lander_y >= gamedata.map_cords[level][approx_x + i] - (vertical_offset + lander_height) && velocity_y <= 0) { hit_the_ground = true; }
                            if (lander_y+24 >= ScreenHeight) { lander_destroyed = true; }
                        }
                    }
                    if (hit_the_ground == true)
                    {
                        thrust_y = 0;
                        velocity_y = 0;
                        thrust_x = 0;
                        show_thrusters[0] = false;
                        show_thrusters[1] = false;
                        show_thrusters[2] = false;

                        //see if it's in landing spot
                        int landing_x = gamedata.map_landing[level];
                        if (lander_x>=landing_x-24 && lander_x<=landing_x+24)
                        {
                            List<Parameter> articleParams = new List<Parameter>{ new Parameter("Fuel", Convert.ToString(fuel)) };
                            FlurryWP8SDK.Api.LogEvent("Level Complete");
                            menu_open = true;
                            level = level + 1; //next level
                            menu_id = 10; //congrates menu page
                            if (level>=level_count)
                            {
                                menu_id = 11; //game over!
                                return;
                            }
                            load_level(level);
                            //map_sprite = Content.Load<Texture2D>("maps\\map" + level.ToString());
                            pause_countdown = 50;
                            score = score + fuel;
                        }


                    }

                    //decrease fuel if thrusters are burning
                    if (show_thrusters[1] == true || show_thrusters[2] == true) { fuel = fuel - 1; }
                    if ((thrust_y - velocity_y) < 0) { fuel = fuel - 1; }

                    if (fuel < 0)
                    {
                        lander_destroyed = true;
                        thrust_x = 0;
                        thrust_y = 0;
                        velocity_x = 0;
                        velocity_y = 0;
                    }

                    //now move the lander
                    lander_y = lander_y + (thrust_y - velocity_y);
                    lander_x = lander_x + thrust_x;

                    //make sure lander is within screen-coordinates
                    if (lander_x <= 1) { lander_x = 1; }
                    if (lander_x >= ScreenWidth - lander_width) { lander_x = ScreenWidth - lander_width; }

                    //if (thrust_y > 0) { thrust_y = thrust_y - 0.5; }
                    //if (thrust_x > 0) { thrust_x = thrust_x - 0.25; } //inertia doesn't change - no friction!

                    // draw bounties
                    int x, y;
                    Rectangle r;
                    for (int i = 0; i <= 2; i++)
                    {
                        x = gamedata.map_bounties[level][i];
                        if (x != 0 && gamedata.map_bounties_active[level][i])
                        {
                            y = gamedata.map_cords[level][Convert.ToInt16(x / 4) + 1];
                            r = new Rectangle(x-16, y - vertical_offset - 32, x + 32, y+16);
                            if (r.Contains((int)lander_x, (int)lander_y))
                            {
                                gamedata.map_bounties_active[level][i] = false;
                                score = score + 1000;
                                if (i == 1) { fuel = 300; }
                            }
                        }
                    }



                } //not paused
            }//menu not open

            // -----------------------------------------------------------------------------------------------
            //                                  LOOK FOR OBJECTS TO PROCESS
            // -----------------------------------------------------------------------------------------------


            // -----------------------------------------------------------------------------------------------
            //                              increment idle_counter
            // -----------------------------------------------------------------------------------------------

            // -----------------------------------------------------------------------------------------------
            //                              Update Game Timer
            // -----------------------------------------------------------------------------------------------

            base.Update(gameTime);
        }


        /// <summary>
        /// Draw menu
        /// </summary>
        /// <param name="gameTime"></param>
        protected void DrawMenu(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // draw background
            if (menu_id == 0)
            {
                spriteBatch.Draw(menu_background, new Vector2(0, 0));
            }
            else
            {
                spriteBatch.Draw(menu_background, new Vector2(0, 0));
            }

            // start drawing menu
            menu_item_count = 0;
            if (menu_id == 0)
            {
                // main menu                
                //spriteBatch.Draw(titleimage, new Rectangle(ScreenWidth - 500, 84, 482, 265), Color.White);  //482x265
                menu_type = 0;
                menu_items[0] = "New Game";
                menu_items[1] = "Resume Game";
                menu_items[2] = "Level Select";
                menu_items[3] = "Instructions";
                menu_items[4] = "About";
                menu_items[5] = "";
                menu_item_count = 4;
                //draw logo
                spriteBatch.Draw(logo, new Rectangle(300, 50, 400, 300), Color.White * 0.9f);
            }
            if (menu_id == 13)
            {
                //draw logo
                spriteBatch.Draw(logo, new Rectangle(400, 50, 400, 300), Color.White * 0.9f);
                menu_type = 0;
                menu_items[0] = "About PlanetFall";
                menu_items[1] = "";
                menu_items[2] = "";
                menu_items[3] = "Feedback";
                menu_items[4] = "";
                menu_items[5] = "Back";
                menu_item_count = 5;
                String[] lines;
                lines = new string[16];
                lines[0] = "Written by Matthew Proctor, Neotronic Studios 2014";
                lines[1] = "Developed using Visual Studio 15, Monogame, XAML and C#";
                lines[2] = "";
                lines[3] = "";
                for (int ii = 0; ii <= 3; ii++)
                {
                    spriteBatch.DrawString(myFont, lines[ii], new Vector2(65, 120 + (ii * 35)), Color.Black);
                    spriteBatch.DrawString(myFont, lines[ii], new Vector2(65, 120 + (ii * 35)), Color.White);
                }
            }
            if (menu_id == 1)
            {
                // level select                
                //spriteBatch.Draw(whiteRectangle, new Rectangle(26, 26, ScreenWidth - 49, ScreenHeight - 49), Color.Black * 0.5f);
                //spriteBatch.Draw(whiteRectangle, new Rectangle(25, 25, ScreenWidth - 50, ScreenHeight - 50), Color.Silver * 0.5f);
                menu_type = 0;
                menu_items[4] = "Back";
                menu_items[0] = "";
                menu_items[1] = "";
                menu_items[2] = "";
                menu_items[3] = "";
                menu_items[5] = "";

                menu_item_count = 4;
                int xx = 50;
                int yy = 110;
                int count = 0;
                for (int i=0;i<=level_count;i++)
                {
                    map_thumbs_location[i] = new Rectangle(xx, yy, 64, 64);
                    spriteBatch.Draw(map_thumbs[i], map_thumbs_location[i], Color.White*0.5f);
                    xx = xx + 80;
                    count = count + 1;
                    if (count >= 8)
                    {
                        count = 0;
                        yy = yy + 80;
                        xx = 50;
                    }
                }
            }
            if (menu_id == 2)
            {
                String[] lines;
                lines = new string[16];
                lines[0] = "Your mission is to land your craft successfully on all " + level_count.ToString() + " planets.";
                lines[1] = "Your craft is antiquated but has three powerful thrusters. ";
                lines[2] = "Two for lateral movement, and one large vertical thruster";
                lines[3] = " to slow your descent. Use them wisely, fuel is scarce.";
                for (int ii = 0; ii <= 3; ii++)
                {
                    spriteBatch.DrawString(myFont, lines[ii], new Vector2(40, 100 + (ii * 35)), Color.Black);
                    spriteBatch.DrawString(myFont, lines[ii], new Vector2(40, 100 + (ii * 35)), Color.White);
                }
                spriteBatch.Draw(button_up_texture, new Vector2(50, 300), Color.White * 0.5f);
                spriteBatch.DrawString(myFont, "Main Thruster", new Vector2(120, 300), Color.Black);
                spriteBatch.DrawString(myFont, "Main Thruster", new Vector2(120, 300), Color.White);
                
                spriteBatch.Draw(button_right_texture, new Vector2(300, 300), Color.White * 0.5f);
                spriteBatch.DrawString(myFont, "Left Thruster", new Vector2(370, 300), Color.Black);
                spriteBatch.DrawString(myFont, "Left Thruster", new Vector2(370, 300), Color.White);

                spriteBatch.Draw(button_left_texture, new Vector2(550, 300), Color.White * 0.5f);
                spriteBatch.DrawString(myFont, "Right Thruster", new Vector2(620, 300), Color.Black);
                spriteBatch.DrawString(myFont, "Right Thruster", new Vector2(620, 300), Color.White);

            }
            if (menu_id == 10)
            {
                //level completed
                spriteBatch.Draw(whiteRectangle, new Rectangle(26, 26, ScreenWidth - 49, ScreenHeight - 49), Color.Black * 0.5f);
                spriteBatch.Draw(whiteRectangle, new Rectangle(25, 25, ScreenWidth - 50, ScreenHeight - 50), Color.Silver * 0.5f);
                menu_type = 1;
                menu_items[0] = "Congratulations";
                menu_items[1] = "Score:" + Convert.ToInt32(score);
                menu_items[2] = "Tap to Continue";
                menu_items[3] = "";
                menu_items[4] = "";
                menu_items[5] = "";
                menu_item_count = 2;
                lander_y = lander_y_start;
                lander_x = Convert.ToInt32(ScreenWidth / 2);
                fuel = fuel_max;
                game_paused = true; //for when they exit the level
                vars.arewepaused = game_paused;
                pause_countdown--;
            }
            if (menu_id == 11)
            {
                //level completed
                //spriteBatch.Draw(whiteRectangle, new Rectangle(26, 26, ScreenWidth - 49, ScreenHeight - 49), Color.Black * 0.5f);
                //spriteBatch.Draw(whiteRectangle, new Rectangle(25, 25, ScreenWidth - 50, ScreenHeight - 50), Color.Silver * 0.5f);
                menu_type = 1;
                menu_items[0] = "Congratulations - Game Over!";
                menu_items[1] = "Score:" + Convert.ToInt32(score);
                menu_items[2] = "";
                menu_items[3] = "";
                menu_items[4] = "";
                menu_items[5] = "";
                menu_item_count = 2;
                lander_y = lander_y_start;
                lander_x = Convert.ToInt32(ScreenWidth / 2);
                fuel = fuel_max;
                game_paused = true; //for when they exit the level
                vars.arewepaused = game_paused;
                pause_countdown--;
                spriteBatch.Draw(logo_final_screen, new Vector2(Convert.ToInt32(ScreenWidth / 2) - Convert.ToInt32(323 / 2), ScreenHeight - 195), Color.Silver * 0.9f);
            }
            if (menu_id == 12)
            {
                //level completed
                //spriteBatch.Draw(whiteRectangle, new Rectangle(26, 26, ScreenWidth - 49, ScreenHeight - 49), Color.Black * 0.5f);
                //spriteBatch.Draw(whiteRectangle, new Rectangle(25, 25, ScreenWidth - 50, ScreenHeight - 50), Color.Silver * 0.5f);
                menu_type = 1;
                menu_items[0] = "Game Over";
                menu_items[1] = "Score:" + Convert.ToInt32(score);
                menu_items[2] = "";
                menu_items[3] = "Tap for Main Menu";
                menu_items[4] = "";
                menu_items[5] = "";
                menu_item_count = 3;
                lander_y = lander_y_start;
                lander_x = Convert.ToInt32(ScreenWidth / 2);
                fuel = fuel_max;
                game_paused = true; //for when they exit the level
                vars.arewepaused = game_paused;
                pause_countdown--;
                spriteBatch.Draw(logo_final_screen, new Vector2(Convert.ToInt32(ScreenWidth / 2) - Convert.ToInt32(323 / 2), ScreenHeight - 195), Color.Silver * 0.9f);
            }
            if (menu_id != 0 && menu_id<=9)
            {
                spriteBatch.DrawString(titleFont2, menu_titles[menu_id], new Vector2(37, 41), Color.Black);
                spriteBatch.DrawString(titleFont2, menu_titles[menu_id], new Vector2(36, 40), Color.White);
                spriteBatch.Draw(menu_close_button, new Rectangle(ScreenWidth - 74, ScreenHeight - 74, 48, 48), Color.White);
            }
            //spriteBatch.DrawString(myFont, "(C)2014-2015 Neotronic Studios", new Vector2(41, ScreenHeight - 91), Color.Black);
            //spriteBatch.DrawString(myFont, "(C)2014-2015 Neotronic Studios", new Vector2(40, ScreenHeight - 90), Color.White);
            

            // draw text menu (if one is needed)
            int y = 0;
            int x = 64; // Convert.ToInt32(ScreenWidth / 2);
            if (menu_item_count >= 1)
            {
                for (int i = 0; i <= menu_item_count; i++)
                {
                    y = y + 56;
                    if (menu_items[i] != "")
                    {
                        if (menu_type == 1)
                        {
                            //center the text
                            x = Convert.ToInt32(ScreenWidth / 2) - Convert.ToInt32(titleFont2.MeasureString(menu_items[i]).Length() / 2);
                        }
                        spriteBatch.DrawString(titleFont2, menu_items[i], new Vector2(x, y), Color.Black);
                        spriteBatch.DrawString(titleFont2, menu_items[i], new Vector2(x - 1, y - 1), Color.White);
                        menu_rectangles[i] = new Rectangle(x - 20, y, Convert.ToInt32(titleFont2.MeasureString(menu_items[i]).Length()) + 40, 50);
                        //spriteBatch.Draw(whiteRectangle, menu_rectangles[i], Color.Yellow * 0.2f);
                    }
                }
            }


            // end drawing menu
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            if (menu_open == true)
            {
                DrawMenu(gameTime);
                return;
            }

            // start sprite processing
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // draw background
            int background_position = background_origin+Convert.ToInt32(lander_x/2);
            Vector2 origin = new Vector2(background_position, 0);
            spriteBatch.Draw(background, new Vector2(0, 0), null, Color.White,0,origin, 1,SpriteEffects.None, 0f);

            // draw map
            spriteBatch.Draw(map_sprite, new Vector2(0, 0));

            //draw logo
            spriteBatch.Draw(logo_text, new Vector2(5, 5), Color.White);

            // draw top message
            string myString = "";
            //myString = myString + " Level:" + Convert.ToString(level);
            //myString = myString + " Diff:" + Convert.ToString(level_type);
            //myString = myString + " Thr:" + Convert.ToString((thrust_y - velocity_y));
            //myString = myString + " Fuel:";// +Convert.ToString(fuel);
            //myString = myString + " LG:" + Convert.ToString(lastGesture);
            //myString = myString + " MaxY:" + Convert.ToString(gamedata.map_cords[level][Convert.ToInt32(lander_x/4)] - vertical_offset);
            //myString = myString + " X/Y:" + Convert.ToString(lander_x) + "," + Convert.ToString(lander_y);
            //spriteBatch.DrawString(retroFont, myString, new Vector2(451, 11), Color.Black);
            //spriteBatch.DrawString(retroFont, myString, new Vector2(450, 10), Color.Cyan);

            //draw fuel guage
            spriteBatch.DrawString(retroFont, "Fuel:", new Vector2(551, 11), Color.Black);
            spriteBatch.DrawString(retroFont, "Fuel:", new Vector2(550, 10), Color.Cyan);
            int fuel_x = Convert.ToInt16(fuel / 2);
            spriteBatch.Draw(whiteRectangle, new Rectangle(600,10,160,20), Color.White* 0.5f);
            if (fuel >= 150)
            {
                spriteBatch.Draw(whiteRectangle, new Rectangle(605, 13, fuel_x, 14), Color.Green * 0.5f);
            }
            else
            {
                spriteBatch.Draw(whiteRectangle, new Rectangle(605, 13, fuel_x, 14), Color.Red * 0.5f);
            }

            // Draw Lander
            if (lander_destroyed)
            {
                explosion_frame = explosion_frame + 1;
                int frame = Convert.ToInt16(explosion_frame / 5);
                if (explosion_frame > 1000) { explosion_frame = 1000; }
                if (frame <= 4)
                {
                    spriteBatch.Draw(explosions[frame], new Vector2(Convert.ToInt32(lander_x), Convert.ToInt32(lander_y)), Color.White);
                }
                else
                {
                    //keep lander off the screen
                    lander_x = -50;
                    lander_y = -50;
                }
            }
            else
            {
                spriteBatch.Draw(lander_sprite, new Vector2(Convert.ToInt32(lander_x), Convert.ToInt32(lander_y)), Color.White);
            }


            // Draw lives
            spriteBatch.DrawString(retroFont, "Lives:", new Vector2(366, 11), Color.Black);
            spriteBatch.DrawString(retroFont, "Lives:", new Vector2(365, 10), Color.Cyan);
            for (int i = 1; i <= lives; i++)
            {
                spriteBatch.Draw(lander_sprite, new Vector2(400+i*30,5), Color.White);
            }

            // Draw Thrusters
            for (int i = 0; i <= 2; i++)
            {
                if (show_thrusters[i] == true)
                {
                    show_thrusters_animid[i]++;
                    if (show_thrusters_animid[i] > 4) { show_thrusters_animid[i] = 0; }
                    if (i == 0)
                    {
                        //down flame
                        spriteBatch.Draw(thruster[show_thrusters_animid[i]], new Vector2(Convert.ToInt32(lander_x), Convert.ToInt32(lander_y + 24)), Color.White);
                    }
                    else if (i == 1)
                    {
                        //move left, flame out the right
                        spriteBatch.Draw(thruster[show_thrusters_animid[i]], new Vector2(Convert.ToInt32(lander_x + 36), Convert.ToInt32(lander_y + 12)), null, Color.White, 4.7123f, new Vector2(12, 12), 1f, SpriteEffects.None, 0);
                    }
                    else if (i == 2)
                    {
                        //move right, flame out the left
                        spriteBatch.Draw(thruster[show_thrusters_animid[i]], new Vector2(Convert.ToInt32(lander_x - 10), Convert.ToInt32(lander_y + 12)), null, Color.White, 1.5707f, new Vector2(12, 12), 1f, SpriteEffects.None, 0);
                    }
                    show_thrusters_countdown[i]--;
                    if (show_thrusters_countdown[i] <= 0) { show_thrusters[i] = false; }
                }
            }

            //draw asteroids
            if (asteroid_count>0)
            {
                for (int ii = 0; ii <= asteroid_count; ii++)
                {
                    if (asteroids[ii].active == true)
                    {
                        //spriteBatch.Draw(asteroids_sprite[ii], new Vector2((int)asteroids[ii].x, (int)asteroids[ii].y), Color.White);
                        spriteBatch.Draw(asteroids_sprite[ii], new Vector2((int)asteroids[ii].x, (int)asteroids[ii].y), null, Color.White, MathHelper.ToRadians(asteroids[ii].rotation), new Vector2(12, 12), 1f, SpriteEffects.None, 0);
                        if (asteroids[ii].x >= 0 && asteroids[ii].x <= ScreenWidth && asteroids[ii].y >= 0 && asteroids[ii].y <= ScreenHeight) { asteroids[ii].inplay = true; } else { asteroids[ii].timer++; }
                    }
                }
            }



            // Draw map
            spriteBatch.Draw(map_sprite, new Vector2(0, 0), new Rectangle(0, vertical_offset, ScreenWidth, ScreenHeight), Color.White);

            int y,x;
#if DEBUG
            // Temporarily draw collision spots
            int upto = 0;
            for (x = 0; x <= 798; x=x+4 )
            {
                y = gamedata.map_cords[level][upto];
                upto++;
                //DrawLine(spriteBatch, new Vector2(x, y - vertical_offset), new Vector2(x + 3, y - vertical_offset),Color.Red);
            }
#endif

            // draw bounties
            for (int i = 0; i <= 2; i++)
            {
                x = gamedata.map_bounties[level][i];
                if (x != 0 && gamedata.map_bounties_active[level][i])
                {
                    y = gamedata.map_cords[level][Convert.ToInt16(x / 4) + 1];
                    spriteBatch.Draw(bounties_sprites[i], new Vector2(x, y - vertical_offset-16), Color.White);
                }
            }


            // Draw landing pad
            int landing_x = gamedata.map_landing[level];
            y = gamedata.map_cords[level][Convert.ToInt32(landing_x/4)];
            //if (gamedata.map_start_point[level] != 0) { y = gamedata.map_start_point[level]; }
            DrawLine(spriteBatch, new Vector2(landing_x - 16, y - vertical_offset), new Vector2(landing_x + 16, y - vertical_offset), Color.Yellow);
            DrawLine(spriteBatch, new Vector2(landing_x - 16, y - vertical_offset + 1), new Vector2(landing_x + 16, y - vertical_offset + 1), Color.Yellow);
            DrawLine(spriteBatch, new Vector2(landing_x - 16, y - vertical_offset - 1), new Vector2(landing_x + 16, y - vertical_offset - 1), Color.Red);

            // Draw buttons and a slightly transparent rectangle behind them to make them stand out
            if (game_paused!=true)
            {
                spriteBatch.Draw(button_cancel, new Vector2(vars.button_down.x, vars.button_down.y), Color.White * 0.9f);
                spriteBatch.Draw(button_up_texture, new Vector2(vars.button_up.x, vars.button_up.y), Color.White * 0.9f);
                spriteBatch.Draw(button_right_texture, new Vector2(vars.button_right.x, vars.button_right.y), Color.White * 0.9f);
                spriteBatch.Draw(button_left_texture, new Vector2(vars.button_left.x, vars.button_left.y), Color.White * 0.9f);
            }

            // draw logo
            //spriteBatch.DrawString(myFont, "(C)2014-2015 Neotronic Studios", new Vector2(41, ScreenHeight - 61), Color.Black);
            //spriteBatch.DrawString(myFont, "(C)2014-2015 Neotronic Studios", new Vector2(40, ScreenHeight - 60), Color.White);

            if (game_paused)
            {
                //draw faded box, and big paused text
                spriteBatch.Draw(whiteRectangle, new Rectangle(126, 126, ScreenWidth - 249, ScreenHeight - 249), Color.Black * 0.5f);
                spriteBatch.Draw(whiteRectangle, new Rectangle(125, 125, ScreenWidth - 250, ScreenHeight - 250), Color.Silver * 0.5f);
                if (level>1)
                {
                    x = Convert.ToInt32(ScreenWidth / 2) - Convert.ToInt32(titleFont.MeasureString("Continue").Length() / 2);
                    spriteBatch.DrawString(titleFont, "Continue", new Vector2(x+2, ScreenHeight - 298), Color.Black);
                    spriteBatch.DrawString(titleFont, "Continue", new Vector2(x, ScreenHeight - 300), Color.White);
                }
                else
                {
                    x = Convert.ToInt32(ScreenWidth / 2) - Convert.ToInt32(titleFont.MeasureString("Start Game").Length() / 2);
                    spriteBatch.DrawString(titleFont, "Start Game", new Vector2(x+2, ScreenHeight - 298), Color.Black);
                    spriteBatch.DrawString(titleFont, "Start Game", new Vector2(x, ScreenHeight - 300), Color.White);
                }
                
            }

            // end drawing menu
            spriteBatch.End();
            base.Draw(gameTime);
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }

    }
}
