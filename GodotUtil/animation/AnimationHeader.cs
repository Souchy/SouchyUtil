using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace souchyutil.godot.animation;

public record AnimationHeader(int LibraryId, int AnimationId, float Length, Animation.LoopModeEnum LoopMode, int FrameCount, int BakedIndex);