using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;


namespace _DatabaseManager
{    
    public class DatabaseManager
    {
        public DatabaseManager(string name)
        {
            //Deserialize();
            //FixDB();
            //SerializeAndRewrite();
            this.name = name;
        }
        string name = "PDB";
        public string Name { get => name; set => name = value; }

        private Database[] dbws;


        public void SerializeAndRewrite()
        {
            DataContractJsonSerializer JsonFormatter = new DataContractJsonSerializer(typeof(Database[]));
            using (FileStream fs = new FileStream(name+".json", FileMode.Create))
            {
                JsonFormatter.WriteObject(fs, dbws.ToArray());
                fs.Close();
            }

        }

        public void Move(string path)
        {
            string dir = Environment.CurrentDirectory + "\\copyes\\" + Path.GetFileName(path);
            try
            {
                if (File.Exists(dir)) File.Delete(dir);
                File.Move(path, dir);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void Copy(string file, string hash, string file2)
        {
            var a = Path.GetFileName(file2).Split('.');
            string name = a[0];
            string ext = a[1];
            string dir = Environment.CurrentDirectory + "\\copyes\\" + name + "Orig" + "." + ext;
            if (File.Exists(dir)) File.Delete(dir);
            File.Copy(file, dir);

        }

        public void FixDB()
        {
            Deserialize();
            for (int i = 0; i < dbws.Length; i++)
            {
                var d = dbws[i];
                string newpath = Environment.CurrentDirectory + "\\uploaded photos\\" + Path.GetFileName(d.Path);
                if (File.Exists(d.Path)) continue;
                else if (File.Exists(newpath))
                {

                    dbws[i].Path = newpath;
                    Console.WriteLine("Moved file (to uploaded photos)" + d.Path + " path has redacted!");
                }
                else if (File.Exists(Environment.CurrentDirectory + "\\" + Path.GetFileName(d.Path)) && (d.Path != Environment.CurrentDirectory + "\\" + Path.GetFileName(d.Path)))
                {
                    newpath = Environment.CurrentDirectory + "\\" + Path.GetFileName(d.Path);
                    dbws[i].Path = newpath;
                    Console.WriteLine("Moved file " + d.Path + " path was redacted!");
                }
                else
                {
                    if (d.Path == "Deleted") continue;
                    Console.WriteLine("Deleted file \"" + d.Path + "\"\nPath was redacted! (Now it's \"Deleted\")");
                    dbws[i].Path = "Deleted";
                }
            }
            SerializeAndRewrite();
        }

        public bool AddDBW(Database p, bool addToDb, float koef )
        {
            bool ret=false;
            float percentOfEdentity = 0;
            if (dbws == null)
            {
                Deserialize();
                FixDB();
            }
            var list = dbws.ToList();
            if (!Directory.Exists(Environment.CurrentDirectory + "\\copyes\\")) Directory.CreateDirectory(Environment.CurrentDirectory + "\\copyes\\");
            for (int i = 0; i < dbws.Length; i++)
            {
                if (AreHashesEqual(dbws[i].Hash, p.Hash, out percentOfEdentity, koef) == false && dbws[i].Path != p.Path)
                {

                    //Console.WriteLine(p.Path+" || "+d.Path);
                    //Console.ReadLine();
                    if (dbws[i].Path == "Deleted")
                    {
                        dbws[i].Path = p.Path;
                        Console.WriteLine("Deleted file was detected, path was fixed. " + p.Path);

                    }
                    else if (File.Exists(dbws[i].Path))
                    {
                        Copy(dbws[i].Path, dbws[i].Hash, p.Path);
                        Move(p.Path);
                        Console.WriteLine("File has copy, orig path: " + dbws[i].Path + "\nCopy: " + p.Path);
                        ret = true;
                    }
                    else
                    {
                        CopyTxt(dbws[i].Path, dbws[i].Hash, p.Path);
                        ret = true;
                    }
                    Console.WriteLine("Edentity: " + percentOfEdentity + "%");
                    return ret;
                }
                else if (dbws[i].Path == p.Path)
                {
                    Console.WriteLine("It's the same file!");
                    return false;
                }
            }
            Console.WriteLine("New file! " + percentOfEdentity + "%");
            if (addToDb)
            {
                list.Add(p);
                dbws = list.ToArray();
                //SerializeAndRewrite();
            }
            return false;
        }

        public void CopyTxt(string file, string hash, string file2)
        {
            var a = Path.GetFileName(file2).Split('.');
            string name = a[0];
            string ext = a[1];
            string dir = Environment.CurrentDirectory + "\\copyes\\" + name + "Orig" + "." + ext;
            if (File.Exists(dir)) File.Delete(dir);
            File.CreateText(Environment.CurrentDirectory + "\\copyes\\" + name + "Orig" + ".txt").WriteLine("File was deleted " + name + "." + ext + " " + hash);

        }

        public bool SearchInDB(string path)
        {
            foreach (Database d in dbws)
            {
                if (dbws.Length != 0 && d.Path == path)
                    return true;
                else if (dbws.Length == 0)
                    break;
            }
            return false;
        }

        public void Deserialize()
        {
            DataContractJsonSerializer JsonFormatter = new DataContractJsonSerializer(typeof(Database[]));
            using (FileStream fs = new FileStream(name+".json", FileMode.OpenOrCreate))
            {
                try
                {
                    dbws = (Database[])JsonFormatter.ReadObject(fs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    dbws = new Database[0];
                }
                fs.Close();
            }
            Console.Title = dbws.Count() + " fields in DB";
        }

        string[] base1 = new string[] { "0000", "0001", "0010", "0011", "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011", "1100", "1101", "1110", "1111" };
        char[] base3 = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p' };

        

        private bool AreHashesEqual(string h1, string h2, out float compareFloat, float koefOfEdentity)
        {
            int errForEdentity=600;
            Console.WriteLine();
            try
            {
                string[] c1 = UnPackHash(h1);
                string[] c2 = UnPackHash(h2);

                int err = 0;
                //return false;
                for (int i = 0; i < c1.Length; i++)
                {
                    if (c1[i] != c2[i]) err++;
                }
                
                compareFloat = ((float)(errForEdentity - err) / errForEdentity) * 100;
                Console.WriteLine(err+" "+compareFloat+"% koeff "+koefOfEdentity+"%");

                if (compareFloat < koefOfEdentity)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            compareFloat = 0;

            return false;
        }

        private string[] UnPackHash(string h1)
        {
            string[] hash = new string[1024];
            string count = "";
            int lastInd = 0;

            for (int i = 0; i < h1.Length; i++)
            {
                if (h1[i] <= '9' && h1[i] >= '0')
                {
                    count += h1[i];
                    continue;
                }
                else
                {
                    int coint = 0;
                    if (!int.TryParse(count, out coint) || coint == 0) coint = 1;
                    int index = 0;
                    for (int j = 0; j < base3.Length; j++)
                    {
                        if (base3[j] == h1[i])
                        {
                            index = j;
                            break;
                        }
                    }
                    for (int k = 0; k < coint; k++)
                    {
                        hash[lastInd] = base1[index];
                        lastInd++;
                    }
                    count = "";
                }
            }
            return hash.ToArray();
        }
    }

    [DataContract]
    public class Database
    {
        public Database(string path, string hash)
        {
            Path = path;
            Hash = hash;
        }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Hash { get; set; }
    }

}
