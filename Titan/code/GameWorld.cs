using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Titan;
using SFML.Graphics;
using SFML.Window;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace Titan
{
    public class GameWorld
    {
        private static readonly Color CornflowerBlue = new Color(100, 149, 237);
        public List<Entity> mEntities;
        public World mPhysics;

        public Spawner mSpawner = new Spawner(640f, 360f);

        public GameWorld()
        {
            mEntities = new List<Entity>();
            mPhysics  = new World(new Vector2(0f, 0.0981f));

            ConvertUnits.SetDisplayUnitToSimUnitRatio(8f);
            Delta.create();
        }

        public void update(RenderWindow _window)
        {
            // Pre-Update
            Delta.update();
            mPhysics.Step(Delta.mDelta);

            if(mEntities.Count < 50)
                mSpawner.update();

            // World Update
            for (int i = 0; i < mEntities.Count; i++)
            {
                mEntities[i].update(_window);
                if (mEntities[i].isDead())
                    mEntities.RemoveAt(i);
            }

            
        }

        public void render(RenderWindow _window)
        {
            _window.Clear(CornflowerBlue);
            for (int i = 0; i < mEntities.Count; i++)
                mEntities[i].render(_window);
        }

        public void add(Entity _entity)
        {
            mEntities.Add(_entity);
            Console.WriteLine("Entity " + _entity + " added to world");
        }

        public void addBatch(List<Entity> _entities)
        {
            for (int i = 0; i < _entities.Count; i++)
                mEntities.Add(_entities[i]);
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

        public void createWorld()
        {
            // Create all platforms, enemies, player and other stuff here
            Player player = new Player(new Vector2f(200f, 200f), "../../resources/player.png");
            player.createBody(mPhysics, BodyType.Dynamic);

            Entity floor1 = new Entity(new Vector2f(0f, 710f), "../../resources/floor.png");
            floor1.createBody(mPhysics, BodyType.Static);

            Entity floor2 = new Entity(new Vector2f(0f, 0f), "../../resources/floor.png");
            floor2.createBody(mPhysics, BodyType.Static);

            Entity wall1 = new Entity(new Vector2f(0f, 0f), "../../resources/wall.png");
            wall1.createBody(mPhysics, BodyType.Static);

            Entity wall2 = new Entity(new Vector2f(1270f, 0f), "../../resources/wall.png");
            wall2.createBody(mPhysics, BodyType.Static);

            Entity platform1 = new Entity(new Vector2f(100f, 650f), "../../resources/platform.png");
            platform1.createBody(mPhysics, BodyType.Static);
            Entity platform2 = new Entity(new Vector2f(500f, 600f), "../../resources/platform.png");
            platform2.createBody(mPhysics, BodyType.Static);
            Entity platform3 = new Entity(new Vector2f(180f, 160f), "../../resources/platform.png");
            platform3.createBody(mPhysics, BodyType.Static);
            Entity platform4 = new Entity(new Vector2f(800f, 200f), "../../resources/platform.png");
            platform4.createBody(mPhysics, BodyType.Static);
            Entity platform5 = new Entity(new Vector2f(900f, 580f), "../../resources/platform.png");
            platform5.createBody(mPhysics, BodyType.Static);
            Entity platform6 = new Entity(new Vector2f(1050f, 290f), "../../resources/platform.png");
            platform6.createBody(mPhysics, BodyType.Static);
            Entity platform7 = new Entity(new Vector2f(500f, 120f), "../../resources/platform.png");
            platform7.createBody(mPhysics, BodyType.Static);
            Entity platform8 = new Entity(new Vector2f(670f, 440f), "../../resources/platform.png");
            platform8.createBody(mPhysics, BodyType.Static);
            Entity platform9 = new Entity(new Vector2f(470f, 360f), "../../resources/platform.png");
            platform9.createBody(mPhysics, BodyType.Static);
            Entity platform10 = new Entity(new Vector2f(100f, 340f), "../../resources/platform.png");
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
            mSpawner.create(player, this, mPhysics);
        }
    }
}
