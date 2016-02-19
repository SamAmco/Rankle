using Lidgren.Network;

namespace GameFinal
{
    class Character
    {
        #region variables
        public float X { get; set; }
        public float Y { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }
        public float Rotation { get; set; }
        public string Name { get; set; }
        public NetConnection Connection { get; set; }
        public bool Self { get; set; }
        public int characterIndex { get; set; }
        public float health { get; set; }
        public float energy { get; set; }
        public int tankSkin { get; set; }

        public bool superGun { get; set; }
        public bool freeInvisibility { get; set; }
        public bool freeMines { get; set; }
        public bool freeMissiles { get; set; }
        public bool freeRifle { get; set; }
        public bool tripleMines { get; set; }
        public bool superTough { get; set; }
        public bool ultraStealth { get; set; }
        #endregion

        public Character(string name, float x, float y, NetConnection conn, int index)
        {
            Name = name;
            X = x;
            Y = y;
            Connection = conn;
            Self = false;
            characterIndex = index;
        }
        public Character(string name, float x, float y, NetConnection conn, bool self)
        {
            Name = name;
            X = x;
            Y = y;
            Connection = conn;
            Self = self;
        }
        public Character(string name, NetConnection conn, int index, int tankSkin)
        {
            this.Name = name;
            this.Connection = conn;
            this.characterIndex = index;
            this.tankSkin = tankSkin;
        }
        public Character()
        {
        }

        public int getIndex()
        {
            return characterIndex;
        }

        public void setBuffs(bool[] buffs)
        {
            if (buffs[0])
                superGun = true;
            else superGun = false;

            if (buffs[1])
                freeInvisibility = true;
            else freeInvisibility = false;

            if (buffs[2])
                freeMines = true;
            else freeMines = false;

            if (buffs[3])
                freeMissiles = true;
            else freeMissiles = false;

            if (buffs[4])
                freeRifle = true;
            else freeRifle = false;

            if (buffs[5])
                tripleMines = true;
            else tripleMines = false;

            if (buffs[6])
                superTough = true;
            else superTough = false;

            if (buffs[7])
                ultraStealth = true;
            else ultraStealth = false;
        }

        public bool[] getBuffs()
        {
            bool[] buffs = new bool[8];
            buffs[0] = superGun;
            buffs[1] = freeInvisibility;
            buffs[2] = freeMines;
            buffs[3] = freeMissiles;
            buffs[4] = freeRifle;
            buffs[5] = tripleMines;
            buffs[6] = superTough;
            buffs[7] = ultraStealth;
            return buffs;
        }

        public bool isValid()
        {
            if (Name != "" && health >= 0 && energy >= 0 && tankSkin < 13 && tankSkin >= 0)
                return true;
            else return false;
        }
    }
}
