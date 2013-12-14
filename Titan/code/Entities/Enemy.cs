using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Random rand = new Random();

        public Enemy(Vector2f _position, String _file, Entity _target, uint _layer = 2)
        {
            create(_position, _file, _layer);
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
        }

        public override void update(RenderWindow _window)
        {
            //base.update(_window);
            if (mHealth == 0)
                destroy();

            if (mState == AIState.Chase)
                chase();

            if (ConvertUnits.ToDisplayUnits(mBody.Position.Y) > 800)
                mBody.Position = ConvertUnits.ToSimUnits(300f, 680f);

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
