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
    public class Bullet : Entity
    {
        public Vector2f mDirection;

        public Bullet(Vector2f _position, Vector2f _direction, String _file)
        {
            create(_position, _file, 1);
            mDirection  = _direction;
            mEntityType = EntityType.Bullet;
        }

        public override void update(RenderWindow _window)
        {
            addPosition(mDirection.X, mDirection.Y);
            mBody.Position = ConvertUnits.ToSimUnits(mPosition.X, mPosition.Y);
        }

        public override void createBody(World _physics, BodyType _type)
        {
            base.createBody(_physics, _type);
            mBody.UserData = this;
            mBody.OnCollision += new OnCollisionEventHandler(collision);
        }

        public override bool collision(Fixture f1, Fixture f2, Contact contact)
        {
            this.destroy();
            return true;
        }
    }
}
