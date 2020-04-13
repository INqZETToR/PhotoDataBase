using System;
using System.Collections.Generic;
using System.Linq;
using _DatabaseManager;
using System.IO;
using _HashAnalyzer;
using System.Threading;

namespace FProgram
{
    class Program
    {
        private DatabaseManager dm;
        private HashAnalyzer ha;

        //private delegate void SetTextDelegate(ProgressBar pb, double msg);

        public Program()
        {
            dm = new DatabaseManager("PDB");
            ha = new HashAnalyzer();
            
        }

        public void CheckAllNewFiles()
        {
            //SetTextDelegate del = SetText; 
            var arr = GenerateFileArray("");
            for(int i = 0;i<arr.Length;i++)
            {
                //dm.AddDBW(new Database(arr[i], ha.AnalizPhoto(arr[i])),true);
                
                //del.Invoke(pb, ((i + 1) * 100 / arr.Length));
                
                //Dispatcher.BeginInvoke(new ThreadStart(delegate { progressBar1.Value += 50; }));
            }
            
            Console.WriteLine("Done!");
        }

        public void ReloadDBFile(string name)
        {
            dm.Name = name;
            dm.Deserialize();
        }

        public void CheckFile(string path,bool b,float koef)
        {
            dm.AddDBW(new Database(path, ha.AnalizPhoto(path)),b, koef);
        }

        public string[] GenerateFileArray(string dir)
        {
            List<string> files = Directory.GetFiles(Environment.CurrentDirectory, "*.jpg").ToList();
            

            string[] a = Directory.GetFiles(Environment.CurrentDirectory, "*.jpeg");
            foreach (string q in a)
            {
                files.Add(q);
            }

            string[] png = Directory.GetFiles(Environment.CurrentDirectory, "*.png");

            foreach (string q in png)
            {
                files.Add(q);
            }

            if (files.Capacity == 0)
            {
                return new string[] { };
            }
            return files.ToArray<string>();
        }

        public void SaveChanges()
        {
            Thread thr = new Thread(new ThreadStart(delegate {
                dm.SerializeAndRewrite();
                Console.WriteLine("Changes saved successfully!");
            }));
            thr.Start();
        }
    }
}
