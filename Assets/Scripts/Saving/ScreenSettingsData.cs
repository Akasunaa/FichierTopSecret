
[System.Serializable]
public class ScreenSettingsData
{
    public int[] resolution;
    public int refreshRate;

    public ScreenSettingsData(int width, int height, int refreshRate)
    {
        resolution = new[] { width, height };
        this.refreshRate = refreshRate;
    }
}