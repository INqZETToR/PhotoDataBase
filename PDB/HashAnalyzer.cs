using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace _HashAnalyzer
{
    
    class HashAnalyzer
    {
        Bitmap bmp; 
        
        public string AnalizPhoto(string arr)
        {
            //string StartTitle = "Sorting programm ";
            int i = 0;
                i++;
                if (!File.Exists(arr)) return "";
                try
                {
                    using (FileStream fs = new FileStream(arr, FileMode.Open))
                    {
                        bmp = new Bitmap(Bitmap.FromStream(fs));
                        fs.Dispose();
                        fs.Close();
                        
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                    //Console.ReadLine();
                }
                Bitmap fbmp = new Bitmap(bmp, new Size(32, 32));
                int simple = 0;
                List<byte> ss = new List<byte>();
                byte[,] bb = new byte[32, 32];
                Bitmap grayScaleBP = new Bitmap(32, 32, PixelFormat.Format16bppGrayScale);
                for (int y = 0; y < fbmp.Height; ++y)
                {
                    for (int x = 0; x < fbmp.Width; ++x)
                    {
                        Color c = fbmp.GetPixel(x, y);
                        byte rgb = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B);
                        fbmp.SetPixel(x, y, Color.FromArgb(c.A, rgb, rgb, rgb));
                        simple += rgb;
                        bb[x, y] = rgb;
                    }
                }
                byte average = (byte)(simple / (fbmp.Height * fbmp.Width));
                for (int y = 0; y < fbmp.Height; ++y)
                {
                    byte[] heshp = new byte[32];
                    for (int x = 0; x < fbmp.Width; ++x)
                    {
                        if (bb[x, y] > average)
                        {
                            heshp[x] = 1;
                            fbmp.SetPixel(x, y, Color.White);
                        }
                        else
                        {
                            heshp[x] = 0;
                            fbmp.SetPixel(x, y, Color.Black);
                        }
                        ss.Add(heshp[x]);
                    }
                }
                string result = "";
                result = GetHash(ss.ToArray());                
                return result;
        }

        private static int c = 0;

        private static string GetHash(byte[] a)
        {
            string res = "";
            for (int i = 0; i < a.Length - 3; i += 4)
            {
                res += Method(new byte[] { a[i], a[i + 1], a[i + 2], a[i + 3] });
                c = i;
            }
            return GetHashOwn(res);
        }

        private static string GetHashOwn(string hash)
        {
            string tlist = "";
            string last = "0";
            int one = 0;
            hash += "l";
            for (int i = 0; i < hash.Length; i++)
            {
                char c = hash[i];
                if (last == "0")
                {
                    last = c.ToString();
                    one++;
                }
                else if (last.Last() == c)
                {
                    one++;
                }
                else if (last.Last() != c && one != 1)
                {
                    tlist += one + last;
                    last = c.ToString();
                    one = 1;
                }
                else if (last.Last() != c)
                {
                    tlist += last;
                    last = c.ToString();
                    one = 1;
                }

            }
            return tlist;
        }

        static string[] base1 = new string[] { "0000", "0001", "0010", "0011", "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011", "1100", "1101", "1110", "1111" };
        static char[] base3 = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p' };

        private static string Method(byte[] bs)
        {
            try
            {
                int i = 0;
                string ss = "";
                foreach (byte b in bs) ss += b.ToString();
                foreach (string c in base1)
                {
                    if (c == ss.ToString())
                    {
                        break;
                    }
                    i++;
                }
                string pres = base3[i].ToString();
                return pres;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message + c);
                return "";
            }
        }

    }
}
