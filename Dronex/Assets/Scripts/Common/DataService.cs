using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace Common
{
    public class DataService : MonoBehaviour
    {
        public static string LoadGameData(string db)
        {
            string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + db;

            createFile(filePath);

            return File.ReadAllText(filePath);
        }

        public static void SaveGameData(string db, string dataAsJson)
        {
            Debug.Log(db + " - " + dataAsJson);
            Debug.Log(Application.persistentDataPath + Path.DirectorySeparatorChar + db);
            string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + db;

            createFile(filePath);

            File.WriteAllText(filePath, dataAsJson);
        }

        private static void createFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                FileStream file = File.Create(filePath);
                file.Close();
                File.WriteAllText(filePath, "{}");
            }
        }

        public static string LoadDefaultGameData(string db)
        {
            string filePath = Application.dataPath
                + Path.DirectorySeparatorChar
                + "Data"
                + Path.DirectorySeparatorChar
                + db;

            createFile(filePath);

            return File.ReadAllText(filePath);
        }
    }
}
