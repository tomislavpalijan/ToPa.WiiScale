namespace WiiScale.Logic.UI.Services.WiiBoard
{
    public interface IWiiBoardService
    {
        float Offset { get; }
        float OffsetMax { get; }
        float OffsetMin { get; }
        float OffsetRange { get; }
        float WeightInKg { get; }
        bool IsInitialized { get; }

        void Init(float offset = 0);
        void StartCalibration();
    }
}