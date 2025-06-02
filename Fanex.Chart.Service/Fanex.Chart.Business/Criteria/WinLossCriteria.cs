using Fanex.Data.Repository;
using System;

namespace Fanex.Chart.Business.Criteria;

public class WinLossCriteria : CriteriaBase
{
    public DateTime WinlostDate { get; set; }

    public override string GetSettingKey()
    {
        return "Acc_Rpt_Letstalk_DCWL";
    }

    public override bool IsValid()
    {
        return true;
    }
}