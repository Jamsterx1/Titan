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
    public class Player : Entity
    {
        public uint     mHealth;
        public Sprite[] mHealthBar = new Sprite[11];
        public uint     mSegment;

        public bool       mInvicible;
        private bool      mOnce;
        private Stopwatch mTimer;
        private Stopwatch mShoot;

        private GameWorld  mWorld;

        public Player(Vector2f _position, String _file, GameWorld _world)
        {
            create(_position, _file, 1);
            mWorld      = _world;

            mEntityType = EntityType.Player;
            mHealth     = 100;
            mSegment    = 10;

            mInvicible = false;
            mOnce      = false;
            mTimer     = new Stopwatch();

            mShoot     = new Stopwatch();
            mShoot.Start();

            for (int i = 0; i < 11; i++)
            {
                Texture tex = new Texture("resources/health/health" + i + ".png");
                mHealthBar[i] = new Sprite(tex);

                Vector2f pos = new Vector2f();
                pos.X -= 17f;
                pos.Y -= 10f;
                mHealthBar[i].Position = mPosition - pos;
            }
        }

        public override void createBody(World _physics, BodyType _type)
        {
            base.createBody(_physics, _type);
            mBody.UserData = this;
            mBody.FixedRotation = true;
            mBody.CollisionCategories = Category.Cat5;
            mBody.CollidesWith = Category.All & ~Category.Cat2;
            mBody.OnCollision += collision;
        }

        public override void update(RenderWindow _window)
        {
            input(_window);
            base.update(_window);

            if (mHealth == 0)
            {
                destroy();
                mWorld.mWin = false;
                mWorld.mGameState = GameWorld.GameState.END;
            }

            if (ConvertUnits.ToDisplayUnits(mBody.Position.Y) > 800)
                mBody.Position = ConvertUnits.ToSimUnits(300f, 680f);
        }

        public override void render(RenderWindow _window)
        {
            base.render(_window);
            if (!mInvicible)
            {
                mSegment = mHealth / 10;
                for (int i = 0; i < mSegment; i++)
                    _window.Draw(mHealthBar[i]);
            }
            else
            {
                _window.Draw(mHealthBar[10]);
                if (mTimer.ElapsedMilliseconds > 30000f)
                {
                    mTimer.Reset();
                    mInvicible = false;
                }
            }
        }

        public override void addPosition(float _x, float _y)
        {
            base.addPosition(_x, _y);
            for (int i = 0; i < mHealthBar.Length; i++)
            {
                Vector2f pos = new Vector2f();
                pos.X -= 17f;
                pos.Y -= 10f;
                mHealthBar[i].Position = mPosition + pos;
            }
        }

        public override void setPosition(float _x, float _y)
        {
            base.setPosition(_x, _y);
            for (int i = 0; i < mHealthBar.Length; i++)
            {
                Vector2f pos = new Vector2f();
                pos.X -= 17f;
                pos.Y -= 10f;
                mHealthBar[i].Position = mPosition + pos;
            }
        }

        public override void input(RenderWindow _window)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                if (mPosition.X > 0)
                    mBody.ApplyLinearImpulse(new Vector2(-0.3f * Delta.mDelta, 0f));
                else
                    mBody.ApplyLinearImpulse(new Vector2(0.3f * Delta.mDelta, 0f));
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                if(mPosition.Y < 1280)
                    mBody.ApplyLinearImpulse(new Vector2( 0.3f * Delta.mDelta, 0f));
                else
                    mBody.ApplyLinearImpulse(new Vector2(-0.3f * Delta.mDelta, 0f));
            }

            if(Keyboard.IsKeyPressed(Keyboard.Key.Return))
            {
                if(!mOnce)
                    invincible();
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                if (mPosition.X > 0 && mPosition.X < 1280)
                    mBody.ApplyLinearImpulse(new Vector2(0f, -0.9f * Delta.mDelta));
            }

            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if (mShoot.ElapsedMilliseconds > 250)
                {
                    Vector2i mousePos   = Mouse.GetPosition(_window);
                    Vector2f translated = _window.MapPixelToCoords(mousePos, _window.GetView());
                    Vector2f aim        = new Vector2f(translated.X - mPosition.X, translated.Y - mPosition.Y);
                    double   angle      = Math.Atan2(aim.Y, aim.X);

                    mWorld.createBullet(mPosition, this, "resources/bullet.png", angle);
                    mShoot.Restart();
                }
            }
        }

        public override bool collision(Fixture f1, Fixture f2, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (f2.CollisionCategories == Category.Cat3)
            {
                if (!mInvicible && mHealth > 0)
                    mHealth--;

                Console.WriteLine("Health = " + mHealth);
            }
            return true;
        }

        public void invincible()
        {
            mInvicible = true;
            mOnce      = true;
            mHealth    = 100;
            mTimer.Start();
        }
    }
}
