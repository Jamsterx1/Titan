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
    public class Boss : Enemy
    {
        public Boss(Vector2f _position, String _file, Entity _target, GameWorld _world, uint _layer = 2)
        {
            create(_position, _file, _layer);
            mHealth     = 1000;
            mWorld      = _world;
            mEntityType = EntityType.Boss;
            mTarget     = _target;
        }

        public override void update(RenderWindow _window)
        {
            // Boss Logic
        }

        public override void render(RenderWindow _window)
        {
            base.render(_window);
        }
    }
}
