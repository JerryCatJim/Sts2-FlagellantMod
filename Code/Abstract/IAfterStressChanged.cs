using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Code.Abstract;
public interface IAfterStressChanged
{
    public Task AfterStressAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource);
}
