using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    // STRUCTURE OF THE CODE IS COMMENTED IN THE SAVEPLAYER AND LOADPLAYER CLASS FUNCTION

    public static void SavePlayer(Player player)
    {
        // create binary formatter
        BinaryFormatter b_formatter = new BinaryFormatter();

        // create a file to save to (first, name the file that you are going to create)
        string path = Application.persistentDataPath + "/player.file";
        FileStream stream = new FileStream(path, FileMode.Create);

        // create data by passing in the player into the constructor
        PlayerData data = new PlayerData(player);

        // write data to file stream
        b_formatter.Serialize(stream, data);
        // close stream when finished
        stream.Close();

        Debug.Log("Saved player data: " + path);
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.file";
        // check if file exists
        if (File.Exists(path))
        {
            // open up a new binary formatter
            BinaryFormatter formatter = new BinaryFormatter();

            /* The using statement automatically calls stream.Close(), so you don't have to remember that,
               and the FileStream object becomes invalid outside the statement scope (outside the two curly braces),
               so you can't accidentally use it when the file is closed. */
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                // read from stream and store in into the variable: data
                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                return data;
            }
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void SavePlayerStats(PlayerStats stats)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerStats.file";

        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerStatsData data = new PlayerStatsData(stats);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved PlayerStats data: " + path);
    }

    public static PlayerStatsData LoadPlayerStats()
    {
        string path = Application.persistentDataPath + "/playerStats.file";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                // read from stream and store in into the variable: data
                PlayerStatsData data = formatter.Deserialize(stream) as PlayerStatsData;
                return data;
            }
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    // TO DELETE FILES FOR TESTING
    // THE LINES OF CODE ARE REFERENCED IN PLAYERSTATS START METHOD

    //  SaveSystem.DeletePlayerFile();
    //  SaveSystem.DeleteLevelFile();

    public static void DeletePlayerFile()
    {
        string path = Application.persistentDataPath + "/player.file";
        File.Delete(path);
        Debug.Log("Deleted: " + path);
    }

    public static void DeletePlayerStatsFile()
    {
        string path = Application.persistentDataPath + "/playerStats.file";
        File.Delete(path);
        Debug.Log("Deleted: " + path);
    }
}
