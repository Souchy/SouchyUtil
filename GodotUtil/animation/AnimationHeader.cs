using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace souchyutil.godot.animation;

public record AnimationHeader(int Id, float Length, int LoopMode, int FrameCount, int BakedIndex);