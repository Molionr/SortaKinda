﻿using System.Collections.Generic;
using System.Linq;
using KamiLib;
using SortaKinda.Models;
using SortaKinda.Views.Windows;

namespace SortaKinda.System;

public class RuleConfigWindowController
{
    public static void AddRuleConfigWindow(SortingRule rule, List<SortingRule> sortingRules)
    {
        if (!KamiCommon.WindowManager.GetWindows().OfType<RuleConfigWindow>().Any(window => window.Rule.Id == rule.Id))
        {
            KamiCommon.WindowManager.AddWindow(new RuleConfigWindow(rule, sortingRules));
        }
    }

    public static void RemoveRuleConfigWindow(RuleConfigWindow caller)
    {
        KamiCommon.WindowManager.RemoveWindow(caller);
    }

    public static void RemoveRuleConfigWindow(string ruleId)
    {
        if (KamiCommon.WindowManager.GetWindows().OfType<RuleConfigWindow>().FirstOrDefault(window => window.Rule.Id == ruleId) is { } configWindow)
        {
            RemoveRuleConfigWindow(configWindow);
        }
    }
}