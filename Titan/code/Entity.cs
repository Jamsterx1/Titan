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
    public class Entity
    {
        public enum LifeState
        {
            LifeState_Active = 0,
            LifeState_Dazed,
            LifeState_Dead
        };

        public enum EntityType
        {
            Default = 0,
            Player,
            Enemy,
            Bullet,
            Boss
        };

        public Vector2f mPosition;
        public Vector2f mOrigin;
        public Vector2u mDimensions;
        public Sprite   mSprite;

        private LifeState mLifeState;
        public EntityType mEntityType;
        public  Body      mBody;
        public  uint      mLayer;

        public Entity()
        {
        }

        public Entity(Vector2f _position, String _file, uint _layer = 0)
        {
            create(_position, _file, _layer);
        }

        public void create(Vector2f _position, String _file, uint _layer = 0)
        {
            Texture tex = new Texture(_file);
            mSprite = new Sprite(tex);

            mEntityType = EntityType.Default;
            mBody       = null;
            mLayer      = _layer;
            mLifeState  = LifeState.LifeState_Active;
            mPosition   = _position;

            mSprite.Position = mPosition;
            mDimensions      = tex.Size;
            mOrigin          = new Vector2f(mPosition.X + (mDimensions.X / 2), mPosition.Y + (mDimensions.Y / 2));
        }

        public virtual void update(RenderWindow _window)
        {
            if (mBody != null)
            {
                Vector2f pos = new Vector2f();
                pos.X = ConvertUnits.ToDisplayUnits(mBody.Position.X) - (mDimensions.X / 2);
                pos.Y = ConvertUnits.ToDisplayUnits(mBody.Position.Y) - (mDimensions.Y / 2);
                setPosition(pos.X, pos.Y);
            }
        }

        public virtual void render(RenderWindow _window)
        {
            if(mLifeState == LifeState.LifeState_Active)
                _window.Draw(mSprite);
        }

        public virtual void input(RenderWindow _window)
        {
        }

        public virtual void createBody(World _physics, BodyType _type)
        {
            Vector2 size   = ConvertUnits.ToSimUnits(mDimensions.X, mDimensions.Y);
            mBody          = BodyFactory.CreateRectangle(_physics, size.X, size.Y, 1f);
            mBody.Position = ConvertUnits.ToSimUnits(mOrigin.X, mOrigin.Y);
            mBody.BodyType = _type;

            mBody.Friction = 0.15f;
            mBody.CollisionCategories = Category.Cat1;
        }

        /*public virtual void input()
        {
        }*/

        public virtual void addPosition(float _x, float _y)
        {
            mPosition.X += _x;
            mPosition.Y += _y;
            mOrigin.X = mPosition.X + (mDimensions.X / 2);
            mOrigin.Y = mPosition.Y + (mDimensions.Y / 2);
            mSprite.Position = mPosition;
        }

        public virtual void setPosition(float _x, float _y)
        {
            mPosition.X = _x;
            mPosition.Y = _y;
            mOrigin.X = mPosition.X + (mDimensions.X / 2);
            mOrigin.Y = mPosition.Y + (mDimensions.Y / 2);
            mSprite.Position = mPosition;
        }

        public virtual bool collision(Fixture f1, Fixture f2, Contact contact)
        {
            return true;
        }

        /* Life Control Functions */
        public void sleep()   { mLifeState = LifeState.LifeState_Dazed;        }
        public void wake()    { mLifeState = LifeState.LifeState_Active;       }
        public void destroy() { mLifeState    = LifeState.LifeState_Dead;      }
        
        public bool isDead()  
        { 
            return mLifeState == LifeState.LifeState_Dead; 
        }
    }
}
