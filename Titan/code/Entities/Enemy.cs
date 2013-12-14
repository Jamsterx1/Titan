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
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace Titan
{
    public class Enemy : Entity
    {
        public enum AIState
        {
            Idle = 0,
            Patrol,
            Chase
        };

        public uint     mHealth;
        public AIState  mState;
        public Entity   mTarget;
        public Vector2f mMovement;

        public  Random    rand   = new Random();
        private Stopwatch mTimer = new Stopwatch();
        private Stopwatch mDie = new Stopwatch();
        private GameWorld mWorld;

        public Enemy(Vector2f _position, String _file, Entity _target, GameWorld _world, uint _layer = 2)
        {
            create(_position, _file, _layer);
            mTimer.Start();
            mDie.Start();

            mWorld      = _world;
            mEntityType = EntityType.Enemy;
            mState      = AIState.Chase;
            mTarget     = _target;
            mMovement   = new Vector2f();
            mHealth     = 3;
        }

        public override void createBody(World _physics, BodyType _type)
        {
            base.createBody(_physics, _type);
            mBody.FixedRotation = true;
            mBody.IgnoreCollisionWith(mTarget.mBody);
            mBody.OnCollision += collision;
            mBody.CollisionCategories = Category.Cat2;
            mBody.CollidesWith = Category.All &~ Category.Cat1;
        }

        public override void update(RenderWindow _window)
        {
            //base.update(_window);
            if (mHealth == 0)
                destroy();

            if (mState == AIState.Chase)
                chase();

            if (mDie.ElapsedMilliseconds > 30000)
            {
                mWorld.createBullet(mPosition, this, "resources/fire.png", 0);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 45);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 90);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 135);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 180);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 225);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 275);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 315);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 0);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 12);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 33);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 135);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 25);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 200);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 128);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 357);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 23);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 368);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 54);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 262);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 231);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 123);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 324);
                destroy();
            }

            if (mTimer.ElapsedMilliseconds > 2000)
            {
                mWorld.createBullet(mPosition, this, "resources/fire.png", 0);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 45);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 90);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 135);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 180);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 225);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 275);
                mWorld.createBullet(mPosition, this, "resources/fire.png", 315);
                mTimer.Restart();
            }

            addPosition(mMovement.X, mMovement.Y);
            mBody.Position = ConvertUnits.ToSimUnits(mPosition.X, mPosition.Y);
        }

        public override void render(RenderWindow _window)
        {
            //_window.Draw(mSight);
            base.render(_window);
        }

        public override bool collision(Fixture f1, Fixture f2, Contact contact)
        {
            hit();
            return true;
        }

        public void chase()
        {
            double dx   = mTarget.mPosition.X - mPosition.X;
            double dy   = mTarget.mPosition.Y - mPosition.Y;
            double angle = Math.Atan2(dy, dx);

            mMovement.X = (float)((0.5f * Math.Cos(angle)) * Delta.mDelta);
            mMovement.Y = (float)((0.5f * Math.Sin(angle)) * Delta.mDelta);
        }

        public void hit() { mHealth--; }
    }
}
