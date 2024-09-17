namespace Nap.Auth.DataCachingPush;

using Fanex.Data.Repository;

public class GetUserFunctionsBySiteCriteria : ICriteria
{
    public int SiteId { get; set; }

    public string GetSettingKey() => "NAP_Caching_GetUserFunctionsBySite";

    public bool IsValid() => true;
}

public class GetAllFunctionsCriteria : ICriteria
{
    public string GetSettingKey() => "NAP_Function_GetAll";

    public bool IsValid() => true;
}

public class GetAllGroupsCriteria : ICriteria
{
    public bool? IsActive => null;

    public string GetSettingKey() => "NAP_Group_GetAll";

    public bool IsValid() => true;
}

public class GetAllUserSiteLevelsCriteria : ICriteria
{
    public string GetSettingKey() => "NAP_Caching_GetUserSiteLevel";

    public bool IsValid() => true;
}

public class GetAllMenuItemsCriteria : ICriteria
{
    public string GetSettingKey() => "NAP_MenuItem_GetAll";

    public bool IsValid() => true;
}


public class GetAllUsersCriteria : ICriteria
{
    public string GetSettingKey() => "NAP_User_GetAll";

    public bool IsValid() => true;
}

public class UserGetOtpCriteria : CriteriaBase
{
    public int CustId { get; set; }

    public string System { get; set; }

    public override string GetSettingKey() => "NAP_Admin_GetOTP";

    public override bool IsValid() => true;
}

public class GetAllSiteCriteria : CriteriaBase
{
    public string Environment { get; set; } = Constant.Uat;

    public bool? IsActive { get; set; }

    public override string GetSettingKey() => "NAP_Site_GetAll";

    public override bool IsValid() => true;
}