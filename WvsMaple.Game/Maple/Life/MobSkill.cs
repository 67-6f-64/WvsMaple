using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Fields;

namespace WvsGame.Maple.Life
{
    public class MobSkill
    {
        public static Dictionary<short, List<int>> Summons { get; set; }

        public byte MapleID { get; set; }
        public byte Level { get; set; }
        public short EffectDelay { get; set; }

        public int Duration { get; set; }
        public short MpCost { get; set; }
        public int ParameterA { get; set; }
        public int ParameterB { get; set; }
        public short Chance { get; set; }
        public short TargetCount { get; set; }
        public Position LT { get; set; }
        public Position RB { get; set; }
        public int Cooldown { get; set; }
        public short PercentageLimitHP { get; set; }
        public short SummonLimit { get; set; }
        public short SummonEffect { get; set; }

        //private IEnumerable<Character> GetAffectedCharacters(Mob caster)
        //{
        //    Rectangle boundingBox = new Rectangle(this.LT + caster.Position, this.RB + caster.Position);

        //    foreach (Character character in caster.Field.Characters)
        //    {
        //        if (character.Position.IsInRectangle(boundingBox))
        //        {
        //            yield return character;
        //        }
        //    }
        //}

        //private IEnumerable<Mob> GetAffectedMobs(Mob caster)
        //{
        //    Rectangle boundingBox = new Rectangle(this.LT + caster.Position, this.RB + caster.Position);

        //    foreach (Mob mob in caster.Field.Mobs)
        //    {
        //        if (mob.Position.IsInRectangle(boundingBox))
        //        {
        //            yield return mob;
        //        }
        //    }
        //}
    }
}