using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Titan;

/* SFML */
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;

/* Other libraries */
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace Titan
{
    public class GameWorld
    {
        public enum GameState
        {
            START = 0,
            GAME,
            END
        };

        private static readonly Color CornflowerBlue = new Color(100, 149, 237);
        public List<Entity> mEntities;
        public List<Enemy> mEnemies;
        public World mPhysics;

        public Spawner mSpawner = new Spawner(640f, 360f);
        public Player  mPlayerRef;
        public bool    mWin;

        public Stopwatch mCountdown = new Stopwatch();
        public bool mBossSpawned    = false;
        private static Font mFont   = new Font("resources/Doodle.ttf");
        public Text mTimeText       = new Text("10:00", mFont, 36);
        private Boss mBoss;

        public Text mStartInfo = new Text("TITAN\n\n-- Controls --\nAD to move\nSPACE to jump\nEnter for invincibility (one time use)\n\nSpace to begin", mFont, 20);
        public Text mWinText = new Text("You WIN!", mFont, 40);
        public Text mLoseText = new Text("You LOSE!", mFont, 40);

        public Music mMusic;
        public GameState mGameState;

        public GameWorld()
        {
            mEntities = new List<Entity>();
            mEnemies  = new List<Enemy>();
            mPhysics  = new World(new Vector2(0f, 0.0981f));
            mWin      = true;

            mTimeText.Position  = new Vector2f(580f, 670f);
            mStartInfo.Position = new Vector2f(300f, 300f);
            mWinText.Position   = new Vector2f(400f, 200f);
            mLoseText.Position  = new Vector2f(400f, 200f);
            //mStartInfo.Color    = new Color(255, 0, 0);

            ConvertUnits.SetDisplayUnitToSimUnitRatio(8f);
            Delta.create();
            mCountdown.Start();
            mGameState = GameState.START;

            mMusic        = new Music("resources/sound/theme.wav");
            mMusic.Loop   = true;
            mMusic.Volume = 60f;
            mMusic.Play();
        }

        public void update(RenderWindow _window)
        {
            if (mGameState == GameState.START)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
                    mGameState = GameState.GAME;
            }
            if(mGameState == GameState.GAME)
            {
                // Pre-Update
                Delta.update();
                mPhysics.Step(Delta.mDelta);

                if (mCountdown.ElapsedMilliseconds < 120000 && mEntities.Count < 100 && !mBossSpawned)
                {
                    mSpawner.update();
                    int elapsed = (int)((120000 - mCountdown.ElapsedMilliseconds) / 1000f);
                    int mins = elapsed / 60;
                    int seconds = elapsed - (mins * 60);

                    String timeText = mins + ":" + seconds;
                    mTimeText.DisplayedString = timeText;
                }
                else if (mCountdown.ElapsedMilliseconds > 120000)
                {
                    /*if (!mBossSpawned)
                    {
                        // Spawn boss
                        destroyEnemies();
                        mCountdown.Reset();
                        mBossSpawned = true;

                        mBoss = new Boss(new Vector2f(440f, 310f), "resources/boss.png", mPlayerRef, this, 0);
                        mBoss.createBody(mPhysics, BodyType.Dynamic);
                        mEntities.Add(mBoss);
                        sort();
                    }*/
                    mGameState = GameState.END;
                }

                // World Update
                for (int i = 0; i < mEntities.Count; i++)
                {
                    mEntities[i].update(_window);
                    if (mEntities[i].isDead())
                    {
                        mEntities[i] = null;
                        mEntities.RemoveAt(i);
                    }
                }

                for (int i = 0; i < mEnemies.Count; i++)
                {
                    mEnemies[i].update(_window);
                    if (mEnemies[i].isDead())
                    {
                        mEnemies[i] = null;
                        mEnemies.RemoveAt(i);
                    }
                }
            }
        }

        public void render(RenderWindow _window)
        {
            _window.Clear(CornflowerBlue);
            if (mEntities.Count > 0)
            {
                for (int i = 0; i < mEntities.Count; i++)
                    mEntities[i].render(_window);
            }

            if (mEnemies.Count > 0)
            {
                for (int i = 0; i < mEnemies.Count; i++)
                    mEnemies[i].render(_window);
            }
            _window.Draw(mTimeText);

            if (mGameState == GameState.START)
                _window.Draw(mStartInfo);
            else if (mGameState == GameState.END)
            {
                if (mWin)
                    _window.Draw(mWinText);
                else
                    _window.Draw(mLoseText);
            }
        }

        public void add(Entity _entity)
        {
            mEntities.Add(_entity);
            Console.WriteLine("Entity " + _entity + " added to world");
        }

        public void addEnemy(Enemy _enemy)
        {
            mEnemies.Add(_enemy);
        }

        public void addBatch(List<Entity> _entities)
        {
            for (int i = 0; i < _entities.Count; i++)
                mEntities.Add(_entities[i]);
        }

        public void destroyEnemies()
        {
            for (int i = 0; i < mEnemies.Count; i++)
                mEnemies[i].destroy();
        }

        public void sort()
        {
            mEntities.Sort
            (
                delegate(Entity entity1, Entity entity2)
                {
                    return entity1.mLayer.CompareTo(entity2.mLayer);
                }
            );
        }

        public void createBullet(Vector2f _position, Entity _ignore, String _file, double angle)
        {
            Bullet bullet = new Bullet(_position, new Vector2f((float)Math.Cos(angle) * (10f * Delta.mDelta), (float)Math.Sin(angle) * (10f * Delta.mDelta)), _file);
            bullet.createBody(mPhysics, BodyType.Dynamic);

            bullet.mBody.IgnoreCollisionWith(_ignore.mBody);
            mEntities.Add(bullet);
        }

        public void createWorld()
        {
            // Create all platforms, enemies, player and other stuff here
            Player player = new Player(new Vector2f(200f, 200f), "resources/player.png", this);
            player.createBody(mPhysics, BodyType.Dynamic);

            Entity floor1 = new Entity(new Vector2f(0f, 710f), "resources/floor.png");
            floor1.createBody(mPhysics, BodyType.Static);

            Entity floor2 = new Entity(new Vector2f(0f, 0f), "resources/floor.png");
            floor2.createBody(mPhysics, BodyType.Static);

            Entity wall1 = new Entity(new Vector2f(0f, 0f), "resources/wall.png");
            wall1.createBody(mPhysics, BodyType.Static);

            Entity wall2 = new Entity(new Vector2f(1270f, 0f), "resources/wall.png");
            wall2.createBody(mPhysics, BodyType.Static);

            Entity platform1 = new Entity(new Vector2f(100f, 650f), "resources/platform.png");
            platform1.createBody(mPhysics, BodyType.Static);
            Entity platform2 = new Entity(new Vector2f(500f, 600f), "resources/platform.png");
            platform2.createBody(mPhysics, BodyType.Static);
            Entity platform3 = new Entity(new Vector2f(180f, 160f), "resources/platform.png");
            platform3.createBody(mPhysics, BodyType.Static);
            Entity platform4 = new Entity(new Vector2f(800f, 200f), "resources/platform.png");
            platform4.createBody(mPhysics, BodyType.Static);
            Entity platform5 = new Entity(new Vector2f(900f, 580f), "resources/platform.png");
            platform5.createBody(mPhysics, BodyType.Static);
            Entity platform6 = new Entity(new Vector2f(1050f, 290f), "resources/platform.png");
            platform6.createBody(mPhysics, BodyType.Static);
            Entity platform7 = new Entity(new Vector2f(500f, 120f), "resources/platform.png");
            platform7.createBody(mPhysics, BodyType.Static);
            Entity platform8 = new Entity(new Vector2f(670f, 440f), "resources/platform.png");
            platform8.createBody(mPhysics, BodyType.Static);
            Entity platform9 = new Entity(new Vector2f(470f, 360f), "resources/platform.png");
            platform9.createBody(mPhysics, BodyType.Static);
            Entity platform10 = new Entity(new Vector2f(100f, 340f), "resources/platform.png");
            platform10.createBody(mPhysics, BodyType.Static);

            mEntities.Add(player);
            mEntities.Add(floor1);
            mEntities.Add(floor2);
            mEntities.Add(wall1);
            mEntities.Add(wall2);

            mEntities.Add(platform1);
            mEntities.Add(platform2);
            mEntities.Add(platform3);
            mEntities.Add(platform4);
            mEntities.Add(platform5);
            mEntities.Add(platform6);
            mEntities.Add(platform7);
            mEntities.Add(platform8);
            mEntities.Add(platform9);
            mEntities.Add(platform10);

            sort();
            mPlayerRef = player;
            mSpawner.create(player, this, mPhysics);
        }
    }
}
