namespace NonebNi.Ui.Cameras
{
    public record CameraConfig(
        CameraControlSetting Setting,
        float DownBound,
        float LeftBound,
        float RightBound,
        float UpBound);
}