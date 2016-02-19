using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFinal.Menu_s
{
    public class GameOptions
    {
        public int map_Number {get; set;}
        public string IP_Adress {get; set;}
        public int skin_Number {get; set;}
        public string player_Name {get; set;}
        public int preferred_Width { get; set; }
        public int preferred_Height { get; set; }
        public bool is_Full_Screen { get; set; }
        public bool is_Server { get; set; }

        public GameOptions(int mapNumber, string IPAdress, int skinNumber, string playerName,
            int preferredWidth, int preferredHeight, bool isServer, bool isFullScreen)
        {
            if (mapNumber < 1)
                mapNumber = 1;
            this.map_Number = mapNumber;
            this.IP_Adress = IPAdress;
            this.skin_Number = skinNumber;
            this.player_Name = playerName;
            this.preferred_Height = preferredHeight;
            this.preferred_Width = preferredWidth;
            this.is_Server = isServer;
            this.is_Full_Screen = isFullScreen;
        }

        public GameOptions()
        {
            map_Number = 1;
            IP_Adress = "";
            skin_Number = 0;
            player_Name = "no name";
            preferred_Width = 1280;
            preferred_Height = 720;
            is_Full_Screen = false;
            is_Server = false;
        }

        public GameOptions clone()
        {
            return new GameOptions(map_Number,
                IP_Adress,
                skin_Number,
                player_Name,
                preferred_Width,
                preferred_Height,
                is_Server,
                is_Full_Screen);
        }
    }
}
