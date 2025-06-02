using Fanex.Data.Repository;

namespace Fanex.Chart.Business.Criteria;

public class GetDWStatusCriteria : CriteriaBase
{
    public string TaskIDList => "38";

    public override string GetSettingKey()
    {
        return "DailyJob_CheckTasksStatus";
    }

    public override bool IsValid()
    {
        return true;
    }
}