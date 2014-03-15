using System;

namespace Common.Net
{
    public enum ClientMessages : byte
    {
        CheckPassword =             0x01,
        SelectWorld =               0x04,
        CheckUserLimit =            0x05,
        LicenceAgreement =          0x06,
        CheckPin =                  0x08,
        RegisterPin =               0x09,
        WorldListRequest =          0x0A,
        SelectCharacter =           0x0B,
        MigrateIn =                 0x0C,
        CheckDuplicatedID =         0x0D,
        CreateNewCharacter =        0x0E,
        Hash =                      0x12,
        LogOut =                    0x14,

        SetField =                  0x17,
        CharacterMovement =         0x1A,
        CharacterSit =              0x1B,
        GeneralChat =               0x22,
        NpcConverse =               0x27,
        NpcResult =                 0x28,
        ItemMovement =              0x2D,
        DistributeAP =              0x36,
        HealOverTime =              0x37,
        DistributeSP =              0x38,
        CharacterInformation =      0x3F,
        SetScriptedField =          0x42,
        QuestOperation =            0x47,
        MobMovement =               0x6A,
        NpcAction =                 0x6F,

    }

    public enum ServerMessages : byte
    {
        Unknown =                   0x00,
        CheckPasswordResult =       0x01,
        CheckUserLimitResult =      0x03,
        PinOperation =              0x07,
        PinAssigned =               0x08,
        WorldInformation =          0x09,
        SelectWorldResult =         0x0A,
        SelectCharacterResult =     0x0B,
        CheckDuplicatedIDResult =   0x0C,
        CreateNewCharacterResult =  0x0D,
        DeleteCharacterResult =     0x0E,

        #region Reserved Headers

        InternetCafe_1 =            0x13,
        InternetCafe_2 =            0x14,

        #endregion

        InventoryOperation =        0x18,
        InventoryGrow =             0x19,
        StatChanged =               0x1A,
        ForcedStatSet =             0x1B,
        ForcedStatReset =           0x1C,
        ChangeSkillRecordResult =   0x1D,
        SkillUseResult =            0x1E,
        GivePopularityResult =      0x1F,
        Message =                   0x20,
        MemoResult =                0x21,
        MapTransferResult =         0x22,
        AntiMacroResult =           0x23,
        ClaimResult =               0x24,
        SetClaimSvrAvailableTime =  0x25,
        ClaimSvrStatusChanged =     0x26,
        EntrustedShopCheckResult =  0x28,
        QuestClear =                0x27,
        SueCharacterResult =        0x29,
        CharacterInfo =             0x2C,
        PartyResult =               0x2D,
        FriendResult =              0x2E,
        GuildResult =               0x30,
        TownPortal =                0x31,
        BroadcastMsg =              0x32,
        IncubatorResult =           0x33,
        SetField =                  0x36,
        SetCashShop =               0x37,
        TrasnferFieldReqIgnored =   0x3A,
        TransferChannelReqIgnored = 0x3B,
        FieldSpecificData =         0x3C,
        GroupMessage =              0x3D,
        Whisper =                   0x3D,
        SummonItemIavailable =      0x3F,
        FieldEffect =               0x40,
        BlowWeather =               0x41,
        PlayJukeBox =               0x42,
        AdminResult =               0x43,
        Quiz =                      0x44,
        Desc =                      0x45,
        FieldClock =                0x46,
        ContiMove =                 0x47,
        ContiState =                0x48,
        SetQuestClear =             0x4A,
        SetQuestTime =              0x4B,
        WarnMessage =               0x4C,
        GeneralChat =               0x51,
        UserEnterField =            0x50,
        UserLeaveField =            0x50,
        CharacterMovement =         0x60,

        CharacterSit =              0x78,
        ShowForeignEffect =         0x79,
        GiveMesoSuccess =           0x7C,
        GiveMesoFailed =            0x7D,
        QuestSomething =            0x7E,

        MobEnterField =             0x86,
        MobLeaveField =             0x87,
        MobChangeController =       0x88,
        NpcEnterField =             0x97,
        NpcLeaveField =             0x98,
        NpcChangeController =       0x99,
        NpcSetSpecialAction =       0x9B,
        DropEnterField =            0xA4,
        DropLeaveField =            0xA5,
        MessageBoxCreateFailed =    0xA8,
        MessageBoxEnterField =      0xA9,
        MessageBoxLeaveField =      0xAA,
        AffectedAreaCreated =       0xAD,
        AffectedAreaRemoved =       0xAE,
        TownPortalCreated =         0xB1,
        TownPortalRemoved =         0xB2,
        ScriptMessage =             0xC5,
    }

    public enum InteroperabilityMessages : byte
    {
        AllocationRequest,
        AllocationResponse,

        ServerListRequest,
        ServerListResponse,

        CheckUserLimitRequest,
        CheckUserLimitResponse,

        CharacterListRequest,
        CharacterListResponse,

        CharacterCreationRequest,
        CharacterCreationResponse,

        MigrateRequest,
        MigrateResponse,

        CharacterRegistrationRequest,
        CharacterDeregistrationRequest
    }
}
