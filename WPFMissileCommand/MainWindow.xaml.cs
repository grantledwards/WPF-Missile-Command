using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Media;

namespace WPFMissileCommand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int groundY, numBases, numCities, difficulty,
            gameWidth, groundYint, missileDrawSize, 
            roundType, multiplier, roundCount, score, 
            flakPoints, cityPoints, gamePhase, 
            branchChance, newCityBenchmark;
        bool colorBool, planeMayhem, branchMayhem, 
            missileMayhem, limitedAmmo, paused, running;
        double planeWait, missileWait, planeGrowRate, 
            missileGrowRate,missileSpeed;
        long bigTimer;
        City[] cityArray;
        Base[] baseArray;
        int[] targets;
        LinkedList<Missile> missiles;
        LinkedList<Explosion> explosions;
        LinkedList<FalseExplosion> falseExplosions;
        LinkedList<Flak> flaks;
        LinkedList<Coord> clickList;
        LinkedList<Plane> planes;
        List<ScoreEntry> scores;
        Random rand;

        public MainWindow()
        {
            limitedAmmo = true;
            rand = new Random();
            InitializeComponent();
        }

        /*
         GAME LOGIC
         ************************************************************
         */
        private int newRand(int mod)
        { return rand.Next() % mod; }
        public void beginGame()
        {
            PrepareGame();
        }

        public void PrepareGame()
        {
            gameWidth = 650;
            groundY = 420;
          //  planeMayhem = false;
          //  branchMayhem = false;
          //  missileMayhem = false;
          //  limitedAmmo = true;
            planeWait = 0;
            missileWait = 1;
            planeGrowRate = 0.001;
            missileGrowRate = 0.001;
            missileDrawSize = 4;


            


            openingLabel.Visibility = Visibility.Hidden;
            pauseButton.Visibility = Visibility.Visible;
            gameCanvas.Cursor = Cursors.Cross;

            newCityBenchmark = 10000;

            score = 0;

            missiles = new LinkedList<Missile>();
            explosions = new LinkedList<Explosion>();
            falseExplosions = new LinkedList<FalseExplosion>();
            clickList = new LinkedList<Coord>();
            flaks = new LinkedList<Flak>();
            planes = new LinkedList<Plane>();
            if (scores == null)
            {
                scores = new List<ScoreEntry>();
                scores.Add(new ScoreEntry(20330, "GRE"));
                scores.Add(new ScoreEntry(13275, "BRE"));
                scores.Add(new ScoreEntry(5632, "CAN"));
                scores.Add(new ScoreEntry(5632, "LAR"));
                scores.Add(new ScoreEntry(1531, "ARC"));
                scores.Add(new ScoreEntry(798, "REN"));
                scores.Add(new ScoreEntry(17435, "BRD"));
                scores.Add(new ScoreEntry(20705, "DAV"));
            }

            cityArray = new City[numCities];
            baseArray = new Base[numBases];
            targets = new int[numBases + numCities];
            gamePhase = 0;
            roundType = 0;
            multiplier = 1;

            switch(numCities)
            {
                case 6:
                    cityArray[0] = new City((gameWidth / 4) - 60 + 20,  falseExplosions);
                    cityArray[1] = new City((gameWidth / 4) + 20, falseExplosions);
                    cityArray[2] = new City((gameWidth / 4) + 60 + 20, falseExplosions);

                    cityArray[3] = new City(((3 * gameWidth) / 4) - 60 - 20, falseExplosions);
                    cityArray[4] = new City(((3 * gameWidth) / 4)      - 20, falseExplosions);
                    cityArray[5] = new City(((3 * gameWidth) / 4) + 60 - 20, falseExplosions);
                    break;
                case 1:
                    cityArray[0] = new City(200, falseExplosions);
                    break;
                case 7:
                    int margin = 60;
                    int incor = 140;
                    cityArray[0] = new City((margin * 0) + incor, falseExplosions);
                    cityArray[1] = new City((margin * 1) + incor, falseExplosions);
                    cityArray[2] = new City((margin * 2) + incor, falseExplosions);
                    cityArray[3] = new City((margin * 3) + incor, falseExplosions);
                    cityArray[4] = new City((margin * 4) + incor, falseExplosions);
                    cityArray[5] = new City((margin * 5) + incor, falseExplosions);
                    cityArray[6] = new City((margin * 6) + incor, falseExplosions);
                    break;
                default:
                    break;
            }

            switch (numBases)
            {
                case 3:
                    baseArray[0] = new Base(50, falseExplosions);
                    baseArray[1] = new Base((gameWidth / 2), falseExplosions);
                    baseArray[2] = new Base(gameWidth - 50, falseExplosions);
                    break;
                
                case 1:
                    baseArray[0] = new Base(gameWidth - 200, falseExplosions);
                    break;
                case 2:
                    baseArray[0] = new Base(50, falseExplosions);
                    baseArray[1] = new Base(gameWidth - 50, falseExplosions);
                    break;
                default:
                    break;
            }

            int i;
            for( i = 0;i<cityArray.Length;i++)
                targets[i] = cityArray[i].GetX();
            for (int j = 0; j < baseArray.Length; i++, j++)
                targets[i] = baseArray[j].GetX();

            
            roundCount = 1;
            UpdateMissileSpeed();

            bigTimer = 0;
            paused = false;

            switch(difficulty)
            {
                case 1:
                    branchChance = 500;
                    break;
                case 2:
                    branchChance = 300;
                    break;
                case 3:
                    limitedAmmo = false;
                    branchChance = 400;
                    break;
                default:
                    branchChance = 500;
                    break;
            }
            
            MainGameLoop();
        }

        /*
         * This is the Main Game Loop. It controls the animation increment, and from here the Model method,
         * View method, and Controller method are called.
         */
        public async void MainGameLoop()
        {
            int frameWait = 40;
            double delta,end,start = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;

            ///model
            Model();
            ///view
            View();
            ///controller
            Controller();

            end = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;
            delta = frameWait - (end - start);
            if (delta>0)
                await Task.Delay((int)delta);
           
        
            if(running)
                MainGameLoop();
        }

        /*
        MODEL
        ************************************************************
        */

      /*
       * This is the model method. It updates the positions of all animating
       * objects, tells them to check for collisions, handles game phases, 
       * tells missiles when to branch, and facilitates flickering colors
       */
        public void Model()
        {
            if (paused)
            {

            }
            else
            {
                switch (gamePhase)
                {
                    case 0:
                        GameRunning();
                        break;
                    case 1:
                        PostgameRunning();
                        break;
                    case 2:
                        IntermissionRunning();
                        break;
                    case 3:
                        GameOverRunning();
                        break;
                    case 4:
                        HighScoreRunning();
                        break;
                    default:
                        break;
                }
                
                colorBool = !colorBool;

                var deletableExplosions = explosions.ToList();
                foreach (Explosion E in deletableExplosions)
                {
                    if (!E.Update())
                    {
                        explosions.Remove(E);
                    }
                }

                var deletableFakeExplosions = falseExplosions.ToList();
                foreach (FalseExplosion FE in deletableFakeExplosions)
                {
                    if (!FE.Update())
                    {
                        falseExplosions.Remove(FE);
                    }
                }

                int missileScore;
                var deletableMissiles = missiles.ToList();
                foreach (Missile M in deletableMissiles)
                {
                    if ((missileScore = M.Update()) >= 0)
                    {
                        missiles.Remove(M);
                        score += multiplier * missileScore;
                    }
                }

                int planeScore;
                var deletablePlanes = planes.ToList();
                foreach (Plane P in deletablePlanes)
                {
                    if ((planeScore = P.Update()) >= 0)
                    {
                        planes.Remove(P);
                        score += multiplier * planeScore;
                    }
                }

                var deletableFlak = flaks.ToList();
                foreach (Flak F in deletableFlak)
                {
                    if (!F.Update())
                    {
                        flaks.Remove(F);
                    }
                }

                foreach (City C in cityArray)
                    C.Update();

                foreach (Base B in baseArray)
                    B.Update();
                
                if (newRand(branchChance) == 0 && missiles.Count() != 0 && missiles.First().GetMyCoords()[1] < 300)
                {
                    BranchingMissileMaker(missiles.First().GetStartCoords(), missiles.First().GetMyCoords(), (newRand(1) + 2));
                   // if(missiles.First().GetMyCoords()[1] < 300)
                        missiles.RemoveFirst();
                }

                if (branchMayhem && missiles.Count() != 0 )
                {
                    BranchingMissileMaker(missiles.First().GetStartCoords(), missiles.First().GetMyCoords(), (newRand(1) + 2));
                    missiles.RemoveFirst();
                }
            }
        }

        /*
         * This is Game Phase 0. It runs every tick during the main game phase. Once the time is up 
         * for the round, it switches to game phase 1, and this method is not called every tick.
         * Each tick, this method calls MissileMaker method to create new missiles at the top of the 
         * screen
         */
        private void GameRunning()
        {
            //game round time
            if (bigTimer > GetRoundLength() && difficulty != 3)
                gamePhase = 1;
            
            MissileMaker();
            PlaneMaker();
            bigTimer++;
        }

        /*
          * This is Game Phase 1. It runs every tick during the post game phase. Once the last 
          * remaining missile is destroyed, it switches to game phase 2, and this method is no longer 
          * called every tick.
          * If all cities have been destroyed at the end of this method's lifetime, it changes the game
          * phase to 3 (game over phase) instead of game phase 2 (intermission)
          */
        private void PostgameRunning()
        {
            if (missiles.Count() == 0)
            {
                bool areAllCitiesDestroyed = true;

                foreach (City C in cityArray)
                    if (!C.IsDestroyed())
                    {
                        areAllCitiesDestroyed = false;
                        break;
                    }

                if (areAllCitiesDestroyed)
                {
                    bigTimer = 0;
                    gamePhase = 3;
                    StartLose();
                }
                else
                {
                    bigTimer = 0;
                    gamePhase = 2;
                    StartIntermission();
                }
            }
        }
        /*
          * This is Game Phase 2. It runs every tick during the intermission phase. Once the phase has 
          * lasted 200 ticks, it switches to game phase 0, and this method is no longer 
          * called every tick.
          */
        private void IntermissionRunning()
        {
            //intermission time
            if (bigTimer > 200)
            {
                gamePhase = 0;
                StartRound();
            }
            bigTimer++;
        }

        /*
         * This is Game Phase 3. It runs every tick during the game over phase. Once the phase has 
         * lasted 100 ticks, it switches to game phase 4, and this method is no longer 
         * called every tick.
         */
        private void GameOverRunning()
        {
            if (bigTimer > 100)
            {
                gamePhase = 4;
                StartScoreTime();
            }
            bigTimer++;
        }

        private void HighScoreRunning()
        {

        }


        private void StartScoreTime()
        {
            EnterNameWindow enterName = new EnterNameWindow(this);
            enterName.ShowDialog();
            string name = enterName.getName();
            if (!name.Equals(""))
            {
                ScoreEntry entry = new ScoreEntry(score, name.ToUpper());
                scores.Add(entry);
            }
            scores.Sort();
            scores.Reverse();
        }

        /*
         * This method is called at the beginning of each intermission phase. (game phase 2)
         * It counts the number of surviving cities and unspent missiles. It increments the score
         * after counting this. It starts the timer over at 0.
         */
        private void StartIntermission()
        {
            SoundPlayer SP = new SoundPlayer();
            SP.SoundLocation = "sounds/bloop.wav";
            SP.Play();

            roundType = newRand(3);

            flakPoints = cityPoints = 0;

            foreach (Base B in baseArray)
            {
                flakPoints += (B.GetAmmunition());
                B.SpendAll();
            }
            foreach (City C in cityArray)
                if (!C.IsDestroyed())
                    cityPoints += 1;

            score += multiplier * ((flakPoints * 5) + (cityPoints * 100));

            if (score >= newCityBenchmark)
                foreach(City Restore in cityArray)
                    if(Restore.IsDestroyed())
                    {
                        newCityBenchmark += 10000;
                        Restore.Restore();
                    }
            bigTimer = 0;
        }

        /*
         * This method is called at the beginning of each round. (game phase 0)
         * It resupplies each bunker with missiles, sets the timer to zero, increments
         * the round counter, and applies the multiplier for the level. It calls update 
         * missile speed, which is called once per round.
         */
        private void StartRound()
        {
            foreach (Base B in baseArray)
                B.Resupply();

            bigTimer = 0;
            roundCount++;
            multiplier = roundType + 1;

            UpdateMissileSpeed();
        }

        /*
         * This method is called at the end of the game instead of the intermission phase.
         * It is called just once. It opens a dialoge box to get the user's name, and creates a new 
         * entry on the scoreboard. 
         */
        private void StartLose() 
        {
            menuSettings.IsEnabled = true;
        }

        /*
         * When this method is called, it creates numMissiles new missiles which start at 
         * coord and target random targets. 
         */
        private void BranchingMissileMaker(int[] tailStartCoord, int[] coord,int numMissiles)
        {
            Tail T = new Tail(tailStartCoord[0], tailStartCoord[1], coord[0], coord[1]);
            Missile M;
                for (int i = 0; i < numMissiles; i++)
                {
                    int target = -1;
                    while (target == -1)
                        target = targets[newRand(targets.Length)];
                    M = new Missile( coord[0], coord[1], groundY, target, missileSpeed, explosions, falseExplosions);

                    if (T != null)
                        M.SetTail(T);
                    T = null;
                    missiles.AddLast(M);
                }
        }
        
        /*
         * When this method is called, it gives each plane a chance to make a missile and then makes it for them, 
         * and then either creates a new missile at the top of the screen or increments the counter that will
         * allow creating a missile
         */
        private void MissileMaker()
        {
            foreach(Plane P in planes)
            {
                int planetarget = -1;
                while (planetarget == -1)
                    planetarget = targets[newRand(targets.Length)];

                if(newRand(90)==0)
                    missiles.AddFirst(
                  new Missile(
                    P.GetMyCoords()[0],
                    P.GetMyCoords()[1],
                    groundY,
                    planetarget,
                    missileSpeed,
                    explosions,
                    falseExplosions)
                    );
            }
            
            missileWait += missileGrowRate;
            if(missileWait >= 1)
            {
                int target = -1;
                while(target == -1 )
                    target = targets[newRand(targets.Length)];
                
                missileWait--;
              //  missilesShot++;
                missiles.AddFirst(
                  new Missile(
                    newRand(gameWidth), 
                    0, 
                    groundY, 
                    target, 
                    missileSpeed, 
                    explosions,
                    falseExplosions)
                    );
            }
        }

        /*
         * This method either makes a plane or increments the counter for plane making.
         * When the counter reaches 1, a plane is made
         */
        private void PlaneMaker()
        {
            planeWait += planeGrowRate;
            if (planeWait >= 1)
            {
                int y = 100 + newRand(100);
                planeWait--;
                planes.AddFirst( new Plane(y,newRand(2),2.5,explosions) );
            }
        }

        
        

    /*
     * This method sets the number of ticks a round will last based on difficulty and round
     */
        private long GetRoundLength()
        {
            long ret = 700;
            switch (difficulty)
            {
                case 1:
                    ret = 300 + (roundCount * 20);
                    break;
                case 2:
                    ret = 300 + (roundCount * 30);
                    break;
                case 3:
                    //"constant" difficulty
                    ret = long.MaxValue;
                    break;
                default:
                    break;
            }
            return ret;
        }
        /*
         * This method sets the speed of missiles in the game to what's appropriate for 
         */
        private void UpdateMissileSpeed()
        {
            if (missileMayhem)
                missileGrowRate = 0.9;
            else
                missileGrowRate = 0.03;
            
            switch(difficulty)
            {
                case 1:
                    planeGrowRate = 0.005;
                    missileSpeed = (0.2 * roundCount) + 1.2;
                    break;
                case 2:
                    planeGrowRate = 0.007;
                    missileSpeed = (0.02 * Math.Pow(roundCount,2)) + (0.2 * roundCount) + 1.3;
                    break;
                case 3:
                    planeGrowRate = 0.006;
                    missileSpeed = 1.8;
                    break;
                default:
                    planeGrowRate = 0.005;
                    break;
            }
            if (planeMayhem)
                planeGrowRate = 0.1;
        }



        /*
         VIEW
         *************************************************************/
        /*
         * This is the view method. It is called from main game loop after model, and before controller.
         * It clears the canvas, calls DrawScenery, DrawGameElements, and DrawHUD, and then invalidates the canvas,
         * forcing a redraw. 
         */
        public void View()
        {
            gameCanvas.Children.Clear();

            DrawScenery();
            DrawGameElements();
            DrawCrosshairs();
            DrawHUD();
            
            gameCanvas.InvalidateVisual();
        }

        /*
         * This is the Draw Scenery method. It draws the ground. It then calls methods to draw cities and bases, and 
         * sends in the coordinates and state information from those subclasses so those methods don't have to access 
         * the base or city subclasses.
         * These scenery elements are drawn underneath all other elements, and is thus called first before DrawGameElements 
         * and DrawHUD.
         */
        private void DrawScenery()
        {
            Rectangle ground = new Rectangle();
            ground.Fill = getColor(2);
                //Brushes.Yellow;
            ground.Margin = new Thickness(0,groundY,0,0);
            ground.Width = gameWidth;
            ground.Height = 64;
            gameCanvas.Children.Add(ground);

            foreach (Base B in baseArray)
                DrawBase(B.GetX(), B.GetAmmunition());

            foreach (City C in cityArray)
                DrawCity(C.GetX(),groundY,4, C.IsDestroyed());
        }

        

        // super method that calls for drawing of missiles, planes, Flaks, explosions, and false explosions
        private void DrawGameElements()
        {
            int[] from, to;

            foreach (Missile M in missiles)
            {
                from = M.GetStartCoords();
                to = M.GetMyCoords();
                DrawMissile(from[0], from[1], to[0], to[1], 0);
                DrawTail(M.GetTail());
            }
            
            foreach (Plane P in planes)
            {
                to = P.GetMyCoords();
                DrawPlane(to[0], to[1], P.GetPlaneType(), P.GetDirection());
            }

            foreach (Flak F in flaks)
            {
                from = F.GetStartCoords();
                to = F.GetMyCoords();
                DrawMissile(from[0], from[1], to[0], to[1], 1);
            }

            foreach (Explosion E in explosions)
            {
                to = E.getCoords();
                DrawExplosion(to[0], to[1], E.getSize());
            }

            foreach (FalseExplosion FE in falseExplosions)
            {
                to = FE.getCoords();
                DrawExplosion(to[0], to[1], FE.getSize());
            }
        }

        

        //Draw Heads up Display. Draws all labels, scores, intermission information, etc.
        private void DrawHUD()
        {
            if(gamePhase == 3)
            {
                Label gameover = new Label();
                gameover.Margin = new Thickness(230, 200, 0, 0);
                gameover.Content = "THE END";
                gameover.FontSize = 30;
                gameover.FontFamily = new FontFamily("Segoe WP Black");
                gameover.Foreground = getColor(0);
                //Brushes.Red;
                gameover.FontWeight = FontWeights.Bold;
                gameCanvas.Children.Add(gameover);

                Label gameOverScore = new Label();
                gameOverScore.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                gameOverScore.Margin = new Thickness(230, 250, 0, 0);
                gameOverScore.Content = "SCORE        " + score;
                gameOverScore.FontSize = 20;
                gameOverScore.FontFamily = new FontFamily("Segoe WP Black");
                gameOverScore.Foreground = getColor(0);
                //Brushes.Red;
                gameOverScore.FontWeight = FontWeights.Bold;
                gameCanvas.Children.Add(gameOverScore);

                return;
            }

            if (gamePhase == 4)
            {
                Label scoreLabel = new Label();
                Label scoreHeaderLabel = new Label();
                scoreHeaderLabel.Margin = new Thickness(190, 50, 0, 0);
                scoreLabel.Margin = new Thickness(200, 60, 0, 0);
                scoreHeaderLabel.Content = "HIGH SCORES";
                scoreLabel.Content = "";
                int l = 0;
                foreach (ScoreEntry SC in scores)
                    if(l<15)
                    {
                        scoreLabel.Content += Environment.NewLine + SC.Get();
                        l++;
                    }
                scoreHeaderLabel.FontSize = 20;
                scoreLabel.FontSize = 15;
                scoreLabel.FontFamily = new FontFamily("Segoe WP Black");
                scoreHeaderLabel.FontFamily = new FontFamily("Segoe WP Black");
                scoreLabel.Foreground = getColor(0);
                scoreHeaderLabel.Foreground = getColor(1);

                scoreLabel.FontWeight = FontWeights.Bold;
                scoreHeaderLabel.FontWeight = FontWeights.Bold;
                

                gameCanvas.Children.Add(scoreLabel);
                gameCanvas.Children.Add(scoreHeaderLabel);
            }
            
            //round counter
            if (gamePhase==0 || gamePhase==1 || missiles.Count() > 0)
            {
                Label round = new Label();
                round.Margin = new Thickness(30, 430, 0, 0);
                if (difficulty != 3)
                    round.Content = "ROUND " + roundCount;
                else
                    round.Content = "ROUND ∞";
                round.FontSize = 20;
                round.FontFamily = new FontFamily("Segoe WP Black");
                round.Foreground = getColor(1);
                //Brushes.Black;
                round.FontWeight = FontWeights.Bold;
                gameCanvas.Children.Add(round);
            }

            if (gamePhase == 2 && missiles.Count() == 0)
            {
                Label bonusLabel = new Label();
                bonusLabel.Margin = new Thickness(200, 100, 0, 0);
                bonusLabel.Content = "BONUS POINTS";
                bonusLabel.FontSize = 20;
                bonusLabel.FontFamily = new FontFamily("Segoe WP Black");
                bonusLabel.Foreground = getColor(1);
                //Brushes.Blue;
                bonusLabel.FontWeight = FontWeights.Bold;
                gameCanvas.Children.Add(bonusLabel);


                Label missilePointsLabel = new Label();
                missilePointsLabel.Margin = new Thickness(210, 150, 0, 0);
                missilePointsLabel.Content = "" + (flakPoints*5);
                missilePointsLabel.FontSize = 20;
                missilePointsLabel.FontFamily = new FontFamily("Segoe WP Black");
                missilePointsLabel.Foreground = getColor(0);
      
                missilePointsLabel.FontWeight = FontWeights.Bold;

                gameCanvas.Children.Add(missilePointsLabel);
                
                for(int k = 0; k < flakPoints; k++)
                    DrawAmmo((270 + (k * 10)), 165);
                

                Label cityPointsLabel = new Label();
                cityPointsLabel.Margin = new Thickness(210, 200, 0, 0);
                cityPointsLabel.Content = "" + (cityPoints * 100);
                cityPointsLabel.FontSize = 20;
                cityPointsLabel.FontFamily = new FontFamily("Segoe WP Black");
                cityPointsLabel.Foreground = getColor(0);

                cityPointsLabel.FontWeight = FontWeights.Bold;

                gameCanvas.Children.Add(cityPointsLabel);

                Label multiplierLabel = new Label();
                multiplierLabel.Margin = new Thickness(210, 250, 0, 0);
                multiplierLabel.Content = (roundType + 1) + "   ✖   POINTS";
                multiplierLabel.FontSize = 20;
                multiplierLabel.FontFamily = new FontFamily("Segoe WP Black");
                multiplierLabel.Foreground = getColor(0);

                multiplierLabel.FontWeight = FontWeights.Bold;

                gameCanvas.Children.Add(multiplierLabel);

                int mCtr = 0;
                foreach (City miniC in cityArray)
                {
                    if (!miniC.IsDestroyed())
                    {
                        DrawCity((280 + (mCtr * 30)), 220,2, false);
                        mCtr++;
                    }
                }
            }

            Label myScoreLabel = new Label();
            myScoreLabel.Margin = new Thickness(375, 430, 0, 0);
            myScoreLabel.Content = "SCORE " + score;
            myScoreLabel.FontSize = 20;
            myScoreLabel.FontFamily = new FontFamily("Segoe WP Black");
            myScoreLabel.Foreground = getColor(1);

            myScoreLabel.FontWeight = FontWeights.Bold;

            gameCanvas.Children.Add(myScoreLabel);
        }

        //get color. returns a Brush. Important for round type. (1X score, 2X score, 3X score)
        private Brush getColor(int type)
        {
            switch (roundType)
            {
                case 0 :
                    switch(type)
                    {
                        case 0:
                            return Brushes.Red;//0,0
                        case 1:
                            return Brushes.Blue;
                        case 2:
                            return Brushes.Yellow;
                        case 3:
                            return Brushes.Cyan;
                        default:
                            return Brushes.Pink;
                    }
                case 1:
                    switch (type)
                    {
                        case 0:
                            return Brushes.Lime;//0,0
                        case 1:
                            return Brushes.Blue;
                        case 2:
                            return Brushes.Yellow;
                        case 3:
                            return Brushes.Cyan;
                        default:
                            return Brushes.Pink;
                    }
                case 2:
                    switch (type)
                    {
                        case 0:
                            return Brushes.Red;//0,0
                        case 1:
                            return Brushes.Lime;
                        case 2:
                            return Brushes.Blue;
                        case 3:
                            return Brushes.Yellow;
                        default:
                            return Brushes.Pink;
                    }
                default:
                    return Brushes.Pink;
            }
        }

        //draw missile (two lines)
        private void DrawMissile(int x1, int y1, int x2, int y2,int type)
        {
            Line line = new Line();
            Line missile = new Line();
            switch (type)
            {
                case 0:
                    line.StrokeThickness = 3;
                    line.Stroke = getColor(0);
                        //Brushes.Red;
                    break;
                case 1:
                    line.StrokeThickness = 1.3;
                    line.Stroke = getColor(1);
                    break;
                default:
                    line.StrokeThickness = 3;
                    line.Stroke = getColor(0);
                    break;
            }
            
            if (colorBool)
                missile.Stroke = Brushes.Red;
            else
                missile.Stroke = Brushes.Yellow;

            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;

            missile.X1 = x2;
            missile.X2 = x2;
            missile.Y1 = y2 + missileDrawSize / 2;
            missile.Y2 = y2 - missileDrawSize / 2;

            
            missile.StrokeThickness = missileDrawSize;

            gameCanvas.Children.Add(line);
            gameCanvas.Children.Add(missile);
        }

        //draw missile tail (one line)
        private void DrawTail(Tail tail)
        {
            if (tail == null)
                return;
            Line line = new Line();
            line.StrokeThickness = 3;
            line.Stroke = getColor(0);

          //  MessageBox.Show(tail.GetStartCoords()+ " "+tail.GetEndCoords());


            line.X1 = tail.GetStartCoords()[0];
            line.X2 = tail.GetEndCoords()[0];
            line.Y1 = tail.GetStartCoords()[1];
            line.Y2 = tail.GetEndCoords()[1];
            
            gameCanvas.Children.Add(line);
        }
        //draw explosion (ellipse)
        public void DrawExplosion(int x, int y, int radius)
        {
            Ellipse circle = new Ellipse { Width = radius * 2, Height = radius * 2 };
            double left = x - radius;
            double top = y - radius;

            circle.Margin = new Thickness(left, top, 0, 0);

            if (colorBool)
                circle.Fill = new SolidColorBrush(Colors.Red);
            else
                circle.Fill = new SolidColorBrush(Colors.Yellow);


            circle.StrokeThickness = 0;

            gameCanvas.Children.Add(circle);
        }

        //draw city (12 lines)
        public void DrawCity(int x, int y,int size,bool destroyed)
        {
             y += size;

            Line[] line = new Line[12];


            for (int i = 0; i < 12; i++)
            {
                line[i] = new Line();
                if (destroyed)
                    line[i].Stroke = getColor(2);
                else
                    line[i].Stroke = getColor(3);
                line[i].StrokeThickness = size;
                line[i].X1 = (i * size) - (6 * size);
                line[i].X2 = (i * size) - (6 * size);
                line[i].Y1 = y;
                line[i].Y2 = y - (size * 1);
                line[i].Margin = new Thickness(x, 0, 0, 0);

                gameCanvas.Children.Add(line[i]);
            }
            if (destroyed)
            {
                line[1].Y2 = line[2].Y2 = line[3].Y2 =
                    line[4].Y2 = line[5].Y2 = line[6].Y2 =
                    line[7].Y2 = line[8].Y2 = line[9].Y2 =
                    line[10].Y2 = y - (size * 2);
            }
            else
            {
                line[0].Y2 = y - (size * 2);
                line[1].Y2 = y - (size * 3);
                line[2].Y2 = y - (size * 5);
                line[3].Y2 = y - (size * 2);
                line[4].Y2 = y - (size * 4);
                line[5].Y2 = y - (size * 2);
                line[6].Y2 = y - (size * 1);
                line[7].Y2 = y - (size * 3);
                line[8].Y2 = y - (size * 2);
                line[9].Y2 = y - (size * 3);
                line[10].Y2 = y - (size * 2);
                line[11].Y2 = y - (size * 1);
            }
        }
        //draw bunker (trapezoid)
        public void DrawBase(int x, int ammo)
        {
            Polygon hill = new Polygon();
            hill.Fill = getColor(2);
            hill.Points = new PointCollection{
                new Point((x-45),(groundY)), 
                new Point((x-15),groundY-30),
                new Point((x+15),groundY-30),
                new Point((x+45),(groundY)) };

            hill.StrokeThickness = 0;

            gameCanvas.Children.Add(hill);
            
            for (int i = 1; i <= 4; i++)
                for (int j = 0; j < i; j++)
                    if (ammo > 0)
                    {
                        DrawAmmo((j * 16 - ((i - 1) * 16) / 2) + x, (groundY - 30 + i * 8));
                        ammo--;
                    }
        }

        //draw plane (two polygons, or one polygon and two lines)
        public void DrawPlane(int x,int y,int planeType,bool LoR)
        {
            Line eyeLeft = new Line();
            Line eyeRigt = new Line();
            Polygon body = new Polygon();
            body.Fill = getColor(0);

            if (planeType == 0)
            {
                Polygon wings = new Polygon();
                wings.Fill = getColor(0);

                if(LoR)
                {
                    wings.Points = new PointCollection{
                    new Point((0),(0)),
                    new Point((-3),(10)),
                    new Point((5),0),
                    new Point((-3),-10)};
                    body.Points = new PointCollection{
                    new Point((-7),(0)),
                    new Point((-10),(3)),
                    new Point((10),0),
                    new Point((-10),-3)};
                }
                else
                {
                    wings.Points = new PointCollection{
                    new Point((0),(0)),
                    new Point((3),(10)),
                    new Point((-5),0),
                    new Point((3),-10)};
                    body.Points = new PointCollection{
                    new Point((7),(0)),
                    new Point((10),(3)),
                    new Point((-10),0),
                    new Point((10),-3)};
                }

                wings.Margin = new Thickness(x, y, 0, 0);
            
                wings.StrokeThickness = 0;
                gameCanvas.Children.Add(wings);

            }
            else
            {
                body.Points = new PointCollection{
                    new Point((-5),(0)),
                    new Point((-10),(10)),
                    new Point((0),5),
                    new Point((10),(10)),
                    new Point((5),0),
                    new Point((10),(-10)),
                    new Point((0),(-5)),
                    new Point((-10),(-10))};

                eyeLeft.Margin = eyeRigt.Margin = new Thickness(x, y, 0, 0);
                eyeLeft.StrokeThickness = eyeRigt.StrokeThickness = 2;
                eyeLeft.Y1 = eyeRigt.Y1 = -3;
                eyeLeft.Y2 = eyeRigt.Y2 = 3;
                eyeLeft.X1 = eyeLeft.X2 = -3;
                eyeRigt.X1 = eyeRigt.X2 = 3;
                eyeLeft.Stroke = eyeRigt.Stroke = getColor(1);
            }
            body.StrokeThickness = 0;
            body.Margin = new Thickness(x, y, 0, 0);

            gameCanvas.Children.Add(body);
            gameCanvas.Children.Add(eyeLeft);
            gameCanvas.Children.Add(eyeRigt);
        }

        //draw bunker ammo (three lines)
        private void DrawAmmo(int x, int y)
        {
            int linesize = 2;

            Line left = new Line();
            Line midl = new Line();
            Line rigt = new Line();

            left.StrokeThickness =
                midl.StrokeThickness =
                rigt.StrokeThickness = linesize;
            left.Stroke =
                midl.Stroke =
                rigt.Stroke = getColor(1);
                //Brushes.RoyalBlue;

            midl.X1 = x;
            midl.X2 = x;
            midl.Y1 = y - (linesize);
            midl.Y2 = y + (linesize * 2);

            left.X1 = x - linesize;
            left.X2 = x - linesize;
            rigt.X1 = x + linesize;
            rigt.X2 = x + linesize;
            rigt.Y1 = left.Y1 = y + (linesize * 1);
            rigt.Y2 = left.Y2 = y + (linesize * 3);

            gameCanvas.Children.Add(left);
            gameCanvas.Children.Add(midl);
            gameCanvas.Children.Add(rigt);
        }

        //draw crosshairs (4 lines)
        private void DrawCrosshairs()
        {
            double linesize = 1.1;
            int outBound = 8, inBound = 6;

            Line topleft = new Line();
            Line topright = new Line();
            Line bottomright = new Line();
            Line bottomleft = new Line();

            topleft.StrokeThickness =
                topright.StrokeThickness =
                bottomright.StrokeThickness =
                bottomleft.StrokeThickness = linesize;
            
            topleft.Stroke =
                topright.Stroke =
                bottomright.Stroke =
                bottomleft.Stroke = Brushes.White;

            topleft.X1 =
                bottomleft.X1 = -outBound;
            topright.X1 =
                bottomright.X1 = outBound;

            bottomright.Y1 =
                bottomleft.Y1 = -outBound;
            topright.Y1 =
                topleft.Y1 = outBound;

            topleft.X2 =
                bottomleft.X2 = -inBound;
            topright.X2 =
                bottomright.X2 = inBound;

            topleft.Y2 =
                topright.Y2 = inBound;
            bottomleft.Y2 =
                bottomright.Y2 = -inBound;

            double y = Mouse.GetPosition(gameCanvas).Y;
            if (y > groundY - 30)
                y = groundY - 30;
            topleft.Margin =
                topright.Margin =
                bottomleft.Margin =
                bottomright.Margin = new Thickness(Mouse.GetPosition(gameCanvas).X, y, 0, 0);

            gameCanvas.Children.Add(topleft);
            gameCanvas.Children.Add(topright);
            gameCanvas.Children.Add(bottomright);
            gameCanvas.Children.Add(bottomleft);
        }

        /*
         CONTROLLER
         *************************************************************/

        /*
         * Clicking inside the game Canvas appends a Coord to a linked list.
         * When called, the controller removes each element of that list and attempts
         * to create a Flak aimed at that coordinate.
         */
        public void Controller()
        {
            int basePickerIndex;
            Coord Ccopy;

            var referenceClicks = clickList.ToList();
            foreach (Coord C in referenceClicks)
            {
                if(gamePhase == 0 || gamePhase == 1)
                {
                    Ccopy = C;
                    if (C.Get()[1] > groundY - 30)
                        Ccopy = new Coord(C.Get()[0], groundY - 30);
                    basePickerIndex = newRand(numBases);
                    CreateFlak(basePickerIndex,Ccopy,-1);
                    
                }
                clickList.Remove(C);
            }
        }

        /*
         * creates a user-made missile, called "Flak"
         */
        private void CreateFlak(int baseIndex, Coord C, int iterIndex)
        {
            if (baseIndex >= baseArray.Length)
                return;
            if (!limitedAmmo)
                flaks.AddFirst(new Flak(baseArray[baseIndex].GetX(), (groundY - 30), C.Get()[0], C.Get()[1], 30, explosions));
            else
                if (baseArray[baseIndex].Spend())
                flaks.AddFirst(new Flak(baseArray[baseIndex].GetX(), (groundY - 30), C.Get()[0], C.Get()[1], 30, explosions));
            else
                CreateFlak(iterIndex + 1, C, iterIndex + 1);
        }

        /*
         GAMELOGIC SUBCLASSES
         *************************************************************/
         
       /*
        * SCORE ENTRY CLASS - stores a single high score entry
        */
        public class ScoreEntry : IComparable<ScoreEntry>
        {
            public int score;
            private string name;
            public ScoreEntry(int v, string s)
            {
                score = v;
                name = s;
            }
            public string Get()
            { return (name + "\t-\t" + score); }

            public int CompareTo(ScoreEntry that)
            {
                return this.score.CompareTo(that.score);
            }
        }

        /*
        * COORD CLASS - stores a single x,y coordinate
        */
        public class Coord
        {
            int xCoord,yCoord;
            public Coord(int x,int y)
            {
                xCoord = x;
                yCoord = y;
            }
            public int[] Get()
            {  return new int[]{xCoord,yCoord};  }
        }

        /*
         EVENTS AND ACCESSORS
         *************************************************************/


    /*
     * used for input from the "new game" subform. It sets the game's number of bases,
     * number of cities, and difficulty
     */
        public void SetGameParams(int nB, int nC, int d)
        {
            numBases = nB;
            numCities = nC;
            difficulty = d;

            running = true;
            //MessageBox.Show(numBases + " " + numCities + " " + difficulty);
        }

    /*
     * used for input from the "settings" subform. It sets the game's funky cheat modes like limitless ammo
     * and extra missiles
     */
        public void SetGameSettings(bool BM,bool MM,bool UA, bool PM)
        {
            limitedAmmo = !UA;
            missileMayhem = MM;
            planeMayhem = PM;
            branchMayhem = BM;
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            CloseProgram();
        }
        
        private void menuNewGame_Click(object sender, RoutedEventArgs e)
        {
            menuSettings.IsEnabled = false;
            running = false;
            DisplayNewGameMessage();

            if(running)
                menuSettings.IsEnabled = false;
            else
                menuSettings.IsEnabled = true;

        }
        
        private void menuSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Settings(this);

            settings.ShowDialog();
        }

        private void DisplayNewGameMessage()
        {
            var view = new Window1(this);

            view.ShowDialog();
        }

        private void menuHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("BASIC PLAY:" + Environment.NewLine +
                "Game > New Game > default > BEGIN" + Environment.NewLine +Environment.NewLine +
                "Game > Settings for game cheats" + Environment.NewLine +
                "Game > Exit to close the game" + Environment.NewLine +
                "About > About for information on the game" + Environment.NewLine +
                "Names for high scores must be 3 letters" );
        }

       

        private void menuAboutSub_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Missile Command by Grant Edwards" + Environment.NewLine +
                "3/23/2017        .NET 4.6");
        }

        private void CloseProgram()
        {
            this.Close();
        }

        /*
         * appends a new coord to the linked list of click coordinates
         */
        private void gameCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((gamePhase == 0 || gamePhase == 1 ) && !paused)
            {
                int x = (int)Mouse.GetPosition(gameCanvas).X;
                int y = (int)Mouse.GetPosition(gameCanvas).Y;

                if(x>=0 && y>=0)
                    clickList.AddLast(new Coord(x,y));
            }
        }

        /*
         * pause button
         */
        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer SP = new SoundPlayer();
            SP.SoundLocation = "sounds/bloop.wav";
            SP.Play();

            if (paused)
                pauseButton.Content = "Pause";
            else
                pauseButton.Content = "Play";
            paused = !paused;
        }
    }
}
