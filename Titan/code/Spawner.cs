using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

/* Other libs */
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace Titan
{
    public class Spawner
    {
        public Vector2f  mPosition;
        public Player    mPlayer;
        public World     mPhysicsWorld;
        public GameWorld mWorld;

        public Stopwatch mTimer;
        public uint      mCount;
        public Random    mRandom;

        public Spawner(float _x, float _y, World _physics = null, GameWorld _world = null, Player _target = null)
        {
            mPosition.X   = _x;
            mPosition.Y   = _y;
            mPlayer       = _target;
            mPhysicsWorld = _physics;
            mWorld        = _world;
            mCount        = 0;

            mTimer = new Stopwatch();
            mRandom = new Random();
            mTimer.Start();
        }

        public void update()
        {
            if (mTimer.ElapsedMilliseconds >= 8000f)
            {
                for(int i = 0; i < mCount; i++)
                    mWorld.add(spawn());

                mCount += 1;
                mTimer.Restart();
            }
        }

        public Entity spawn()
        {
            Vector2f newPos = new Vector2f();
            int randX = mRandom.Next(-500, 501);
            int randY = mRandom.Next(-300, 301);

            newPos.X = mPosition.X + randX;
            newPos.Y = mPosition.Y + randY;

            Enemy enemy = new Enemy(newPos, "../../resources/enemy.png", mPlayer);
            enemy.createBody(mPhysicsWorld, BodyType.Dynamic);
            return enemy;
        }

        public void create(Player _target, GameWorld _world, World _physics)
        {
            mPlayer = _target;
            mPhysicsWorld = _physics;
            mWorld = _world;
            mWorld.add(spawn());
        }
    }
}
