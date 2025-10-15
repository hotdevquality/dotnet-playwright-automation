namespace FrameworkProject
{
    public class PlaywrightSettings
    {
        public string Browser { get; set; } = "chromium";
        public bool Headless { get; set; } = true;
        public int SlowMo { get; set; } = 0;
        public bool RecordVideo { get; set; } = true;
        public bool RecordHar { get; set; } = false;
        public int ViewportWidth { get; set; } = 1280;
        public int ViewportHeight { get; set; } = 800;
        public string VideoDir { get; set; } = "artifacts/videos";
        public string HarPath { get; set; } = "artifacts/network.har";
    }
}
    
