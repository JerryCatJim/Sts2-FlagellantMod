using Flagellant.Code.Abstract;
using Flagellant.Code.Commands;
using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Code.Powers;
public class ResoluteOrMeltdownPowerModel : FlagellantPowerModel
{
    //这个类和其派生类都只是为了把美德和折磨效果具象化为一个仅供展示的挂在人物下方的BUFF图标
    //美德和折磨的具体实现应在ResoluteOrMeltdown文件夹下实现，因为我想把美德和折磨设计为类似观者姿态的那种与人物BUFF和DEBUFF区分开的一种独特效果
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}