using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndertaleModLib.Models;
using static UndertaleModLib.Models.UndertaleSequence;

namespace UndertaleModTool.ProjectTool.Resources
{
    public class GMSprite : ResourceBase
    {
        public enum BboxMode
        {
            Automatic,
            FullImage,
            Manual
        }
        public enum CollisionKind
        {
            Precise,
            Rectangle,
            Ellipse,
            Diamond,
            PrecisePerFrame,
            RotatedRectangle
        }
        public enum Type
        {
            Bitmap,
            SWF,
            Spine
        }
        public enum Origin
        {
            TopLeft,
            TopCentre,
            TopRight,
            MiddleLeft,
            MiddleCentre,
            MiddleRight,
            BottomLeft,
            BottomCentre,
            BottomRight,
            Custom
        }

        public BboxMode bboxMode { get; set; } = BboxMode.Automatic;
        public CollisionKind collisionKind { get; set; } = CollisionKind.Rectangle;
        public Type type { get; set; } = Type.Bitmap;
        public Origin origin { get; set; } = Origin.TopLeft;
        public bool preMultiplyAlpha { get; set; } = false;
        public bool edgeFiltering { get; set; } = false;
        public int collisionTolerance { get; set; } = 0; // 0 to 255, for Precise collision kind
        public float swfPrecision { get; set; } = 2.525f;
        public int bbox_left { get; set; }
        public int bbox_right { get; set; }
        public int bbox_top { get; set; }
        public int bbox_bottom { get; set; }
        public bool HTile { get; set; } = false;
        public bool VTile { get; set; } = false;
        public bool For3D { get; set; } = false;
        public bool DynamicTexturePage { get; set; } = false;
        public uint width { get; set; } = 64;
        public uint height { get; set; } = 64;
        public IdPath textureGroupId { get; set; } = new IdPath("Default", "texturegroups/");
        public List<uint> swatchColours { get; } // Ctrl+Click a color in the sprite editor to change it. The sprite file saves that into this list (apparently)
        public uint gridX { get; set; } = 0;
        public uint gridY { get; set; } = 0;
        public List<GMSpriteFrame> frames { get; set; } = new();
        public GMSequence sequence { get; set; } = new();
        public List<GMImageLayer> layers { get; set; } = new();
        public GMNineSliceData nineSlice { get; set; } = null;

        /// <summary>
        /// Translate an UndertaleTexturePageItem into a frame and add it
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="index"></param>
        public void AddFrameFrom(UndertaleTexturePageItem texture, int index)
        {
            if (_framesTrack == null)
            {
                _framesTrack = new();
                sequence.tracks.Add(_framesTrack);
            }

            string frameGuid = Dump.ToGUID($"{name}.{index}");
            frames.Add(new GMSpriteFrame { name = frameGuid });

            SpriteFrameKeyframe keyframe = new();
            keyframe.Id = new IdPath(frameGuid, $"sprites/{name}/{name}.yy");
            Keyframe<SpriteFrameKeyframe> keyframeHolder = new();
            keyframeHolder.id = Dump.ToGUID($"{name}.{index}k");
            keyframeHolder.Key = index;
            keyframeHolder.Channels.Add("0", keyframe);
            _framesTrack.keyframes.Keyframes.Add(keyframeHolder);
        }

        private GMSpriteFramesTrack _framesTrack;

