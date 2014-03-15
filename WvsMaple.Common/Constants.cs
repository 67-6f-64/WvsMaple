using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class Constants
    {
        public const ushort MapleVersion = 28;
        public const string PatchLocation = "1";
        public const byte Localisation = 8;

        public const string CommandIndicator = "!";
        public const string PlayerCommandIndicator = "@";

        public const bool RequestPin = false;

        public const string LogPath = @"Logs\";

        public static bool IsAlphaNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            foreach (char c in value)
            {
                if (!char.IsLetter(c) && !char.IsNumber(c))
                {
                    return false;
                }
            }

            return true;
        }

        public static string ClearFormatters(this string value)
        {
            return value.Replace("{", "{{").Replace("}", "}}");
        }
    }

    public enum ServerType
    {
        Login,
        World,
        Game,
        Shop,
        Farm,
        MapGen,
        Claim,
        ITC,
        Unknown
    }

    public enum LoginResponse : byte
    {
        Valid,
        Banned = 3,
        IncorrectPassword,
        NotRegistered,
        SystemError,
        AlreadyLoggedIn,
        SystemError2,
        SystemError3,
        TooManyConnections,
        AgeLimit,
        NotMasterIP = 13,
        WrongGatewayInformationKorean,
        ProcessKorean,
        VerifyEmail,
        WrongGatewayInformation,
        VerifyEmail2 = 21,
        LicenceAgreement = 23,
        MapleEuropeNotice = 25,
        RequireFullVersion = 27
    }

    public enum PinResponse : byte
    {
        Valid,
        Register,
        Invalid,
        Error,
        Request,
        Cancel
    }

    public enum ItemType : byte
    {
        Equipment = 1,
        Usable,
        Setup,
        Etcetera,
        Cash
    }
    public enum EquippedQueryMode
    {
        Any,
        Cash,
        Normal
    }

    public enum EquipmentSlot : sbyte
    {
        Hat = -1,
        Face = -2,
        Eye = -3,
        Mantle = -4,
        Top = -5,
        Bottom = -6,
        Shoes = -7,
        Gloves = -8,
        Cape = -9,
        Shield = -10,
        Weapon = -11,
        Ring = -12,
        Necklace = -17,
        Mount = -18,
        CashHat = -101,
        CashFace = -102,
        CashEye = -103,
        CashTop = -104,
        CashOverall = -105,
        CashBottom = -106,
        CashShoes = -107,
        CashGloves = -108,
        CashCape = -109,
        CashShield = -110,
        CashWeapon = -111,
        CashRing = -112,
        CashNecklace = -117,
        CashMount = -118
    }

    public enum FieldTrasnferMode : int
    {
        Portal = -1
    }

    [Flags]
    public enum CharacterStatus : int
    {
        Curse = 0x01,
        Weakness = 0x02,
        Darkness = 0x04,
        Seal = 0x08,
        Poison = 0x10,
        Stun = 0x20,
        Slow = 0x40,
        Seduce = 0x80,
        Zombify = 0x100,
        CrazySkull = 0x200
    }

    [Flags]
    public enum MobStatus : int
    {
        Null,

        WeaponAttackIcon = 0x01,
        WeaponDefenceIcon = 0x02,
        MagicAttackIcon = 0x04,
        MagicDefenceIcon = 0x08,
        AccuracyIcon = 0x10,
        AvoidabilityIcon = 0x20,
        SpeedIcon = 0x40,

        Stunned = 0x80,
        Frozen = 0x100,
        Poisoned = 0x200,
        Sealed = 0x400,

        Unknown1 = 0x800,

        WeaponAttackUp = 0x1000,
        WeaponDefenseUp = 0x2000,
        MagicAttackUp = 0x4000,
        MagicDefenseUp = 0x8000,

        Doom = 0x10000,
        ShadowWeb = 0x20000,

        WeaponImmunity = 0x40000,
        MagicImmunity = 0x80000,

        Unknown2 = 0x100000,
        Unknown3 = 0x200000,
        NinjaAmbush = 0x400000,
        Unknown4 = 0x800000,
        VenomousWeapon = 0x1000000,
        Unknown5 = 0x2000000,
        Unknown6 = 0x4000000,
        Empty = 0x8000000,
        Hypnotized = 0x10000000,
        WeaponDamageReflect = 0x20000000,
        MagicDamageReflect = 0x40000000
    }

    public enum CharacterDisease : long
    {
        Null,
        Slow = 0x1,
        Seduce = 0x80,
        Fishable = 0x100,
        Confuse = 0x80000,
        Stun = 0x2000000000000,
        Poison = 0x4000000000000,
        Sealed = 0x8000000000000,
        Darkness = 0x10000000000000,
        Weaken = 0x4000000000000000
    }

    public enum MobSkillName : int
    {
        WeaponAttackUp = 100,
        MagicAttackUp = 101,
        WeaponDefenseUp = 102,
        MagicDefenseUp = 103,

        WeaponAttackUpAreaOfEffect = 110,
        MagicAttackUpAreaOfEffect = 111,
        WeaponDefenseUpAreaOfEffect = 112,
        MagicDefenseUpAreaOfEffect = 113,
        HealAreaOfEffect = 114,
        SpeedUpAreaOfEffect = 115,

        Seal = 120,
        Darkness = 121,
        Weakness = 122,
        Stun = 123,
        Curse = 124,
        Poison = 125,
        Slow = 126,
        Dispel = 127,
        Seduce = 128,
        SendToTown = 129,
        PoisonMist = 131,
        Confuse = 132,
        Zombify = 133,

        WeaponImmunity = 140,
        MagicImmunity = 141,
        ArmorSkill = 142,

        WeaponDamageReflect = 143,
        MagicDamageReflect = 144,
        AnyDamageReflect = 145,

        WeaponAttackUpMonsterCarnival = 150,
        MagicAttackUpMonsterCarnival = 151,
        WeaponDefenseUpMonsterCarnival = 152,
        MagicDefenseUpMonsterCarnival = 153,
        AccuracyUpMonsterCarnival = 154,
        AvoidabilityUpMonsterCarnival = 155,
        SpeedUpMonsterCarnival = 156,
        SealMonsterCarnival = 157,

        Summon = 200
    }

    public enum NotificationType : byte
    {
        Notice = 0x00,
        PopupBox = 0x01,
        Megaphone = 0x02,
        SuperMegaphone = 0x03,
        Header = 0x04,
        RedText = 0x05
    }

    public enum QuestAction : byte
    {
        RestoreLostItem,
        Start,
        Complete,
        Forfeit,
        ScriptStart,
        ScriptEnd
    }

    public enum DialogType : byte
    {
        Normal = 0,
        YesNo = 1,
        GetText = 2,
        GetNumber = 3,
        Menu = 4,
        Question = 5,
        Quiz = 6,
        Style = 7,
        Pet = 8,
        AcceptDecline = 0x0C
    }

    public enum StatisticType : int
    {
        Skin = 0x01,
        Face = 0x02,
        Hair = 0x04,
        Pet = 0x08,
        Level = 0x10,
        Job = 0x20,
        Strength = 0x40,
        Dexterity = 0x80,
        Intelligence = 0x100,
        Luck = 0x200,
        CurrentHP = 0x400,
        MaxHP = 0x800,
        CurrentMP = 0x1000,
        MaxMP = 0x2000,
        AvailableAP = 0x4000,
        AvailableSP = 0x8000,
        Experience = 0x10000,
        Fame = 0x20000,
        Meso = 0x40000
    }

    public enum FieldEffect
    {
        ShipArrive,
        ShipLeave
    }

    public enum ExpirationTime : long
    {
        DefaultTime = 150842304000000000L,
        ZeroTime = 94354848000000000L,
        Permanent = 150841440000000000L
    }
}
