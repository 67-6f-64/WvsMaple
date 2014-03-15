using reNX;
using reNX.NXProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Data
{
    public class CachedStyles
    {
        public List<byte> Skins { get; set; }
        public List<int> MaleHairs { get; set; }
        public List<int> FemaleHairs { get; set; }
        public List<int> MaleFaces { get; set; }
        public List<int> FemaleFaces { get; set; }

        public CachedStyles(NXFile dataFile)
            : base()
        {
            this.Skins = new List<byte>();

            this.MaleHairs = new List<int>();
            this.FemaleHairs = new List<int>();

            this.MaleFaces = new List<int>();
            this.FemaleFaces = new List<int>();

            foreach (NXNode skinNode in dataFile.ResolvePath("/Character"))
            {
                try
                {
                    if (int.Parse(skinNode.Name.Replace(".img", "")) < 3000)
                    {
                        this.Skins.Add((byte)(int.Parse(skinNode.Name.Replace(".img", "")) % 100));
                    }
                }
                catch
                {
                    break;
                }
            } 
            foreach (NXNode hairNode in dataFile.ResolvePath("/Character/Hair"))
            {
                switch (int.Parse(hairNode.Name.Replace(".img", "")))
                {
                    case 30:
                    case 33:
                        this.MaleHairs.Add(int.Parse(hairNode.Name.Replace(".img", "")));
                        break;
                    default:
                        this.FemaleHairs.Add(int.Parse(hairNode.Name.Replace(".img", "")));
                        break;
                }
            }

            foreach (NXNode faceNode in dataFile.ResolvePath("/Character/Face"))
            {
                switch (int.Parse(faceNode.Name.Replace(".img", "")) / 1000)
                {
                    case 20:
                        this.MaleFaces.Add(int.Parse(faceNode.Name.Replace(".img", "")));
                        break;
                    default:
                        this.FemaleFaces.Add(int.Parse(faceNode.Name.Replace(".img", "")));
                        break;
                } 
            }
        }
    }
}
