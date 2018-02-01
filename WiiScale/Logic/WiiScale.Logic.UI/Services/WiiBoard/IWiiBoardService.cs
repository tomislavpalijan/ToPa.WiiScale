using System;

namespace WiiScale.Logic.UI.Services.WiiBoard
{
    public interface IWiiBoardService
    {
        float BatteryState { get; }
        float Offset { get; }
        float OffsetMax { get; }
        float OffsetMin { get; }
        float OffsetRange { get; }
        float WeightInKg { get; }
        bool IsInitialized { get; }

        event EventHandler<float> BatteryStateChanged;
        event EventHandler<float> OffsetChanged;
        event EventHandler<float> WeightInKgChanged;
        event EventHandler<WiiBoardServiceState> WiiBoardServiceStateChanged;

        void Init(float offset = 0);
        void StartCalibration();
    }
}