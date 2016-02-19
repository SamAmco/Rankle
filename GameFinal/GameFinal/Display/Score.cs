using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFinal.Display
{
    class Score : IComparable
    {
        public String name { get; set; }
        public int kills { get; set; }
        public int deaths { get; set; }

        public Score(string name)
        {
            this.name = name;
            this.kills = 0;
            this.deaths = 0;
        }

        public string getName()
        {
            return name;
        }

        public int getKills()
        {
            return kills;
        }

        public int getDeaths()
        {
            return deaths;
        }

        public float getKD()
        {
            if (deaths == 0)
                return (float)Math.Round((double)((float)(kills) / (float)(deaths + 1)), 2);
            else
                return (float)Math.Round((double)((float)(kills) / (float)(deaths)), 2);
        }

        public void Death()
        {
            deaths++;
        }

        public void Kill()
        {
            kills++;
        }

        public int CompareTo(object s)
        {
            if (this.getKD() > ((Score)s).getKD())
                return -1;
            else if (this.getKD() < ((Score)s).getKD())
                return 1;
            else return 0;
        }

    }
}