        /// <summary>
        /// Translate an UndertaleSprite to a new GMSprite
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static GMSprite From(UndertaleSprite source)
        {
            if (source == null) return null;

            GMSprite target = new GMSprite();
            target.name = source.Name.Content;
            (target.width, target.height) = (source.Width, source.Height);
            target.For3D = TpageAlign.IsSeparateTexture(source);
            target.textureGroupId.SetName(TpageAlign.TextureForOrDefault(source).Name.Content);
            target.nineSlice = GMNineSliceData.From(source.V3NineSlice);

            #region Sprite origin

            target.origin = 0;

            if (source.OriginX == MathF.Floor(target.width / 2f))
                target.origin += 1;
            else if (source.OriginX == target.width)
                target.origin += 2;
            else if (source.OriginX != 0)
                target.origin = Origin.Custom;

            if (target.origin != Origin.Custom)
            {
                if (source.OriginY == MathF.Floor(target.height / 2f))
                    target.origin += 3;
                else if (source.OriginY == target.height)
                    target.origin += 6;
                else if (source.OriginY != 0)
                    target.origin = Origin.Custom;
            }

            #endregion
            #region Bounding box

            target.bbox_left = source.MarginLeft;
            target.bbox_right = source.MarginRight;
            target.bbox_bottom = source.MarginBottom;
            target.bbox_top = source.MarginTop;
            target.bboxMode = (BboxMode)source.BBoxMode;

            if (source.SepMasks == UndertaleSprite.SepMaskType.Precise)
            {
                if (source.CollisionMasks.Count > 1)
                    target.collisionKind = CollisionKind.PrecisePerFrame;
                else
                {
                    target.collisionKind = CollisionKind.Precise;

                    // TODO: could be diamond or ellipse
                }
            }
            else if (source.SepMasks == UndertaleSprite.SepMaskType.RotatedRect)
                target.collisionKind = CollisionKind.RotatedRectangle;

            #endregion
            #region Sequence

            target.sequence.name = target.name;
            target.sequence.playback = GMSequence.PlaybackType.Looped;
            target.sequence.playbackSpeed = source.GMS2PlaybackSpeed;
            target.sequence.playbackSpeedType = (GMSequence.PlaybackSpeedType)source.GMS2PlaybackSpeedType;
            target.sequence.length = source.Textures.Count;
            (target.sequence.xorigin, target.sequence.yorigin) = (source.OriginX, source.OriginY);

            for (var i = 0; i < source.Textures.Count; ++i)
            {
                UndertaleTexturePageItem texture = source.Textures[i].Texture;
                target.AddFrameFrom(texture, i);
            }

            #endregion

            target.parent = new IdPath("Sprites", "folders/Sprites.yy");
            return target;
        }
    }
    public class GMSpriteFrame : ResourceBase
    {
        public GMSpriteFrame()
        {
            resourceVersion = "1.1";
        }
    }
    public class GMImageLayer : ResourceBase
    {
        public enum BlendMode
        {
            Normal,
            Add,
            Subtract,
            Multiply
        }
        public bool visible { get; set; } = true;
        public bool isLocked { get; set; } = false;
        public BlendMode blendMode { get; set; } = BlendMode.Normal;
        public float opacity { get; set; } = 100.0f;
        public string displayName = "default";
    }
    public class GMNineSliceData : ResourceBase
    {
        private static uint DEFAULT_GUIDE_COLOUR = 4294902015;
        private static uint DEFAULT_HIGHLIGHT_COLOUR = 1728023040;

        public enum HighlightStyle
        {
            Inverted,
            Overlay
        }
        public enum TileMode
        {
            Stretch,
            Repeat,
            Mirror,
            BlankRepeat,
            Hide
        }

        public int left { get; set; } = 0;
        public int top { get; set; } = 0;
        public int right { get; set; } = 0;
        public int bottom { get; set; } = 0;
        public uint[] guideColour { get; set; } = new uint[] { DEFAULT_GUIDE_COLOUR, DEFAULT_GUIDE_COLOUR, DEFAULT_GUIDE_COLOUR, DEFAULT_GUIDE_COLOUR };
        public uint highlightColour { get; set; } = DEFAULT_HIGHLIGHT_COLOUR;
        public HighlightStyle highlightStyle { get; set; } = HighlightStyle.Inverted;
        public bool enabled { get; set; } = false;
        public TileMode[] tileMode { get; set; } = new TileMode[] { TileMode.Stretch, TileMode.Stretch, TileMode.Stretch, TileMode.Stretch, TileMode.Stretch };
        public object loadedVersion { get; set; } = null; // ???

        /// <summary>
        /// Translate an UndertaleSprite.NineSlice to GMNineSliceData
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static GMNineSliceData From(UndertaleSprite.NineSlice source)
        {
            if (source == null) return null;

            GMNineSliceData target = new();

            target.left = source.Left;
            target.top = source.Top;
            target.right = source.Right;
            target.bottom = source.Bottom;
            target.enabled = source.Enabled;
            for (int i = 0; i < 5; i++)
                target.tileMode[i] = (TileMode)source.TileModes[i];

            return target;
        }
    }
}
