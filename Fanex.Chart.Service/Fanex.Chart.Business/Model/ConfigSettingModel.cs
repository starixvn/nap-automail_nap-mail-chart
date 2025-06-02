using System;
using System.Collections.Generic;

namespace Fanex.Chart.Business.Model;

public class ConfigSettingModel
{
    public string LetstalkServiceApi { get; set; }

    public string TargetName { get; set; }

    public string Group { get; set; }

    public string Path { get; set; }

    public string InternalPath { get; set; }

    public string Password { get; set; }

    public bool IsCheckEnoughData { get; set; }

    public bool IsSentDirectly { get; set; }

    public bool IsManual { get; set; }

    public DateTime WinlostDate { get; set; }

    public Dictionary<string, ImageProperty> Image { get; set; } = new();

    public bool Done { get; set; }
}