using System;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

public class Power_Dialog_Extract(Tuple<PowerDef, PowerDef> pwr, bool closed = false)
{
    public bool isClosedNormaly = closed;

    public Tuple<PowerDef, PowerDef> powers = pwr;
}