﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using SortaKinda.Interfaces;

namespace SortaKinda.System;

public unsafe class SortingThreadController : IDisposable
{
    private readonly List<Task> sortingTasks = new();
    private bool SortPending => sortingTasks.Any(task => task.Status is TaskStatus.Created);
    private readonly CancellationTokenSource cancellationTokenSource = new();

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
    }

    public void AddSortingTask(InventoryType type, params IInventoryGrid[] grids)
    {
        var sortingTask = new Task(() =>
        {
            InventorySorter.SortInventory(type, grids);
        }, cancellationTokenSource.Token);
        
        sortingTasks.Add(sortingTask);
    }

    public void Update()
    {
        if (SortPending)
        {
            PluginLog.Verbose($"Launching sorting tasks. {sortingTasks.Where(task => task.Status is TaskStatus.Created).Count()} Tasks Pending.");
            
            foreach (var task in sortingTasks.Where(task => task.Status is TaskStatus.Created))
            {
                PluginLog.Verbose("Starting Task");
                task.Start();
            }

            PluginLog.Verbose("Scheduling Continuation");
            Task.WhenAll(sortingTasks).ContinueWith(_ => OnCompletion(), cancellationTokenSource.Token);
        }
    }

    private void OnCompletion()
    {
        PluginLog.Verbose("Continuing!");
        
        Service.Framework.RunOnTick(() =>
        {
            PluginLog.Debug("Marked ItemODR as changed.");

            ItemOrderModule.Instance()->UserFileEvent.IsSavePending = true;
        }, TimeSpan.Zero, 5);

        sortingTasks.RemoveAll(task => task.Status is TaskStatus.RanToCompletion or TaskStatus.Canceled or TaskStatus.Faulted);
    }
}