using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WindowsPhone;
using PlanetFall.Resources;

namespace PlanetFall
{
    public partial class GamePage : PhoneApplicationPage
    {
        private Game1 _game;

        // Constructor
        public GamePage()
        {
            InitializeComponent();
            _game = XamlGame<Game1>.Create("", this);

            //kick off timer to check for advert changes
            DispatcherTimerSetup();
        }

        DispatcherTimer dispatcherTimer;
        //int timesTicked = 1;
        //int timesToTick = 5;

        void dispatcherTimer_Tick(object sender, object e)
        {
            if (vars.arewepaused == false)
            {
                adc1.Visibility = Visibility.Visible;
                adDuplexAd.Visibility = Visibility.Collapsed;
            } else
            {
                adc1.Visibility = Visibility.Collapsed;
                adDuplexAd.Visibility = Visibility.Visible;
            }
            if (vars.openfeedback == true)
            {
                Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=fa29eb18-2b31-46a9-ae4c-b783ffb273dc"));
                vars.openfeedback = false;
            }
            //if (timesTicked > timesToTick)
            //{
            //    //dispatcherTimer.Stop();
            //}
            //timesTicked++;
        }
        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        
    }
}