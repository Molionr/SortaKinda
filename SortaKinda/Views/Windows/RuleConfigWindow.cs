﻿using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Style;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using SortaKinda.Models;
using SortaKinda.System;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.Views.Windows;

public class RuleConfigWindow : Window
{
    private readonly List<SortingRule> ruleList;
    private readonly SortingRuleView view;
    public SortingRule Rule;

    public RuleConfigWindow(SortingRule sortingRule, List<SortingRule> sortingRules) : base($"SortaKinda Rule Configuration - {sortingRule.Name}###RuleConfig{sortingRule.Id}")
    {
        Rule = sortingRule;
        ruleList = sortingRules;
        view = new SortingRuleView(sortingRule);

        Position = ImGui.GetMainViewport().Size / 2.0f - new Vector2(500.0f, 400.0f) / 2.0f;
        PositionCondition = ImGuiCond.Appearing;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500.0f, 400.0f),
            MaximumSize = new Vector2(9999, 9999)
        };

        IsOpen = true;
    }

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, StyleModelV1.DalamudStandard.WindowPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, StyleModelV1.DalamudStandard.FramePadding);
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, StyleModelV1.DalamudStandard.CellPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, StyleModelV1.DalamudStandard.ItemSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, StyleModelV1.DalamudStandard.ItemInnerSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, StyleModelV1.DalamudStandard.IndentSpacing);
    }
    
    public override void Draw()
    {
        DrawHeader();
        view.Draw();
    }
    
    public override void PostDraw()
    {
        ImGui.PopStyleVar(6);
    }

    private void DrawHeader()
    {
        DrawColorEdit();
        DrawNameEdit();
        DrawDeleteButton();
    }

    private void DrawColorEdit()
    {
        var region = ImGui.GetContentRegionAvail();

        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X / 4.0f - ImGuiHelpers.GlobalScale * 50.0f + ImGui.GetStyle().ItemSpacing.X / 2.0f });
        var imGuiColor = Rule.Color;
        if (ImGui.ColorEdit4("##ColorConfig", ref imGuiColor, ImGuiColorEditFlags.NoInputs))
        {
            Rule.Color = imGuiColor;
        }
    }

    private void DrawNameEdit()
    {
        var region = ImGui.GetContentRegionAvail();

        ImGui.SameLine();
        ImGui.SetNextItemWidth(region.X / 2.0f - ImGui.GetItemRectSize().X - ImGui.GetStyle().ItemSpacing.X);
        var imGuiName = Rule.Name;
        if (ImGui.InputText("##NameEdit", ref imGuiName, 1024, ImGuiInputTextFlags.AutoSelectAll))
        {
            Rule.Name = imGuiName;
            WindowName = $"SortaKinda Rule Configuration - {Rule.Name}###RuleConfig{Rule.Id}";
        }
    }

    private void DrawDeleteButton()
    {
        var hotkeyHeld = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;
        if (!hotkeyHeld) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
        ImGui.SameLine();
        if (ImGui.Button("Delete", ImGuiHelpers.ScaledVector2(100.0f, 23.0f)) && hotkeyHeld)
        {
            ruleList.Remove(Rule);
            IsOpen = false;
        }
        if (!hotkeyHeld) ImGui.PopStyleVar();
        if (ImGui.IsItemHovered() && !hotkeyHeld)
        {
            ImGui.SetTooltip("Hold Shift + Control while clicking to delete this rule");
        }
        ImGuiHelpers.ScaledDummy(5.0f);
    }

    public override void OnClose()
    {
        SortaKindaController.SortController.SaveConfig();
        RuleConfigWindowController.RemoveRuleConfigWindow(this);
    }
}