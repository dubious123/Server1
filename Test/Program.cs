using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
namespace Testing
{
    public enum ClassType
    {
        Knight,
        Archer,
        Mage
    }
    public class Player
    {
        public int Level { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public ClassType _ClassType { get; set; }
        public List<int> Items { get; set; } = new List<int>();
    }
    class TraceTest
    {
        static void Main()
        {
            Random rand = new Random();
            List<Player> players = new List<Player>();
            for(int i = 0; i < 100; i++)
            {
                ClassType type = (ClassType)rand.Next(0, 3);
                Player player = new Player()
                {
                    Attack = rand.Next(1, 100),
                    Level = rand.Next(1, 100),
                    Hp = rand.Next(1, 100),
                    _ClassType = type
                };
                players.Add(player);
                for(int j = 0; j < 100; j++)
                {
                    player.Items.Add(rand.Next(1, 101));
                }
            }

            var ps =
                from p in players
                where p._ClassType == ClassType.Knight && p.Level >= 50
                orderby p.Level
                select p;
            var count = ps.ToList().Count;

            var ps2 =
                from p in players
                from i in p.Items
                where i < 30
                select new { p, i };
            var li = ps2.ToList();

            var ps3 =
                from p in players
                group p by p.Level into g
                orderby g.Key
                select new { g.Key, Players = g };


            List<int> levels = new List<int>() { 1, 5, 10 };
            var ps4 =
                from p in players
                join j in levels
                on p.Level equals j
                select p;

            var ps5 =
                 from p in players
                 where p._ClassType == ClassType.Knight && p.Level >= 50
                 orderby p.Level
                 select p;

            var ps6 = players
                .Where(p => p._ClassType == ClassType.Knight && p.Level >= 50)
                .OrderBy(j => j.Level)
                .Select(o => o);
        }

    }
}