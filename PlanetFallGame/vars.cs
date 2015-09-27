using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetFall
{
    class vars
    {

        //cross page stuff
        public static bool arewepaused = false;
        public static bool openfeedback = false;


        // General Settings
        public bool sound_enabled;

        // Shared Game Variables
        public int current_level;
        public int level_max = 16;


        // 1=moon, 2=mars, 3=asteroid, 4=crazy planet
        public static int[] level_types = new int[] { 1, 1, 1, 2, 3, 4, 1, 2, 3, 4, 3, 4, 3, 4, 3, 1 };
        public static double[] gravity_values = new double[] { 0.98, 0.6, 0.3, 1.4 };

        public class _bounties
        {
            public int x { get; set; }
            public int y { get; set; }
            public bool found { get; set; }
        }

        public class _asteroids
        {
            public bool active { get; set; }
            public bool inplay { get; set; }
            public double x { get; set; }
            public double y { get; set; }
            public double velocity_x { get; set; }
            public double velocity_y { get; set; }
            public bool destroyed { get; set; }
            public float rotation { get; set; }
            public int timer { get; set; }
        }


        public class _myrect
        {
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; }
            public int h { get; set; }
        }

        public static _myrect button_left;
        public static _myrect button_right;
        public static _myrect button_up;
        public static _myrect button_down;

        public static _myrect[] controlbuttons;


    }
}
