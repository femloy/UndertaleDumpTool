using System.Collections.Generic;

namespace UndertaleModTool.ProjectTool.Resources
{
    public class GMSequence : ResourceBase
    {
        public GMSequence() : base()
        {
            resourceVersion = "1.4";
        }

        public enum PlaybackType
        {
            None,
            Looped,
            Bounce
        }
        public enum PlaybackSpeedType
        {
            FramesPerSecond,
            FramesPerGameFrame
        }

        public IdPath spriteId { get; set; } = null; // ???
        public int timeUnits { get; set; } = 1; // ???
        public PlaybackType playback { get; set; } = PlaybackType.None;
        public float playbackSpeed { get; set; } = 60.0f;
        public PlaybackSpeedType playbackSpeedType { get; set; } = PlaybackSpeedType.FramesPerSecond;
        public bool autoRecord { get; set; } = true;
        public float volume { get; set; } = 1.0f;
        public float length { get; set; } = 60.0f;
        public KeyframeStore<MessageEventKeyframe> events { get; set; } = new();
        public KeyframeStore<MomentsEventKeyframe> moments { get; set; } = new();
        public List<GMBaseTrack> tracks { get; set; } = new();
        public object visibleRange { get; set; } = null; // ???
        public bool lockOrigin { get; set; } = false;
        public bool showBackdrop { get; set; } = true;
        public bool showBackdropImage { get; set; } = false;
        public string backdropImagePath { get; set; } = "";
        public float backdropImageOpacity { get; set; } = 0.5f;
        public uint backdropWidth { get; set; } = 1366;
        public uint backdropHeight { get; set; } = 768;
        public float backdropXOffset { get; set; } = 0.0f;
        public float backdropYOffset { get; set; } = 0.0f;
        public int xorigin { get; set; } = 0;
        public int yorigin { get; set; } = 0;
        public Dictionary<string, string> eventToFunction { get; set; } = new();
        public IdPath eventStubScript { get; set; } = null;
    }
    public class KeyframeStore<T> : ResourceBase
    {
        public KeyframeStore() : base()
        {
            resourceType = $"KeyframeStore<{typeof(T).Name}>";
        }
        public List<Keyframe<T>> Keyframes { get; set; } = new();
    }

    /// <summary>
    /// Sequence track types
    /// </summary>
    public class GMBaseTrack : ResourceBase
    {
        public enum BuiltinName
        {
            None,
            // ...
            Rotation = 8,
            // ...
            Position = 14,
            Scale = 15,
            Origin = 16
        }
        public enum Interpolation
        {
            None,
            Linear
        }

        public uint trackColour { get; set; } = 0;
        public bool inheritsTrackColour { get; set; } = true;
        public BuiltinName builtinName { get; set; } = BuiltinName.None;
        public int traits { get; set; } = 0; // ???
        public Interpolation interpolation { get; set; } = Interpolation.Linear;
        public List<GMBaseTrack> tracks { get; set; } = new(); // Nested tracks
        public List<object> events { get; set; } = new(); // ???
        public bool isCreationTrack { get; set; } = false;
        public List<object> modifiers { get; set; } = new(); // ???
    }
    public class GMSpriteFramesTrack : GMBaseTrack
    {
        public GMSpriteFramesTrack() { name = "frames"; }
        public IdPath spriteId { get; set; } = null; // ???
        public KeyframeStore<SpriteFrameKeyframe> keyframes { get; set; } = new();
    }

    /// <summary>
    /// Sequence keyframe types
    /// </summary>
    public class Keyframe<T> : ResourceBase
    {
        public Keyframe() : base()
        {
            resourceType = $"Keyframe<{typeof(T).Name}>";
        }

        public string id { get; set; } // GUID
        public float Key { get; set; } = 0.0f;
        public float Length { get; set; } = 1.0f;
        public bool Stretch { get; set; } = false;
        public bool Disabled { get; set; } = false;
        public bool IsCreationKey { get; set; } = false;
        public Dictionary<string, T> Channels { get; set; } = new();
    }
    public class MessageEventKeyframe : ResourceBase
    {
        // TODO
    }
    public class MomentsEventKeyframe : ResourceBase
    {
        // TODO
    }
    public class SpriteFrameKeyframe : ResourceBase
    {
        public IdPath Id { get; set; }
    }
}
