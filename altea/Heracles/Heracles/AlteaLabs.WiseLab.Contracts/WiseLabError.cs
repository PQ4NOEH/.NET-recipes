namespace AlteaLabs.WiseLab.Contracts
{
    public enum WiseLabError
    {
        None = 0x00,
        UnknownError = 0xF0,
        HuntDataExists = 0xF1,
        HuntDataNotExists = 0xF2,
        HuntDataInboxOverflow = 0xF3,
        HuntDataWorkInProgress = 0xF4,
        FinishNotAllowed = 0xF5,
        InvalidStatus = 0xF6
    }
}
