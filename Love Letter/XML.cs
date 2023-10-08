using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Love_Letter
{
    public class XML
    {
        public static XML SaveGame(Player[] playerArray, int roundNumber, bool SP)
        {
            string saveFile = "SavedGame.xml";
            XmlTextWriter xmlWriter = new XmlTextWriter(saveFile, System.Text.Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteComment("This file is to save current game progress");
            xmlWriter.WriteStartElement("CurrentGame");
            int playerCount = 0;
            foreach(Player player in playerArray)
            {
                
                xmlWriter.WriteStartElement("Player" + playerCount);
                xmlWriter.WriteElementString("PlayerType", Utilities.GetTypeName(player));
                xmlWriter.WriteElementString("Name", player.viewName());
                xmlWriter.WriteElementString("Tokens", player.showToken().ToString());
                xmlWriter.WriteEndElement();
                playerCount++;
            }
            xmlWriter.WriteStartElement("NumberofPlayers");
            xmlWriter.WriteElementString("Number", playerCount.ToString());
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Rounds");
            xmlWriter.WriteElementString("RoundNumber", roundNumber.ToString());
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("SinglePlayer");
            xmlWriter.WriteElementString("SP", SP.ToString());
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();

            Console.WriteLine("Progress saved succesfully;");
                return null;
        }
        public static XML LoadGame()
        {        
            XmlDocument loadFile = new XmlDocument();
            loadFile.Load("SavedGame.xml");
            XmlNodeList nodes = loadFile.SelectNodes("//CurrentGame");
            int numberofPlayers = 0;
            foreach (XmlNode node in nodes)
            {
                XmlNode playerCount = node.SelectSingleNode("NumberofPlayers");
                if(playerCount != null)
                {
                    numberofPlayers = int.Parse(playerCount.InnerText);
                }      
            }
            MainGame.playerArray = new Player[numberofPlayers];
            for(int i = 0; i < numberofPlayers; i++)
            {
                foreach (XmlNode node in nodes)
                {
                    XmlNode playerInfo = node.SelectSingleNode("Player" + i);
                    if (playerInfo != null)
                    {
                        
                        XmlNode playerType = playerInfo.SelectSingleNode("PlayerType");                      
                        XmlNode playerName = playerInfo.SelectSingleNode("Name");
                        XmlNode playerTokens = playerInfo.SelectSingleNode("Tokens");
                        if (playerType.InnerText == "Human") { MainGame.playerArray[i] = new Human(playerName.InnerText.ToString()); }
                        else { MainGame.playerArray[i] = new Robot(playerName.InnerText.ToString()); }
                        int token = int.Parse(playerTokens.InnerText);
                        for (int j = 0; j < token; j++) { MainGame.playerArray[i].addToken();  }
                    }
                }
            }
            return null;
        }
        
        public static bool SinglePlayerCheck()
        {
            XmlDocument loadFile = new XmlDocument();
            loadFile.Load("SavedGame.xml");
            XmlNodeList nodes = loadFile.SelectNodes("//CurrentGame");
            string SP = null;
            foreach (XmlNode node in nodes)
            {
                XmlNode singlePlayer = node.SelectSingleNode("SinglePlayer");
                if (singlePlayer != null)
                {              
                    SP = singlePlayer.InnerText.ToString();
                }
            }
            if(SP == "True") { return true; }
            else if (SP == "False") { return false; }

            return false;
        }
    }
}
